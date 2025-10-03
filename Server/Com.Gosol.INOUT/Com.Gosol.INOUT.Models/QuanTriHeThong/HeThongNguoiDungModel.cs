using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models.QuanTriHeThong
{
    public class HeThongNguoiDungModel
    {
        public int NguoiDungID { get; set; }
        public string TenNguoiDung { get; set;}
        public string MatKhau { get; set; }
        public string GhiChu { get; set; }
        public int? TrangThai { get; set; }
        public int? CanBoID { get; set; }
        public int? CoQuanID { get; set; }
        public string PublicKeys { get; set; }
        public string TenCanBo { get; set; }
        public string TenCoQuan { get; set; }
        public int? VaiTro { get; set; }
        public string MatKhauCu { get; set; }
        public HeThongNguoiDungModel() { }
        public HeThongNguoiDungModel(int NguoiDungID, string TenNguoiDung, string MatKhau, string GhiChu, int TrangThai, int CanBoID, string PublicKeys, int CoQuanID)
        {

            this.NguoiDungID = NguoiDungID;
            this.TenNguoiDung = TenNguoiDung;
            this.MatKhau = MatKhau;
            this.GhiChu = GhiChu;
            this.TrangThai = TrangThai;
            this.CanBoID = CanBoID;
            this.CoQuanID = CoQuanID;
            this.PublicKeys = PublicKeys;


        }

    }
    public class HeThongNguoiDungModelPartial
    {
        public int NguoiDungID { get; set; }
        public string TenNguoiDung { get; set; }
        public string MatKhau { get; set; }
        public string GhiChu { get; set; }
        public int? TrangThai { get; set; }

        public string TenCanBo { get; set; }
        public string PublicKeys { get; set; }
        public string TenCoQuan { get; set; }
        public int? CanBoID { get; set; }
        public int? CoQuanID { get; set; }
        public string Url { get; set; }
        public HeThongNguoiDungModelPartial()
        {

        }
        public HeThongNguoiDungModelPartial(int NguoiDungID, string TenNguoiDung, string MatKhau, string GhiChu, int TrangThai, string TenCanBo, string PublicKeys, string TenCoQuan, int? CanBoID, int? CoQuanID, string Url)
        {
            this.NguoiDungID = NguoiDungID;
            this.TenNguoiDung = TenNguoiDung;
            this.MatKhau = MatKhau;
            this.GhiChu = GhiChu;
            this.TrangThai = TrangThai;
            this.TenCanBo = TenCanBo;
            this.PublicKeys = PublicKeys;
            this.TenCoQuan = TenCoQuan;
            this.CanBoID = CanBoID;
            this.CoQuanID = CoQuanID;
            this.Url = Url;

        }
        public class DoiMatKhauModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public DoiMatKhauModel()
            {

            }
            public DoiMatKhauModel(string OldPassword, string NewPassword)
            {
                this.OldPassword = OldPassword;
                this.NewPassword = NewPassword;

            }
        }

    }
    public class QuenMatKhauModel
    {
        public int QuenMatKhauID { get; set; }
        public string TaiKhoan { get; set; }
        public string MaDuocGui { get; set; }
        public DateTime ThoiGianGui { get; set; }
        public bool TrangThai { get; set; }
        public string EmailGuiLink { get; set; }
        public QuenMatKhauModel()
        {


        }
        public QuenMatKhauModel(int QuenMatKhauID, string TaiKhoan, string MaDuocGui, DateTime ThoiGianGui, bool TrangThai, string EmailGuiLink)
        {
            this.QuenMatKhauID = QuenMatKhauID;
            this.TaiKhoan = TaiKhoan;
            this.MaDuocGui = MaDuocGui;
            this.ThoiGianGui = ThoiGianGui;
            this.TrangThai = TrangThai;
            this.EmailGuiLink = EmailGuiLink;
        }
    }
    public class QuenMatKhauModelPar
    {
        public string TenDangNhap { get; set; }
        public string MatKhauMoi { get; set; }
        public QuenMatKhauModelPar()
        {
                
        }
        public QuenMatKhauModelPar(string TenDangNhap, string MatKhauMoi)
        {
            this.TenDangNhap = TenDangNhap;
            this.MatKhauMoi = MatKhauMoi;
        }
    }
}
