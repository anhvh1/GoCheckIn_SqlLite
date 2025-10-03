using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models.InOut
{
    public class ThongTinKhachModel
    {
        public int ThongTinKhachID { get; set; }
        public string HoVaTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string HoKhau { get; set; }
        public string DienThoai { get; set; }
        public string SoCMND { get; set; }
        public string NoiCapCMND { get; set; }
        public DateTime? NgayCapCMND { get; set; }
        public int FaceID { get; set; }

        public ThongTinKhachModel()
        {
        }
    }
}
