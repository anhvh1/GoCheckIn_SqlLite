using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.DanhMuc
{
    public interface IDanhMucLoaiTaiSanBUS
    {
        public int Insert(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message);
        public int Update(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message);
        public List<string> Delete(List<int> ListLoaiTaiSanID);
        public DanhMucLoaiTaiSanModel GetByID(int LoaiTaiSanID);
        //public List<DanhMucLoaiTaiSanModel> FilterByName(string tenLoaiTaiSan);
        public List<DanhMucLoaiTaiSanModel> GetAll();
        public List<DanhMucLoaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
    }
    public class DanhMucLoaiTaiSanBUS : IDanhMucLoaiTaiSanBUS
    {

        // Insert loai tai san
        public int Insert(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message)
        {
            int val = 0;
            try
            {
                val = new DanhMucLoaiTaiSanDAL().Insert(DanhMucLoaiTaiSanModel, ref Message);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        // Update loai tai san
        public int Update(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message)
        {
            int val = 0;
            try
            {
                val = new DanhMucLoaiTaiSanDAL().Update(DanhMucLoaiTaiSanModel, ref Message);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        // delete loai tai san
        public List<string> Delete(List<int> ListLoaiTaiSanID)
        {
            List<string> dic = new List<string>();
            try
            {
                dic = new DanhMucLoaiTaiSanDAL().Delete(ListLoaiTaiSanID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        // Get by id
        public DanhMucLoaiTaiSanModel GetByID(int LoaiTaiSanID)
        {
            var loaiTaiSan = new DanhMucLoaiTaiSanModel();
            try
            {
                loaiTaiSan = new DanhMucLoaiTaiSanDAL().GetLTSByID(LoaiTaiSanID);
                return loaiTaiSan;
            }
            catch (Exception ex)
            {
                return new DanhMucLoaiTaiSanModel();
                throw;
            }
        }
        // Filter by Name
        //public List<DanhMucLoaiTaiSanModel> FilterByName(string tenLoaiTaiSan)
        //{
        //    var list = new List<DanhMucLoaiTaiSanModel>();
        //    try
        //    {
        //        list = new DanhMucLoaiTaiSanDAL().FilterByName(tenLoaiTaiSan);
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<DanhMucLoaiTaiSanModel>();
        //        throw;
        //    }
        //}
        public List<DanhMucLoaiTaiSanModel> GetAll()
        {
            var list = new List<DanhMucLoaiTaiSanModel>();
            try
            {
                list = new DanhMucLoaiTaiSanDAL().GetAll();
                return list;
            }
            catch (Exception ex)
            {
                return new List<DanhMucLoaiTaiSanModel>();
                throw;
            }
        }
        public List<DanhMucLoaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            var list = new List<DanhMucLoaiTaiSanModel>();
            try
            {
                list = new DanhMucLoaiTaiSanDAL().GetPagingBySearch(p, ref TotalRow);
                return list;
            }
            catch (Exception ex)
            {
                return new List<DanhMucLoaiTaiSanModel>();
                throw;
            }
        }
    }
}
