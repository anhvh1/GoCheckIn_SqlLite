using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Gosol.INOUT.Models.DanhMuc
{
    public class DanhMucTrangThaiModel
    {
        [Required]
        public int TrangThaiID { get; set; }
        [StringLength(50)]
        public string TenTrangThai { get; set; }
        public bool? TrangThaiSuDung { get; set; }

        public bool ChoPhepSua { get; set; } = true;
        public int? VaiTro { get; set; }
        public List<int> ListRoleID { get; set; }
        public DanhMucTrangThaiModel() { }
        public DanhMucTrangThaiModel(int TrangThaiID, string TenTrangThai, bool TrangThaiSuDung)
        {
            this.TrangThaiID = TrangThaiID;
            this.TenTrangThai = TenTrangThai;
            this.TrangThaiSuDung = TrangThaiSuDung;
        }
    }
}
