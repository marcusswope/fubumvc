using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Packaging;

namespace FubuMVC.Core.Runtime.Files
{
    public class FubuApplicationFiles : IFubuApplicationFiles
    {
        private readonly Cache<string, IFubuFile> _files;

        private readonly Lazy<IEnumerable<ContentFolder>> _folders
            = new Lazy<IEnumerable<ContentFolder>>(ContentFolder.FindAllContentFolders);

        public FubuApplicationFiles()
        {
            _files = new Cache<string, IFubuFile>(findFile);
        }

        public IEnumerable<ContentFolder> Folders
        {
            get { return _folders.Value; }
        }

        // I'm okay with this finding nulls

        public IEnumerable<IFubuFile> FindFiles(FileSet fileSet)
        {
            fileSet.AppendExclude(FubuMvcPackageFacility.FubuContentFolder + "/*.*");

            return _folders.Value.SelectMany(folder => folder.FindFiles(fileSet)).Where(x => !x.RelativePath.Contains(FubuMvcPackageFacility.FubuContentFolder));
        }

        public IFubuFile Find(string relativeName)
        {
            return _files[relativeName.ToLower()];
        }

        public IEnumerable<ContentFolder> AllFolders
        {
            get { return _folders.Value; }
        }

        private IFubuFile findFile(string name)
        {
            var fileSet = new FileSet{
                DeepSearch = true,
                Include = name.ToLower()
            };

            return FindFiles(fileSet).FirstOrDefault();
        }
    }
}