using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.DanhMuc
{
    public interface IDanhMucCoQuanDonViBUS
    {
        public int Insert(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref int CoQuanID, int NguoiDungID, ref string Message);
        public int Update(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref string Message);
        public Dictionary<int, string> Delete(List<int> ListCoQuanID);
        public List<DanhMucCoQuanDonViModel> FilterByName(string TenCoQuan);
        public DanhMucCoQuanDonViPartialNew GetByID(int CoQuanID);
        public List<DanhMucCoQuanDonViModel> GetListByidAndCap();
        public List<DanhMucCoQuanDonViModelPartial> GetAllByCap(int ID, int Cap, string Keyword);
        public List<DanhMucCoQuanDonViModelPartial> GetALL(int ID, int CapCoQuanID, string Keyword);
        public List<DanhMucCoQuanDonViModel> GetListByUser(int CoQuanID, int NguoiDungID);
        public int ImportFile(string FilePath, ref string Message, int NguoiDungID);
        public List<DanhMucCoQuanDonViModel> GetListByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID);
        public List<DanhMucCoQuanDonViModel> GetByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID, string KeyWord);
        public BaseResultModel CheckMaCQ(int? CoQuanID, string MaCQ);
        public DanhMucCoQuanDonViModel GetCoQuanSuDungPhanMem_By_CoQuanDangNhap(int? CoQuanID);
    }
    public class DanhMucCoQuanDonViBUS : IDanhMucCoQuanDonViBUS
    {

        private IDanhMucCoQuanDonViDAL _DanhMucCoQuanDonViDAL;
        public DanhMucCoQuanDonViBUS(IDanhMucCoQuanDonViDAL coQuanDonViDAL)
        {
            this._DanhMucCoQuanDonViDAL = coQuanDonViDAL;
        }
        public int Insert(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref int CoQuanID, int NguoiDungID, ref string Message)
        {
            int val = 0;
            try
            {
                val = _DanhMucCoQuanDonViDAL.Insert(DanhMucCoQuanDonViModel, ref CoQuanID, NguoiDungID, ref Message);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        public int Update(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref string Message)
        {
            int val = 0;
            try
            {
                val = _DanhMucCoQuanDonViDAL.Update(DanhMucCoQuanDonViModel, ref Message);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        public Dictionary<int, string> Delete(List<int> ListCoQuanID)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            try
            {
                dic = _DanhMucCoQuanDonViDAL.Delete(ListCoQuanID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        public List<DanhMucCoQuanDonViModel> FilterByName(string TenCoQuan)
        {
            return _DanhMucCoQuanDonViDAL.FilterByName(TenCoQuan);
        }
        public DanhMucCoQuanDonViPartialNew GetByID(int CoQuanID)
        {
            return _DanhMucCoQuanDonViDAL.GetByID(CoQuanID);
        }
        public List<DanhMucCoQuanDonViModel> GetListByidAndCap()
        {
            return _DanhMucCoQuanDonViDAL.GetListByidAndCap();
        }
        public List<DanhMucCoQuanDonViModelPartial> GetAllByCap(int ID, int Cap, string Keyword)
        {
            return _DanhMucCoQuanDonViDAL.GetAllByCap(ID, Cap, Keyword);
        }

        public List<DanhMucCoQuanDonViModelPartial> GetALL(int ID, int CapCoQuanID, string Keyword)
        {
            return _DanhMucCoQuanDonViDAL.GetALL(ID, CapCoQuanID, Keyword);
        }

        public List<DanhMucCoQuanDonViModel> GetListByUser(int CoQuanID, int NguoiDungID)
        {
            return _DanhMucCoQuanDonViDAL.GetListByUser(CoQuanID, NguoiDungID);
        }
        public int ImportFile(string FilePath, ref string Message, int NguoiDungID)
        {
            return _DanhMucCoQuanDonViDAL.ImportFile(FilePath, ref Message, NguoiDungID);
        }

        public List<DanhMucCoQuanDonViModel> GetListByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID)
        {
            return _DanhMucCoQuanDonViDAL.GetListByUser_FoPhanQuyen(CoQuanID, NguoiDungID);
        }

        public List<DanhMucCoQuanDonViModel> GetByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID, string KeyWord)
        {
            return _DanhMucCoQuanDonViDAL.GetByUser_FoPhanQuyen(CoQuanID, NguoiDungID, KeyWord);
        }
        public BaseResultModel CheckMaCQ(int? CoQuanID, string MaCQ)
        {
            return _DanhMucCoQuanDonViDAL.CheckMaCQ(CoQuanID, MaCQ);
        }

        public DanhMucCoQuanDonViModel GetCoQuanSuDungPhanMem_By_CoQuanDangNhap(int? CoQuanID)
        {
            return _DanhMucCoQuanDonViDAL.GetCoQuanSuDungPhanMem_By_CoQuanDangNhap(CoQuanID);
        }
    }
}
