using FubuMVC.Core.Continuations;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.HelloPartial;
using FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesPartial;

namespace FubuMVC.IntegrationTesting.ViewEngines.Spark.PartialNoLayout.Features.UsesTransferTo
{
    public class TransferToController
    {
         public FubuContinuation Tranfer(TransferToRequest request)
         {
             return FubuContinuation.TransferTo(new HelloPartialInputModel());
         }

        public TransferToModel Render()
        {
            return new TransferToModel();
        }
    }

    public class TransferToModel
    {
    }

    public class TransferToRequest  
    {
    }

    public class RedirectToController
    {
         public FubuContinuation Redirect()
         {
             return FubuContinuation.RedirectTo<UsesPartialController>(x => x.Execute());
         }
    }
}