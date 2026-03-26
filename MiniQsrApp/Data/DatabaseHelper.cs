using Microsoft.Data.Sqlite;
using System.Data;

namespace MiniQsrApp.Data
{
    public class DatabaseHelper
    {
        #region Private Fields

        private readonly string _databasePath;
        private readonly string _connectionString;
        private readonly string _initScriptPath;

        #endregion Private Fields

        #region Constructors

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initialisiert den Datenbankhelfer und bereitet alle benötigten Pfade sowie die Verbindungszeichenfolge vor.
        /// </summary>
        /// <param name="databasePath">Pfad zur SQLite-Datenbankdatei.</param>
        public DatabaseHelper(string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new ArgumentException("Der Datenbankpfad darf nicht leer sein.", nameof(databasePath));
            }

            _databasePath = databasePath;
            _connectionString = $"Data Source={_databasePath}";
            _initScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "init.sql");
        }

        #endregion Constructors

        #region Public Methods

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Initialisiert die Datenbank anhand des SQL-Initialisierungsskripts.
        /// </summary>
        public void InitializeDatabase()
        {
            if (!File.Exists(_initScriptPath))
            {
                throw new FileNotFoundException(
                    "Die Datei 'init.sql' wurde nicht gefunden.",
                    _initScriptPath);
            }

            string sqlScript = File.ReadAllText(_initScriptPath);

            if (string.IsNullOrWhiteSpace(sqlScript))
            {
                throw new InvalidOperationException("Die Datei 'init.sql' ist leer.");
            }

            ExecuteNonQuery(sqlScript);
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Fügt einen Testdatensatz in die Tabelle Test ein.
        /// </summary>
        public void InsertTestData()
        {
            const string sql = "INSERT INTO Test DEFAULT VALUES;";
            ExecuteNonQuery(sql);
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Lädt die Standarddaten aus der Tabelle Clients.
        /// </summary>
        /// <returns>Standarddaten als DataTable.</returns>
        public DataTable GetData()
        {
            const string sql = "SELECT * FROM Clients;";
            return ExecuteQuery(sql);
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Führt eine SELECT-Abfrage aus und gibt das Ergebnis als DataTable zurück.
        /// </summary>
        /// <param name="sql">Auszuführende SQL-Abfrage.</param>
        /// <returns>Abfrageergebnis als DataTable.</returns>
        public DataTable RunQuery(string sql)
        {
            ValidateSql(sql);
            return ExecuteQuery(sql);
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Führt einen SQL-Befehl ohne Rückgabemenge aus.
        /// </summary>
        /// <param name="sql">Auszuführender SQL-Befehl.</param>
        public void ExecuteCommand(string sql)
        {
            ValidateSql(sql);
            ExecuteNonQuery(sql);
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gibt alle Benutzertabellen der SQLite-Datenbank zurück.
        /// </summary>
        public List<string> GetTables()
        {
            List<string> tables = new();

            DataTable dt = RunQuery("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'");

            foreach (DataRow row in dt.Rows)
            {
                string? name = row["name"]?.ToString();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    tables.Add(name);
                }
            }

            return tables;
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Gibt alle Spaltennamen aus allen Benutzertabellen der Datenbank zurück.
        /// Doppelte Namen werden entfernt.
        /// </summary>
        public List<string> GetAllColumns()
        {
            List<string> columns = new();
            DataTable tables = RunQuery("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'");

            foreach (DataRow tableRow in tables.Rows)
            {
                string tableName = tableRow["name"]?.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(tableName))
                {
                    continue;
                }

                DataTable tableInfo = RunQuery($"PRAGMA table_info({tableName})");

                foreach (DataRow columnRow in tableInfo.Rows)
                {
                    string columnName = columnRow["name"]?.ToString() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(columnName) && !columns.Contains(columnName))
                    {
                        columns.Add(columnName);
                    }
                }
            }

            return columns;
        }

        #endregion Public Methods

        #region Private Methods

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Erstellt eine neue SQLite-Verbindung auf Basis der aktuellen Verbindungszeichenfolge.
        /// </summary>
        /// <returns>Neue Instanz einer SQLite-Verbindung.</returns>
        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Prüft, ob eine gültige SQL-Anweisung übergeben wurde.
        /// </summary>
        /// <param name="sql">Zu prüfende SQL-Anweisung.</param>
        private static void ValidateSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException("Die SQL-Anweisung darf nicht leer sein.", nameof(sql));
            }
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Führt eine SQL-Anweisung ohne Rückgabedaten aus.
        /// </summary>
        /// <param name="sql">Auszuführende SQL-Anweisung.</param>
        private void ExecuteNonQuery(string sql)
        {
            try
            {
                using var connection = CreateConnection();
                using var command = connection.CreateCommand();

                connection.Open();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException(
                    $"Fehler beim Ausführen des SQL-Befehls: {ex.Message}",
                    ex);
            }
        }

        /// --------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Führt eine SQL-Abfrage aus und lädt das Ergebnis in eine DataTable.
        /// </summary>
        /// <param name="sql">Auszuführende SQL-Abfrage.</param>
        /// <returns>Abfrageergebnis als DataTable.</returns>
        private DataTable ExecuteQuery(string sql)
        {
            try
            {
                using var connection = CreateConnection();
                using var command = connection.CreateCommand();

                connection.Open();
                command.CommandText = sql;

                using var reader = command.ExecuteReader();

                DataTable table = new();
                table.Load(reader);

                return table;
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException(
                    $"Fehler beim Laden der Daten: {ex.Message}",
                    ex);
            }
        }

        #endregion Private Methods
    }
}