using System;
using System.Linq;

namespace Tools.DevConsole.Commands
{
    public class CommandArgument
    {
        public string Name { get; }
        public bool Required { get; }
        public string Description { get; }
        public Type ParameterType { get; }

        public CommandArgument(string Name, bool required, string description, Type type)
        {
            this.Name = Name;
            Required = required;
            this.Description = description;
            ParameterType = type;

            Type[] allowedTypes = {
                typeof(string),
                typeof(int),
                typeof(float),
                typeof(bool)
            };

            if (type != null && !allowedTypes.Contains(type))
                throw new Exception($"The type '{type.Name}' is not allowed for a parameter! Only {string.Join(",", allowedTypes.Select(x => x.Name))} are allowed.");

        }

        public override string ToString()
        {
            var str = $"{$"{GetFormatedName()}".PadRight(16)} - {Description}";
            return str;
        }

        public string GetFormatedName()
        {
            var st = Name;

            if (ParameterType != null)
                st += $" <{ParameterType.Name.ToLower()}>";

            if (!Required)
                st = $"[{st}]";

            return st;
        }
    }
}
