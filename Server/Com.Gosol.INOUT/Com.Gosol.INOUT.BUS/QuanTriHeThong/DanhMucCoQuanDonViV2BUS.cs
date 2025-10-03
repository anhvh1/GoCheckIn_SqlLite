using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.DanhMuc
{
    public interface IDanhMucCoQuanDonViV2BUS
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
        public BaseResultModel KhoiTaoDonViSuDungPhanMem(ThongTinDangKyModel DangKyInfo);
        public List<ThongTinDangKyModel> DanhSachDonViSuDungPhanMem(BasePagingParams p, ref int TotalRow);
        public BaseResultModel CapNhatTrangThaiSuDungPhanMem(int CoQuanID, bool? TrangThai);
        public ThongTinDangKyModel ChiTietCoQuanSuDungPhanMem(int CoQuanID);
        public BaseResultModel CapNhatThongTinDonViSuDungPhanMem(ThongTinDangKyModel data);
        public List<HeThongCanBoV2Model> DanhSachCanBoThuocDonViSuDungPhanMen(BasePagingParamsForFilter p, ref int TotalRow);
        public BaseResultModel ThemCanBoVaoDonViSuDungPhanMen(ThongTinDangKyModel DangKyInfo);
        public BaseResultModel SuaPhongBan(int CoQuanID, string TenCoQuan);
        public BaseResultModel XoaPhongBan(int CoQuanID);
    }
    public class DanhMucCoQuanDonViV2BUS : IDanhMucCoQuanDonViV2BUS
    {

        private IDanhMucCoQuanDonViV2DAL _DanhMucCoQuanDonViDAL;
        public DanhMucCoQuanDonViV2BUS(IDanhMucCoQuanDonViV2DAL coQuanDonViDAL)
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

        public BaseResultModel KhoiTaoDonViSuDungPhanMem(ThongTinDangKyModel DangKyInfo)
        {
            return _DanhMucCoQuanDonViDAL.KhoiTaoDonViSuDungPhanMem(DangKyInfo);
        }

        public List<ThongTinDangKyModel> DanhSachDonViSuDungPhanMem(BasePagingParams p, ref int TotalRow)
        {
            return _DanhMucCoQuanDonViDAL.DanhSachDonViSuDungPhanMem(p, ref TotalRow);
        }

        public BaseResultModel CapNhatTrangThaiSuDungPhanMem(int CoQuanID, bool? TrangThai)
        {
            return _DanhMucCoQuanDonViDAL.CapNhatTrangThaiSuDungPhanMem(CoQuanID, TrangThai);
        }

        public ThongTinDangKyModel ChiTietCoQuanSuDungPhanMem(int CoQuanID)
        {
          return  _DanhMucCoQuanDonViDAL.ChiTietDonViSuDungPhanMem(CoQuanID);
        }

        public BaseResultModel CapNhatThongTinDonViSuDungPhanMem(ThongTinDangKyModel data)
        {
            return _DanhMucCoQuanDonViDAL.CapNhatThongTinDonViSuDungPhanMem(data);
        }

        public List<HeThongCanBoV2Model> DanhSachCanBoThuocDonViSuDungPhanMen(BasePagingParamsForFilter p, ref int TotalRow)
        {
            return _DanhMucCoQuanDonViDAL.DanhSachCanBoThuocDonViSuDungPhanMen(p, ref TotalRow);
        }

        public BaseResultModel ThemCanBoVaoDonViSuDungPhanMen(ThongTinDangKyModel DangKyInfo)
        {
           return _DanhMucCoQuanDonViDAL.ThemCanBoVaoDonViSuDungPhanMen(DangKyInfo);
        }

        public BaseResultModel SuaPhongBan(int CoQuanID, string TenCoQuan)
        {
            return _DanhMucCoQuanDonViDAL.SuaPhongBan(CoQuanID,TenCoQuan);  
        }

        public BaseResultModel XoaPhongBan(int CoQuanID)
        {
            return _DanhMucCoQuanDonViDAL.XoaPhongBan(CoQuanID);    
        }
    }
}
