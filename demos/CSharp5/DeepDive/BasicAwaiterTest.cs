using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CallerInfo;

namespace DeepDive
{
    public class BasicAwaiterTest : BaseAsyncTest
    {
        public static Logger BasicTest = Logger.Get();
        private BasicAwaiter _basicAwaiter;

        protected override void Because()
        {
            AwaitCustomAwaiter();
            BasicTest.Log("Left AwaitCustomAwaiter");
            BasicTest.Log("Will now force completion");
            _basicAwaiter.Complete();
            BasicTest.Log("Basic awaiter complete called");
        }

        private async void AwaitCustomAwaiter()
        {
            _basicAwaiter = new BasicAwaiter(isCompleted: false, result: "await result");
            BasicTest.Log("Created awaiter, will await ");
            string result = await _basicAwaiter;
            BasicTest.Log(string.Format("Finished awaiting, result is: {0}", result));
        }
    }

    internal class BasicAwaiter : INotifyCompletion
    {
        private static Logger InsideAwaiter = Logger.Get();
        private bool _isCompleted;
        private readonly string _result;
        private Action _continuation;

        public BasicAwaiter(bool isCompleted, string result)
        {
            _isCompleted = isCompleted;
            _result = result;
        }

        public BasicAwaiter GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted
        {
            get
            {
                InsideAwaiter.Log(string.Format("IsCompleted called will return {0}", _isCompleted));
                return _isCompleted;
            } 
        }

        public void OnCompleted(Action continuation)
        {            
            _continuation = continuation;
            InsideAwaiter.Log("OnCompleted called");
        }

        public string GetResult()
        {
            InsideAwaiter.Log("GetResult called");
            return _result;
        }

        public void Complete()
        {
            if(_isCompleted == false)
            {
                // Technically we don't have to set this field
                // it's only because we wan't to use _isCompleted in constructor
                _isCompleted = true;
                _continuation();    
            }
            
        }
    }
}