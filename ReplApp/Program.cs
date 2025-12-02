using System;
using System.Linq;
using System.Collections.Generic;
using DBLibrary;

class Program
{
    static void Main()
    {
        Console.WriteLine("Choose a database: mongo | postgres | neo4j");
        var dbChoice = Console.ReadLine()?.Trim().ToLower();

        Console.WriteLine("Enter connection string:");
        var connectionString = Console.ReadLine()?.Trim();

        IDBConnector connector = dbChoice switch
        {
            "mongo" => new MongoConnector(connectionString),
            "postgres" => new PostgresConnector(connectionString),
            "neo4j" => ParseNeo4jConnector(connectionString),
            _ => null
        };

        if (connector == null)
        {
            Console.WriteLine("Invalid DB type.");
            return;
        }

        Console.WriteLine("Pinging database...");
        if (!connector.Ping())
        {
            Console.WriteLine("Connection failed.");
            return;
        }
        Console.WriteLine("Connection successful!");

        var data = Enumerable.Range(1, 20).Select(i => $"Record #{i}").ToList();
        connector.InsertData(data);
        Console.WriteLine("Inserted 20 records.");

        Console.WriteLine("Enter index of record to retrieve (0-19):");
        if (!int.TryParse(Console.ReadLine(), out int idx)) idx = 0;

        var res = connector.ReadData(idx);
        Console.WriteLine($"Retrieved: {res}");
    }

    static IDBConnector ParseNeo4jConnector(string cs)
    {
        // Accept "bolt://neo4j:password@localhost:7687"
        try
        {
            if (cs.Contains("@"))
            {
                var parts = cs.Split(new[] { "://", "@", ":" }, StringSplitOptions.RemoveEmptyEntries);
                // parts example: ["bolt", "neo4j", "password", "localhost", "7687"]

                string user = parts[1];
                string password = parts[2];
                string host = parts[3] + ":" + parts[4];

                string uri = "bolt://" + host;
                return new Neo4jConnector(uri, user, password);
            }
            else
            {
                // Accept "bolt://localhost:7687"
                return new Neo4jConnector(cs);
            }
        }
        catch
        {
            return null;
        }
    }
}
