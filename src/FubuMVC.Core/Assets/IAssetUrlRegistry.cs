namespace FubuMVC.Core.Assets
{
    using Core.Http;
    using Files;
    using FubuCore;

    public interface IAssetUrlRegistry
    {
        /// <summary>
        /// Resolve the url for a named asset
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        string UrlForAsset(AssetFolder? folder, string name);
        //why do we need the folder?
    }

    public class NulloAssetUrlRegistry : IAssetUrlRegistry
    {
        public string UrlForAsset(AssetFolder? folder, string name)
        {
            return "";
        }
    }

    public class AssetUrlRegistry : IAssetUrlRegistry
    {
        public static readonly string AssetsUrlFolder = "_content";

        private readonly ICurrentHttpRequest _httpRequest;

        public AssetUrlRegistry(ICurrentHttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        public string UrlForAsset(AssetFolder? folder, string name)
        {
            var relativeUrl = DetermineRelativeAssetUrl(folder, name);
            return _httpRequest.ToFullUrl(relativeUrl);
        }

        // TODO -- move the unit tests
        public static string DetermineRelativeAssetUrl(IAssetTagSubject subject)
        {
            var folder = subject.Folder;
            var name = subject.Name;

            return DetermineRelativeAssetUrl(folder, name);
        }

        // TODO -- move the unit tests
        public static string DetermineRelativeAssetUrl(AssetFolder? folder, string name)
        {
            return "{0}/{1}/{2}".ToFormat(AssetsUrlFolder, folder, name);
        }
    }

    public class CdnAssetUrlRegistry : IAssetUrlRegistry
    {
        public string UrlForAsset(AssetFolder? folder, string name)
        {
            return "";
        }
    }
}