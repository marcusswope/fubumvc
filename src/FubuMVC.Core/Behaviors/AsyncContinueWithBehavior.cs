using System;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncContinueWithBehavior<T> : AsyncContinueWithBehavior where T : class
    {
        public AsyncContinueWithBehavior(IFubuRequest fubuRequest) : base(fubuRequest)
        {
        }

        protected override void InnerInvoke(Action<IActionBehavior> behaviorAction)
        {
            var task = FubuRequest.Get<Task<T>>();
            if(task.IsCompleted)
			{
				onComplete(task, behaviorAction);
				return;
			}
            task.ContinueWith(x =>
            {
            	onComplete(x, behaviorAction);
            }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);
        }
		
		private void onComplete(Task<T> task, Action<IActionBehavior> behaviorAction)
		{
			if(task.IsFaulted || task.Exception != null)
				return;
				
			FubuRequest.Set(task.Result);
			behaviorAction(InsideBehavior);
		}
    }

    public class AsyncContinueWithBehavior : IActionBehavior
    {
        private readonly IFubuRequest _fubuRequest;

        public AsyncContinueWithBehavior(IFubuRequest fubuRequest)
        {
            _fubuRequest = fubuRequest;
        }

        public IFubuRequest FubuRequest { get { return _fubuRequest; } }
        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            InnerInvoke(x => x.Invoke());
        }

        public void InvokePartial()
        {
            InnerInvoke(x => x.InvokePartial());
        }

        protected virtual void InnerInvoke(Action<IActionBehavior> behaviorAction)
        {
            var task = FubuRequest.Get<Task>();
            task.ContinueWith(x =>
            {
            	if(x.IsFaulted || x.Exception != null)
					return;
				
                if (InsideBehavior != null)
                    behaviorAction(InsideBehavior);
            }, TaskContinuationOptions.AttachedToParent);
        }
    }
}