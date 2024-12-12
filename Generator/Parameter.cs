namespace Generator
{
    public struct Parameter : IEquatable<Parameter>
    {
        public string Name;
        public string Type;
        public bool IsOut;
        public bool IsVoid;
        public bool IsGeneric;
        public bool IsSpan;
        public bool IsRef;
        public bool IsString;
        public string? GenericType;

        public Parameter(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Parameter parameter && Equals(parameter);
        }

        public readonly bool Equals(Parameter other)
        {
            return Name == other.Name &&
                   Type == other.Type;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Name, Type);
        }

        public static bool operator ==(Parameter left, Parameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Parameter left, Parameter right)
        {
            return !(left == right);
        }
    }
}