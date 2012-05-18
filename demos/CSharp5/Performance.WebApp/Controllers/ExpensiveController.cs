using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Linq;

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
                                QueryResult = _database.GetResults(
@"WAITFOR DELAY '00:00:01';
SELECT * FROM [dbo].[Words]")
                            });
        }



    }

    public class RemoteService
    {
        public string ReadData()
        {
            var httpRequest = WebRequest.CreateHttp("http://localhost:8001/ExternalData");
            using (var response = (HttpWebResponse)httpRequest.GetResponse())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
    }

    public class DatabaseQuery
    {        
        public IEnumerable<string> GetResults(string sqlQuery)
        {           
            var connectionString = ConfigurationManager.ConnectionStrings["Simple"];
            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                var query = new SqlCommand(sqlQuery, connection);
                using (var reader = query.ExecuteReader())
                {
                    return reader.Select(dr => dr[0].ToString()).ToList();
                }

            }
        }
    }
}