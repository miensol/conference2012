using System.Collections.Generic;
using System.Linq;
using CallerInfo;
using NUnit.Framework;

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

    public class FifoCoorindatorTest
    {
        public static Logger Fifo = Logger.Get();

        [Test]
        public void should_show_how_coroutines_work()
        {
            var coordinator = new FifoCoordinator();

            FirstRoutine(coordinator);
            SecondRoutine(coordinator);
            ThirdRooutine(coordinator);
            
            coordinator.Flush();
        }

        private async void FirstRoutine(FifoCoordinator coordinator)
        {
            Fifo.Log("Entered First");
            
            await coordinator.AtEndOfQueue();
            Fifo.Log("Back to First");
            
            await coordinator.AtEndOfQueue();
            Fifo.Log("Ending First");
        }

        private async void SecondRoutine(FifoCoordinator coordinator)
        {
            Fifo.Log("Entered Second");
            
            await coordinator.AtEndOfQueue();
            
            Fifo.Log("Ending Second");
        }

        private async void ThirdRooutine(FifoCoordinator coordinator)
        {
            Fifo.Log("Entered Third");
            
            await coordinator.AtEndOfQueue();
            Fifo.Log("Back to Third");
            
            await coordinator.AtEndOfQueue();
            Fifo.Log("Back again to Third");
            
            await coordinator.AtEndOfQueue();
            Fifo.Log("Ending Third");
        }
    }
}