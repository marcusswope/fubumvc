using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
    using Core.Assets;

    [TestFixture]
    public class UrlRegistryTester
    {
        [Test]
        public void determine_asset_url_simple()
        {
            var file = new AssetFile("jquery.forms.js")
            {
                Folder = AssetFolder.scripts
            };

            AssetUrlRegistry.DetermineRelativeAssetUrl(file)
                .ShouldEqual("_content/scripts/jquery.forms.js");
        }

        [Test]
        public void determine_asset_url_complex()
        {
            var file = new AssetFile("shared/jquery.forms.js")
            {
                Folder = AssetFolder.scripts
            };

            AssetUrlRegistry.DetermineRelativeAssetUrl(file)
                .ShouldEqual("_content/scripts/shared/jquery.forms.js");
        }

        [Test]
        public void determine_asset_url_respects_absolute_path()
        {
            var currentHttpRequest = new StubCurrentHttpRequest{TheApplicationRoot = "http://server"};
            var registry = new UrlRegistry(null, null, 
                currentHttpRequest,
                new AssetUrlRegistry(currentHttpRequest));
            registry.UrlForAsset(AssetFolder.images, "icon.png")
                .ShouldEqual("http://server/_content/images/icon.png");
        }
    }
}