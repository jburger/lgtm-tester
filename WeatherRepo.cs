using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;

namespace lgtm_tester
{
    public class WeatherRepo
    {
        private static string DbFile => $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}WeatherApi.sqlite";
        public static SQLiteConnection DbConnection()
        {
            var connectionString = $"Data Source={DbFile}";
            return new SQLiteConnection(connectionString);
        }

        public static void Init()
        {
            if (File.Exists(DbFile) == false)
            {
                CreateDatabase();
            }
        }

        public IEnumerable<WeatherSummary> SummarySearchByName(string summaryName)
        {
            using var connection = DbConnection();
            connection.Open();
            var results = connection.Query<WeatherSummary>($"select * from weather_summary where name like '%" + summaryName + "%'");
            return results;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static void CreateDatabase()
        {
            using var connection = DbConnection();
            connection.Open();
            connection.Execute(
                @"create table weather_summary
                        (
                            Name    varchar(100) not null
                        )");
            foreach (var summaryName in Summaries)
            {
                connection.Query($"insert into weather_summary ( Name ) values ('{summaryName}')");
            }
        }

        public IEnumerable<WeatherSummary> FetchAllSummaries()
        {
            using var connection = DbConnection();
            connection.Open();
            var results = connection.Query<WeatherSummary>("select * from weather_summary");
            return results;
        }
    }
}
