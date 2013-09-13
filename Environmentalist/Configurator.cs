using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Castle.Components.DictionaryAdapter;

[assembly : InternalsVisibleTo("Environmentalist.Tests")]

namespace Environmentalist
{
    public static class Configurator
    {
        public static TConfig Create<TConfig>(object @default)
        {
            var propertyNames = Ancillary.GetPropertyNames(typeof (TConfig));
            var dict = Ancillary.BuildDictionary(propertyNames, @default);
            return new DictionaryAdapterFactory().GetAdapter<TConfig>(dict);
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class FromEnvironmentAttribute : Attribute
    {
        private readonly string _variableName;

        public FromEnvironmentAttribute(string variableName)
        {
            _variableName = variableName;
        }

        public string VariableName { get { return _variableName; } }
    }

    internal static class Ancillary
    {
        public static IDictionary<string, string> GetPropertyNames(Type type)
        {
            return type.GetMembers().
                        Select(member =>
                               new
                                   {
                                       Member = member,
                                       Attribute = (FromEnvironmentAttribute)
                                   member.GetCustomAttributes(typeof (FromEnvironmentAttribute), false)
                                         .SingleOrDefault()
                                   }).
                        Where(ma => ma.Attribute != null).
                        ToDictionary(ma => ma.Member.Name, ma => ma.Attribute.VariableName);
        }

        private static string GetPropertyValue(this object src, string propertyName)
        {
            return (string) src.GetType().GetProperty(propertyName).GetValue(src);
        }

        public static IDictionary BuildDictionary(IDictionary<string, string> propertyNames, object defaultConfig)
        {
            return propertyNames.
                Select(pair => new
                    {
                        Property = pair.Key,
                        Value =
                                   Environment.GetEnvironmentVariable(pair.Value) ??
                                   defaultConfig.GetPropertyValue(pair.Key)
                    }).
                ToDictionary(pv => pv.Property, pv => pv.Value);
        }
    }
}
