using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CallerInfo;

namespace DownloadPage.BeginEnd
{
    class Program
    {
        private static Logger BeginEndProgram = Logger.Get();

        static void Main(string[] args)
        {
            PerformRequest();

            BeginEndProgram.Log("Hit enter to close");
            Console.ReadLine();
        }

        private static void PerformRequest()
        {
            //here exception may be thrown also
            var address = new Uri("http://www.ddgoodgle.com");
            var request = WebRequest.CreateHttp(address);
            BeginEndProgram.Log("Created http request");
            //here another exception may be thrown
            request.BeginGetResponse(EndGetResposne, request);
            BeginEndProgram.Log("Fired BeginGetResponse");
        }

        private static void EndGetResposne(IAsyncResult ar)
        {
            BeginEndProgram.Log(string.Format("Completed synchrounously {0}", ar.CompletedSynchronously));
            var request = (HttpWebRequest) ar.AsyncState;
            try
            {
                var response = (HttpWebResponse)request.EndGetResponse(ar);
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    BeginEndProgram.Log("Got response stream");
                    BeginEndProgram.Log(string.Format("{0} ...",streamReader.ReadToEnd().Substring(0, 100)));
                }
            }
            catch (WebException exception)
            {
                BeginEndProgram.Log("Excepiton occured");
                BeginEndProgram.Log(exception.Message);
            }            
        }
    }
}
