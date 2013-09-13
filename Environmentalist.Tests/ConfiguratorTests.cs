using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace Environmentalist.Tests
{
    [TestFixture]
    public class ConfiguratorTests
    {
        public interface ITestConfig
        {
            [FromEnvironment("COOL_PROPERTY")]
            string CoolProperty { get; }
        }

        private class DefaultTestConfig : ITestConfig
        {
            public string CoolProperty
            {
                get { return "lol"; }
            }
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable("COOL_PROPERTY", null);
        }

        [Test]
        public void Create_should_get_a_valid_Config()
        {
            Environment.SetEnvironmentVariable("COOL_PROPERTY", "rofl");

            var config = Configurator.Create<ITestConfig>(new DefaultTestConfig());

            Assert.That(config.CoolProperty, Is.EqualTo("rofl"));
        }

        [Test]
        public void Create_should_get_a_valid_Config_from_defaults()
        {
            var config = Configurator.Create<ITestConfig>(new DefaultTestConfig());

            Assert.That(config.CoolProperty, Is.EqualTo("lol"));
        }

        [Test]
        public void BuildDictionary_should_delegate_to_default()
        {
            Environment.SetEnvironmentVariable("COOL_PROPERTY", null);
            var propertyNames = new Dictionary<string, string> {{"CoolProperty", "COOL_PROPERTY"}};

            var obj = Ancillary.BuildDictionary(propertyNames, new DefaultTestConfig());

            Assert.That(obj["CoolProperty"], Is.EqualTo("lol"));
        }

        [Test]
        public void BuildDictionary_should_get_stuff_from_environment()
        {
            Environment.SetEnvironmentVariable("COOL_PROPERTY", "rofl");
            var propertyNames = new Dictionary<string, string> {{"CoolProperty", "COOL_PROPERTY"}};

            var obj = Ancillary.BuildDictionary(propertyNames, new DefaultTestConfig());

            Assert.That(obj["CoolProperty"], Is.EqualTo("rofl"));
        }

        [Test]
        public void GetPropertyNames_should_get_correct_dict_for_test_config()
        {
            var propertyNames = Ancillary.GetPropertyNames(typeof (ITestConfig));
            
            Assert.That(propertyNames.Count(), Is.EqualTo(1));
            Assert.That(propertyNames["CoolProperty"], Is.EqualTo("COOL_PROPERTY"));
        }
    }
}
