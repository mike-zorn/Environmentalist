using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace Environmentalist.Tests
{
    [TestFixture]
    public class StringConfiguratorTests
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

    }
}
