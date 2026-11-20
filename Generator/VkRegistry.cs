namespace Generator
{
    using ClangSharp;
    using HexaGen;
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [XmlRoot("registry")]
    public class VkRegistry
    {
        private static readonly XmlSerializer serializer = new(typeof(VkRegistry));

        [XmlArray("commands")]
        [XmlArrayItem("command")]
        public List<VkCommand> Commands { get; set; } = new List<VkCommand>();

        [XmlElement("enums")]
        public List<Enums> EnumGroups { get; set; } = new List<Enums>();

        [XmlElement("feature")]
        public List<Feature> Features { get; set; } = new List<Feature>();

        [XmlElement("extensions")]
        public Extensions Extensions { get; set; } = new();

        [XmlElement("types")]
        public Types VkTypes { get; set; } = new();

        public static VkRegistry LoadFromFile(string file)
        {
            using var fs = File.OpenRead(file);
            return (VkRegistry)serializer.Deserialize(fs)!;
        }
    }

    [XmlRoot(ElementName = "types")]
    public class Types
    {
        [XmlAttribute(AttributeName = "comment")]
        public string Comment { get; set; }

        [XmlElement(ElementName = "type")]
        public List<VkType> VkTypes { get; set; }
    }

    [XmlRoot(ElementName = "type")]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class VkType : IXmlSerializable
    {
        [XmlAttribute(AttributeName = "category")]
        public string Category { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "alias")]
        public string? Alias { get; set; }

        [XmlAttribute(AttributeName = "structextends")]
        public string? StructExtends { get; set; }

        [XmlAttribute(AttributeName = "requires")]
        public string? Requires { get; set; }

        [XmlElement(ElementName = "member")]
        public List<Member> Member { get; set; } = [];

        public string? Type { get; set; }

        [XmlText]
        public string Text { get; set; }

        public List<ParsePart> Parts { get; set; } = [];

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Category = reader.GetAttribute("category")!;
            Name = reader.GetAttribute("name")!;
            Alias = reader.GetAttribute("alias");
            Requires = reader.GetAttribute("requires");
            StructExtends = reader.GetAttribute("structextends");

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("type");
                return;
            }

            StringBuilder sb = new();
            int depth = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    Parts.Add(new ParsePart(true, false, false, reader.Value));
                    sb.Append(reader.Value);
                }

                while (reader.NodeType == XmlNodeType.Element && reader.Name == "member")
                {
                    Member member = new();
                    member.ReadXml(reader);
                    Member.Add(member);
                }

                if (depth == 0 && reader.NodeType == XmlNodeType.Element && reader.Name == "name")
                {
                    reader.ReadStartElement();
                    Name = reader.ReadContentAsString();
                    Parts.Add(new ParsePart(false, true, false, Name));
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "type")
                {
                    reader.ReadStartElement();
                    depth++;
                    Type = reader.ReadContentAsString();

                    Parts.Add(new ParsePart(false, false, true, Type));
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "type")
                {
                    depth--;
                    if (depth == -1)
                    {
                        break;
                    }
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException();
        }

        private string GetDebuggerDisplay()
        {
            return Name;
        }
    }

    public struct ParsePart
    {
        public bool IsText;
        public bool IsName;
        public bool IsType;
        public string Content;

        public ParsePart(bool isText, bool isName, bool isType, string content)
        {
            IsText = isText;
            IsName = isName;
            IsType = isType;
            Content = content;
        }
    }

    [XmlRoot(ElementName = "member")]
    public class Member : IXmlSerializable
    {
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "values")]
        public string? Values { get; set; }

        public string? Api { get; set; }

        public string? Comment { get; set; }

        public string PostText { get; set; }

        public string PreText { get; set; }

        [XmlAttribute(AttributeName = "optional")]
        public string? Optional { get; set; }

        public bool IsArrayType { get; set; }

        public List<string> ArrayTypes { get; set; } = [];

        public bool IsBitfield { get; set; }

        public int BitSize { get; set; }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Optional = reader.GetAttribute("optional");
            Values = reader.GetAttribute("values");
            Api = reader.GetAttribute("api");

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("member");
                return;
            }

            bool arrayCapture = false;
            StringBuilder sb = new();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    if (Type == null)
                    {
                        PreText = reader.Value;
                    }
                    else
                    {
                        PostText += reader.Value;
                    }

                    // Process character by character (not per text-node) so a dimension's size
                    // still parses correctly when it comes from a child <enum> element, which
                    // splits what would otherwise be one contiguous "[A][B]" text run across
                    // several Text nodes (e.g. "[", "A", "][", "B", "]").
                    foreach (char c in reader.Value)
                    {
                        if (c == '[')
                        {
                            IsArrayType = true;
                            arrayCapture = true;
                            sb.Clear();
                        }
                        else if (c == ']')
                        {
                            ArrayTypes.Add(sb.ToString());
                            sb.Clear();
                            arrayCapture = false;
                        }
                        else if (arrayCapture)
                        {
                            sb.Append(c);
                        }
                    }
                }

                if (arrayCapture)
                {
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "type")
                {
                    reader.ReadStartElement();
                    Type = reader.ReadContentAsString();
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "name")
                {
                    reader.ReadStartElement();
                    Name = reader.ReadContentAsString();
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "comment")
                {
                    reader.ReadStartElement();
                    Comment = reader.ReadContentAsString();
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "member")
                {
                    break;
                }
            }

            int index = PostText?.IndexOf(':') ?? -1;

            if (index != -1)
            {
                IsBitfield = true;
                BitSize = int.Parse(PostText.AsSpan(index + 1));
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException();
        }
    }

    public enum FunctionLevel
    {
        Global,
        Instance,
        Device,
        Unknown
    }

    [XmlRoot(ElementName = "command")]
    public class VkCommand : IXmlSerializable
    {
        [XmlElement(ElementName = "proto")]
        public Proto Proto { get; set; }

        [XmlElement(ElementName = "param")]
        public List<Param> Params { get; set; }

        [XmlElement(ElementName = "alias")]
        public Alias Alias { get; set; }

        [XmlAttribute(AttributeName = "successcodes")]
        public string? SuccessCodes { get; set; }

        [XmlAttribute(AttributeName = "errorcodes")]
        public string? ErrorCodes { get; set; }

        [XmlAttribute(AttributeName = "api")]
        public string? Api { get; set; }

        [XmlIgnore]
        public string Name { get => Proto?.Name ?? AltName!; set => Proto.Name = value; }

        [XmlIgnore]
        public string? AltAlias { get; set; }

        [XmlIgnore]
        public string? AltName { get; set; }

        public VkCommand? AliasCommand { get; set; }

        public static readonly Dictionary<string, FunctionLevel> ExplicitMap = new()
        {
            { "vkCreateInstance", FunctionLevel.Global },
            { "vkEnumerateInstanceVersion", FunctionLevel.Global },
            { "vkEnumerateInstanceLayerProperties", FunctionLevel.Global },
            { "vkEnumerateInstanceExtensionProperties", FunctionLevel.Global },
            { "vkGetInstanceProcAddr", FunctionLevel.Global },
            { "vkGetDeviceProcAddr", FunctionLevel.Global },
            { "vkDestroyInstance", FunctionLevel.Global },
            { "vkDestroyDevice", FunctionLevel.Global }
        };

        public FunctionLevel Level
        {
            get
            {
                if (ExplicitMap.TryGetValue(Name, out var level))
                {
                    return level;
                }

                if (AliasCommand != null)
                {
                    return AliasCommand.Level;
                }

                if (Params == null || Params.Count == 0)
                {
                    return FunctionLevel.Unknown;
                }

                string firstParamType = Params[0].Type;

                return firstParamType switch
                {
                    "VkInstance" or "VkPhysicalDevice" => FunctionLevel.Instance,
                    "VkDevice" or "VkQueue" or "VkCommandBuffer" => FunctionLevel.Device,
                    _ => FunctionLevel.Unknown,
                };
            }
        }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            AltAlias = reader.GetAttribute("alias");
            AltName = reader.GetAttribute("name");
            SuccessCodes = reader.GetAttribute("successcodes");
            ErrorCodes = reader.GetAttribute("errorcodes");
            Api = reader.GetAttribute("api");

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("command");
                return;
            }

            int targetDepth = reader.Depth + 1;
            while (reader.Read())
            {
                if (targetDepth == reader.Depth && reader.NodeType == XmlNodeType.Element && reader.Name == "proto")
                {
                    Proto = new();
                    Proto.ReadXml(reader);
                }

                while (targetDepth == reader.Depth && reader.NodeType == XmlNodeType.Element && reader.Name == "param")
                {
                    Param param = new();
                    param.ReadXml(reader);
                    Params ??= [];
                    Params.Add(param);
                }

                if (targetDepth == reader.Depth && reader.NodeType == XmlNodeType.Element && reader.Name == "alias")
                {
                    Alias = new();
                    Alias.ReadXml(reader);
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "command")
                {
                    break;
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotSupportedException();
        }
    }

    [XmlRoot(ElementName = "alias")]
    public class Alias : IXmlSerializable
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute("name")!;

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("alias");
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "alias")
                {
                    break;
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    [XmlRoot(ElementName = "proto")]
    public class Proto : IXmlSerializable, IVkType
    {
        public string Kind { get; set; }

        public string Prefix { get; set; }

        public string Type { get; set; }

        public string Postfix { get; set; }

        public string Name { get; set; }

        public List<ParsePart> Parts { get; set; } = [];

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Kind = reader.GetAttribute("kind");

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("proto");
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    if (Type == null)
                    {
                        Prefix = reader.Value.Trim();
                    }
                    else
                    {
                        Postfix = reader.Value.Trim();
                    }
                    Parts.Add(new ParsePart(true, false, false, reader.Value));
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "type")
                {
                    reader.ReadStartElement();
                    Type = reader.ReadContentAsString();
                    Parts.Add(new ParsePart(false, false, true, Type));
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "name")
                {
                    reader.ReadStartElement();
                    Name = reader.ReadContentAsString();
                    Parts.Add(new ParsePart(false, true, false, Name));
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "proto")
                {
                    break;
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            // not important for us.
            throw new NotSupportedException("");
        }
    }

    [XmlRoot(ElementName = "param")]
    public class Param : IXmlSerializable, IVkType
    {
        public string Group { get; set; }

        public string Kind { get; set; }

        public string Len { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public string Postfix { get; set; }

        public string? Api { get; set; }

        public List<bool> Optional { get; set; } = [];

        public List<ParsePart> Parts { get; set; } = [];

        public XmlSchema? GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Group = reader.GetAttribute("group")!;
            Kind = reader.GetAttribute("kind")!;
            Len = reader.GetAttribute("len")!;
            Api = reader.GetAttribute("api");
            var optionalAttr = reader.GetAttribute("optional");
            if (optionalAttr != null)
            {
                Optional.AddRange(optionalAttr.Split(',').Select(bool.Parse));
            }

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("param");
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    if (Type == null)
                    {
                        Prefix = reader.Value.Trim();
                    }
                    else
                    {
                        Postfix = reader.Value.Trim();
                    }
                    Parts.Add(new ParsePart(true, false, false, reader.Value));
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "type")
                {
                    reader.ReadStartElement();
                    Type = reader.ReadContentAsString();
                    Parts.Add(new ParsePart(false, false, true, Type));
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "name")
                {
                    reader.ReadStartElement();
                    Name = reader.ReadContentAsString();
                    Parts.Add(new ParsePart(false, true, false, Name));
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "param")
                {
                    break;
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            // not important for us.
            throw new NotSupportedException("");
        }
    }

    [XmlRoot(ElementName = "type")]
    public class Type
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "comment")]
        public string Comment { get; set; }

        [XmlAttribute(AttributeName = "requires")]
        public string Requires { get; set; }

        [XmlElement(ElementName = "apientry")]
        public object Apientry { get; set; }
    }

    [XmlRoot(ElementName = "unused")]
    public class Unused
    {
        [XmlAttribute(AttributeName = "start")]
        public string Start { get; set; }

        [XmlAttribute(AttributeName = "end")]
        public string End { get; set; }

        [XmlAttribute(AttributeName = "comment")]
        public string Comment { get; set; }
    }

    [XmlRoot(ElementName = "enums")]
    public class Enums
    {
        [XmlElement(ElementName = "enum")]
        public List<Enum> Items { get; set; }

        [XmlElement(ElementName = "unused")]
        public Unused Unused { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "comment")]
        public string? Comment { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "enum")]
    public class Enum : IVersionedMember
    {
        private string group;

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string? Type { get; set; }

        [XmlAttribute(AttributeName = "comment")]
        public string? Comment { get; set; }

        [XmlAttribute(AttributeName = "bitpos")]
        public string? Bitpos
        {
            get
            {
                if (int.TryParse(Value, out int parsedValue))
                {
                    // Calculate the bit position if the value is a power of 2
                    return (parsedValue > 0 && (parsedValue & (parsedValue - 1)) == 0)
                        ? Math.Log2(parsedValue).ToString()
                        : null;
                }
                return null;
            }
            set
            {
                if (int.TryParse(value, out int parsedBitpos))
                {
                    // Derive the value based on the bit position
                    Value = (1 << parsedBitpos).ToString();
                }
                else
                {
                    Value = null; // Invalid bit position
                }
            }
        }

        [XmlAttribute(AttributeName = "alias")]
        public string? Alias { get; set; }

        [XmlAttribute(AttributeName = "deprecated")]
        public string? Deprecated { get; set; }

        [XmlIgnore]
        public string? SummaryComment { get; set; }

        [XmlIgnore]
        public List<string> GroupList { get; } = [];

        [XmlIgnore]
        public HashSet<string> SupportedVersions { get; } = [];

        [XmlIgnore]
        public string? SupportedVersionText { get; set; }

        [XmlIgnore]
        public HashSet<string> UsedByExtensions { get; } = [];

        [XmlIgnore]
        public string? UsedByExtensionsText { get; set; }

        [XmlIgnore]
        public bool IsInGLEnum { get; set; }
    }

    [XmlRoot(ElementName = "enum")]
    public class FeatureEnum
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }

        [XmlAttribute(AttributeName = "alias")]
        public string? Alias { get; set; }

        // Present when this entry adds a value to an existing enum group (e.g. a KHR extension
        // adding a VkStructureType member) instead of requiring an already-defined one.
        [XmlAttribute(AttributeName = "extends")]
        public string? Extends { get; set; }

        [XmlAttribute(AttributeName = "offset")]
        public string? Offset { get; set; }

        [XmlAttribute(AttributeName = "extnumber")]
        public string? ExtNumber { get; set; }

        [XmlAttribute(AttributeName = "dir")]
        public string? Dir { get; set; }

        [XmlAttribute(AttributeName = "bitpos")]
        public string? Bitpos { get; set; }

        [XmlAttribute(AttributeName = "deprecated")]
        public string? Deprecated { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [XmlRoot(ElementName = "require")]
    public class Require
    {
        [XmlElement(ElementName = "enum")]
        public List<FeatureEnum> Enums { get; set; }

        [XmlElement(ElementName = "command")]
        public List<FeatureCommand> Commands { get; set; }

        [XmlElement(ElementName = "type")]
        public List<FeatureType> Types { get; set; }

        [XmlAttribute(AttributeName = "profile")]
        public string Profile { get; set; }

        [XmlAttribute(AttributeName = "comment")]
        public string Comment { get; set; }

        public override string ToString()
        {
            return Comment;
        }
    }

    [XmlRoot(ElementName = "type")]
    public class FeatureType
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [XmlRoot(ElementName = "command")]
    public class FeatureCommand
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [XmlRoot(ElementName = "feature")]
    public class Feature
    {
        [XmlElement(ElementName = "require")]
        public List<Require> Require { get; set; }

        [XmlElement(ElementName = "remove")]
        public List<Remove> Remove { get; set; }

        [XmlAttribute(AttributeName = "api")]
        public string Api { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "number")]
        public string Number { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [XmlRoot(ElementName = "extension")]
    public class Extension
    {
        private string? supported;

        [XmlElement(ElementName = "require")]
        public List<Require> Require { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "number")]
        public string? Number { get; set; }

        // Presence of either attribute (even with an empty value, e.g. deprecatedby="") means
        // the extension is deprecated/obsoleted, regardless of what it names as the replacement.
        [XmlAttribute(AttributeName = "deprecatedby")]
        public string? DeprecatedBy { get; set; }

        [XmlAttribute(AttributeName = "obsoletedby")]
        public string? ObsoletedBy { get; set; }

        [XmlIgnore]
        public bool IsDeprecated => DeprecatedBy != null || ObsoletedBy != null;

        [XmlAttribute(AttributeName = "supported")]
        public string? Supported
        {
            get => supported;
            set
            {
                supported = value;
                SupportedList.Clear();
                if (value != null)
                {
                    SupportedList.AddRange(value.Split(','));
                }
            }
        }

        [XmlIgnore]
        public List<string> SupportedList { get; set; } = [];

        public override string ToString()
        {
            return Name;
        }
    }

    [XmlRoot(ElementName = "extensions")]
    public class Extensions
    {
        [XmlElement(ElementName = "extension")]
        public List<Extension> Extension { get; set; }
    }

    [XmlRoot(ElementName = "remove")]
    public class Remove
    {
        [XmlElement(ElementName = "enum")]
        public List<FeatureEnum> Enums { get; set; }

        [XmlElement(ElementName = "command")]
        public List<FeatureCommand> Commands { get; set; }

        [XmlAttribute(AttributeName = "profile")]
        public string Profile { get; set; }

        [XmlAttribute(AttributeName = "comment")]
        public string Comment { get; set; }
    }
}