using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MercadinhoRFID.Monitor.Driver
{
    public static class RegistryConfig
    {
        public static void Save(object that, string prefix = null)
        {
            var propertyAtt = GetPropertyAttributes(that);
            prefix = prefix ?? string.Empty;
            foreach (var propertyAttribute in propertyAtt)
            {
                var propertyInfo = propertyAttribute.Key;
                var value = propertyInfo.GetValue(that, null);
                RegistryAccess.Write(prefix + "." + propertyInfo.Name, value);
            }
        }

        public static void Load(object that, string prefix = null)
        {
            var propertyAtt = GetPropertyAttributes(that);
            prefix = prefix ?? string.Empty;
            foreach (var propertyAttribute in propertyAtt)
            {
                var propertyInfo = propertyAttribute.Key;
                var value = RegistryAccess.Read(prefix + "." + propertyInfo.Name, propertyInfo.PropertyType) ??
                            propertyAttribute.Value.Default;
                propertyInfo.SetValue(that, value, null);
            }
        }

        public static T New<T>()
        {
            var that = (T) Activator.CreateInstance(typeof(T));
            var propertyAtt = GetPropertyAttributes(that);
            foreach (var propertyAttribute in propertyAtt)
            {
                var propertyInfo = propertyAttribute.Key;
                var value = propertyAttribute.Value.Default;
                propertyInfo.SetValue(that, value, null);
            }
            return that;
        }

        private static Dictionary<PropertyInfo, RegistryPropertyAttribute> GetPropertyAttributes(object that)
        {
            return (from property in that.GetType().GetProperties()
                    let att = (RegistryPropertyAttribute)property.GetCustomAttributes(typeof(RegistryPropertyAttribute), false).FirstOrDefault()
                    where att != null
                    select new { property, att }).ToDictionary(p => p.property, p => p.att);
        }

        public static RegistryAccess RegistryAccess
        {
            get { return _registryAccess; }
            set { _registryAccess = value; }
        }
        private static RegistryAccess _registryAccess = new RegistryAccess();
    }

    public class RegistryPropertyAttribute : Attribute
    {
        public object Default { get; set; }
    }
}
