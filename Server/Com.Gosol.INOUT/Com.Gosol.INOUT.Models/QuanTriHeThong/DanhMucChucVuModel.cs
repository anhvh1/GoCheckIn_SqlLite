using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Gosol.INOUT.Models.DanhMuc
{
    public class DanhMucChucVuModel
    {
        [Required]
        public int ChucVuID { get; set; }
        [StringLength(50)]
        public string TenChucVu { get; set; }

        public string GhiChu { get; set; }
        public bool? KeKhaiHangNam{ get; set; }
        public DanhMucChucVuModel() { }
        public DanhMucChucVuModel(int ChucVuID, string TenChucVu, string GhiChu)
        {
            this.ChucVuID = ChucVuID;
            this.TenChucVu = TenChucVu;
            this.GhiChu = GhiChu;

        }
    }
}
