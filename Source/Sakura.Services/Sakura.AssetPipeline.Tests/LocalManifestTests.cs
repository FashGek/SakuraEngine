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
            var res = ServiceProgram.Invoke<object, AssetType[]>("SakuraAsset", "ListTypes", null);
            Assert.True(res is not null);
        }
    }
}