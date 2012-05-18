using System.Collections.Generic;

namespace Performance.WebApp.Controllers
{
    public class ExecuteExpensiveViewModel 
    {
        public string ExternalData { get; set; }
        public IEnumerable<string> QueryResult { get; set; }
    }
}