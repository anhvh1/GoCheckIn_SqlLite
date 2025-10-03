using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.QuanTriHeThong
{
    public interface ISystemConfigBUS
    {
        List<SystemConfigModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(SystemConfigModel SystemConfigModel);
        public BaseResultModel Update(SystemConfigModel SystemConfigModel);
        public BaseResultModel Delete(List<int> ListSystemConfigID);
        public SystemConfigModel GetByID(int SystemConfigID);
        public SystemConfigModel GetByKey(string ConfigKey);
    }
    public class SystemConfigBUS : ISystemConfigBUS
    {
        private ISystemConfigDAL _SystemConfigDAL;
        public SystemConfigBUS(ISystemConfigDAL SystemConfigDAL)
        {
            _SystemConfigDAL = SystemConfigDAL;
        }
        public BaseResultModel Insert(SystemConfigModel SystemConfigModel)
        {
            return _SystemConfigDAL.Insert(SystemConfigModel);
        }
        public BaseResultModel Update(SystemConfigModel SystemConfigModel)
        {
            return _SystemConfigDAL.Update(SystemConfigModel);
        }
        public BaseResultModel Delete(List<int> ListSystemConfigID)
        {
            return _SystemConfigDAL.Delete(ListSystemConfigID);
        }
        public SystemConfigModel GetByID(int SystemConfigID)
        {
            return _SystemConfigDAL.GetByID(SystemConfigID);
        }
        public SystemConfigModel GetByKey(string ConfigKey)
        {
            return _SystemConfigDAL.GetByKey(ConfigKey);
        }
        public List<SystemConfigModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _SystemConfigDAL.GetPagingBySearch(p, ref TotalRow);
        }

       
    }
}
