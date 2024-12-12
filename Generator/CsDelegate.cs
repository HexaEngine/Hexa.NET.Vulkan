namespace Generator
{
    using HexaGen;
    using Irony.Parsing.Construction;
    using System.Collections.Generic;
    using System.Text;

    public class CsDelegate
    {
        public CsDelegate(string name, string returnType, List<Parameter> parameters)
        {
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
        }

        public string Name { get; set; }

        public string ReturnType { get; set; }

        public List<Parameter> Parameters { get; set; }

        public static CsDelegate Parse(VkType type, CsCodeGeneratorConfig config)
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

            List<Parameter> parameters = [];

            for (int i = 1; i < type.Parts.Count; i++)
            {
                var part = type.Parts[i];
                if (!part.IsType) continue;

                var paramType = part.Content;

                if (config.TypeMappings.TryGetValue(paramType, out var mapping))
                {
                    paramType = mapping;
                }

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
                    parameters.Add(ParseParameter(paramType, sp));
                }
            }

            return new CsDelegate(type.Name, returnType.ToString(), parameters);
        }

        private static Parameter ParseParameter(string paramType, ReadOnlySpan<char> sp)
        {
            StringBuilder typeBuilder = new(paramType);

            int i;
            for (i = 0; i < sp.Length; i++)
            {
                char c = sp[i];
                if (char.IsWhiteSpace(c) || char.IsLetter(c))
                {
                    break;
                }
                typeBuilder.Append(c);
            }
            sp = sp[i..];

            return new Parameter(sp.ToString(), typeBuilder.ToString());
        }

        public string ToDelegateDefinition()
        {
            StringBuilder sb = new();
            sb.Append($"delegate {ReturnType} {Name}(");
            bool first = true;
            foreach (Parameter p in Parameters)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;
                sb.Append($"{p.Type} {p.Name}");
            }
            sb.Append(");");
            return sb.ToString();
        }

        public string ToDelegatePointer()
        {
            //delegate* unmanaged[Cdecl]<DateTime, void> @delegate;

            StringBuilder sb = new();

            sb.Append("delegate* unmanaged[Cdecl]<");
            bool first = true;
            foreach (Parameter p in Parameters)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;
                sb.Append(p.Type);
            }
            if (!first)
            {
                sb.Append(", ");
            }
            sb.Append(ReturnType);
            sb.Append('>');
            return sb.ToString();
        }
    }
}