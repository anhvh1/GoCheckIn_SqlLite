using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models.QuanTriHeThong
{
    public class HuongDanSuDungModel
    {
        public int HuongDanSuDungID { get; set; }
        public int? ChucNangID { get; set; }
        public string TieuDe { get; set; }
        public string TenFileGoc { get; set; }
        public string TenFileHeThong { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public int NguoiCapNhat { get; set; }
        public int TrangThai { get; set; }
        public string TenChucNang { get; set; }
        public int ChucNangChaID { get; set; }
        public string MaChucNang { get; set; }
        public string Base64String { get; set; }
        public string UrlFile { get; set; }
        public HuongDanSuDungModel()
        {
        }


    }
}
