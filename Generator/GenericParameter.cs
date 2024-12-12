namespace Generator
{
    public class GenericParameter
    {
        public string Name;
        public string Constrain;

        public GenericParameter(string name, string constrain)
        {
            Name = name;
            Constrain = constrain;
        }
    }
}