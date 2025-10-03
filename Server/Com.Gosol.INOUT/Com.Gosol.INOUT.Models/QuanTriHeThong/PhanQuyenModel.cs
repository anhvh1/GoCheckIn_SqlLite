using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models.QuanTriHeThong
{
    public class NhomNguoiDungModel
    {
        public int NhomNguoiDungID { get; set; }
        public String TenNhom { get; set; }
        public String GhiChu { get; set; }
        public int CoQuanID { get; set; }
        public int? NhomTongID { get; set; }
        public int? ApDungCho { get; set; }
        public string TenCoQuan { get; set; }
        public List<int> DanhSachCoQuanID { get; set; }
        public List<CoQuanModel> DanhSachCoQuan { get; set; }
        public int? CoQuanTao { get; set; }
        public NhomNguoiDungModel()
        {

        }
        public NhomNguoiDungModel(int NhomNguoiDungID, string TenNhom, string GhiChu, int ApDungCho)
        {
            this.NhomNguoiDungID = NhomNguoiDungID;
            this.TenNhom = TenNhom;
            this.GhiChu = GhiChu;
            this.ApDungCho = ApDungCho;
        }
        public NhomNguoiDungModel(int NhomNguoiDungID, string TenNhom, string GhiChu, int CoQuanID, int NhomTongID, int ApDungCho, List<int> DanhSachCoQuanID)
        {
            this.NhomNguoiDungID = NhomNguoiDungID;
            this.TenNhom = TenNhom;
            this.GhiChu = GhiChu;
            this.CoQuanID = CoQuanID;
            this.NhomTongID = NhomTongID;
            this.ApDungCho = ApDungCho;
            this.DanhSachCoQuanID = DanhSachCoQuanID;
        }
    }

    public class NhomNguoiDungDetailModel : NhomNguoiDungModel
    {
        public List<NguoiDungModel> DanhSachNguoiDung { set; get; }
        public List<ChucNangModel> DanhSachChucNang { set; get; }
        public List<NhomNguoiDungModel> NhomNguoiDungCon { get; set; }

    }

    public class PhanQuyenModel
    {
        public int PhanQuyenID { set; get; }
        public int NhomNguoiDungID { set; get; }
        public int ChucNangID { set; get; }
        public int Quyen { set; get; } = 0;
        public int? Xem { set; get; } = 0;
        public int? Them { set; get; } = 0;
        public int? Sua { set; get; } = 0;
        public int? Xoa { set; get; } = 0;
        public PhanQuyenModel() { }
        public PhanQuyenModel(int NhomNguoiDungID, int ChucNangID, int Xem, int Them, int Sua, int Xoa)
        {
            this.NhomNguoiDungID = NhomNguoiDungID;
            this.ChucNangID = ChucNangID;
            this.Quyen += (Xem == 1 ? 1 : 0) + (Them == 1 ? 2 : 0) + (Sua == 1 ? 4 : 0) + (Xoa == 1 ? 8 : 0);
            this.Xem = Xem; this.Them = Them; this.Sua = Sua; this.Xoa = Xoa;
        }
    }

    public class NguoiDungNhomNguoiDungModel
    {
        public int NhomNguoiDungID { set; get; }
        public int NguoiDungID { set; get; }
        public string TenNguoiDung { set; get; }
    }

    public class CoQuanModel
    {
        public int CoQuanID { get; set; }
        public string TenCoQuan { get; set; }
        public CoQuanModel(int CoQuanID, string TenCoQuan)
        {
            this.CoQuanID = CoQuanID;
            this.TenCoQuan = TenCoQuan;
        }
    }
}
