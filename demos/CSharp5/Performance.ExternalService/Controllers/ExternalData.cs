using System;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Performance.ExternalService.Controllers
{
    public class ExternalDataController : ApiController
    {
         public HttpResponseMessage Get()
         {
             var waitTime = TimeSpan.FromSeconds(1);
             Thread.Sleep(waitTime);
             return new HttpResponseMessage
                        {
                            Content = new StringContent(
                                string.Format("This is very important data that took more than {0} to prepare", waitTime))
                        };
         }
    }
}