using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using System.Web.Http;

namespace Performance.ExternalService
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpConfig = new HttpSelfHostConfiguration("http://localhost:8001");
            httpConfig.Routes.MapHttpRoute("Default", "{controller}/{id}", new
                                                                               {
                                                                                   id = RouteParameter.Optional
                                                                               });

            using (var server = new HttpSelfHostServer(httpConfig))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Listening at " + httpConfig.BaseAddress);
                Console.ReadLine();
            }
        }
    }
}
