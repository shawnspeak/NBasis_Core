using Microsoft.Extensions.Configuration;

namespace NBasis.S3ConfigSource
{
    public class S3ConfigurationProvider : ConfigurationProvider
    {
        readonly Amazon.S3.IAmazonS3 _client;
        readonly string _bucket;
        readonly string _key;

        public S3ConfigurationProvider(Amazon.S3.IAmazonS3 client, string bucket, string key)
        {
            _client = client;
            _bucket = bucket;
            _key = key;
        }
        

        public override void Load()
        {
            var parser = new JsonParser();
            var stream = _client.GetObjectStreamAsync(_bucket, _key, null).GetAwaiter().GetResult();
            Data = parser.Parse(stream);
        }        
    }
}
