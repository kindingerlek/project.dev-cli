using System;
using System.Collections.Generic;
using Tools.DevConsole.Exceptions;

namespace Tools.DevConsole
{
    internal static class Helper
    {
        public static dynamic Parse(string value, Type type)
        {
            try
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                var result = converter.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, value);

                return result;
            }
            catch (Exception)
            {
                throw new IncorrectParse($"The value '{value}' could not be parsed to '{type.Name}'");
            }
        }

        public static bool IsAnOption(string value)
        {
            return value.StartsWith("-") || value.StartsWith("--");
        }

        public static string[] GetCommandParts(string commandString)
        {
            LinkedList<char> parmChars = new (commandString.ToCharArray());
            bool inQuote = false;
            var node = parmChars.First;

            while (node != null)
            {

                if (node.Value == '"')
                {
                    inQuote = !inQuote;

                    var nextNode = node.Next;
                    parmChars.Remove(node);

                    if (nextNode == null)
                        break;

                    node = nextNode;
                }

                if (!inQuote && node.Value == ' ')
                    node.Value = '\n';

                if (!inQuote && node.Value != ' ')
                    node.Value = char.ToLower(node.Value);

                node = node.Next;
            }

            char[] parmCharsArr = new char[parmChars.Count];
            parmChars.CopyTo(parmCharsArr, 0);
            return (new string(parmCharsArr)).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
