using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace FaceDetection.Model.Recognition
{
    internal class DataManager
    {
        private readonly SQLiteConnection _dbConnection;

        public DataManager()
        {
            try
            {
                if (!File.Exists(Properties.Settings.Default.DetectionSqlFile))
                    SQLiteConnection.CreateFile(Properties.Settings.Default.DetectionSqlFile);

                _dbConnection =
                    new SQLiteConnection($"Data Source={Properties.Settings.Default.DetectionSqlFile};Version=3");
                _dbConnection.Open();
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine("Could not initialize db connection: " + ex);
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Could not create db file: " + ex);
            }

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            const string sql = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'faces'";
            var command = new SQLiteCommand(sql, _dbConnection);
            if (command.ExecuteScalar() == null)
            {
                CreateTables();
            }
        }

        private void CreateTables()
        {
            try
            {
                const string sql = "CREATE TABLE faces (id INT, username TEXT(50), faceSample BLOB, userId INT, PRIMARY KEY(id ASC))";
                var command = new SQLiteCommand(sql, _dbConnection);
                command.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine("Could not create table in db: " + ex);
            }
            
        }
    }
}
