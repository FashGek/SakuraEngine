namespace Sakura.AssetPipeline.Tests
{
    using Sakura.Service;
    using System.Collections.Generic;
    using Xunit;
    using Xunit.Abstractions;

    public class LocalManifestTests
    {
        private readonly ITestOutputHelper Output;

        public LocalManifestTests(ITestOutputHelper testOutputHelper)
        {
            Output = testOutputHelper;
        }

        public class AssetType
        {
            public string TypeName { get; }
            public string[] ValidExtensionNames { get; }
        }

        [Fact]
        public void TestInitialize_ShouldBe_Success()
        {
            var MetaString = localManifest.QueryMeta("PATH");
            Assert.True(MetaString is not null);
            Output.WriteLine(MetaString);
        }

        LocalManifest localManifest = new LocalManifest("EngineAssets", "Library");
    }
}