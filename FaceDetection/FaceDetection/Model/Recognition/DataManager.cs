using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace FaceDetection.Model.Recognition
{
    internal class DataManager: IDisposable
    {
        private SQLiteConnection _dbConnection;

        public DataManager()
        {
            InitializeDatabase();
            InitializeConnection();
        }

        private bool InitializeConnection()
        {
            try
            {
                _dbConnection =
                    new SQLiteConnection($"Data Source={Properties.Settings.Default.DetectionSqlFile};Version=3");
                _dbConnection.Open();

                return true;
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine("Could not initialize db connection: " + ex);
            }

            return false;
        }

        private bool CloseConnection()
        {
            try
            {
                _dbConnection?.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't disconnect from database: " + ex);
            }

            return false;
        }

        private bool InitializeDatabase()
        {
            try
            {
                if (!File.Exists(Properties.Settings.Default.DetectionSqlFile))
                    SQLiteConnection.CreateFile(Properties.Settings.Default.DetectionSqlFile);
                const string sql = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'faces'";
                var command = new SQLiteCommand(sql, _dbConnection);
                if (command.ExecuteScalar() == null)
                {
                    CreateTables();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not initialize db: " + ex);
            }

            return false;
        }

        private bool DeleteDatabase()
        {
            try
            {
                File.Delete(Properties.Settings.Default.DetectionSqlFile);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't delete database: " + ex);
            }
            return false;
        }

        /// <summary>
        /// Resets the database.
        /// </summary>
        /// <returns>true if successfull otherwise false.</returns>
        public bool ResetDatabase()
        {
            return CloseConnection() && DeleteDatabase() && InitializeDatabase() && !InitializeConnection();
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _dbConnection.Dispose();
        }
    }
}
