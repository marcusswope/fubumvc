namespace FubuMVC.Core.Assets
{
    using Files;

    public class CdnExtension : IFubuRegistryExtension
    {
        string _root;

        public void SetRoot(string path)
        {
            _root = path;
        }

        public void Configure(FubuRegistry registry)
        {
            registry.Services(s=>
            {
                s.ReplaceService<IAssetPipeline, CdnAssetPipeline>();
                s.ReplaceService<IAssetUrlBuilder>(new CdnAssetUrlBuilder(_root)); //new concept
            });
        }
    }
}