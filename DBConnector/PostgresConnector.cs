using Npgsql;

namespace DBConnector;

public class PostgresConnector : IDBConnector
{
    private NpgsqlConnection connection;

    public PostgresConnector(string connectionString)
    {
        connection = new NpgsqlConnection(connectionString);
    }

    public async Task<bool> ping()
    {
        try
        {
            await connection.OpenAsync();

            await using (var cmd = new NpgsqlCommand("SELECT 1", connection))
            {
                var result = await cmd.ExecuteScalarAsync();

                if (result == null)
                    return false;

                return (int)result == 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ping failed: " + ex.Message);
            return false;
        }
    }
}

