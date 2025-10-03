using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models.DanhMuc
{
    public class DanhMucNhomTaiSanModel { 
   
        public DanhMucNhomTaiSanModel(int NhomTaiSanID, int NhomTaiSanChaID, string TenNhomTaiSan, int CoQuanID, bool TrangThaiSuDung, int ThuTuSapXep, string MoTa)
        {
            this.NhomTaiSanID = NhomTaiSanID;
            this.NhomTaiSanChaID = NhomTaiSanChaID;
            this.TenNhomTaiSan = TenNhomTaiSan;
            this.CoQuanID = CoQuanID;
            this.TrangThaiSuDung = TrangThaiSuDung;
            this.ThuTuSapXep = ThuTuSapXep;
            this.MoTa = MoTa;

        }
        public  DanhMucNhomTaiSanModel() { }
        public int NhomTaiSanID { get; set; }
        public int? NhomTaiSanChaID { get; set; }
        public string TenNhomTaiSan { get; set; }
        public int? CoQuanID { get; set; }
        public bool? TrangThaiSuDung { get; set; }
        public int? ThuTuSapXep { get; set; }
        public string MoTa { get; set; }
    }
}
