using System.Web.Mvc;

namespace Performance.WebApp.Controllers
{
    public class ExpensiveController : Controller
    {
        private readonly RemoteService _remoteService;
        private readonly DatabaseQuery _database;

        public ExpensiveController():this(new RemoteService(), new DatabaseQuery())
        {
        }


        public ExpensiveController(RemoteService remoteService, DatabaseQuery database)
        {
            _remoteService = remoteService;
            _database = database;
        }

        public ActionResult Execute()
        {
            return View(new ExecuteExpensiveViewModel
                            {
                                ExternalData = _remoteService.ReadData(),
                                QueryResult = _database.GetInt("")
                            });
        }



    }

    public class RemoteService
    {
        public string ReadData()
        {
            throw new System.NotImplementedException();
        }
    }

    public class DatabaseQuery
    {
        public int GetInt(string sqlQuery)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ExecuteExpensiveViewModel 
    {
        public string ExternalData { get; set; }
        public int QueryResult { get; set; }
    }
}