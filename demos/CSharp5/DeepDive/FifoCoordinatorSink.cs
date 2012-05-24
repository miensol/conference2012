using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeepDive
{
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
}