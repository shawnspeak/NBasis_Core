using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace NBasis.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IConfiguration NBasisConfig(this IConfiguration configuration)
        {
            var nbasisSection = configuration.GetSection("NBasis");
            if (nbasisSection != null)
                return nbasisSection;

            return new EmptyConfiguration();
        }

        public static string Get(this IConfiguration configuration, string key)
        {
            return Get(configuration, key, null);
        }

        public static string Get(this IConfiguration configuration, string key, string defaultValue)
        {
            if (configuration == null) return defaultValue;
            try
            {
                return configuration[key] ?? defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static int GetInt(this IConfiguration configuration, string key, int defaultValue = 0)
        {
            int val;
            return Int32.TryParse(Get(configuration, key), out val) ? val : defaultValue;
        }

        static string[] _truths = new string[] { "true", "1", "yes" };

        public static bool GetBool(this IConfiguration configuration, string key, bool defaultValue)
        {
            return _truths.Contains(Get(configuration, key, defaultValue.ToString()).ToLower());
        }
    }
}
