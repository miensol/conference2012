using System;
using System.Runtime.CompilerServices;

namespace DeepDive
{
    public abstract class AwaiterVoidBase<TAwaiter> : INotifyCompletion
    {
        public virtual void GetResult(){}
        public abstract bool IsCompleted { get; }
        public abstract void OnCompleted(Action continuation);
        public TAwaiter GetAwaiter()
        {
            object awaiter = this;
            return (TAwaiter)awaiter;
        }
    }
}