using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CallerInfo;

namespace DownloadPage.Reactive
{
    internal class Program
    {
        private static Logger Reactive = Logger.Get();

        private static void Main(string[] args)
        {
            PerformRequest();

            Reactive.Log("Hit enter to close");
            Console.ReadLine();
        }

        private static void PerformRequest()
        {
            var address = new Uri("http://www.ddgoodgle.com");

            var request = WebRequest.CreateHttp(address);
            Reactive.Log("Created http request");
            //here another exception may be thrown

            var observable = Observable.FromAsyncPattern(
                request.BeginGetResponse,
                result => (HttpWebResponse) request.EndGetResponse(result)
            );

            observable().Subscribe(ReadResponseStream, OnError);

            Reactive.Log("Fired BeginGetResponse");
        }

        private static void ReadResponseStream(HttpWebResponse obj)
        {
            try
            {
                using (var streamReader = new StreamReader(obj.GetResponseStream()))
                {
                    Reactive.Log("Received response stream");
                    Reactive.Log(string.Format("{0}...", streamReader.ReadToEnd().Substring(0,100)));
                }
            }
            catch (ProtocolViolationException ex)
            {
                Reactive.Log("Connection was abruptly closed?");
            }
        }

        public static void OnError(Exception ex)
        {
            Reactive.Log("Exception occured");
            Reactive.Log(ex.Message);
        }
    }
}