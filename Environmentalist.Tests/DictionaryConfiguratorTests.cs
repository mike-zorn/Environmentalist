using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Environmentalist.Tests
{
    public class DictionaryConfiguratorTests
    {
        public interface ITestConfig
        {
            [FromEnvironment("COOL_DICTIONARY")]
            IDictionary<string, string> CoolDictionary { get; }
        }

        private class DefaultTestConfig : ITestConfig
        {
            public IDictionary<string, string> CoolDictionary
            {
                get { return Dictionary; }
            }
        }

        private static readonly IDictionary<string, string> Dictionary = new Dictionary<string, string>();
        
        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable("COOL_DICTIONARY", null);
        }

        [Test]
        public void Create_should_get_a_valid_Config()
        {
            Environment.SetEnvironmentVariable("COOL_DICTIONARY", @"{
                ""foo"": ""bar""
            }");

            var config = Configurator.Create<ITestConfig>(new DefaultTestConfig());

            Assert.That(config.CoolDictionary["foo"], Is.EqualTo("bar"));
            Assert.That(config.CoolDictionary.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_should_throw_an_exception_on_malformed_json()
        {
            Environment.SetEnvironmentVariable("COOL_DICTIONARY", @"{[]asdaah: sad}Dda");

            Assert.Throws<JsonReaderException>(() => Configurator.Create<ITestConfig>(new DefaultTestConfig()));
        }

        [Test]
        public void Create_should_get_a_valid_Config_from_defaults()
        {
            var config = Configurator.Create<ITestConfig>(new DefaultTestConfig());

            Assert.That(config.CoolDictionary, Is.EqualTo(Dictionary));
        }
    }
}
