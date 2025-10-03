using Com.Gosol.INOUT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Model.QuanTriHeThong
{
    public class NguoidungInfo
    {
        public int NguoiDungID { get; set; }
        public string TenNguoiDung { get; set; }
        public string MatKhau { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public int CanBoID { get; set; }
        public int CoQuanID { get; set; }
        public int ChuoiNhaThuocid { get; set; }
        public int NhaThuocid { get; set; }
        public string TenCanBo { get; set; }
        public bool IsDangKy { get; set; }
        public DateTime NgayActive { get; set; }
        public string TenNhaThuoc { get; set; }
        public int Khoid { get; set; }
        public int RoleID { get; set; }
        public int PhanLoaiNhaThuoc { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public string TenChuoiNhaThuoc { get; set; }
        public string RoleName { get; set; }
        public int GioiTinh { get; set; }
        public int NhaThuocParentid { get; set; }
        
    }

    public class NguoidungParams : BasePagingParams
    {
        public int NhaThuocid { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class NguoidungDTO
    {
        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
                 
    }
    public class LoginInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
        public string device_token { get; set; }
        public string platform { get; set; }

    }
}
