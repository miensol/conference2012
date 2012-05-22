using System;
using System.Collections.Generic;

namespace DeepDive
{
    public static class ClassicMerge
    {
        public static IEnumerable<TValue> MergeClassic<TValue>(
            this IEnumerable<TValue> leftSequence,
            IEnumerable<TValue> rightSequence)
            where TValue : IComparable<TValue>
        {
            // enumerate the sequences
            using (var left = leftSequence.GetEnumerator())
            using (var right = rightSequence.GetEnumerator())
            {
                // start the enumeration
                bool leftHasItems = left.MoveNext();
                bool rightHasItems = right.MoveNext();

                // continue until out of items
                while (leftHasItems || rightHasItems)
                {
                    // determine whether to output from left or right
                    bool outputLeft;
                    if (leftHasItems && rightHasItems)
                        outputLeft = left.Current.CompareTo(right.Current) <= 0;
                    else
                        outputLeft = leftHasItems;

                    // output correct element
                    if (outputLeft)
                    {
                        yield return left.Current;
                        leftHasItems = left.MoveNext();
                    }
                    else
                    {
                        yield return right.Current;
                        rightHasItems = right.MoveNext();
                    }
                }
            }
        }
    }

    public class ClassicMergeTEst : BaseMergeSequenceTest
    {
        public override IEnumerable<int> Merge(IEnumerable<int> leftSequence, IEnumerable<int> rightSequence)
        {
            return leftSequence.MergeClassic(rightSequence);
        }
    }


}