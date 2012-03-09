using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuMVC.Core.Assets.Tags
{
    public class AssetTagBuilder : IAssetTagBuilder
    {
        private readonly Cache<MimeType, Func<IAssetTagSubject, HtmlTag>>
            _builders = new Cache<MimeType, Func<IAssetTagSubject, HtmlTag>>();

        private readonly IMissingAssetHandler _missingHandler;
        private readonly IUrlRegistry _urls;

        public AssetTagBuilder(IMissingAssetHandler missingHandler, IUrlRegistry urls)
        {
            _missingHandler = missingHandler;
            _urls = urls;

            _builders[MimeType.Javascript] = (subject) =>
            {
                return new HtmlTag("script")
                    // http://stackoverflow.com/a/1288319/75194 
                    .Attr("type", "text/javascript")
                    .Attr("src", assetUrl(subject));
            };

            _builders[MimeType.Css] = (subject) =>
            {
                return new HtmlTag("link")
                    .Attr("href", assetUrl(subject))
                    .Attr("rel", "stylesheet")
                    .Attr("type", MimeType.Css.Value);
            };
        }


        /// <summary>
        /// Takes an AssetTagPlan and turns it into the right HtmlTags
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public IEnumerable<HtmlTag> Build(AssetTagPlan plan)
        {
            // This will happen when a user tries to request an asset set
            // with no assets -- think optional sets
            if (!plan.Subjects.Any())
            {
                return new HtmlTag[0];
            }

            var missingSubjects = plan.RemoveMissingAssets();
            var tagBuilder = _builders[plan.MimeType];
            Func<IAssetTagSubject, HtmlTag> builder = subject =>
            {
                return tagBuilder(subject);
            };

            var missingTags = _missingHandler.BuildTagsAndRecord(missingSubjects);
            var assetTags = plan.Subjects.Select(builder);
            return missingTags.Union(assetTags); 
        }

        string assetUrl(IAssetTagSubject subject)
        {
            //the asset tag subject is where I could maybe use a CDN marker
            //instead of a different AssetUrlBuilder
            return _urls.UrlForAsset(subject.Folder, subject.Name);
        }
    }
}