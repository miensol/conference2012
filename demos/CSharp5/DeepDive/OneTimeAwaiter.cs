using System;

namespace DeepDive
{
    public class OneTimeAwaiter : AwaiterVoidBase<OneTimeAwaiter>
    {
        private static Action NoOp = () => { };
        private Action _continuation = NoOp;
        private bool _isCompleted;

        public override bool IsCompleted { get { return _isCompleted; } }

        public override void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }
        
        public void Resume()
        {
            _isCompleted = true;
            _continuation();    
        }
    }
}