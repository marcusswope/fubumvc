namespace FubuMVC.Core.Assets
{
    using Files;
    using Tags;

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
                //s.ReplaceService<IAssetTagPlanner, CdnAssetTagPlanner>();
                s.ReplaceService<IAssetUrlRegistry>(new CdnAssetUrlRegistry(_root));
                s.ReplaceService<IAssetTagBuilder, CdnAssetTagBuilder>();
                
            });
        }
    }
}