namespace Generator
{
    using System.Collections.Generic;

    public class Overload : IEquatable<Overload?>
    {
        public string Name;
        public Parameter[] Parameters;
        public List<GenericParameter> Generics = [];
        public bool CallApi;
        public string Comment;

        public Overload(string name, Parameter[] parameters, string comment)
        {
            Name = name;
            Parameters = parameters;
            Comment = comment;
        }

        public Overload CreateVariation(Parameter[] parameters)
        {
            return new(Name, parameters, Comment);
        }

        public Parameter this[int index]
        {
            get => Parameters[index];
            set => Parameters[index] = value;
        }

        public bool IsSame(Overload other)
        {
            if (Name != other.Name) return false;
            bool matches = true;
            for (int i = 0; i < Parameters.Length; i++)
            {
                var p0 = Parameters[i];
                var p1 = other[i];
                if (p0.Type != p1.Type)
                {
                    matches = false;
                    break;
                }
            }

            return matches;
        }

        public override int GetHashCode()
        {
            HashCode hc = new();
            hc.Add(Name);
            for (int i = 0; i < Parameters.Length; i++)
            {
                hc.Add(Parameters[i].GetHashCode());
            }
            return hc.ToHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Overload);
        }

        public bool Equals(Overload? other)
        {
            return other is not null &&
                   Name == other.Name &&
                   EqualityComparer<Parameter[]>.Default.Equals(Parameters, other.Parameters);
        }

        public static bool operator ==(Overload? left, Overload? right)
        {
            return EqualityComparer<Overload>.Default.Equals(left, right);
        }

        public static bool operator !=(Overload? left, Overload? right)
        {
            return !(left == right);
        }
    }
}