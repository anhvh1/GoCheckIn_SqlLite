using Com.Gosol.INOUT.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public class RestoreService
    {
        private readonly string _connectionString;
        private readonly string _backupFolderFullPath;
        private readonly string[] _systemDatabaseNames = { "master", "tempdb", "model", "msdb" };

        public RestoreService(string connectionString, string backupFolderFullPath)
        {
            _connectionString = connectionString;
            _backupFolderFullPath = backupFolderFullPath;
        }

        public int RestoreDatabase(string fileName)
        {
            try
            {
                if (!System.IO.File.Exists(_backupFolderFullPath + "\\" + fileName))
                {
                    return 0;
                }
                using (var connection = new SqlConnection(_connectionString))
                {
                    string database = connection.Database.ToString();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    string sqlStmt2 = string.Format("ALTER DATABASE [" + database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                    SqlCommand bu2 = new SqlCommand(sqlStmt2, connection);
                    bu2.ExecuteNonQuery();

                    string sqlStmt3 = "USE MASTER RESTORE DATABASE [" + database + "] FROM DISK='" + _backupFolderFullPath + "\\" + fileName + "'WITH REPLACE;";
                    SqlCommand bu3 = new SqlCommand(sqlStmt3, connection);
                    bu3.ExecuteNonQuery();

                    string sqlStmt4 = string.Format("ALTER DATABASE [" + database + "] SET MULTI_USER");
                    SqlCommand bu4 = new SqlCommand(sqlStmt4, connection);
                    bu4.ExecuteNonQuery();
                    connection.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
                throw ex;
            }
        }

        //public void RestoreAllUserDatabases()
        //{
        //    foreach (string databaseName in GetAllUserDatabases())
        //    {
        //        //RestoreDatabase(databaseName);
        //    }
        //}
        //private IEnumerable<string> GetAllUserDatabases()
        //{
        //    var databases = new List<String>();

        //    DataTable databasesTable;

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();

        //        databasesTable = connection.GetSchema("Databases");

        //        connection.Close();
        //    }

        //    foreach (DataRow row in databasesTable.Rows)
        //    {
        //        string databaseName = row["database_name"].ToString();

        //        if (_systemDatabaseNames.Contains(databaseName))
        //            continue;

        //        databases.Add(databaseName);
        //    }

        //    return databases;
        //}
    }
}
