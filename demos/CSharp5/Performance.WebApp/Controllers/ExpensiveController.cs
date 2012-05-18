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
                                           QueryResult = await _database.GetresultsAsync(
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
        public IEnumerable<string> GetResults(string sqlQuery)
        {
            return WithConnectionDo(connection =>
            {
                var query = new SqlCommand(sqlQuery, connection);
                using (var reader = query.ExecuteReader())
                {
                    return reader.Select(dr => dr[0].ToString()).ToList();
                }
            });
        }

        public Task<IEnumerable<string>> GetresultsAsync(string sqlQuery)
        {
            return WithConnectionDo(async connection =>
            {
                var query = new SqlCommand(sqlQuery, connection);
                using (var reader = await query.ExecuteReaderAsync())
                {
                    return (IEnumerable<string>)reader.Select(dr => dr[0].ToString()).ToList();
                }
            });
        }

        private TResult WithConnectionDo<TResult>(Func<SqlConnection, TResult> getResults)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Simple"];
            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                return getResults(connection);
            }
        }
        
        private async Task<TResult> WithConnectionDo<TResult>(Func<SqlConnection, Task<TResult>> getResults)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Simple"];
            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                return await getResults(connection);
            }
        }
    }
}