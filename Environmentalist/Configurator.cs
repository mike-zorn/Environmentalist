using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.Components.DictionaryAdapter;
using Newtonsoft.Json;

[assembly : InternalsVisibleTo("Environmentalist.Tests")]

namespace Environmentalist
{
    public static class Configurator
    {
        public static TConfig Create<TConfig>(object @default)
        {
            var configType = typeof (TConfig);
            var propertyNames = Ancillary.GetPropertyNames(configType);
            var dict = Ancillary.BuildDictionary(propertyNames, @default, configType);
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

        private static object DeserializePropertyValue(string value, string propertyName, Type containingType)
        {
            var propertyType = GetPropertyType(propertyName, containingType);
            if (value == null)
            {
                return null;
            }
            else if (propertyType  == typeof (string))
            {
                return value;
            }
            else
            {
                return JsonConvert.DeserializeObject(value, propertyType);
            }
        }

        private static Type GetPropertyType(string propertyName, Type containingType)
        {
            return containingType.GetMember(propertyName).
                                  Where(member =>
                                        member.MemberType == MemberTypes.Property).
                                  Cast<PropertyInfo>().
                                  Select(property => property.PropertyType).
                                  Single();
        }

        private static object GetPropertyValue(this object src, string propertyName)
        {
            return src.GetType().GetProperty(propertyName).GetValue(src);
        }

        public static IDictionary BuildDictionary(IDictionary<string, string> propertyNames, object defaultConfig, Type type)
        {
            return propertyNames.
                Select(pair => new
                    {
                        Property = pair.Key,
                        Value =
                                   DeserializePropertyValue(Environment.GetEnvironmentVariable(pair.Value), pair.Key, type) ??
                                   defaultConfig.GetPropertyValue(pair.Key)
                    }).
                ToDictionary(pv => pv.Property, pv => pv.Value);
        }
    }
}
