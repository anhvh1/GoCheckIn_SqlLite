using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.DanhMuc
{
    public interface IDanhMucChucVuBUS
    {
        List<DanhMucChucVuModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(DanhMucChucVuModel DanhMucChucVuModel);
        public BaseResultModel Update(DanhMucChucVuModel DanhMucChucVuModel);
        public List<string> Delete(List<int> ListChucVuID);
        public DanhMucChucVuModel GetByID(int? ChucVuID);
        public BaseResultModel ImportChucVu(string FilePath);
    }
    public class DanhMucChucVuBUS : IDanhMucChucVuBUS
    {
        private IDanhMucChucVuDAL _DanhMucChucVuDAL;
        public DanhMucChucVuBUS(IDanhMucChucVuDAL danhMucChucVuDAL)
        {
            _DanhMucChucVuDAL = danhMucChucVuDAL;
        }
        public BaseResultModel Insert(DanhMucChucVuModel DanhMucChucVuModel)
        {
            return _DanhMucChucVuDAL.Insert(DanhMucChucVuModel);
        }
        public BaseResultModel Update(DanhMucChucVuModel DanhMucChucVuModel)
        {
            return _DanhMucChucVuDAL.Update(DanhMucChucVuModel);
        }
        public List<string> Delete(List<int> ListChucVuID)
        {
            return _DanhMucChucVuDAL.Delete(ListChucVuID);
        }
        public DanhMucChucVuModel GetByID(int? ChucVuID)
        {
            return _DanhMucChucVuDAL.GetChucVuByID(ChucVuID);
        }     
        public List<DanhMucChucVuModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            return _DanhMucChucVuDAL.GetPagingBySearch(p, ref TotalRow);
        }
        public BaseResultModel ImportChucVu(string FilePath)
        {
            return _DanhMucChucVuDAL.ImportChucVu(FilePath);
        }
    }
}
