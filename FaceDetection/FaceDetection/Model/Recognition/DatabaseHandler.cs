using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

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

                OpenConnection();
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not initialize db connection: " + ex);
            }

            return false;
        }

        private static void OpenConnection()
        {
            _dbConnection?.Open();
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
                Logger.Error("Couldn't disconnect from database: " + ex);
            }

            return false;
        }

        private static bool InitializeDatabase()
        {
            try
            {
                if (!File.Exists(Properties.Settings.Default.DetectionSqlFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Properties.Settings.Default.DetectionSqlFile));
                    SQLiteConnection.CreateFile(Properties.Settings.Default.DetectionSqlFile);
                }
                    
                return InitializeConnection() && CreateTables();
            }
            catch (Exception ex)
            {
                Logger.Error("Could not initialize db: " + ex);
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
                Logger.Error("Couldn't delete database: " + ex);
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

        private static bool CreateTables()
        {
            try
            {
                var sql =
                    "CREATE TABLE IF NOT EXISTS faces (id INTEGER PRIMARY KEY AUTOINCREMENT, grayframe BLOB NOT NULL, original BLOB NOT NULL, userID INTEGER NOT NULL, width INTEGER NOT NULL, height INTEGER NOT NULL)";

                using (var command = new SQLiteCommand(sql, _dbConnection))
                    command.ExecuteNonQuery();

                sql =
                    "CREATE TABLE IF NOT EXISTS users (id INTEGER NOT NULL, username TEXT(50) NOT NULL, firstname TEXT(50) DEFAULT 'Unset', lastname TEXT(50) DEFAULT 'Unset', PRIMARY KEY(id ASC))";
                using (var command = new SQLiteCommand(sql, _dbConnection))
                    command.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not create table in db: " + ex);
            }

            return false;
        }

        /// <summary>
        /// Updates data in the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>True if the operation was successfull otherwise false.</returns>
        public static bool Update(string statement, params SQLiteParameter[] parameters)
        {
            try
            {
                using (var command = new SQLiteCommand(statement, _dbConnection))
                {
                    command.Parameters.AddRange(parameters);

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Could not execute update statement: {statement} - Exception: {ex}");
            }

            return false;
        }

        /// <summary>
        /// Asynchronously updates data in the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>True if the operation was successfull otherwise false.</returns>
        public static async Task<bool> UpdateAsync(string statement, params SQLiteParameter[] parameters)
        {
            return await Task.Run(() => Update(statement, parameters));
        }

        /// <summary>
        /// Selects data from the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The <see cref="SQLiteDataReader"/> storing the returned data or null if an error occured.</returns>
        public static SQLiteDataReader Select(string statement, params SQLiteParameter[] parameters)
        {
            try
            {
                using (var command = new SQLiteCommand(statement, _dbConnection))
                {
                    command.Parameters.AddRange(parameters);

                    var reader = command.ExecuteReader();

                    return reader;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Could not execute select statement: {statement} - Exception: {ex}");
            }

            return null;
        }

        /// <summary>
        /// Asynchonously selects data from the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The <see cref="SQLiteDataReader"/> storing the returned data or null if an error occured.</returns>
        public static async Task<SQLiteDataReader> SelectAsync(string statement, params SQLiteParameter[] parameters)
        {
            return await Task.Run(() => Select(statement, parameters));
        }

        /// <summary>
        /// Selects data from the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters</param>
        /// <returns>Returns the result of the resultset or null if no resultset is present or an error occured.</returns>
        public static object SelectObject(string statement, params SQLiteParameter[] parameters)
        {
            try
            {
                using (var command = new SQLiteCommand(statement, _dbConnection))
                {
                    command.Parameters.AddRange(parameters);

                    var result = command.ExecuteScalar();

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Could not execute select statement: {statement} - Exception: {ex}");
            }

            return null;
        }

        /// <summary>
        /// Asynchronously selects data from the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters"></param>
        /// <returns>Returns the result of the resultset or null if no resultset is present or an error occured.</returns>
        public static async Task<object> SelectObjectAsync(string statement, params SQLiteParameter[] parameters)
        {
            return await Task.Run(() => SelectObject(statement, parameters));
        }

        /// <summary>
        /// Inserts data into the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The rowid of the newly inserted data or -1 if an error occured.</returns>
        public static long Insert(string statement, params SQLiteParameter[] parameters)
        {
            if (!Update(statement, parameters))
                return -1;

            var result = SelectObject("select last_insert_rowid()");
            if (result == null)
                return -1;

            return (long)result;
        }

        /// <summary>
        /// Asynchronously inserts data into the database.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The rowid of the newly inserted data or -1 if an error occured.</returns>
        public static async Task<long> InsertAsync(string statement, params SQLiteParameter[] parameters)
        {
            return await Task.Run(() => Insert(statement, parameters));
        }
    }
}
