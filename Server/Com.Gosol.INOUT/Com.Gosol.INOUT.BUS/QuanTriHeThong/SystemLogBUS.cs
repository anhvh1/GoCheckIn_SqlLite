using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Com.Gosol.INOUT.BUS.QuanTriHeThong
{
    public interface ISystemLogBUS
    {
        int Insert(SystemLogModel systemLogInfo);
        List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow);
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay);
        public int Insert_Notify(SystemLogModel systemLogInfo, ref int LogID);
    }
    public class SystemLogBUS : ISystemLogBUS
    {
        private ISystemLogDAL _SystemLogDAL;
        public SystemLogBUS(ISystemLogDAL systemLogDAL)
        {
            _SystemLogDAL = systemLogDAL;
        }
        public int Insert(SystemLogModel systemLogInfo)
        {
            return _SystemLogDAL.Insert(systemLogInfo);
        }
        public int Insert_Notify(SystemLogModel systemLogInfo,ref int LogID)
        {
            return _SystemLogDAL.Insert_Notify(systemLogInfo,ref LogID);
        }
        public List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _SystemLogDAL.GetPagingBySearch(p, ref TotalRow);
        }

        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow)
        {
            return _SystemLogDAL.GetPagingByQuanTriDuLieu(p, ref TotalRow);
        }
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay)
        {
            return _SystemLogDAL.CreateLogFile(SystemLogPath,TuNgay,DenNgay);
        }
    }
}
