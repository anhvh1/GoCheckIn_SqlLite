using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models.DanhMuc
{
    public class DanhMucCapCongTrinhModel
    {
        public int CapCongTrinhID { get; set; }
        public string TenCapCongTrinh { get; set; }
        public int CoQuanID { get; set; }
        public bool? TrangThaiSuDung { get; set; }
    }
}
