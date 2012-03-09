namespace FubuMVC.Core.Assets.Tags
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Combination;
    using HtmlTags;
    using Runtime;

    public class CdnAssetTagBuilder : IAssetTagBuilder
    {
        private readonly FubuCore.Util.Cache<MimeType, Func<IAssetTagSubject, HtmlTag>>
            _builders = new FubuCore.Util.Cache<MimeType, Func<IAssetTagSubject, HtmlTag>>();

        readonly IMissingAssetHandler _missingHandler;

        public CdnAssetTagBuilder(IMissingAssetHandler missingHandler)
        {
            _missingHandler = missingHandler;
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

        public IEnumerable<HtmlTag> Build(AssetTagPlan plan)
        {
            if(!plan.Subjects.Any())
            {
                return new HtmlTag[0];
            }

            var missingSubjects = plan.RemoveMissingAssets();
            var tagBuilder = _builders[plan.MimeType];
            Func<IAssetTagSubject, HtmlTag> converter = subject =>
            {
                return tagBuilder(subject);
            };

            var missingTags = _missingHandler.BuildTagsAndRecord(missingSubjects);
            var tags = plan.Subjects.Select(converter);
            return missingTags.Union(tags);
        }

        string assetUrl(IAssetTagSubject subject)
        {
            return "/cdn/" + subject.Folder + "/" + subject.Name;
        }
    }
}