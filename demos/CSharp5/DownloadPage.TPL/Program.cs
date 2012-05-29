using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CallerInfo;

namespace DownloadPage.TPL
{
    internal class Program
    {
        private static Logger TPL = Logger.Get();
        private static Logger TPL_Async = Logger.Get();

        private static void Main(string[] args)
        {
            PerformRequest()
                .ContinueWith(t=>
                {
                    // we could catch all exceptions inside the StartNew
                    // but if we can't we have to check for result or feault
                    // if we miss some exception that occured in Task
                    // entire AppDomain will be brought down
                    if(t.IsFaulted)
                    {
                        TPL.Log("Got exception in continuation");
                        TPL.Log(t.Exception.Message);
                    }
                });


            PerformRequestAsync()
                .ContinueWith(t =>
                {
                    if(t.IsFaulted)
                    {
                        TPL_Async.Log("Go exception in continuation");
                        TPL_Async.Log(t.Exception.Message);
                    }
                });

            TPL.Log("Hit enter to close");
            Console.ReadLine();
        }

        private static Task PerformRequest()
        {
            //here an exceptio may be thrown also
            var address = new Uri("http://www.goodgle.com");
            var request = WebRequest.CreateHttp(address);
            TPL.Log("Created http request");
            // here we are not using async http request but sheduling job to be executed in future
            return Task.Factory.StartNew(() =>
            {
                TPL.Log("Started task");
                try
                {
                    var response = request.GetResponse();
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        TPL.Log("Received response stream");
                        TPL.Log(string.Format("{0}...", streamReader.ReadToEnd().Substring(0, 100)));
                    }
                }
                catch(ProtocolViolationException ex) 
                {
                    TPL.Log("Connection was abruptly closed?");
                    TPL.Log(ex.Message);    
                }                
            });            
        }

        private static Task PerformRequestAsync()
        {
            //here an exceptio may be thrown also
            var address = new Uri("http://www.goodgle.com");
            var request = WebRequest.CreateHttp(address);
            TPL_Async.Log("Created http request");
         
            // here we are not sheduling a Task but we use async http requets under the hood
            var taskCompletion = new TaskCompletionSource<long>();            
            request.BeginGetResponse(state =>
            {
                try
                {
                    var response = request.EndGetResponse(state);
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        TPL_Async.Log("Received response stream");
                        TPL_Async.Log(string.Format("{0}...", streamReader.ReadToEnd().Substring(0, 100)));
                        taskCompletion.SetResult(response.ContentLength);
                    }
                }
                catch (Exception ex)
                {
                    taskCompletion.SetException(ex);
                }                

            }, request);
            TPL_Async.Log("Fired request");
            return taskCompletion.Task;

        }

    }
}
