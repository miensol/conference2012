using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CallerInfo;

namespace DownloadPage.Async
{
    class Program
    {
        private static Logger Async = Logger.Get();

        static void Main(string[] args)
        {
            PerformRequestVoid();
            
            Async.Log("Hit enter to continue");
            Console.ReadLine();
            
            PerformRequestResult();

            Async.Log("Hit enter to continue");
            Console.ReadLine();
            
        }

        private static async void PerformRequestVoid()
        {
            var address = new Uri("http://www.goodgle.com");

            var request = WebRequest.CreateHttp(address);
            Async.Log("Created http request");            
            try
            {
                var response = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    Async.Log("Received response stream");
                    Async.Log(string.Format("{0}...", streamReader.ReadToEnd().Substring(0, 100)));
                }
            }
            catch (WebException ex)
            {
                Async.Log("Exception occured");
                Async.Log(ex.Message);
            }


            Async.Log("Fired BeginGetResponse");
        }

        private static async void PerformRequestResult()
        {
            Async.Log("Will perform request");
            string startOfPage = await GetPageStartLetters(100);
            Async.Log("Got start of the page form [GetPageStartLetters]");
            Async.Log(string.Format("{0}...", startOfPage));
        }

        private static async Task<string> GetPageStartLetters(int length)
        {
            var address = new Uri("http://www.goodgle.com");

            var request = WebRequest.CreateHttp(address);
            Async.Log("Created http request");
            try
            {
                var response = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    Async.Log("Received response stream");
                    var wholePage = await streamReader.ReadToEndAsync();
                    return wholePage.Substring(0,length);
                }
            }
            catch (WebException ex)
            {
                Async.Log("Exception occured");
                Async.Log(ex.Message);
            }
            
            return string.Empty;
        }
    }
}
