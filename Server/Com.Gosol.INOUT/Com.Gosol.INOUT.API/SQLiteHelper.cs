using System.Data.SQLite;

namespace Com.Gosol.INOUT.API
{
    public class SQLiteHelper
    {
        private readonly string _connectionString;
        public SQLiteHelper(string connectionString) => _connectionString = connectionString;

        public SQLiteConnection GetConnection() => new SQLiteConnection(_connectionString);
    }
}
