using Microsoft.Extensions.Configuration;
using System;

namespace NBasis.S3ConfigSource
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddS3Config(this IConfigurationBuilder config, string bucket, string key, Amazon.S3.IAmazonS3 client = null)
        {
            return config.Add(new S3ConfigurationSource(client ?? new Amazon.S3.AmazonS3Client(), bucket, key));
        }
    }
}
