using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.DanhMuc
{
    public interface IDanhMucNhomTaiSanBUS
    {
        public int Insert(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message);
        public int Update(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message);
        public List<string> Delete(List<int> NhomTaiSanID);
        public DanhMucNhomTaiSanModel GetNTSByID(int NhomTaiSanID);
        public List<DanhMucNhomTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public List<DanhMucNhomTaiSanModel> GetAllNhomTaiSanCha();
    }
    public class DanhMucNhomTaiSanBUS : IDanhMucNhomTaiSanBUS
    {
        public int Insert(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message)
        {
            int val = 0;
            try
            {
                val = new DanhMucNhomTaiSanDAL().Insert(DanhMucNhomTaiSanModel, ref Message);
                return val;
            }

            catch (Exception ex)
            {
                return val;
                throw ex;
            }

        }
        public int Update(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message)
        {
            int val = 0;
            try
            {
                val = new DanhMucNhomTaiSanDAL().Update(DanhMucNhomTaiSanModel, ref Message);
                return val;
            }

            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        public List<string> Delete(List<int> ListNhomTaiSanID)
        {
            List<string> dic = new List<string>();
            try
            {
                dic = new DanhMucNhomTaiSanDAL().Delete(ListNhomTaiSanID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        public List<DanhMucNhomTaiSanModel> GetAllNTS()
        {
            var list = new List<DanhMucNhomTaiSanModel>();
            try
            {
                list = new DanhMucNhomTaiSanDAL().GetAllNTS();
                return list;
            }
            catch (Exception ex)
            {
                return new List<DanhMucNhomTaiSanModel>();
                throw ex;
            }
        }
        public DanhMucNhomTaiSanModel GetNTSByID(int NhomTaiSanID)
        {
            var list = new DanhMucNhomTaiSanModel();
            try
            {
                list = new DanhMucNhomTaiSanDAL().GetNTSByID(NhomTaiSanID);
                return list;
            }
            catch (Exception ex)
            {
                return new DanhMucNhomTaiSanModel();
                throw ex;
            }
        }
        //public List<DanhMucNhomTaiSanModel> FilterByName(string TenNhomTaiSan)
        //{
        //    var list = new List<DanhMucNhomTaiSanModel>();
        //    try
        //    {
        //        list = new DanhMucNhomTaiSanDAL().FilterByName(TenNhomTaiSan);
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<DanhMucNhomTaiSanModel>();
        //        throw ex;
        //    }
        //}
        public List<DanhMucNhomTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            var list = new List<DanhMucNhomTaiSanModel>();
            try
            {
                list = new DanhMucNhomTaiSanDAL().GetPagingBySearch(p, ref TotalRow);
                return list;
            }
            catch (Exception ex)
            {
                return new List<DanhMucNhomTaiSanModel>();
                throw;
            }
        }

        public List<DanhMucNhomTaiSanModel> GetAllNhomTaiSanCha()
        {
            return new DanhMucNhomTaiSanDAL().GetAllNhomTaiSanCha();
        }
    }
}
