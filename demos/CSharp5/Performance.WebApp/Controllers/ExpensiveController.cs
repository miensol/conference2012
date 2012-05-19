using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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

        public async Task<ActionResult> ExecuteAsync()
        {
            return View("Execute", new ExecuteExpensiveViewModel
                                       {
                                           ExternalData = await _remoteService.ReadDataAsync(),
                                           QueryResult = await _database.GetResultsAsync(
                                               @"WAITFOR DELAY '00:00:01';
SELECT * FROM [dbo].[Words]")
                                       });
        }



    }

    public class RemoteService
    {
        public string ReadData()
        {
            var httpRequest = CreateeRequest();
            using (var response = (HttpWebResponse)httpRequest.GetResponse())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        public async Task<string> ReadDataAsync()
        {
            var httpRequest = CreateeRequest();
            using (var response = await httpRequest.GetResponseAsync())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        private static HttpWebRequest CreateeRequest()
        {
            return WebRequest.CreateHttp("http://localhost:8001/ExternalData");
        }
    }

    public class DatabaseQuery
    {
        private ConnectionStringSettings _connectionString;

        public DatabaseQuery()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Simple"];
        }

        public IEnumerable<string> GetResults(string sqlQuery)
        {
            using (var connection = new SqlConnection(_connectionString.ConnectionString))
            {
                connection.Open();
                var query = new SqlCommand(sqlQuery, connection);
                using (var reader = query.ExecuteReader())
                {
                    return reader.Select(dr => dr[0].ToString()).ToList();
                }
            }
        }

        public async Task<IEnumerable<string>> GetResultsAsync(string sqlQuery)
        {
            using (var connection = new SqlConnection(_connectionString.ConnectionString))
            {
                connection.Open();
                var query = new SqlCommand(sqlQuery, connection);
                using (var reader = await query.ExecuteReaderAsync())
                {
                    return (IEnumerable<string>)reader.Select(dr => dr[0].ToString()).ToList();
                }
            }
        }
    }
}