using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.QuanTriHeThong
{
    public interface IQuanTriDuLieuBUS
    {

        public int BackupData(string fileName);
        public int RestoreDatabase(string fileName);
        public List<QuanTriDuLieuModel> GetFileInDerectory();
    }
    public class QuanTriDuLieuBUS : IQuanTriDuLieuBUS
    {
        private IQuanTriDuLieuDAL _QuanTriDuLieuDAL;
        public QuanTriDuLieuBUS(IQuanTriDuLieuDAL QuanTriDuLieuDAL)
        {
            _QuanTriDuLieuDAL = QuanTriDuLieuDAL;
        }
        public int BackupData(string fileName)
        {
            return _QuanTriDuLieuDAL.BackupData(fileName);
        }

        public int RestoreDatabase(string fileName)
        {
            return _QuanTriDuLieuDAL.RestoreData(fileName);
        }

        public List<QuanTriDuLieuModel> GetFileInDerectory()
        {
            return _QuanTriDuLieuDAL.GetFileInDerectory();
        }

    }
}
