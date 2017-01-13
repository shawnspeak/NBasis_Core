using Microsoft.Extensions.Configuration;

namespace NBasis.S3ConfigSource
{
    public class S3ConfigurationSource : IConfigurationSource
    {
        readonly Amazon.S3.IAmazonS3 _client;
        readonly string _bucket;
        readonly string _key;

        public S3ConfigurationSource(Amazon.S3.IAmazonS3 client, string bucket, string key)
        {
            _client = client;
            _bucket = bucket;
            _key = key;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new S3ConfigurationProvider(_client, _bucket, _key);
        }
    }
}
