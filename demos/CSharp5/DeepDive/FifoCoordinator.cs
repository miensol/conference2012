using System.Collections.Generic;
using System.Linq;

namespace DeepDive
{
    public class FifoCoordinator
    {
        private readonly Queue<OneTimeAwaiter> _queue = new Queue<OneTimeAwaiter>();

        public OneTimeAwaiter AtEndOfQueue()
        {
            var oneTimeAwaiter = new OneTimeAwaiter();
            _queue.Enqueue(oneTimeAwaiter);
            return oneTimeAwaiter;
        }

        public void Flush()
        {
            while(_queue.Any())
            {
                _queue.Dequeue().Resume();
            }
        }
    }
}