using System.Threading;
using System.Threading.Tasks;
using CallerInfo;
using Nito.AsyncEx;

namespace DeepDive
{
    public class ProgramFlow : BaseAsyncTest
    { 
        private static Logger StandardFlow = Logger.Get();
        
        protected override void Because()
        {
            StandardFlow.Log("Will call async method without await");
            AsyncMethod();
            StandardFlow.Log("Continuing after async method was called");
        }

        private async void AsyncMethod()
        {
            StandardFlow.Log("Entered AsyncMethod");
            await LongRunninOperation();
            StandardFlow.Log("Done awaiting long running operation");
        }

        private Task LongRunninOperation()
        {
            return Task.Factory.StartNew(() =>
            {
                StandardFlow.Log("Started long running operation");
                Thread.Sleep(100);
                StandardFlow.Log("Ended long running operation");
            });

        }
    }

    public class SynchronousProgramFlow : BaseAsyncTest
    {
        private static Logger Synchronous = Logger.Get();

        protected override void Because()
        {
            Synchronous.Log("Will call async method without await");
            AsyncMethod();
            Synchronous.Log("Continuing after async method was called");
        }

        private async void AsyncMethod()
        {
            Synchronous.Log("Entered AsyncMethod");
            await SynchronouslyEndingOperation();
            Synchronous.Log("Done awaiting long running operation");
        }

        private Task SynchronouslyEndingOperation()
        {
            Synchronous.Log("Entered synchronously ending operation");
            Thread.Sleep(100);
            var tcs = new TaskCompletionSource();            
            tcs.SetResult();
            Synchronous.Log("Leaving synchronously ending operation");
            return tcs.Task;

        }

    }

}