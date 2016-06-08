using System.Data.SQLite;
using System.Diagnostics;

namespace FrameDetection.Model.Recognition
{
    class DataManager
    {
        private readonly SQLiteConnection _dbConnection;

        public DataManager()
        {
            SQLiteConnection.CreateFile(Properties.Settings.Default.DetectionSqlFile);

            _dbConnection = new SQLiteConnection($"Data Source={Properties.Settings.Default.DetectionSqlFile};Version=3");
            _dbConnection.Open();

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
