namespace FubuMVC.Core.Assets.Files
{
    using System.Collections.Generic;

    public class CdnAssetPipeline : IAssetPipeline
    {
        string _cdnRoot = "/Content";
        public AssetFile Find(string path)
        {
            var file = new AssetFile(path);
            file.FullPath = path;
            //need to set full path?
            return file;
        }


        public AssetPath AssetPathOf(AssetFile file)
        {
            throw new System.NotImplementedException();
        }

        public AssetFile FindByPath(string path)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AssetFile> AllFiles()
        {
            //in the base model this returns all known files in all packages
            return new AssetFile[0];
        } 
        public AssetFile Find(AssetPath path)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<PackageAssets> AllPackages
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}