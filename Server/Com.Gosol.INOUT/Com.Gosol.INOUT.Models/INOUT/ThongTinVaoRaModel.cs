using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models.InOut
{
    public class ThongTinVaoRaModel : ThongTinKhachModel
    {
        public int ThongTinVaoRaID { get; set; }
        //public int ThongTinKhachID { get; set; }
        public DateTime GioVao { get; set; }
        public DateTime? GioRa { get; set; }
        public int LyDoGap { get; set; }
        public string LyDoKhac { get; set; }
        public int GapCanBo { get; set; }
        public int DonViCaNhan { get; set; } //1 - gặp đơn vị, 2 - gặp cá nhân
        public int? LeTan_Vao { get; set; }
        public int? LeTan_Ra { get; set; }
        public string MaThe { get; set; }

        public int FileDinhKemID { get; set; }
        public string TenFileHeThong { get; set; }
        public int GapBoPhanID { get; set; }
        public string TenBoPhanGap { get; set; }
        public string TenCanBoGap { get; set; }
        public string AnhCMND_MTBase64 { get; set; }
        public string AnhCMND_MSBase64 { get; set; }
        public string AnhChanDungBase64 { get; set; }
        public int LoaiFile { get; set; }
        public string TenCoQuan { get; set; }
        public int? DonViSuDungID { get; set; }
        public string TenDonViSuDung { get; set; }
        public string ThanNhiet { get; set; }
        public string GioiTinh { get; set; }
        public string LoaiGiayTo { get; set; }
        public string QRCode { get; set; }
        public string sessionCode { get; set; }
        public int CoQuanSuDungPhanMem { get; set; }
        public bool LaCanBo { get; set; }
        public int CanBoID { get; set; }

        public ThongTinVaoRaModel()
        {
        }
    }

    public class TongHopTheoNgay
    {
        public int Tong { get; set; }
        public int DangGap { get; set; }
        public int DaVe { get; set; }
    }

    public class DoiTuongGap
    {
        public int ID { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; } // 1 cơ quan, 2 cán bộ
        public List<DoiTuongGap> Children { get; set; }
        public bool? Active { get; set; } = false;
    }
}
