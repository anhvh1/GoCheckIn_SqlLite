using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface IQuanTriDuLieuDAL
    {
        public int BackupData(string fileName);
        public int RestoreData(string fileName);
        public List<QuanTriDuLieuModel> GetFileInDerectory();
    }
    public class QuanTriDuLieuDAL : IQuanTriDuLieuDAL
    {
        public int BackupData(string fileName)
        {
            var backupPath = new SystemConfigDAL().GetByKey("BackUp_Path").ConfigValue.ToString();
            //var backupService = new BackupService(SQLHelper.appConnectionStrings, SQLHelper.backupPath);
            var backupService = new BackupService(SQLHelper.appConnectionStrings, backupPath);
            try
            {
                return backupService.BackupDatabase(fileName, backupPath);
            }
            catch (Exception ex)
            {
                return -1;
                throw ex;
            }
        }


        public int RestoreData(string fileName)
        {
            var backupPath = new SystemConfigDAL().GetByKey("BackUp_Path").ConfigValue.ToString();
            //var restoreService = new RestoreService(SQLHelper.appConnectionStrings, SQLHelper.backupPath);
            var restoreService = new RestoreService(SQLHelper.appConnectionStrings, backupPath);
            try
            {
                return restoreService.RestoreDatabase(fileName);
            }
            catch (Exception ex)
            {
                return -1;
                throw ex;
            }
        }

        public List<QuanTriDuLieuModel> GetFileInDerectory()
        {
            try
            {
                var backupPath = new SystemConfigDAL().GetByKey("BackUp_Path").ConfigValue.ToString();
                var Result = new List<QuanTriDuLieuModel>();
                //var fullFile = Directory.GetFiles(SQLHelper.backupPath, "*.bak");
                var fullFile = Directory.GetFiles(backupPath, "*.bak");
                if (fullFile.Length > 0)
                {
                    for (int i = 0; i < fullFile.Length; i++)
                    {
                        var fileName = Path.GetFileName(fullFile[i].ToString()).Trim();
                        if (fileName.StartsWith("INOUT_"))
                        {
                            Result.Add(new QuanTriDuLieuModel(fileName));
                        }
                    }
                }
                return Result;
            }
            catch (Exception)
            {
                return new List<QuanTriDuLieuModel>();
                throw;
            }
        }

    }
}
