using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.DanhMuc
{
    public interface IDanhMucTrangThaiBUS
    {
        List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel);
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel, int CoQuanID);
        public BaseResultModel Delete(List<int> ListTrangThaiID);
        public DanhMucTrangThaiModel GetByID(int TrangThaiID);
        public DanhMucTrangThaiModel GetByIDAndCoQuanID(int TrangThaiID, int? CoQuanID);
        public List<CoQuanTrangThaiModel> GetByCoQuanTrangThai(int? TrangThaiID, int? CoQuanID);
    }
    public class DanhMucTrangThaiBUS : IDanhMucTrangThaiBUS
    {
        private IDanhMucTrangThaiDAL _DanhMucTrangThaiDAL;
        public DanhMucTrangThaiBUS(IDanhMucTrangThaiDAL DanhMucTrangThaiDAL)
        {
            _DanhMucTrangThaiDAL = DanhMucTrangThaiDAL;
        }
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            return _DanhMucTrangThaiDAL.Insert(DanhMucTrangThaiModel);
        }
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel, int CoQuanID)
        {
            return _DanhMucTrangThaiDAL.Update(DanhMucTrangThaiModel, CoQuanID);
        }
        public BaseResultModel Delete(List<int> ListTrangThaiID)
        {
            return _DanhMucTrangThaiDAL.Delete(ListTrangThaiID);
        }
        public DanhMucTrangThaiModel GetByID(int TrangThaiID)
        {
            return _DanhMucTrangThaiDAL.GetByID(TrangThaiID);
        }
        public List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _DanhMucTrangThaiDAL.GetPagingBySearch(p, ref TotalRow);
        }
        public DanhMucTrangThaiModel GetByIDAndCoQuanID(int TrangThaiID, int? CoQuanID)
        {
            return _DanhMucTrangThaiDAL.GetByIDAndCoQuanID(TrangThaiID, CoQuanID);
        }

        public List<CoQuanTrangThaiModel> GetByCoQuanTrangThai(int? TrangThaiID, int? CoQuanID)
        {
            return _DanhMucTrangThaiDAL.GetByCoQuanTrangThai(TrangThaiID, CoQuanID);
        }
    }
}
