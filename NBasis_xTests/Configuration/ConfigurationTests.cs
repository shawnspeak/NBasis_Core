using Microsoft.Extensions.Configuration;
using NBasis.S3ConfigSource;
using Xunit;

namespace NBasis_xTests.Configuration
{
    public class ConfigurationTests
    {
        [Fact]
        public void ConfigFromS3Registered()
        {
            var builder = new ConfigurationBuilder()
                                .AddS3Config("kfconfigtest", "test/config1.json", new Amazon.S3.AmazonS3Client(Amazon.RegionEndpoint.USWest2));
            var config = builder.Build();

            var section1 = config.GetSection("Section1");
            Assert.NotNull(section1);

            Assert.Equal(section1["Value1"], "False");
            Assert.Equal(section1["Value2"], "stringvalue");
            Assert.Equal(section1["Value3"], "123");

            var section2 = config.GetSection("Section2");
            Assert.NotNull(section2);

            var subsection1 = section2.GetSection("SubSection1");
            Assert.NotNull(subsection1);

            Assert.Equal(subsection1["SubValue1"], "test");
        }
    }
}
