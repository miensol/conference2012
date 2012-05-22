using System;
using System.Collections.Generic;
using System.Linq;
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
        private Queue<SmallestValueAwaiter<TValue>> _queue = new Queue<SmallestValueAwaiter<TValue>>();
        public Tuple<bool, TValue> Smallest { get; set; }

        public MergeCoordinator(IEnumerable<TValue> leftSequence, IEnumerable<TValue> rightSequence)
        {
            Smallest = new Tuple<bool, TValue>(false, default(TValue));
            _leftSequence = leftSequence;
            _rightSequence = rightSequence;
        }

        public IEnumerable<TValue> GetResult()
        {
            var result = new List<TValue>();
            Merge(_leftSequence, result);
            Merge(_rightSequence, result);
            while(_queue.Any())
            {
                _queue.Dequeue().Resume();
            }
            return result;
        }

        private async void Merge(IEnumerable<TValue> sequence, IList<TValue> result)
        {
            foreach (var value in sequence)
            {
                if(HasSmallestValue == false)
                {                    
                    SetSmallestValue(value);
                    await SwitchToOther();
                }
                await SmallestValueIs(value);
                result.Add(value);
            }

        }

        private Swi SwitchToOther()
        {
            throw new NotImplementedException();
        }

        protected bool HasSmallestValue
        {
            get { return Smallest.Item1; }
        }


        private void SetSmallestValue(TValue value)
        {
            Smallest = Tuple.Create(true, value);
        }

        private SmallestValueAwaiter<TValue> SmallestValueIs(TValue currentValue)
        {
            return new SmallestValueAwaiter<TValue>(this, currentValue);
        }


        public void Enqueue(SmallestValueAwaiter<TValue> smallestValueAwaiter)
        {
            _queue.Enqueue(smallestValueAwaiter);
        }
    }

    public class SmallestValueAwaiter<T> : INotifyCompletion
           where T : IComparable<T>
    {
        private readonly MergeCoordinator<T> _coordinator;
        private readonly T _currentValue;
        private Action _continuation;

        public SmallestValueAwaiter(MergeCoordinator<T> coordinator, T currentValue)
        {
            _coordinator = coordinator;
            _currentValue = currentValue;
        }

        public bool IsCompleted
        {
            get
            {                
                if (_currentValue.CompareTo(_coordinator.Smallest.Item2) <= 0)
                {
                    _coordinator.Smallest = Tuple.Create(true, _currentValue);
                    return true;
                }
                return false;
            }
        }

        public T GetResult()
        {
            return _currentValue;
        }

        public void OnCompleted(Action continuation)
        {
            _coordinator.Enqueue(this);
            _continuation = continuation;
        }

        public SmallestValueAwaiter<T> GetAwaiter()
        {
            return this;
        }

        public void Resume()
        {
            _continuation();
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