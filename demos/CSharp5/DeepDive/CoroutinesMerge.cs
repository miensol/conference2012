using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeepDive
{
    public static class CoroutinesMerge
    {
        public static IEnumerable<TValue> MergeWithCoroutines<TValue>(
             this IEnumerable<TValue> leftSequence, 
             IEnumerable<TValue> rightSequence )
             where TValue : IComparable<TValue>
         {
             var fifo = new FifoCoordinator();
             var currentValueInOrder = default(TValue);
             var result = new List<TValue>();
             Action<bool,IEnumerable<TValue>> mergeCouroutine = async (isFirst,sequence) =>
             {
                 foreach (var value in sequence)
                 {
                     if(isFirst || value.CompareTo(currentValueInOrder) > 0)
                     {
                         currentValueInOrder = value;                         
                         await fifo.AtEndOfQueue();
                     }
                     result.Add(value);
                 }                                    
             };
             mergeCouroutine(true, leftSequence);
             mergeCouroutine(false, rightSequence);
             fifo.Flush();
             return result;
         }

        public static IEnumerable<TValue> MergeWithCoroutinesLazy<TValue>(
            this IEnumerable<TValue> leftSequence, 
            IEnumerable<TValue> rightSequence )
            where TValue : IComparable<TValue>
        {
            var fifoSink = new FifoCoordinatorSink<TValue>();
            var currentValueInOrder = default(TValue);
            Action<bool, IEnumerable<TValue>> mergeCouroutine = async (isFirst, sequence) =>
            {
                foreach (var value in sequence)
                {
                    if (isFirst || value.CompareTo(currentValueInOrder) > 0)
                    {
                        currentValueInOrder = value;
                        await fifoSink.AtEndOfQueue();
                    }
                    fifoSink.Add(value);
                }
            };
            mergeCouroutine(true, leftSequence);
            mergeCouroutine(false, rightSequence);
            
            return fifoSink;
        }

        public static IEnumerable<TValue> MergeWithCoroutines2<TValue>(
            this IEnumerable<TValue> leftSequence, 
            IEnumerable<TValue> rightSequence )
            where TValue : IComparable<TValue>
        {
            var mergeCoordintator = new MergeCoordinator<TValue>(leftSequence, rightSequence);
            return mergeCoordintator.GetResult();
        }
    }

    public class FifoCoordinatorSink<T> : IEnumerable<T>
    {
        public Queue<OneTimeAwaiter> _queue = new Queue<OneTimeAwaiter>();
        public List<T> _results = new List<T>();
    
        public IEnumerator<T> GetEnumerator()
        {
            while(_queue.Any())
            {
                _queue.Dequeue().Resume();
                if(_results.Any())
                {
                    foreach (var result in _results)
                    {
                        yield return result;
                    }
                }
                
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public OneTimeAwaiter AtEndOfQueue()
        {
            var oneTimeAwaiter = new OneTimeAwaiter();
            _queue.Enqueue(oneTimeAwaiter);
            return oneTimeAwaiter;
        }

        public void Add(T value)
        {
            _results.Add(value);
        }
    }


    public class MergeWithCourotines2Test : BaseMergeSequenceTest
    {
        public override IEnumerable<int> Merge(IEnumerable<int> leftSequence, IEnumerable<int> rightSequence)
        {
            return leftSequence.MergeWithCoroutines2(rightSequence);
        }
    }

    public class MergeWithCourotinesLazyTest : BaseMergeSequenceTest
    {
        public override IEnumerable<int> Merge(IEnumerable<int> leftSequence, IEnumerable<int> rightSequence)
        {
            return leftSequence.MergeWithCoroutinesLazy(rightSequence);
        }
    }
    
    public class MergeWithCoroutinesTest : BaseMergeSequenceTest
    {
        public override IEnumerable<int> Merge(IEnumerable<int> leftSequence, IEnumerable<int> rightSequence)
        {
            return leftSequence.MergeWithCoroutines(rightSequence);
        }
    }
}