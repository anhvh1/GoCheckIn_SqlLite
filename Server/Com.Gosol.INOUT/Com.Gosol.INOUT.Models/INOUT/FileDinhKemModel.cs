using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models.InOut
{
    public class FileDinhKemModel
    {
        public int FileDinhKemID { get; set; }
        public string TenFileGoc { get; set; }
        public string TenFileHeThong { get; set; }
        public int? LoaiFile { get; set; } // 1 Mặt trước, CMND, 2 Mặt sau CMND, 3 Ảnh chân dung
        public string FileUrl { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr { get; set; }
        public Boolean? CoBaoMat { get; set; }
        public Boolean? LaCanBo { get; set; }
        public int? NguoiTaoID { get; set; }
        public int? ThongTinVaoRaID { get; set; }
        public string Base64File { get; set; }
        //public int? FileVanBanOrNhiemVu { get; set; }
        public int LoaiUpload { get; set; } // 1 - scan (Base64File is not null); 2 - chụp ảnh
        public int? FeatureID { get; set; }
    }
}
