using System;
using System.Linq;

namespace Tools.DevConsole.Commands
{
    public class CommandOption
    {
        public char? Alias { get; }
        public string Name { get; }
        public string Description { get; }
        public Type ParameterType { get; }

        public CommandOption(char? alias, string name, string description, Type type)
        {
            if (name.Length < 2 || name.Length > 12)
                throw new Exception("Option names must be between 2 and 12 characters in length");

            Alias = alias;
            Name = name;
            Description = description;
            ParameterType = type;

            Type[] allowedTypes = { typeof(string), typeof(int), typeof(float), typeof(bool) };

            if (type != null && !allowedTypes.Contains(type))
                throw new Exception($"The type '{type.Name}' is not allowed for a parameter! Only {string.Join(",", allowedTypes.Select(x => x.Name))} are allowed.");

        }

        public override string ToString()
         {
            var str = $"-{Alias},--{Name}".PadRight(12);
            
            if (ParameterType != null)
                str += $"<{ParameterType.Name}>";

            str += $" {Description}";

            return str;
        }

    }
}
