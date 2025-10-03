using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Com.Gosol.INOUT.Models.DanhMuc
{
    public class DanhMucLoaiTaiSanModel
    {
        [Required]
        public int LoaiTaiSanID { get; set; }
        public int? NhomTaiSanID { get; set; }
        [StringLength(50)]
        public string TenLoaiTaiSan { get; set; }
        public bool?  TrangThaiSuDung{ get; set; }
        public DanhMucLoaiTaiSanModel() { }
        public DanhMucLoaiTaiSanModel(int LoaiTaiSanID,int NhomTaiSanID,string tenLoaiTaiSan,bool TrangThaiSuDung)
        {
            this.LoaiTaiSanID = LoaiTaiSanID;
            this.NhomTaiSanID = NhomTaiSanID;
            this.TenLoaiTaiSan = tenLoaiTaiSan;
            this.TrangThaiSuDung = TrangThaiSuDung;
        }
    }
}
