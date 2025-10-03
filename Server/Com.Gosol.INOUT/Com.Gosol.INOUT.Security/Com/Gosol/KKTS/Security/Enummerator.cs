using System;
using System.Web;

namespace Com.Gosol.INOUT.Security
{
    public enum ChucNangEnum
    {
        // Quản trị hệ thống
        HT_QuanTriHeThong = 100,
        HT_QuanLyTaiKhoan = 101,
        HT_PhanQuyen = 102,
        HT_CauHinh = 103,
        HT_ThamSoHeThong = 104,
        HT_NhatKyHeThong = 105,
        HT_HuongDanSuDung = 106,
        HT_DanhMucCoQuan = 107,
        HT_DanhMucChucVu = 108,
        HT_DanhMucTrangThaiCongViec = 109,


        // Nhiệm vụ

        NV_DanhSachNhiemVu = 201,
        NV_DanhSachCongViec = 202,
        NV_PhanCongCongViec = 203,
        NV_XuLyCongViec = 204,


        //// Báo cáo, tra cứu
        //BC_BaoCao = 301,
        //TC_TraCuu = 401,


        //inout
        CheckIn_CheckOut = 200,
        BaoCao = 300,
        // có từ v2 - dùng để truy vết y tế: toàn tỉnh dùng cho admin và sở y tế. trong đơn vị dùng cho các cán bộ quản lý y tế của từng đơn vị và admin đơn vị
        TruyVetYTe = 400,
        TruyVetYTe_ToanTinh = 401,
        TruyVetYTe_TrongDonVi = 402




    }


};
