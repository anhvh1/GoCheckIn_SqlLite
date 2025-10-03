using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models.QuanTriHeThong
{
    public class CanBoModel
    {
        public int CanBoID { get; set; }
        public string TenCanBo { get; set; }
        public DateTime NgaySinh { get; set; }
        public string NgaySinhStr { get; set; }
        public int GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public int ChuoiNhaThuocid { get; set; }
        public int NhaThuocid { get; set; }
        public int Khoid { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string TenChuoiNhaThuoc { get; set; }
        public string TenNhaThuoc { get; set; }
        public int LoaiHinhThuc { get; set; }
        public int NhaThuocParentid { get; set; }
    }
    public class NhanVienDTO
    {
        public int NhanVienid { get; set; }
        public string TenNhanVien { get; set; }

    }
    public class CanBoPartialModel : CanBoModel
    {
        public int ID { get; set; }

        public string Ten { get; set; }
    }
}
