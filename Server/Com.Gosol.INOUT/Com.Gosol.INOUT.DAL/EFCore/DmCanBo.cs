using System;
using System.Collections.Generic;

namespace Com.Gosol.INOUT.DAL.EFCore
{
    public partial class DanhMucCanBo
    {
        public long CanBoId { get; set; }
        public string TenCanBo { get; set; }
        public DateTime? NgaySinh { get; set; }
        public sbyte? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public long? ChucVuId { get; set; }
        public sbyte? QuyenKy { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }
        public long? PhongBanId { get; set; }
        public long? CoQuanId { get; set; }
        public long? RoleId { get; set; }
        public int? QuanTridonVi { get; set; }
        public long? CoQuanCuId { get; set; }
        public long? CanBoCuId { get; set; }
        public sbyte? XemTaiLieuMat { get; set; }
        public bool? IsStatus { get; set; }

        public virtual DanhMucChucVu ChucVu { get; set; }
    }
}
