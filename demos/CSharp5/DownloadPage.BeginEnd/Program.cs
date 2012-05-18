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
        private static Logger ProgramLog = Logger.Get();

        static void Main(string[] args)
        {
            var address = new Uri("http://www.ddgoodgle.com");

            var request = WebRequest.CreateHttp(address);
            ProgramLog.Log("Created http request");
            //here another exception may be thrown
            request.BeginGetResponse(EndGetResposne, request);
            ProgramLog.Log("Fired BeginGetResponse");
            
            ProgramLog.Log("Hit enter to close");
            Console.ReadLine();
        }

        private static void EndGetResposne(IAsyncResult ar)
        {
            ProgramLog.Log(string.Format("Completed synchrounously {0}", ar.CompletedSynchronously));
            var request = (HttpWebRequest) ar.AsyncState;
            try
            {
                var response = (HttpWebResponse)request.EndGetResponse(ar);
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    ProgramLog.Log("Got response stream");
                    ProgramLog.Log(string.Format("{0} ...",streamReader.ReadToEnd().Substring(0, 100)));
                }
            }
            catch (WebException exception)
            {
                ProgramLog.Log("Excepiton occured");
                ProgramLog.Log(exception.Message);
            }            
        }
    }
}
