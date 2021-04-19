using CountryChatbotExercise.Core.Models.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CountryChatbotExercise.Core.Helpers
{
    internal static class RestCountryPropertyInfoProvider
    {
        private static readonly IEnumerable<PropertyInfo> Properties = CollectProperties();

        private static readonly Random Random = new Random();

        public static IEnumerable<(string PropertyName, string Value)> GetRandomProperties(RestCountry country, int amount)
        {
            return Properties
                .OrderBy(prop => Random.Next())
                .Take(amount)
                .Select(prop => (prop.Name, prop.GetValue(country).ToString()))
                .ToArray();
        }

        private static IEnumerable<PropertyInfo> CollectProperties()
        {
            var type = typeof(RestCountry);
            PropertyInfo[] ignoredProperties = { type.GetProperty(nameof(RestCountry.Name)), type.GetProperty(nameof(RestCountry.Flag)) };
            return type.GetProperties()
                .Where(prop => prop.PropertyType == typeof(string) || prop.PropertyType == typeof(int))
                .Except(ignoredProperties);
        }
    }
}
