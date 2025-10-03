using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using Grpc.Core;
//using Microsoft.SqlServer.Management.Common;
//using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
//using Server = Microsoft.SqlServer.Management.Smo.Server;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public class BackupService
    {
        private readonly string _connectionString;
        private readonly string _backupFolderFullPath;
        private readonly string[] _systemDatabaseNames = { "master", "tempdb", "model", "msdb" };
        public BackupService()
        {

        }
        public BackupService(string connectionString, string backupFolderFullPath)
        {
            //Initialize();
            _connectionString = connectionString;
            _backupFolderFullPath = backupFolderFullPath;
        }

        public int BackupDatabase(string fileName, string pathBackup)
        {
            string filePath = BuildBackupPathWithFilename(fileName);
            if (System.IO.File.Exists(filePath))
            {
                return 0;
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}' WITH NOFORMAT, NOINIT,  NAME = N'data-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10", connection.Database.ToString(), filePath);

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
                throw ex;
            }

        }
        public int BackupDatabase1(string fileName, string pathBackup)
        {
            string filePath = BuildBackupPathWithFilename(fileName);
            if (System.IO.File.Exists(filePath))
            {
                return 0;
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    //Backup backup = new Backup();
                    //backup.Action = BackupActionType.Database;
                    //backup.BackupSetDescription = "BackupDataBase description";
                    //backup.BackupSetName = "Backup";
                    //backup.Database = connection.Database.ToString();
                    //backup.Initialize = true;
                    //backup.Checksum = true;
                    //backup.ContinueAfterError = true;
                    //// backup.ExpirationDate = DateTime.Now.AddDays(3);
                    //backup.LogTruncation = BackupTruncateLogType.Truncate;
                    //BackupDeviceItem deviceItem = new BackupDeviceItem(filePath, DeviceType.File);
                    //backup.Devices.Add(deviceItem);
                    //ServerConnection connection1 = new ServerConnection(@"192.168.100.45");
                    //connection1.LoginSecure = false;
                    //connection1.Login = "anhvh";
                    //connection1.Password = "gosol@123";

                    //Server sqlServer = new Server(connection1);
                    //sqlServer.BackupDirectory = pathBackup;
                    //backup.SqlBackup(sqlServer);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return -1;
                throw ex;
            }

        }
        private string BuildBackupPathWithFilename(string fileName)
        {
            string filename = string.Format("{0}{1}.bak", "INOUT_", fileName);

            return Path.Combine(_backupFolderFullPath, filename);
        }
        // Lấy thông tin bản backup database cuối cùng
        public SystemLogModel GetLastBackUpDB(int? CanBoID)
        {
            SystemLogModel SystemLogModel = new SystemLogModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("CanBoID",SqlDbType.Int)
            };
            parameters[0].Value = CanBoID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_GetLastBackUpDB", parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogModel = new SystemLogModel();
                        SystemLogModel.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        SystemLogModel.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        SystemLogModel.LogInfo = Utils.ConvertToString(dr["LogInfo"], string.Empty);
                        SystemLogModel.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.Now);
                        SystemLogModel.LogType = Utils.ConvertToInt32(dr["LogType"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return SystemLogModel;
        }

        //public void BackupAllUserDatabases()
        //{
        //    foreach (string databaseName in GetAllUserDatabases())
        //    {
        //        string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
        //        BackupDatabase(filename);
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



        //private string BuildBackupPathWithFilename_Fixed(string databaseName)
        //{
        //    string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));

        //    return Path.Combine(_backupFolderFullPath, filename);
        //}
    }
}
