using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DevConsole.Commands;
using Tools.DevConsole.Exceptions;

namespace Tools.DevConsole.Tests
{
    public class OptionListTests
    {
        public List<CommandOption> Options => new List<CommandOption>()
        {
            new CommandOption('b', "bool_opt"   , "", typeof(bool)),
            new CommandOption('i', "int_opt"    , "", typeof(int)),
            new CommandOption('s', "string_opt" , "", typeof(string)),
            new CommandOption('f', "float_opt"  , "", typeof(float)),
            new CommandOption('n', "null_opt"   , "", null)
        };

        [Test]
        public void OptionIsProvidedButCommandDoNotHasOne_SHOULD_ThrowsAnException()
        {
            // ARRANGE
            string command = "-s value";

            // ASSERT
            Assert.Throws<ExceededOptions>(() =>
            {
                // ACT
                var optList = new OptionsList(command.Split(' ').ToArray(), null, out string[] remain);
            });
        }

        [Test]
        public void OptionCouldNotFound_SHOULD_ThrowsAnException()
        {
            // ARRANGE
            string command = "-z value";

            // ASSERT
            Assert.Throws<UnknowOption>(() =>
            {
                // ACT
                var optList = new OptionsList(command.Split(' ').ToArray(), Options, out string[] remain);
            });

        }
        [Test]
        public void NoParametersIsProvided_SHOULD_ThrowsAnException()
        {
            // ARRANGE
            string command = "test -f";
            
            // ASSERT
            Assert.Throws<MissingParameter>(() =>
            {
                // ACT
                var optList = new OptionsList(command.Split(' ').ToArray(), Options, out string[] remain);
            });
        }

        [Test]
        public void ParameterCanNotBeParsed_SHOULD_ThrowsAnException()
        {
            // ARRANGE
            string command = "-f value";

            // ASSERT
            Assert.Throws<IncorrectParse>(() =>
            {
                // ACT
                var optList = new OptionsList(command.Split(' ').ToArray(), Options, out string[] remain);

                var value = optList["float_opt"];
            });
        }

        [Test]
        public void GroupIsProvided_SHOULD_ReturnListWithAll()
        {
            // ARRANGE
            string command = "-nfs 2.3 placeholder";

            // ACT
            var optList = new OptionsList(command.Split(' ').ToArray(), Options, out string[] remain);

            //ASSERT
            Assert.AreEqual(3, optList.Count);

        }

        [Test]
        public void GroupIsProvidedWithParametersNullFloatString_SHOULD_ListWithRespectivelyTypes()
        {
            // ARRANGE
            string command = "-nfs 2.3 placeholder";

            // ACT
            var optList = new OptionsList(command.Split(' ').ToArray(), Options, out string[] remain);

            // ASSERT
            Assert.AreEqual(true, optList["null_opt"]);
            Assert.AreEqual(2.3f, optList["float_opt"]);
            Assert.AreEqual("placeholder", optList["string_opt"]);
        }

        [Test]
        public void GroupIsProvidedAndSingleFlagsToo_SHOULD_ReturnListWithAll()
        {
            // ARRANGE
            string command = "-nfs 2.3 placeholder -b true -i 36";

            // ACT
            var optList = new OptionsList(command.Split(' ').ToArray(), Options, out string[] remain);

            //ASSERT
            Assert.AreEqual(5, optList.Count);
        }


        [Test]
        public void OptionsAndOtherElementsIsProvided_SHOULD_ExportStringArrayWithoutOptions()
        {
            // ARRANGE
            string command = "-nfs 2.3 placeholder arg1 -b true arg2 -i 36 arg3";

            // ACT
            new OptionsList(command.Split(' ').ToArray(), Options, out string[] remain);

            //ASSERT
            Assert.AreEqual(3, remain.Length);
            Assert.AreEqual("arg1", remain[0]);
            Assert.AreEqual("arg2", remain[1]);
            Assert.AreEqual("arg3", remain[2]);
        }

    }
}
