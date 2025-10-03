using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models.QuanTriHeThong
{
    public class CoQuanTrangThaiModel
    {
        public int TrangThaiCongViecCoQuanID { get; set; }
        public int TrangThaiCongViecID { get; set; }
        public int CoQuanID { get; set; }
        //public bool? TrangThaiSuDung { get; set; }
        public int VaiTro { get; set; }
        public CoQuanTrangThaiModel() { }
        public CoQuanTrangThaiModel(int TrangThaiCongViecCoQuanID, int TrangThaiCongViecID, int CoQuanID, int VaiTro)
        {
            this.TrangThaiCongViecCoQuanID = TrangThaiCongViecCoQuanID;
            this.TrangThaiCongViecID = TrangThaiCongViecID;
            this.CoQuanID = CoQuanID;
            this.VaiTro = VaiTro;
        }
    }
}
