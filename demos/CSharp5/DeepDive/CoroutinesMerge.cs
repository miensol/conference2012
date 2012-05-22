using System;
using System.Collections.Generic;

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

        public static IEnumerable<TValue> MergeWithCoroutines2<TValue>(
            this IEnumerable<TValue> leftSequence, 
            IEnumerable<TValue> rightSequence )
            where TValue : IComparable<TValue>
        {
            var mergeCoordintator = new MergeCoordinator<TValue>(leftSequence, rightSequence);
            return mergeCoordintator.GetResult();
        }
    }


    public class MergeWithCourotines2Test : BaseMergeSequenceTest
    {
        public override IEnumerable<int> Merge(IEnumerable<int> leftSequence, IEnumerable<int> rightSequence)
        {
            return leftSequence.MergeWithCoroutines2(rightSequence);
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