using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DevConsole;
using Tools.DevConsole.Commands;
using Tools.DevConsole.Exceptions;

namespace Tools.DevConsole.Tests
{
    public class ArgumentListTests
    {
        List<CommandArgument> Arguments => new List<CommandArgument>()
        {
            new CommandArgument("bool_arg",false,"",typeof(bool)),
            new CommandArgument("int_arg",false,"",typeof(int)),
            new CommandArgument("string_arg",false,"",typeof(string)),
            new CommandArgument("float_arg",false,"",typeof(float)),
            new CommandArgument("null_arg",false,"",null),
            new CommandArgument("required_arg",true,"",null)
        };

        [Test]
        public void ArgumentCouldNotBeFound_SHOULD_ThrowsAnException()
        {
            // ARRANGE
            string command = "arg1 value1";

            //ASSERT
            Assert.Throws<UnknowArgument>(() =>
            {
                //PERFORM
                var argList = new ArgumentsList(command.Split(' ').ToArray(), Arguments);
            });
        }

        [Test]
        public void NoParametersIsProvided_SHOULD_ThrowsException()
        {
            // ARRANGE
            string command = "required_arg string_arg";

            //ASSERT
            Assert.Throws<MissingParameter>(() =>
            {
                //PERFORM
                var argList = new ArgumentsList(command.Split(' ').ToArray(), Arguments);
            });
        }

        [Test]
        public void ParameterCanNotBeParsed_SHOULD_ThrowsException()
        {
            // ARRANGE
            string command = "float_arg string_value required_arg";

            //ASSERT
            Assert.Throws<IncorrectParse>(() =>
            {
                //PERFORM
                var argList = new ArgumentsList(command.Split(' ').ToArray(), Arguments);
                float value = argList["float_arg"];
            });
        }

        [Test]
        public void RequiredArgumentIsNotProvided_SHOULD_ThrowsException()
        {
            // ARRANGE
            string command = "";
            
            //ASSERT
            Assert.Throws<MissingRequiredArgument>(() =>
            {
                //PERFORM
                var argList = new ArgumentsList(command.Split(' ').ToArray(), Arguments);
            });
        }
    }
}
