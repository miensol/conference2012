using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Performance.WebApp.Controllers
{
    public static class Extensions
    {

        public static IEnumerable<T> Select<T>(
            this SqlDataReader reader, Func<SqlDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }
    }
}