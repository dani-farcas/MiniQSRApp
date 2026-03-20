using Microsoft.Data.Sqlite;
using System.Data;

namespace MiniQsrApp.Data
{
    public class DatabaseHelper
    {
        private string _connectionString;

        public DatabaseHelper(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
        }

        public void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sql = File.ReadAllText("init.sql");
                var command = connection.CreateCommand();

                command.CommandText = sql;

                command.ExecuteNonQuery();
            }
        }

        public void InsertTestData()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Test DEFAULT VALUES;";
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetData()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Clients;";

                var reader = command.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                return table;
            }
        }

        public DataTable RunQuery(string sql)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = sql;
                var reader = command.ExecuteReader();

                var table = new DataTable();
                table.Load(reader);

                return table;
            }
        }

        public void ExecuteCommand(string sql)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = sql;

                command.ExecuteNonQuery();
            }
        }
    }
}
