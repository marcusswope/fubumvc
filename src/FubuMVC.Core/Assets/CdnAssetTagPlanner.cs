namespace FubuMVC.Core.Assets
{
    using System.Collections.Generic;
    using Combination;
    using Files;
    using FubuCore;
    using Runtime;
    using System.Linq;

    public class CdnAssetTagPlanner : IAssetTagPlanner
    {
        readonly IAssetPipeline _pipeline;

        public CdnAssetTagPlanner(IAssetPipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public AssetTagPlan BuildPlan(MimeType mimeType, IEnumerable<string> names)
        {
            var plan = new AssetTagPlan(mimeType);
            plan.AddSubjects(FindSubjects(names));

            validateMatchingMimetypes(mimeType, plan, names);

            if (plan.Subjects.Count == 1)
            {
                return plan;
            }

            //combinations

            return plan;
        }

        public AssetTagPlan BuildPlan(AssetPlanKey key)
        {
            return BuildPlan(key.MimeType, key.Names);
        }

        public IEnumerable<IAssetTagSubject> FindSubjects(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var file = _pipeline.Find(name);
                if (file != null)
                {
                    yield return file;
                }
                else
                {
                    yield return new MissingAssetTagSubject(name);
                }
            }
        }

        private static void validateMatchingMimetypes(MimeType mimeType, AssetTagPlan plan, IEnumerable<string> names)
        {
            if (plan.Subjects.Any(x => x.MimeType != mimeType))
            {
                var message = "Files {0} have mixed mimetype's".ToFormat(names.Join(", "));
                throw new MixedMimetypeException(message);
            }
        }
    }
}