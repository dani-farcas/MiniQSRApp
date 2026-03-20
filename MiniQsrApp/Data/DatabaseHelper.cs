using Microsoft.Data.Sqlite;
using System.Data;

namespace MiniQsrApp.Data
{
    public class DatabaseHelper
    {
        // #############################
        // FELDER
        // Speichert Verbindungsdaten und Pfade
        // #############################

        private readonly string _databasePath;
        private readonly string _connectionString;
        private readonly string _initScriptPath;

        // #############################
        // CONSTRUCTOR
        // Validiert den DB-Pfad und bereitet alle internen Werte vor
        // #############################

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

        // #############################
        // DATENBANK INITIALISIERUNG
        // Liest das SQL-Setup-Skript und erstellt die DB-Struktur
        // #############################

        public void InitializeDatabase()
        {
            if (!File.Exists(_initScriptPath))
            {
                throw new FileNotFoundException(
                    "Die Datei 'init.sql' wurde nicht gefunden.",
                    _initScriptPath
                );
            }

            string sqlScript = File.ReadAllText(_initScriptPath);

            if (string.IsNullOrWhiteSpace(sqlScript))
            {
                throw new InvalidOperationException("Die Datei 'init.sql' ist leer.");
            }

            ExecuteNonQuery(sqlScript);
        }

        // #############################
        // TESTDATEN
        // Fügt einen Testdatensatz in die Tabelle Test ein
        // #############################

        public void InsertTestData()
        {
            const string sql = "INSERT INTO Test DEFAULT VALUES;";
            ExecuteNonQuery(sql);
        }

        // #############################
        // STANDARDDATEN LADEN
        // Lädt alle Clients für die Standardanzeige
        // #############################

        public DataTable GetData()
        {
            const string sql = "SELECT * FROM Clients;";
            return ExecuteQuery(sql);
        }

        // #############################
        // BELIEBIGE SELECT-ABFRAGE
        // Führt eine SQL-Abfrage aus und gibt das Ergebnis als DataTable zurück
        // #############################

        public DataTable RunQuery(string sql)
        {
            ValidateSql(sql);
            return ExecuteQuery(sql);
        }

        // #############################
        // BELIEBIGER SQL-BEFEHL
        // Führt technische Befehle wie INSERT, UPDATE oder DELETE aus
        // #############################

        public void ExecuteCommand(string sql)
        {
            ValidateSql(sql);
            ExecuteNonQuery(sql);
        }

        // #############################
        // CONNECTION ERZEUGEN
        // Erstellt eine neue SQLite-Verbindung
        // #############################

        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        // #############################
        // SQL VALIDIERUNG
        // Prüft, ob überhaupt SQL übergeben wurde
        // #############################

        private static void ValidateSql(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentException("Die SQL-Anweisung darf nicht leer sein.", nameof(sql));
            }
        }

        // #############################
        // NON-QUERY AUSFÜHRUNG
        // Führt SQL ohne Ergebnisliste aus
        // #############################

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
                    ex
                );
            }
        }

        // #############################
        // QUERY AUSFÜHRUNG
        // Führt SELECT aus und lädt das Ergebnis in eine DataTable
        // #############################

        private DataTable ExecuteQuery(string sql)
        {
            try
            {
                using var connection = CreateConnection();
                using var command = connection.CreateCommand();

                connection.Open();
                command.CommandText = sql;

                using var reader = command.ExecuteReader();

                var table = new DataTable();
                table.Load(reader);

                return table;
            }
            catch (SqliteException ex)
            {
                throw new InvalidOperationException(
                    $"Fehler beim Laden der Daten: {ex.Message}",
                    ex
                );
            }
        }
    }
}