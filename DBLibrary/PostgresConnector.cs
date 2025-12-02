using System;
using System.Collections.Generic;
using Npgsql;

namespace DBLibrary
{
    public class PostgresConnector : IDBConnector
    {
        private readonly string _conn;

        public PostgresConnector(string connectionString)
        {
            _conn = connectionString;
        }

        public bool Ping()
        {
            try
            {
                using var conn = new NpgsqlConnection(_conn);
                conn.Open();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void InsertData(List<string> values)
        {
            using var conn = new NpgsqlConnection(_conn);
            conn.Open();

            using (var cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS records(id SERIAL PRIMARY KEY, value TEXT);", conn))
            {
                cmd.ExecuteNonQuery();
            }

            foreach (var v in values)
            {
                using var insertCmd = new NpgsqlCommand("INSERT INTO records(value) VALUES(@v);", conn);
                insertCmd.Parameters.AddWithValue("v", v);
                insertCmd.ExecuteNonQuery();
            }
        }

        public string ReadData(int index)
        {
            using var conn = new NpgsqlConnection(_conn);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT value FROM records ORDER BY id ASC OFFSET @idx LIMIT 1;", conn);
            cmd.Parameters.AddWithValue("idx", index);
            var result = cmd.ExecuteScalar();
            return result?.ToString();
        }
    }
}
