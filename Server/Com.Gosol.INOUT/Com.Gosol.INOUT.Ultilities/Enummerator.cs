using System;
using System.Web;

namespace Com.Gosol.INOUT.Ultilities
{

    #region AnhVH

    //InOut
    public enum EnumLoaiFile
    {
        MatTruocCMND = 1,
        MatSauCMND = 2,
        AnhChanDung = 3

    }
    public enum EnumLoaiCheckOut
    {
        MaThe = 1,
        CMND = 2,
        HoTen = 3

    }

    public enum EnumLyDoGap
    {
        Hop = 1,
        GapCanBo = 2,
        Khac = 3
    }


    /// <summary>
    /// //////////////
    /// </summary>

    public enum EnumLogType
    {
        Error = 0, // lỗi
        //Action = 1, // thực hiện các chức năng
        DangNhap = 100,

        Insert = 101,
        InsertWithNotify = 1011,// Thêm mới có thông báo
        InsertWithoutNotify =1022, // Thêm mới không thông báo
        Update = 102,
        UpdateWithNotify = 1021,  // update có thông báo
        UpdateWithoutNotify = 1022,//update không thông báo
        Delete = 103,
        DeleteWithNotify = 1031,  // Xoá có thông báo
        DeleteWithoutNotify =1032, // Xoá không thông báo
        GetByID = 201,// lấy dữ liệu theo ID
        GetByName = 202, // lấy dữ liệu theo tên, key
        GetList = 203, // lấy danh sách dữ liệu      

        BackupDatabase = 901,
        RestoreDatabase = 902,

        Other = 500,

    }

    /// <summary>
    /// loại log của người sử dụng
    /// </summary>
    public enum EnumSystemLogType
    {
        Error = 0, // lỗi
        HeThong = 1, // thao tác trên hệ thống các danh mục + Thêm, xoá Nhiệm vụ
        NghiepVu = 2, // thao tác trên nhiệm vụ, công việc


    }
    public enum EnumQuanHe : Int32
    {
        Vo = 1,
        Chong = 2,
        ConChuaThanhNien = 3,
    }

    public enum EnumTrangThaiDuyet
    {
        KeKhai = 100,                     // cán bộ tạo kê khai tài sản
        HuyPheDuyetCap1 = 101,          // Chủ tịch (xã, huyện) gửi lại bản kê khai kèm ghi chú, file đính kèm
        GuiBanKeKhai = 200,               // cán bộ gửi bản kê khai lên chủ tịch (Xã hoặc huyện)
        HuyDuyetCap2 = 201,             // Cán bộ phòng nội vụ/ thanh tra gửi lại bản kê khai kèm ghi chú, file đính kèm
        DuyetCap1 = 300,                  // Chủ tịch (Xã, Huyện) duyệt bản kê khai (duyệt cấp 1) 
        HoSoPhapLy = 400,                 // duyệt cấp 2



        // trạng thái < 200 thì người kê khai mới được sửa, xóa
        // trạng thái == 200 thì chủ tịch mới được duyệt lần 1
        // 200 <=  trạng thái < 300 thì người duyệt lần 1 mới được thao tác
        // Trạng thái == 300 thì cán bộ phòng nội vụ/ thanh tra mới được duyệt lần 2
        // trạng thái == 400 không được tác động tới nữa
    }

    public enum EnumPheDuyetBanKeKhai
    {
        PheDuyet = 1,
        XuLyLai = 2,
    }

    public enum EnumCapCoQuan : Int32
    {
        CapTrungUong = 0,
        CapTinh = 1,
        CapSo = 2,
        CapHuyen = 3,
        CapPhong = 4,
        CapXa = 5,
    }

    public enum EnumCapQuanLyCanBo : Int32
    {
        CapTinh = 1,
        CapHuyen = 2,
        ToanTinh = 3
    }
    public enum EnumLoaiDotKeKhai : Int32
    {
        HangNam = 1, // kê khai hàng năm
        BoSung = 2, // Kê khai bổ sung
        BoNhiem = 3, // Kê khai phục vụ công tác cán bộ
        LanDau = 4, // Kê khai lần đầu
    }



    public enum EnumTrangThaiCanBo
    {
        DangLamViec = 1,
        NghiHuu = 2,
    }
    public enum EnumVaiTroCanBo
    {
        Admin = 1,
        LanhDao = 2,
        TruongPhong = 3, // thêm 05/05/2020 by AnhVH
        ChuyenVien = 4, // sửa từ 3-4 ngày 05/05/2020 by AnhVH
    }

    /// <summary>
    /// biến động tài sản của cán bộ
    /// </summary>
    public enum EnumBienDongTaiSan : Int32
    {
        KhongBienDong = 0,
        Giam = 1,
        Tang = 2,
    }

    #endregion




    #region old
    public enum TrangThaiPhanGiaiQuyet
    {
        DangXuLy = 1,
        DangDuyet = 2,
        DaDuyet = 3
    }

    public enum DetailBookType
    {
        CashBook = 1,
        BankBook = 2,
        ADBook = 3,
        BCBook = 4,
        IBook = 5,
        PMCBook = 6,
        OPBook = 7,
        BDBook = 8,
        PDBook = 9,
        AsBook = 10,
        AsDBook = 11
    }

    public enum GeneralBookType
    {
        MainBook = 1,
        DDBook = 2,
        DDRBook = 3
    }

    public enum DocumentType
    {
        SoftCopy = 1, HardCopy = 2
    };

    public enum ValidType
    {
        Valid = 1,
        NotValid = 0
    }

    public enum RecruitmentType
    {
        State = 1, //Biên chế
        Recruit = 2, //Tiếp nhận
        Contract = 3, //Hợp đồng
        Job = 4, //Khoán
        Discontinue = 5 // Thôi, chuyển
    }

    public enum ActionType
    {
        EDanhMucS = 1,
        PMS = 2,
        HRMS = 3,
        CDanhMucS = 4,
        MS = 5
    }

    public enum FelicitationType
    {
        Reward = 1,
        Punish = 0
    }
    public enum ChuoiNhaThuocEnum
    {
        Gosolutions = 1
    }

    public enum VaiTroEnum
    {
        PhuTrach = 1,
        PhoiHop = 2,
        TheoDoi = 3
    }

    public enum EnumChucVu
    {
        LanhDao = 1,
        //TruongPhong = 2,
        NhanVien = 3,
    }

    public enum EnumLoaiHinhThuc
    {
        NhaThuocTuNhan = 1,
        ChuoiNhaThuoc = 2,
        NhaThuocBenhVien = 3,
    }

    public enum EnumNhomMacDinh
    {
        QuanTri = 1,
        NhanVien = 2,
        Staff = 3,
    }

    public enum EnumLoaiKiemKe
    {
        KiemKeQuayThuoc = 1,
        KiemKeNhaThuoc = 2,
    }

    public enum EnumTrangThaiCapNhat
    {
        ChuaCapNhat = 1,
        DaCapNhat = 2,
    }

    public enum EnumLoaiHuy
    {
        QuayThuoc = 2,
        NhaThuoc = 1,
    }

    public enum EnumLoaiKho
    {
        QuayThuoc = 1,
        NhaThuoc = 2,
        PhaChe = 5,
    }

    public enum EnumNoiLuuTru
    {
        Quay1 = 8,
        Quay2 = 12,
        NhaThuoc = 7,
    }

    public enum EnumKho
    {
        Quay1 = 8,
        Quay2 = 12,
        NhaThuoc = 7,
    }

    public enum EnumLoaiHoaDon
    {
        NhapThuocKhachHangTraLai = 2,
        BanLeThuocChoBenhNhan = 1,
    }





    public enum EnumTrangThaiTra
    {
        DaTra = 2,
        ChuaTra = 1,

    }

    public enum EnumTrangThaiLinh
    {
        ChoLinh = 1,
        DaLinh = 2,
        DaXuatChoQuay = 3, // phieu du tru da xuat cho quay
    }
    public enum EnumTrangThaiduTru
    {
        ChuaNhanYeuCau = 1,
        DaNhanYeuCau = 2,
    }
    public enum EnumTrangThaiKhoiTao
    {
        /*Trang thai khoi tao*/
        ChuaKhoiTao = 1,
        DaKhoiTao = 2,
    }
    public enum EnumTrangThaiXuatThuocPhaChe
    {
        ChuaXuat = 1,
        DaXuat = 2,
    }

    public enum EnumLoaiduTru
    {
        DuTruThuong = 1,
        DuTruXuatNhuong = 2,
        DuTruLinhThuocTuKho = 3,
    }
    public enum EnumLoaiduTru_QuayThuoc
    {
        DuTruDinhKy = 1,
        DuTruDotXuat = 2,
    }

    public enum EnumLoaiThuoc
    {
        ThuocNoi = 1,
        ThuocNgoai = 2,
    }

    public enum enumHuyThuocPhaChe
    {
        ChuaHuy = 1,
        DaHuy = 2,
    }
    public enum enumTrangThaiNhapDuocPhamPhaChe
    {
        ChuaNhan = 1,
        DaNhan = 2,
        DaHuy = 3,
        KhongNhan = 4,
    }
    // ant
    public enum EnumKieuNhapXuat
    {
        NhaThuoc = 1,
        QuayThuoc = 2,
    }

    public enum EnumHinhThucDangKyGoi
    {
        ThayDoiGoidichVu = 1,
        GiaHanSuDung = 2,
    }
    #endregion
};