// See https://aka.ms/new-console-template for more information
using HexaGen;
using HexaGen.Core;
using HexaGen.Core.CSharp;
using HexaGen.Metadata;
using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Generator
{
    internal partial class Program
    {
        private static readonly LoggerBase logger = new();
        private static readonly ConfigComposer composer = new();

        public static readonly CsType StringType = new("byte*", CsPrimitiveType.Byte) { StringType = CsStringType.StringUTF8 };

        private const string ApiName = "vulkan";
        private static readonly Dictionary<string, int> constantMap = [];

        private static readonly Dictionary<string, CsDelegate> nameToDelegate = new();

        private static void Main(string[] args)
        {
            logger.LogToConsole();
            XmlSerializer serializer = new(typeof(VkRegistry));
            using var fs = File.OpenRead("vk.xml");
            VkRegistry registry = (VkRegistry)serializer.Deserialize(fs)!;

            CsCodeGeneratorConfig config = CsCodeGeneratorConfig.Load("generator.base.json");

            FrozenSet<string> enumNames = registry.EnumGroups.Select(g => g.Name).ToFrozenSet();

            foreach (var command in registry.Commands)
            {
                if (command.AltAlias != null)
                {
                    command.AliasCommand = registry.Commands.FirstOrDefault(x => x.Name == command.AltAlias);

                    if (command.AliasCommand!.Level == FunctionLevel.Unknown)
                    {
                        logger.LogWarn($"Unknown level for '{command.Name}'");
                    }
                }
                else
                {
                    if (command.Level == FunctionLevel.Unknown)
                    {
                        logger.LogWarn($"Unknown level for '{command.Name}'");
                    }
                }
            }

            foreach (var type in registry.VkTypes.VkTypes)
            {
                if (type.Category == "bitmask")
                {
                    if (type.Requires != null && enumNames.Contains(type.Requires))
                    {
                        config.TypeMappings.Add(type.Requires, config.GetCsCleanName(type.Name));
                    }
                    else
                    {
                        switch (type.Type)
                        {
                            case "VkFlags":
                                config.TypeMappings.Add(type.Name, "uint");
                                break;

                            case "VkFlags64":
                                config.TypeMappings.Add(type.Name, "ulong");
                                break;
                        }
                    }
                }
                else if (type.Category == "funcpointer")
                {
                    var dele = CsDelegate.Parse(type, config);
                    nameToDelegate.Add(dele.Name, dele);
                }
            }

            const string outputDir = "../../../../Hexa.NET.Vulkan/Generated";

            WriteConstants(registry, config, outputDir);

            var groupToEnumName = GenerateEnums(config, registry, outputDir);
            CsCodeGenerator generator = new(config);
            generator.LogToConsole();
            generator.Generate("include/main.h", outputDir);

            var allowedTypes = GatherAllowedTypes(registry).ToFrozenSet();
            WriteTypes(allowedTypes, registry, config, outputDir);

            WriteFunctionsCore(registry, config, outputDir, groupToEnumName);
        }

        private static void WriteFunctionsCore(VkRegistry registry, CsCodeGeneratorConfig config, string outputPath, Dictionary<string, string> groupToEnumName)
        {
            string functionsFolder = Path.Combine(outputPath, "Functions");
            if (Directory.Exists(functionsFolder))
            {
                Directory.Delete(functionsFolder, true);
            }
            Directory.CreateDirectory(functionsFolder);

            var allowedFunctions = GatherAllowedFunctions(registry).ToFrozenSet();
            FunctionTableBuilder fnBuilderGlobal = new();
            FunctionTableBuilder fnBuilderInstance = new();
            FunctionTableBuilder fnBuilderDevice = new();

            {
                using CsSplitCodeWriter writer = new(Path.Combine(functionsFolder, "Functions.cs"), config.Namespace, ["System", "HexaGen.Runtime", "System.Runtime.CompilerServices", "System.Numerics"], null);

                writer.BeginBlock($"public static unsafe partial class {config.ApiName}");
                WriteFunctions(FunctionLevel.Global, writer, registry, config, allowedFunctions, groupToEnumName, fnBuilderGlobal, "public", "static");
                writer.EndBlock();

                WriteFunctionTable(config, Path.Combine(outputPath, "FunctionTable.cs"), fnBuilderGlobal);
            }

            {
                using CsSplitCodeWriter writer = new(Path.Combine(functionsFolder, "Functions.Instance.cs"), config.Namespace, ["System", "HexaGen.Runtime", "System.Runtime.CompilerServices", "System.Numerics"], null);

                writer.BeginBlock($"public unsafe partial class Instance");
                WriteFunctions(FunctionLevel.Instance, writer, registry, config, allowedFunctions, groupToEnumName, fnBuilderInstance, "public");
                writer.EndBlock();

                WriteFunctionTableInstanceLevel(config, Path.Combine(outputPath, "FunctionTable.Instance.cs"), "Instance", fnBuilderInstance);
            }

            {
                using CsSplitCodeWriter writer = new(Path.Combine(functionsFolder, "Functions.Device.cs"), config.Namespace, ["System", "HexaGen.Runtime", "System.Runtime.CompilerServices", "System.Numerics"], null);

                writer.BeginBlock($"public unsafe partial class Device");
                WriteFunctions(FunctionLevel.Device, writer, registry, config, allowedFunctions, groupToEnumName, fnBuilderDevice, "public");
                writer.EndBlock();

                WriteFunctionTableDeviceLevel(config, Path.Combine(outputPath, "FunctionTable.Device.cs"), "Device", fnBuilderDevice);
            }
        }

        private static void WriteFunctionTable(CsCodeGeneratorConfig config, string outputPath, FunctionTableBuilder functionTableBuilder)
        {
            var initString = functionTableBuilder.Finish(out var count);
            using var writer = new CsCodeWriter(outputPath, config.Namespace, ["System", "HexaGen.Runtime", "System.Numerics"], config.HeaderInjector);
            using (writer.PushBlock($"public static unsafe partial class {config.ApiName}"))
            {
                writer.WriteLine("internal static FunctionTable funcTable;");

                writer.WriteLine();

                using (writer.PushBlock($"public static void InitApi()"))
                {
                    writer.WriteLine($"funcTable = new FunctionTable(GetLibraryName(), {count});");
                    writer.WriteLines(initString);
                }
                writer.WriteLine();
            }
        }

        private static void WriteFunctionTableInstanceLevel(CsCodeGeneratorConfig config, string outputPath, string name, FunctionTableBuilder functionTableBuilder)
        {
            var initString = functionTableBuilder.Finish(out var count);
            using var writer = new CsCodeWriter(outputPath, config.Namespace, ["System", "HexaGen.Runtime", "System.Numerics"], config.HeaderInjector);
            using (writer.PushBlock($"public unsafe partial class {name} : IDisposable"))
            {
                writer.WriteLine("internal readonly VkInstance instance;");
                writer.WriteLine("internal readonly VkAllocationCallbacks* allocationCallbacks;");
                writer.WriteLine("internal readonly InstanceNativeContext context;");
                writer.WriteLine("internal FunctionTable funcTable;");
                writer.WriteLine("private bool ownsInstance;");
                writer.WriteLine();

                using (writer.PushBlock($"protected void InitTable()"))
                {
                    writer.WriteLine($"funcTable = new FunctionTable(context, {count});");
                    writer.WriteLines(initString);
                }
                writer.WriteLine();

                using (writer.PushBlock($"public void Dispose()"))
                {
                    writer.WriteLine("context.Dispose();");
                    writer.WriteLine("funcTable.Dispose();");
                    using (writer.PushBlock("if (ownsInstance)"))
                    {
                        writer.WriteLine("Vulkan.DestroyInstance(instance, allocationCallbacks);");
                    }
                }
            }
        }

        private static void WriteFunctionTableDeviceLevel(CsCodeGeneratorConfig config, string outputPath, string name, FunctionTableBuilder functionTableBuilder)
        {
            var initString = functionTableBuilder.Finish(out var count);
            using var writer = new CsCodeWriter(outputPath, config.Namespace, ["System", "HexaGen.Runtime", "System.Numerics"], config.HeaderInjector);
            using (writer.PushBlock($"public unsafe partial class {name} : IDisposable"))
            {
                writer.WriteLine("internal readonly VkDevice device;");
                writer.WriteLine("internal readonly VkAllocationCallbacks* allocationCallbacks;");
                writer.WriteLine("internal readonly DeviceNativeContext context;");
                writer.WriteLine("internal FunctionTable funcTable;");
                writer.WriteLine("private bool ownsDevice;");
                writer.WriteLine();

                using (writer.PushBlock($"protected void InitTable()"))
                {
                    writer.WriteLine($"funcTable = new FunctionTable(context, {count});");
                    writer.WriteLines(initString);
                }
                writer.WriteLine();

                using (writer.PushBlock($"public void Dispose()"))
                {
                    writer.WriteLine("context.Dispose();");
                    writer.WriteLine("funcTable.Dispose();");
                    using (writer.PushBlock("if (ownsDevice)"))
                    {
                        writer.WriteLine("Vulkan.DestroyDevice(device, allocationCallbacks);");
                    }
                }
            }
        }

        private static void GenerateExtension(VkRegistry registry, CsCodeGeneratorConfig config, string name, string outputPath)
        {
            var allowed = GatherAllowedTypes(registry).ToFrozenSet();

            WriteTypes(allowed, registry, config, outputPath);
        }

        private static void WriteFunctions(FunctionLevel level, CsSplitCodeWriter writer, VkRegistry registry, CsCodeGeneratorConfig config, FrozenSet<string> allowedFunctions, Dictionary<string, string> groupToEnumName, FunctionTableBuilder fnTableBuilder, params string[] modifiers)
        {
            StringBuilder sbDefault = new();

            foreach (var command in registry.Commands)
            {
                if (command.Proto == null) continue;
                if (command.Level != level) continue;
                string name = command.Proto.Name;
                if (command.Api != null && command.Api != ApiName) continue;
                if (!allowedFunctions.Contains(name)) continue;

                if (name.StartsWith("vk"))
                {
                    name = name[2..];
                }

                string returnType = ParseType(config, command.Proto);

                List<Parameter> parameters = [];

                for (int i = 0; i < command.Params.Count; i++)
                {
                    Param param = command.Params[i];
                    var paramType = ParseType(config, param);
                    var paramName = config.GetParameterName(i, param.Name);
                    var paramIsBool = paramType == "byte";

                    if (param.Group != null)
                    {
                        paramType = groupToEnumName[param.Group];
                    }

                    if (config.TypeMappings.TryGetValue(paramType, out var newType))
                    {
                        paramType = newType;
                    }

                    if (paramIsBool)
                    {
                        paramType = "bool";
                    }

                    parameters.Add(new(paramName, paramType));
                }

                var arrayParams = parameters.ToArray();

                bool isBool = returnType == "byte";

                int idx = fnTableBuilder.Add(command.Name);
                const string tableInstance = "funcTable";

                string commandComment = ""; //MakeComment(nameToComment, sbDefault, command);

                string paramSignature = MakeApiSignature(sbDefault, arrayParams);

                writer.WriteLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                writer.BeginBlock($"internal{(modifiers.Contains("static") ? " static" : "")} {returnType} {name}Native({paramSignature})");
                writer.WriteLine("#if NET5_0_OR_GREATER");
                writer.WriteLine(MakeDelegateInvokeSignatureFull(sbDefault, config, idx, tableInstance, returnType, arrayParams, false));
                writer.WriteLine("#else");
                writer.WriteLine(MakeDelegateInvokeSignatureFull(sbDefault, config, idx, tableInstance, returnType, arrayParams, true));
                writer.WriteLine("#endif");

                writer.EndBlock();
                writer.WriteLine();

                Overload overload = new(name, arrayParams, commandComment);
                WriteFunction(returnType, name, arrayParams, writer, [], overload, modifiers);

                logger.LogInfo($"defined {$"{(isBool ? "bool" : returnType)} {name}({paramSignature})"}");

                WriteVariations(writer, returnType, overload, modifiers);
            }
        }

        private static void WriteVariations(ICodeWriter writer, string returnType, Overload overload, string[] modifiers)
        {
            Parameter[] parameters = overload.Parameters;

            long mask = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameter parameter = parameters[i];
                var pointerDepth = parameter.Type.AsSpan().Count('*');
                if (parameter.Type == "void*" || parameter.Type == "byte*" || (pointerDepth == 1 && parameter.Type != "void*" && !parameter.IsOut))
                {
                    mask |= 1L << i;
                }
            }

            long maxVariations = (long)Math.Pow(2L, parameters.Length);
            HashSet<Overload> defs = [overload];
            for (long ix = 1; ix < maxVariations; ix++)
            {
                if ((ix & mask) == 0)
                {
                    continue;
                }

                Parameter[] stringVariation = new Parameter[parameters.Length];
                Parameter[] spanVariation = new Parameter[parameters.Length];
                Parameter[] refVariation = new Parameter[parameters.Length];
                Parameter[] voidVariation = new Parameter[parameters.Length];
                Parameter[] voidSpanVariation = new Parameter[parameters.Length];

                Overload stringOverload = overload.CreateVariation(stringVariation);
                Overload spanOverload = overload.CreateVariation(spanVariation);
                Overload refOverload = overload.CreateVariation(refVariation);
                Overload voidOverload = overload.CreateVariation(voidVariation);
                Overload voidSpanOverload = overload.CreateVariation(voidSpanVariation);

                for (int j = 0; j < parameters.Length; j++)
                {
                    var bit = (ix & 1 << j - 64) != 0;

                    Parameter parameter = parameters[j];

                    if (bit)
                    {
                        if (parameter.Type == "void*")
                        {
                            voidVariation[j] = new(parameter.Name, "nint") { IsVoid = true };
                            string genericType = $"T{parameter.Name.CapitalizeCopy()}"; // parameter names have to be unique so it's fine to use them as generic too with T prefix.
                            voidSpanOverload.Generics.Add(new(genericType, $"where {genericType} : unmanaged"));
                            voidSpanVariation[j] = new(parameter.Name, $"Span<{genericType}>") { IsGeneric = true, IsSpan = true, GenericType = genericType };
                        }
                        else
                        {
                            voidSpanVariation[j] = voidVariation[j] = parameter;
                        }

                        if (parameter.Type == "byte*")
                        {
                            stringVariation[j] = new(parameter.Name, "string") { IsString = true };
                            spanVariation[j] = new(parameter.Name, "ReadOnlySpan<byte>") { IsSpan = true };
                        }
                        else
                        {
                            spanVariation[j] = stringVariation[j] = parameter;
                        }

                        var pointerDepth = parameter.Type.AsSpan().Count('*');
                        if (pointerDepth == 1 && parameter.Type != "void*" && !parameter.IsOut)
                        {
                            var cleanName = parameter.Type.Replace("*", string.Empty);
                            refVariation[j] = new(parameter.Name, $"ref {cleanName}") { IsRef = true };
                            spanVariation[j] = new(parameter.Name, $"Span<{cleanName}>") { IsSpan = true };
                        }
                        else
                        {
                            refVariation[j] = parameter;
                        }
                    }
                    else
                    {
                        voidSpanVariation[j] = voidVariation[j] = refVariation[j] = spanVariation[j] = stringVariation[j] = parameter;
                    }
                }

                WriteFunction(returnType, overload.Name, parameters, writer, defs, stringOverload, modifiers);
                WriteFunction(returnType, overload.Name, parameters, writer, defs, spanOverload, modifiers);
                WriteFunction(returnType, overload.Name, parameters, writer, defs, refOverload, modifiers);
                WriteFunction(returnType, overload.Name, parameters, writer, defs, voidOverload, modifiers);
                WriteFunction(returnType, overload.Name, parameters, writer, defs, voidSpanOverload, modifiers);
            }
        }

        private static void WriteFunction(string returnType, string name, Parameter[] rootParameters, ICodeWriter writer, HashSet<Overload> defs, Overload variation, string[] modifiers)
        {
            foreach (var def in defs)
            {
                if (def.IsSame(variation))
                {
                    return;
                }
            }

            defs.Add(variation);

            StringBuilder sb = new();
            StringBuilder sbNameless = new();
            StringBuilder sbParam = new();
            bool first = true;

            int blocks = 0;
            int stringIdx = 0;
            List<(string name, string fixedName, string type, int index)> fixedStack = [];
            List<(string name, int index)> stringStack = [];
            for (int i = 0; i < variation.Parameters.Length; i++)
            {
                var parameter = variation[i];
                var paramName = parameter.Name;
                var paramType = parameter.Type;

                string paramListName = paramName;

                if (parameter.IsString)
                {
                    stringStack.Add((paramName, stringIdx));
                    paramListName = $"pStr{stringIdx}";
                    stringIdx++;
                }

                if (parameter.IsSpan || parameter.IsRef)
                {
                    if (parameter.IsGeneric)
                    {
                        paramListName = $"p{paramName.Replace("@", string.Empty)}{fixedStack.Count}";
                        fixedStack.Add((paramName, paramListName, $"{parameter.GenericType!}*", i));
                    }
                    else
                    {
                        paramListName = $"p{paramName.Replace("@", string.Empty)}{fixedStack.Count}";
                        fixedStack.Add((paramName, paramListName, rootParameters[i].Type, i));
                    }
                }

                if (!first)
                {
                    sb.Append(", ");
                    sbParam.Append(", ");
                    sbNameless.Append(", ");
                }
                first = false;

                if (paramType == "nint" && parameter.IsVoid)
                {
                    sbParam.Append($"(void*){paramListName}");
                }
                else
                {
                    sbParam.Append(paramListName);
                }

                sb.Append($"{paramType} {paramName}");
                sbNameless.Append(paramType);
            }

            string paramSignatureNameless = sbNameless.ToString();

            if (first)
            {
                sbNameless.Append(returnType);
            }
            else
            {
                sbNameless.Append($", {returnType}");
            }

            string delegateSig;
            if (returnType != "void")
            {
                delegateSig = $"{returnType} ret = {name}Native({sbParam});";
            }
            else
            {
                delegateSig = $"{name}Native({sbParam});";
            }

            bool isBool = returnType == "byte";

            string genericsString = variation.Generics.Count == 0 ? string.Empty : $"<{string.Join(", ", variation.Generics.Select(x => x.Name))}>";
            string genericsConstrain = variation.Generics.Count == 0 ? string.Empty : $" {string.Join(" ", variation.Generics.Select(x => x.Constrain))}";

            string functionSignatureApi = $"{(isBool ? "bool" : returnType)} {name}{genericsString}({sb}){genericsConstrain}";
            logger.LogInfo($"defined {functionSignatureApi}");

            string functionHeader = $"{string.Join(" ", modifiers)} {functionSignatureApi}";

            writer.WriteLines(variation.Comment);
            writer.BeginBlock(functionHeader);

            foreach (var str in stringStack)
            {
                MarshalHelper.WriteStringConvertToUnmanaged(writer, StringType, str.name, str.index);
            }

            foreach (var fx in fixedStack)
            {
                if (variation[fx.index].Type.Contains("ref"))
                {
                    writer.BeginBlock($"fixed ({fx.type} {fx.fixedName} = &{fx.name})");
                }
                else
                {
                    writer.BeginBlock($"fixed ({fx.type} {fx.fixedName} = {fx.name})");
                }

                blocks++;
            }

            writer.WriteLine(delegateSig);

            foreach (var str in stringStack)
            {
                MarshalHelper.WriteFreeString(writer, str.index);
            }

            if (returnType != "void")
            {
                if (isBool)
                {
                    writer.WriteLine("return ret != 0;");
                }
                else
                {
                    writer.WriteLine("return ret;");
                }
            }

            for (int i = 0; i < blocks; i++)
            {
                writer.EndBlock();
            }

            writer.EndBlock();
            writer.WriteLine();
        }

        private static string ParseType(CsCodeGeneratorConfig config, IVkType type)
        {
            type.Prefix = type.Prefix?.Replace("const", string.Empty)?.Trim()!;
            type.Type = type.Type?.Replace("const", string.Empty)?.Trim()!;
            type.Postfix = type.Postfix?.Replace("const", string.Empty)?.Trim()!;

            string? result = type.Type;

            {
                if (result != null)
                {
                    if (config.TypeMappings.TryGetValue(result, out var newType))
                    {
                        result = newType;
                    }

                    if (nameToDelegate.TryGetValue(result, out var dele))
                    {
                        result = dele.ToDelegatePointer();
                    }
                }

                if (type.Prefix == "void")
                {
                    result = "void";
                }
                else
                {
                    if (type.Prefix == null || type.Prefix == "const" || type.Prefix == "struct")
                    {
                        result = $"{result}{type.Postfix}";
                    }
                    else
                    {
                        result = $"{type.Prefix}{result}{type.Postfix}";
                    }
                }
            }

            result = result.Replace(" ", string.Empty);

            if (result == "float[2]") return "Vector2";
            if (result == "float[3]") return "Vector3";
            if (result == "float[4]") return "Vector4";

            return result;
        }

        private static string MakeInvokeSignatureFull(StringBuilder sb, string funcName, string returnType, Parameter[] parameters)
        {
            string delegateSigApi;
            if (returnType != "void")
            {
                delegateSigApi = $"{returnType} ret = {funcName}Native({MakeInvokeSignature(sb, parameters)});";
            }
            else
            {
                delegateSigApi = $"{funcName}Native({MakeInvokeSignature(sb, parameters)});";
            }

            return delegateSigApi;
        }

        private static string MakeInvokeSignature(StringBuilder sb, Parameter[] parameters)
        {
            sb.Clear();

            bool first = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameter param = parameters[i];
                var paramName = param.Name;

                if (!first)
                {
                    sb.Append(", ");
                }

                first = false;

                sb.Append(paramName);
            }

            string delegateSigApi = sb.ToString();

            sb.Clear();

            return delegateSigApi;
        }

        private static string MakeApiSignatureFull(StringBuilder sb, string funcName, string returnType, Parameter[] parameters)
        {
            bool isBool = returnType == "byte";

            string csSigApi = $"{(isBool ? "bool" : returnType)} {funcName}({MakeApiSignature(sb, parameters)})";

            return csSigApi;
        }

        private static string MakeApiSignature(StringBuilder sb, Parameter[] parameters)
        {
            sb.Clear();

            bool first = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameter param = parameters[i];
                var paramType = param.Type;
                var paramName = param.Name;

                if (!first)
                {
                    sb.Append(", ");
                }

                first = false;
                sb.Append($"{paramType} {paramName}");
            }

            string csSigApi = sb.ToString();

            sb.Clear();

            return csSigApi;
        }

        private static string MakeDelegateSignature(StringBuilder sb, CsCodeGeneratorConfig config, string returnType, Parameter[] parameters, bool compatibility)
        {
            sb.Clear();

            bool first = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameter param = parameters[i];
                var paramType = param.Type;
                var paramIsBool = paramType == "bool";

                if (paramIsBool)
                {
                    paramType = "bool";
                }

                if (!first)
                {
                    sb.Append(", ");
                }

                first = false;

                if (Config.DelegateTypes.Contains(paramType))
                {
                    sb.Append(compatibility ? "nint" : "void*");
                    continue;
                }

                if (paramIsBool)
                {
                    sb.Append("byte");
                    continue;
                }

                if (compatibility && paramType.Contains('*'))
                {
                    sb.Append("nint");
                    continue;
                }

                sb.Append(paramType);
            }

            if (compatibility && returnType == "bool")
            {
                returnType = config.GetBoolType();
            }
            if (compatibility && returnType.Contains('*'))
            {
                returnType = "nint";
            }

            if (first)
            {
                sb.Append(returnType);
            }
            else
            {
                sb.Append($", {returnType}");
            }

            string result = sb.ToString();

            sb.Clear();

            return result;
        }

        private static string MakeDelegateInvokeSignature(StringBuilder sb, Parameter[] parameters, bool compatibility)
        {
            sb.Clear();

            bool first = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameter param = parameters[i];
                var paramType = param.Type;
                var paramName = param.Name;
                var paramIsBool = paramType == "bool";

                if (!first)
                {
                    sb.Append(", ");
                }

                first = false;

                if (Config.DelegateTypes.Contains(paramType))
                {
                    sb.Append(compatibility ? $"Utils.GetFunctionPointerForDelegate({paramName})" : $"(void*)Utils.GetFunctionPointerForDelegate({paramName})");
                    continue;
                }

                if (paramIsBool)
                {
                    sb.Append($"*((byte*)(&{paramName}))");
                    continue;
                }

                if (compatibility && paramType.Contains('*'))
                {
                    sb.Append($"(nint){paramName}");
                    continue;
                }

                sb.Append(paramName);
            }

            string result = sb.ToString();

            sb.Clear();

            return result;
        }

        private static string MakeDelegateInvokeSignatureFull(StringBuilder sb, CsCodeGeneratorConfig config, int idx, string tableInstance, string returnType, Parameter[] parameters, bool compatibility)
        {
            if (returnType != "void")
            {
                if (compatibility)
                {
                    return $"return ({returnType})((delegate* unmanaged[Cdecl]<{MakeDelegateSignature(sb, config, returnType, parameters, true)}>){tableInstance}[{idx}])({MakeDelegateInvokeSignature(sb, parameters, compatibility)});";
                }
                else
                {
                    return $"return ((delegate* unmanaged[Cdecl]<{MakeDelegateSignature(sb, config, returnType, parameters, compatibility)}>){tableInstance}[{idx}])({MakeDelegateInvokeSignature(sb, parameters, compatibility)});";
                }
            }

            return $"((delegate* unmanaged[Cdecl]<{MakeDelegateSignature(sb, config, returnType, parameters, compatibility)}>){tableInstance}[{idx}])({MakeDelegateInvokeSignature(sb, parameters, compatibility)});";
        }

        public static void WriteStringConvertToUnmanagedWithSizeArray(ICodeWriter writer, string name, int i)
        {
            writer.WriteLine($"byte** pStrArray{i} = null;");
            writer.WriteLine($"int* pStrArraySizes{i} = null;");
            writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name}) + {name}.Length * sizeof(int);");
            using (writer.PushBlock($"if ({name} != null)"))
            {
                using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                {
                    writer.WriteLine($"pStrArraySizes{i} = (int*)Utils.Alloc<int>({name}.Length);");
                    writer.WriteLine($"pStrArray{i} = (byte**)Utils.Alloc<byte>(pStrArraySize{i});");
                }

                using (writer.PushBlock("else"))
                {
                    writer.WriteLine($"byte* pStrArraySizesStack{i} = stackalloc byte[{name}.Length * sizeof(int)];");
                    writer.WriteLine($"pStrArraySizes{i} = (int*)pStrArraySizesStack{i};");
                    writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                    writer.WriteLine($"pStrArray{i} = (byte**)pStrArrayStack{i};");
                }
            }

            using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
            {
                writer.WriteLine($"pStrArraySizes{i}[i] = Utils.GetByteCountUTF8({name}[i]);");
                writer.WriteLine($"pStrArray{i}[i] = (byte*)Utils.StringToUTF8Ptr({name}[i]);");
            }
        }

        public static void WriteFreeUnmanagedStringArrayWithSizeArray(ICodeWriter writer, string name, int i)
        {
            using (writer.PushBlock("for (int i = 0; i < " + name + ".Length; i++)"))
            {
                writer.WriteLine($"Utils.Free(pStrArray{i}[i]);");
            }

            using (writer.PushBlock($"if (pStrArraySize{i} >= Utils.MaxStackallocSize)"))
            {
                writer.WriteLine($"Utils.Free(pStrArray{i});");
                writer.WriteLine($"Utils.Free(pStrArraySizes{i});");
            }
        }

        private static void WriteConstants(VkRegistry registry, CsCodeGeneratorConfig config, string outputPath)
        {
            string constantsFolder = Path.Combine(outputPath, "Constants");
            if (Directory.Exists(constantsFolder))
            {
                Directory.Delete(constantsFolder, true);
            }
            Directory.CreateDirectory(constantsFolder);

            foreach (var group in registry.EnumGroups)
            {
                if (group.Type != "constants")
                {
                    continue;
                }

                var fileName = Path.Combine(constantsFolder, $"{group.Name}.cs");
                using CsCodeWriter writer = new(fileName, config.Namespace, GetUsings(config), null);
                GenerateConstantsFile(config, group, writer);
            }
        }

        private static List<string> GetUsings(CsCodeGeneratorConfig config)
        {
            List<string> usings =
            [
               "System", "System.Diagnostics", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime",
                .. config.Usings,
            ];
            return usings;
        }

        /*
                private static Dictionary<string, string> mapping = new Dictionary<string, string>()
                {
                    { "uint64_t", "ulong" },
                    { "uint32_t", "uint" },
                    { "uint8_t", "byte" },
                    { "int32_t", "int" },
                    { "char", "byte" }
                };
        */

        private static IEnumerable<string> GatherAllowedTypes(VkRegistry registry)
        {
            foreach (var feature in registry.Features)
            {
                if (feature.Name == "VKSC_VERSION_1_0") continue;
                foreach (var req in feature.Require)
                {
                    foreach (var type in req.Types)
                    {
                        yield return type.Name;
                    }
                }
            }
        }

        private static IEnumerable<string> GatherAllowedFunctions(VkRegistry registry)
        {
            foreach (var feature in registry.Features)
            {
                if (feature.Name == "VKSC_VERSION_1_0") continue;
                foreach (var req in feature.Require)
                {
                    foreach (var type in req.Commands)
                    {
                        yield return type.Name;
                    }
                }
            }
        }

        private static void WriteTypes(FrozenSet<string> allowedTypes, VkRegistry registry, CsCodeGeneratorConfig config, string output)
        {
            string structsFolder = Path.Combine(output, "Structs");
            if (Directory.Exists(structsFolder))
            {
                Directory.Delete(structsFolder, true);
            }
            Directory.CreateDirectory(structsFolder);

            string handleFolder = Path.Combine(output, "Handles");
            if (Directory.Exists(handleFolder))
            {
                Directory.Delete(handleFolder, true);
            }
            Directory.CreateDirectory(handleFolder);

            string delegatesFolder = Path.Combine(output, "Delegates");
            if (Directory.Exists(delegatesFolder))
            {
                Directory.Delete(delegatesFolder, true);
            }
            Directory.CreateDirectory(delegatesFolder);

            foreach (var type in registry.VkTypes.VkTypes)
            {
                if (!allowedTypes.Contains(type.Name)) continue;
                switch (type.Category)
                {
                    case "struct":
                        {
                            var fileName = Path.Combine(structsFolder, $"{type.Name}.cs");
                            using CsCodeWriter writer = new(fileName, config.Namespace, GetUsings(config), null);
                            WriteType(writer, type, config, false);
                            break;
                        }

                    case "handle":
                        {
                            var fileName = Path.Combine(handleFolder, $"{type.Name}.cs");
                            using CsCodeWriter writer = new(fileName, config.Namespace, GetUsings(config), null);
                            WriteHandle(writer, type, config);
                            break;
                        }
                    case "union":
                        {
                            var fileName = Path.Combine(structsFolder, $"{type.Name}.cs");
                            using CsCodeWriter writer = new(fileName, config.Namespace, GetUsings(config), null);
                            WriteType(writer, type, config, true);
                            break;
                        }
                    case "funcpointer":
                        {
                            var fileName = Path.Combine(delegatesFolder, $"{type.Name}.cs");
                            using CsCodeWriter writer = new(fileName, config.Namespace, GetUsings(config), null);
                            WriteDelegate(writer, type, config);
                            break;
                        }
                }
            }
        }

        private static void WriteDelegate(CsCodeWriter writer, VkType type, CsCodeGeneratorConfig config)
        {
            var firstPart = type.Parts[0];
            var span = firstPart.Content.AsSpan();
            if (!span.StartsWith("typedef "))
            {
                throw new NotSupportedException();
            }
            span = span["typedef ".Length..];
            int idx = span.IndexOf('(');
            if (idx == -1)
            {
                throw new NotSupportedException();
            }
            idx--;
            var returnType = span[..idx];

            StringBuilder sb = new();

            sb.Append('(');
            bool first = true;
            for (int i = 1; i < type.Parts.Count; i++)
            {
                var part = type.Parts[i];
                if (!part.IsType) continue;

                var paramType = part.Content;

                if (config.TypeMappings.TryGetValue(paramType, out var mapping))
                {
                    paramType = mapping;
                }

                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;

                sb.Append(paramType);

                i++;
                part = type.Parts[i];
                if (part.IsText)
                {
                    int closeIdx = part.Content.IndexOf(')');
                    if (closeIdx == -1)
                    {
                        closeIdx = part.Content.IndexOf(',');
                        if (closeIdx == -1)
                        {
                            closeIdx = part.Content.Length;
                        }
                    }

                    var sp = part.Content.AsSpan(0, closeIdx);

                    AppendParamPart(sb, sp);
                }
            }

            sb.Append(')');
            writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.Cdecl)]");
            writer.WriteLine($"public unsafe delegate {returnType} {type.Name}{sb};");
        }

        private static void AppendParamPart(StringBuilder sb, ReadOnlySpan<char> part)
        {
            bool first = true;

            foreach (var c in part)
            {
                if (char.IsWhiteSpace(c))
                {
                    first = true;
                    continue;
                }
                if (first && c != '*')
                {
                    sb.Append(' ');
                }
                first = false;
                sb.Append(c);
            }
        }

        private static void WriteHandle(CsCodeWriter writer, VkType type, CsCodeGeneratorConfig config)
        {
            writer.WriteLine("#if NET5_0_OR_GREATER");
            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            writer.WriteLine("#endif");
            using (writer.PushBlock($"public readonly partial struct {type.Name} : IEquatable<{type.Name}>"))
            {
                string handleType = "nint";
                string nullValue = "0";

                writer.WriteLine($"public {type.Name}({handleType} handle) {{ Handle = handle; }}");
                writer.WriteLine($"public {handleType} Handle {{ get; }}");
                writer.WriteLine($"public bool IsNull => Handle == 0;");

                writer.WriteLine($"public static {type.Name} Null => new {type.Name}({nullValue});");
                writer.WriteLine($"public static implicit operator {type.Name}({handleType} handle) => new {type.Name}(handle);");
                writer.WriteLine($"public static bool operator ==({type.Name} left, {type.Name} right) => left.Handle == right.Handle;");
                writer.WriteLine($"public static bool operator !=({type.Name} left, {type.Name} right) => left.Handle != right.Handle;");
                writer.WriteLine($"public static bool operator ==({type.Name} left, {handleType} right) => left.Handle == right;");
                writer.WriteLine($"public static bool operator !=({type.Name} left, {handleType} right) => left.Handle != right;");
                writer.WriteLine($"public bool Equals({type.Name} other) => Handle == other.Handle;");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override bool Equals(object obj) => obj is {type.Name} handle && Equals(handle);");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override int GetHashCode() => Handle.GetHashCode();");
                writer.WriteLine("#if NET5_0_OR_GREATER");
                writer.WriteLine($"private string DebuggerDisplay => string.Format(\"{type.Name} [0x{{0}}]\", Handle.ToString(\"X\"));");
                writer.WriteLine("#endif");
            }
        }

        private static readonly Dictionary<string, int> primitiveToSize = new()
        {
            { "byte", 1 },
            { "int", 4 },
            { "uint", 4 },
            { "float", 4 },
        };

        private static void WriteType(CsCodeWriter writer, VkType type, CsCodeGeneratorConfig config, bool isUnion)
        {
            if (isUnion)
            {
                writer.WriteLine("[StructLayout(LayoutKind.Explicit)]");
            }
            using (writer.PushBlock($"public unsafe struct {type.Name}"))
            {
                foreach (var member in type.Member)
                {
                    if (member.Api != null && member.Api != ApiName) continue;
                    var fieldName = config.GetFieldName(member.Name);
                    var fieldType = member.Type;
                    if (config.TypeMappings.TryGetValue(fieldType, out var mappingValue))
                    {
                        fieldType = mappingValue;
                    }
                    if (nameToDelegate.TryGetValue(fieldType, out var dele))
                    {
                        fieldType = dele.ToDelegatePointer();
                    }

                    string normalizedType = Normalize(fieldType, member);

                    if (member.IsArrayType)
                    {
                        if (primitiveToSize.ContainsKey(fieldType))
                        {
                            WriteArrayFixed(writer, member, fieldName, fieldType, isUnion);
                        }
                        else
                        {
                            WriteArrayUnroll(writer, member, fieldName, fieldType, isUnion);
                        }
                    }
                    else
                    {
                        if (isUnion)
                        {
                            writer.WriteLine("[FieldOffset(0)]");
                        }
                        writer.WriteLine($"public {normalizedType} {fieldName};");
                    }
                }
            }
        }

        private static void WriteArrayFixed(CsCodeWriter writer, Member member, string fieldName, string fieldType, bool isUnion)
        {
            StringBuilder sb = new();

            foreach (var arrayType in member.ArrayTypes)
            {
                sb.Append('[');
                if (int.TryParse(arrayType, out var count))
                {
                    sb.Append($"{count}");
                }
                else
                {
                    sb.Append($"{constantMap[arrayType]}");
                }
                sb.Append(']');
            }

            if (isUnion)
            {
                writer.WriteLine("[FieldOffset(0)]");
            }

            writer.WriteLine($"public fixed {fieldType} {fieldName}{sb};");
        }

        private static void WriteArrayUnroll(CsCodeWriter writer, Member member, string fieldName, string fieldType, bool isUnion)
        {
            int[] counts = new int[member.ArrayTypes.Count];
            for (int i = 0; i < member.ArrayTypes.Count; i++)
            {
                string arrayType = member.ArrayTypes[i];
                if (int.TryParse(arrayType, out var count))
                {
                    counts[i] = count;
                }
                else
                {
                    counts[i] = constantMap[arrayType];
                }
            }

            if (isUnion)
            {
                throw new Exception();
            }

            GenerateFields(writer, fieldType, fieldName, counts, 0, "");
        }

        private static void GenerateFields(CsCodeWriter writer, string fieldType, string fieldName, int[] counts, int dimension, string indexPrefix)
        {
            if (dimension == counts.Length)
            {
                writer.WriteLine($"public {fieldType} {fieldName}{indexPrefix};");
                return;
            }

            for (int i = 0; i < counts[dimension]; i++)
            {
                GenerateFields(writer, fieldType, fieldName, counts, dimension + 1, indexPrefix + "_" + i);
            }
        }

        private static string Normalize(string fieldType, Member member)
        {
            StringBuilder builder = new();

            if (member.PreText != null)
            {
                Filter(member.PreText, builder);
            }

            builder.Append(fieldType);
            if (member.PostText != null)
            {
                Filter(member.PostText, builder);
            }

            return builder.ToString();
        }

        private static void Filter(string text, StringBuilder builder)
        {
            foreach (var item in text.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                builder.Append(item.Replace("struct", string.Empty).Replace("const", string.Empty));
            }
        }

        private static Dictionary<string, string> GenerateEnums(CsCodeGeneratorConfig config, VkRegistry registry, string outputPath)
        {
            Dictionary<string, string> groupToEnumName = [];
            Dictionary<string, Enum> nameToEnum = [];

            foreach (var enumGroup in registry.EnumGroups)
            {
                foreach (var @enum in enumGroup.Items)
                {
                    nameToEnum.Add(@enum.Name, @enum);
                }
            }

            Dictionary<string, string?> groupToComment = [];
            foreach (var group in registry.EnumGroups)
            {
                if (group.Name == null) continue;
                groupToComment.Add(group.Name, config.WriteCsSummary(group.Comment));
            }

            Dictionary<string, string> nameToComment = [];
            //using var fs = File.OpenRead($"{apiTarget}.json");
            //nameToComment = JsonSerializer.Deserialize<Dictionary<string, string>>(fs)!;

            StringBuilder builder = new();
            foreach (var group in registry.EnumGroups)
            {
                if (group.Type == "constants")
                {
                    continue;
                }
                string enumName = config.GetCsCleanName(group.Name);
                EnumPrefix prefix = config.GetEnumNamePrefixEx(group.Name);
                Array.Resize(ref prefix.Parts, prefix.Parts.Length + 1);
                prefix.Parts[^1] = "Vk";

                groupToEnumName.Add(group.Name, enumName);

                groupToComment.TryGetValue(group.Name, out var comment);

                CsEnumMetadata metadata = new(group.Name, enumName, [], comment);
                metadata.BaseType = "uint";

                foreach (var item in group.Items)
                {
                    if (item.Deprecated == "aliased") continue;
                    string csEnumItemName = config.GetEnumName(item.Name, prefix);

                    if (nameToComment.TryGetValue(item.Name, out var itemCommentDict))
                    {
                        builder.AppendLine("/// <summary>");
                        builder.AppendLine($"/// {itemCommentDict}");
                        builder.AppendLine("/// </summary>");
                    }

                    builder.Append($"/// <remarks>");
                    if (item.SupportedVersionText != null)
                    {
                        builder.Append(item.SupportedVersionText);
                    }
                    if (item.UsedByExtensionsText != null)
                    {
                        if (item.SupportedVersionText != null)
                        {
                            builder.Append("<br/><br/>");
                        }
                        builder.Append(item.UsedByExtensionsText);
                    }

                    builder.AppendLine("</remarks>");
                    string itemComment = builder.ToString();
                    builder.Clear();

                    item.SummaryComment = itemComment;

                    string value;
                    if (item.Alias == null)
                    {
                        value = $"unchecked(({metadata.BaseType}){NormalizeValue(item.Value)})";
                    }
                    else
                    {
                        value = config.GetEnumName(item.Alias, prefix);
                    }

                    CsEnumItemMetadata csEnumItem = new(item.Name, item.Value, csEnumItemName, value, [], itemComment);

                    if (item.Deprecated != null)
                    {
                        csEnumItem.Attributes.Add($"[Obsolete(\"{item.Deprecated}\")]");
                    }

                    metadata.Items.Add(csEnumItem);
                }

                config.TypeMappings[group.Name] = enumName;

                config.CustomEnums.Add(metadata);
            }

            return groupToEnumName;
        }

        private static void GenerateConstantsFile(CsCodeGeneratorConfig config, Enums group, CsCodeWriter writer)
        {
            using (writer.PushBlock($"public static unsafe partial class {config.ApiName}"))
            {
                foreach (var item in group.Items)
                {
                    string constantName = config.GetConstantName(item.Name);

                    string baseType = item.Type!;

                    if (config.TypeMappings.TryGetValue(baseType, out var mappedType))
                    {
                        baseType = mappedType;
                    }

                    string rawValue = NormalizeValue(item.Value);
                    string value;
                    if (item.Alias == null)
                    {
                        value = $"unchecked(({baseType}){rawValue})";
                    }
                    else
                    {
                        value = config.GetConstantName(item.Alias);
                    }

                    if (baseType == "uint")
                    {
                        if (int.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                        {
                            constantMap.Add(constantName, val);
                        }

                        writer.WriteLine($"public const {baseType} {constantName} = {value};");
                        writer.WriteLine();
                    }
                }
            }
        }

        private static string NormalizeValue(string value)
        {
            return value.Replace("ULL", "UL");
        }

        private static readonly Regex WhitespaceRegex = CreateWhitespaceRegex();

        [GeneratedRegex(@"\s+")]
        private static partial Regex CreateWhitespaceRegex();
    }
}