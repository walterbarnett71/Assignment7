using MongoDB.Bson;
using MongoDB.Driver;

namespace DBConnector;

public class MongoConnector : IDBConnector
{
    private string m_connectionString;

    public MongoConnector(string connectionString)
    {
        m_connectionString = connectionString;
    }

    public async Task<bool> ping()
    {
        try
        {
            var client = new MongoClient(m_connectionString);
            // Ping the database
            var database = client.GetDatabase("admin"); // "admin" is safe to use for ping
            var command = new BsonDocument("ping", 1);
            var result = await database.RunCommandAsync<BsonDocument>(command);

            Console.WriteLine("Ping successful: " + result.ToJson());
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ping failed: " + ex.Message);
            return false;
        }
    }
}
