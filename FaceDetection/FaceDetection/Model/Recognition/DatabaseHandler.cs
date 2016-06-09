using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace FaceDetection.Model.Recognition
{
    internal static class DatabaseHandler
    {
        private static SQLiteConnection _dbConnection;

        static DatabaseHandler()
        {
            InitializeDatabase();
        }

        private static bool InitializeConnection()
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

        private static bool CloseConnection()
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

        private static bool InitializeDatabase()
        {
            try
            {
                if (!File.Exists(Properties.Settings.Default.DetectionSqlFile))
                    SQLiteConnection.CreateFile(Properties.Settings.Default.DetectionSqlFile);

                if (!InitializeConnection())
                    return false;

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

        private static bool DeleteDatabase()
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
        public static bool ResetDatabase()
        {
            return CloseConnection() && DeleteDatabase() && InitializeDatabase();
        }

        private static void CreateTables()
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
        /// Sends an UPDATE statement to the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>True if the operation was successfull otherwise false.</returns>
        public static bool Update(string statement)
        {
            try
            {
                var command = new SQLiteCommand(statement, _dbConnection);
                command.ExecuteNonQuery();

                return true;
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"Could not execute update statement: {statement} - Exception: {ex}");
            }

            return false;
        }

        /// <summary>
        /// Selects data from the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>The <see cref="SQLiteDataReader"/> storing the returned data or null if an error occured.</returns>
        public static SQLiteDataReader Select(string statement)
        {
            try
            {
                var command = new SQLiteCommand(statement, _dbConnection);
                return command.ExecuteReader();
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"Could not execute select statement: {statement} - Exception: {ex}");
            }

            return null;
        }

        /// <summary>
        /// Selects data from the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>Returns the result of the resultset or null if no resultset is present or an error occured.</returns>
        public static object SelectObject(string statement)
        {
            try
            {
                var command = new SQLiteCommand(statement, _dbConnection);
                return command.ExecuteScalar();
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"Could not execute select statement: {statement} - Exception: {ex}");
            }

            return null;
        }

        /// <summary>
        /// Inserts data into the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>The rowid of the newly inserted data or -1 if an error occured.</returns>
        public static long Insert(string statement)
        {
            if (!Update(statement))
                return -1;

            var result = SelectObject("select last_insert_rowid()");
            if (result == null)
                return -1;

            return (long)result;
        }
    }
}
