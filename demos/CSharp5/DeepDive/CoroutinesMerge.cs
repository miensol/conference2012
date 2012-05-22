using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DeepDive
{
    public static class CoroutinesMerge
    {
         public static IEnumerable<TValue> MergeCoroutines<TValue>(
             this IEnumerable<TValue> leftSequence, 
             IEnumerable<TValue> rightSequence )
             where TValue : IComparable<TValue>
         {
             var mergeCoordintator = new MergeCoordinator<TValue>(leftSequence, rightSequence);
             return mergeCoordintator.GetResult();
         }
    }


    public class MergeCoordinator<TValue>
        where TValue : IComparable<TValue>
    {
        private readonly IEnumerable<TValue> _leftSequence;
        private readonly IEnumerable<TValue> _rightSequence;
        private readonly FifoCoordinator _coroutinesQueue;


        public MergeCoordinator(IEnumerable<TValue> leftSequence, IEnumerable<TValue> rightSequence)
        {
            _leftSequence = leftSequence;
            _rightSequence = rightSequence;
            _coroutinesQueue = new FifoCoordinator();
        }


        public IEnumerable<TValue> GetResult()
        {
            var result = new List<TValue>();
            BeginMerge(_leftSequence, result);
            BeginMerge(_rightSequence, result);
            _coroutinesQueue.Flush();
            return result;
        }

        private TValue OtherSequenceCurrent { get; set; }

        private bool HasSmallestValue { get; set; }

        private async void BeginMerge(IEnumerable<TValue> sequence, ICollection<TValue> result)
        {
            foreach (var value in sequence)
            {
                if(HasSmallestValue == false || value.CompareTo(OtherSequenceCurrent) > 0)
                {                                        
                    await OtherSeqeunce(value);
                }
                result.Add(value);
            }

        }

        private OneTimeAwaiter OtherSeqeunce(TValue currentSequenceValue)
        {
            OtherSequenceCurrent =  currentSequenceValue;
            HasSmallestValue = true;
            return _coroutinesQueue.AtEndOfQueue();
        }
    }

   

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

    public class CoroutinesMergeTest : BaseMergeSequenceTest
    {
        public override IEnumerable<int> Merge(IEnumerable<int> leftSequence, IEnumerable<int> rightSequence)
        {
            return leftSequence.MergeCoroutines(rightSequence);
        }
    }
}