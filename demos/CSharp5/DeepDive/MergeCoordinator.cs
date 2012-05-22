using System;
using System.Collections.Generic;

namespace DeepDive
{
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

        private bool HasOtherSequenceCurrent { get; set; }

        private async void BeginMerge(IEnumerable<TValue> sequence, ICollection<TValue> result)
        {
            foreach (var value in sequence)
            {
                if(HasOtherSequenceCurrent == false || value.CompareTo(OtherSequenceCurrent) > 0)
                {                                        
                    await OtherSeqeunce(value);
                }
                result.Add(value);
            }

        }

        private OneTimeAwaiter OtherSeqeunce(TValue currentSequenceValue)
        {
            OtherSequenceCurrent = currentSequenceValue;
            HasOtherSequenceCurrent = true;
            return _coroutinesQueue.AtEndOfQueue();
        }
    }
}