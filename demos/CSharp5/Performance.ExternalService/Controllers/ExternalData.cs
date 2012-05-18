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
             Thread.Sleep(TimeSpan.FromMilliseconds(500));
             return new HttpResponseMessage
                        {
                            Content = new StringContent("This is very important data that took more than 500ms to prepare")
                        };
         }
    }
}