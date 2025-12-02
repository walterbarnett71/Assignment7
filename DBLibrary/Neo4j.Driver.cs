using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBLibrary
{
    public class Neo4jConnector : IDBConnector, IDisposable
    {
        private readonly IDriver _driver;

        public Neo4jConnector(string uri, string? user = null, string? password = null)
        {
            // Create driver with or without authentication
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
                _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            else
                _driver = GraphDatabase.Driver(uri);
        }

        public bool Ping()
        {
            try
            {
                using var session = _driver.AsyncSession();

                var resultTask = session.RunAsync("RETURN 1");
                resultTask.Wait();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void InsertData(List<string> values)
        {
            using var session = _driver.AsyncSession();

            // Create constraint
            var constraint = session.RunAsync(
                "CREATE CONSTRAINT IF NOT EXISTS FOR (n:Record) REQUIRE n.id IS UNIQUE"
            );
            constraint.Wait();

            // Insert records
            for (int i = 0; i < values.Count; i++)
            {
                var insertTask = session.RunAsync(
                    "CREATE (n:Record {id:$id, value:$value})",
                    new { id = i, value = values[i] }
                );
                insertTask.Wait();
            }
        }

        public string ReadData(int index)
        {
            using var session = _driver.AsyncSession();

            var runTask = session.RunAsync(
                "MATCH (n:Record {id:$id}) RETURN n.value AS value",
                new { id = index }
            );
            runTask.Wait();

            var cursor = runTask.Result;

            var listTask = cursor.ToListAsync();
            listTask.Wait();

            var record = listTask.Result.FirstOrDefault();
            if (record == null)
                return "Not found";

            return record["value"].As<string>();
        }

        public void Dispose() => _driver?.Dispose();
    }
}
