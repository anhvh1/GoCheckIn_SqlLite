using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Ultilities
{
    public static class ConstantLogMessage
    {
        public static readonly string CreateLogFile = "Tao file log";
        #region API Mess
        public static readonly string API_Success = "Thực hiện thành công!";
        public static readonly string API_Error = "Có lỗi! Thử lại!";
        public static readonly string API_Error_Duplicate = "Có trường trùng lặp với đối tượng khác !Thử lại!";
        public static readonly string API_Error_System = "Lỗi hệ thống";
        public static readonly string API_Error_NotSelected = "Chưa chọn đối tượng hoặc đối tượng không tồn tại! Thử lại";
        public static readonly string API_Error_Exist = "Đối tượng đã tồn tại ! Thử lại!";
        public static readonly string API_Error_NotExist = "Đối tượng có trường không tồn tại ! Thử lại!";
        public static readonly string API_Error_NotSpecialCharacter = "Không thể chứa kí tự đặc biệt ! Thử lại!";
        public static readonly string API_Error_NotFill = "Không được để trống trường bắt buộc!";
        public static readonly string API_Insert_Success = "Thêm mới thành công!";
        public static readonly string API_Update_Success = "Update thành công";
        public static readonly string API_Delete_Success = "Xóa thành công";
        public static readonly string API_NoData = "Không có dữ liệu";
        public static readonly string API_Error_TooLong = "Dữ liệu nhập vượt quá số ký tự quy định. Vui lòng kiểm tra lại";
        public static readonly string API_Success_Import = "Import dữ liệu thành công!";
        public static readonly string API_Error_Email = "Email không đúng định dạng!";
        public static readonly string API_Error_PhoneNumBer = "Số điện thoại không đúng định dạng!";
        public static readonly string API_SendMail = "Gửi Mail!";
        public static readonly string API_Error_Status = "Trạng thái sử dụng không đúng định dạng";
        public static readonly string API_Export_ExelFile = "Export Exel file";
        #endregion
        #region Danh Mục
        //Chức Vụ
        public static readonly string DM_ChucVu_GetListPaging = "Lấy danh sách phân trang danh mục chức vụ";
        public static readonly string DM_ChucVu_Error_Exist = "Chức vụ đã tồn tại. Vui lòng kiểm tra lại";
        public static readonly string DM_ChucVu_ThemChucVu = "Thêm mới danh mục Chức Vụ";
        public static readonly string DM_ChucVu_SuaChucVu = "Sửa danh mục chức vụ";
        public static readonly string DM_ChucVu_XoaChucVu = "Xóa danh mục chức vụ";
        public static readonly string DM_ChucVu_GetByID = "Get By id danh mục chức vụ";
        public static readonly string DM_ChucVu_FilterByName = "Lấy danh sách danh mục chức vụ";
        public static readonly string DM_ChucVu_ImportFile = "Import File Chức Vụ";
        //Loại Tài Sản
        public static readonly string DanhMuc_LoaiTaiSan_GetListPaging = "Lấy danh sách phân trang danh mục loại tài sản";
        public static readonly string DanhMuc_LoaiTaiSan_ThemLoaiTaiSan = "Thêm danh mục loại tài sản";
        public static readonly string DanhMuc_LoaiTaiSan_SuaLoaiTaiSan = "Sửa danh mục loại tài sản";
        public static readonly string DanhMuc_LoaiTaiSan_XoaLoaiTaiSan = "Xóa danh mục loại tài sản";
        public static readonly string DanhMuc_LoaiTaiSan_FilterByName = "Lọc theo tên danh mục loại tài sản";
        public static readonly string DanhMuc_LoaiTaiSan_GetByID = "Lấy theo id danh mục loại tài sản";
        // Nhóm Tài Sản
        public static readonly string DanhMuc_NhomTaiSan_GetListPaging = "Lấy danh sách phân trang danh mục nhóm tài sản";
        public static readonly string DanhMuc_NhomTaiSan_ThemNhomTaiSan = "Thêm danh mục nhóm tài sản";
        public static readonly string DanhMuc_NhomTaiSan_SuaNhomTaiSan = "Sửa danh mục nhóm tài sản";
        public static readonly string DanhMuc_NhomTaiSan_XoaNhomTaiSan = "Xóa danh mục nhóm tài sản";
        public static readonly string DanhMuc_NhomTaiSan_FilterByName = "Lọc theo tên danh mục loại tài sản";
        public static readonly string DanhMuc_NhomTaiSan_GetByID = "Lấy theo id danh mục nhóm tài sản";
        //Địa Giới Hành Chính
        public static readonly string DanhMuc_DiaGioiHanhChinh_GetListByidAndCap = "Lấy danh sách danh mục địa giới hành chính";
        public static readonly string DanhMuc_DiaGioiHanhChinh_ThemDGHC = "Thêm danh mục cơ quan địa giới hành chính ";
        public static readonly string DanhMuc_DiaGioiHanhChinh_SuaDGHC = "Sửa danh mục cơ quan địa giới hành chính ";
        public static readonly string DanhMuc_DiaGioiHanhChinh_XoaDGHC = "Xóa danh mục cơ quan địa giới hành chính ";
        public static readonly string DanhMuc_DiaGioiHanhChinh_ThemHuyen = "Thêm danh mục cơ quan địa giới hành chính Huyện";
        public static readonly string DanhMuc_DiaGioiHanhChinh_SuaHuyen = "Sửa danh mục cơ quan địa giới hành chính Huyện";
        public static readonly string DanhMuc_DiaGioiHanhChinh_XoaHuyen = "Xóa danh mục cơ quan địa giới hành chính Huyện";
        public static readonly string DanhMuc_DiaGioiHanhChinh_ThemXa = "Thêm danh mục cơ quan địa giới hành chính Xã";
        public static readonly string DanhMuc_DiaGioiHanhChinh_SuaXa = "Sửa danh mục cơ quan địa giới hành chính Xã";
        public static readonly string DanhMuc_DiaGioiHanhChinh_XoaXa = "Xóa danh mục cơ quan địa giới hành chính Xã";
        public static readonly string DanhMuc_DiaGioiHanhChinh_FilterByName = "Lọc theo tên danh mục cơ quan địa giới hành chính";
        public static readonly string DanhMuc_DiaGioiHanhChinh_GetByID = "Lấy theo mã định danh danh mục cơ quan địa giới hành chính";
        //Cơ Quan Đơn Vị
        public static readonly string DanhMuc_CoQuanDonVi_GetAllByCap = "Lấy danh sách theo cấp cơ quan đơn vị";
        public static readonly string DanhMuc_CoQuanDonVi_GetListPaging = "Lấy danh sách phân trang danh mục cơ quan đơn vị";
        public static readonly string DanhMuc_CoQuanDonVi_ThemCoQuanDonVi = "Thêm danh mục cơ quan cơ quan đơn vị";
        public static readonly string DanhMuc_CoQuanDonVi_SuaCoQuanDonVi = "Sửa danh mục cơ quan đơn vị";
        public static readonly string DanhMuc_CoQuanDonVi_XoaCoQuanDonVi = "Xóa danh mục cơ quan đơn vị";
        public static readonly string DanhMuc_CoQuanDonVi_FilterByName = "Lọc theo tên danh mục cơ quan đơn vị";
        public static readonly string DanhMuc_CoQuanDonVi_GetByID = "Lấy theo id danh mục cơ quan đơn vị";
        public static readonly string DanhMuc_CoQuanDonVi_GetForPhanQuyen = "Lấy danh sách phân trang danh mục cơ quan đơn vị";

        //danh mục trạng thái
        public static readonly string DanhMuc_TrangThai_GetListPaging = "Lấy danh sách phân trang danh mục Trạng thái";
        public static readonly string DanhMuc_TrangThai_Them = "Thêm danh mục Trạng thái";
        public static readonly string DanhMuc_TrangThai_Sua = "Sửa danh mục Trạng thái";
        public static readonly string DanhMuc_TrangThai_Xoa = "Xóa danh mục Trạng thái";
        public static readonly string DanhMuc_TrangThai_FilterByName = "Lọc theo tên danh mục Trạng thái";
        public static readonly string DanhMuc_TrangThai_GetByID = "Lấy Danh mục Trạng thái theo ID";
        //
        public static readonly string DanhMuc_CapCongTrinh_GetListPaging = "Lấy danh sách phân trang danh mục cấp công trình";
        public static readonly string DanhMuc_CapCongTrinh_ThemCapCongTrinh = @"Thêm mới danh mục cấp công trình";
        public static readonly string DanhMuc_CapCongTrinh_SuaCapCongTrinh = "Sửa danh mục cấp công trình";
        public static readonly string DanhMuc_CapCongTrinh_XoaCapCongTrinh = "Xóa danh mục cấp công trình";
        public static readonly string DanhMuc_CapCongTrinh_GetByID = "Lấy danh mục Cấp công trình theo ID";
        public static readonly string DanhMuc_CapCongTrinh_FilterByName = "Lấy danh sách danh mục cấp công trình theo tên";
        #endregion
        #region Hệ thống
        //Cán Bộ
        public static readonly string HT_CanBo_GetListPaging = "Lấy danh sách phân trang hệ thống cán bộ";
        public static readonly string HT_CanBo_ThemCanBo = "Thêm hệ thống cán bộ";
        public static readonly string HT_CanBo_SuaCanBo = "Sửa hệ thống cán bộ";
        public static readonly string HT_CanBo_XoaCanBo = "Xóa cán bộ";
        public static readonly string HT_CanBo_FilterByName = "Lọc theo tên hệ thống cán bộ";
        public static readonly string HT_CanBo_GetByID = "Lấy theo id hệ thống cán bộ";
        public static readonly string HT_CanBo_ImportFile = "Import File vào database";
        public static readonly string HT_CanBo_ExportFile = "Xuất File Exel mẫu";
        //Người Dùng
        public static readonly string HT_Nguoidung_GetListPaging = "Lấy danh sách phân trang hệ thống người dùng";
        public static readonly string HT_Nguoidung_ThemNguoidung = "Thêm hệ thống người dùng";
        public static readonly string HT_Nguoidung_SuaNguoidung = "Sửa hệ thống người dùng";
        public static readonly string HT_Nguoidung_XoaNguoidung = "Xóa hệ thống người dùng";
        public static readonly string HT_Nguoidung_FilterByName = "Lọc theo tên hệ thống người dùng";
        public static readonly string HT_Nguoidung_GetByID = "Lấy theo mã định danh hệ thống người dùng";
        //SystemLog
        public static readonly string HT_SystemLog_Them = "Thêm SystemLog";
        public static readonly string HT_SystemLog_Sua = "Sửa SystemLog";
        public static readonly string HT_SystemLog_Xoa = "Xóa SystemLog";
        public static readonly string HT_SystemLog_FilterByNam = "Lọc SystemLog theo SystemLogInfo";
        public static readonly string HT_SystemLog_FilterByID = "Lấy SystemLog theo ID";
        public static readonly string HT_SystemLog_GetListPaging = "Lấy danh sách phân trang SystemLog";
        // VaoRa
        public static readonly string HT_SystemConfig_Them = "Thêm tham số hệ thống";
        public static readonly string HT_SystemConfig_Sua = "Sửa tham số hệ thống";
        public static readonly string HT_SystemConfig_Xoa = "Xóa tham số hệ thống";
        public static readonly string HT_SystemConfig_GetByKey = "Lọc tham số hệ thống theo tên";
        public static readonly string HT_SystemConfig_GetByID = "Lấy tham số hệ thống theo ID";
        public static readonly string HT_SystemConfig_GetListPaging = "Lấy danh sách phân trang tham số hệ thống";

        // Quản trị dữ liệu
        public static readonly string HT_QuanTriDuLieu_BackupDatabase = "Sao lưu dữ liệu";
        public static readonly string HT_QuanTriDuLieu_RestoreDatabase = "Phục hồi dữ liệu";
        public static readonly string HT_QuanTriDuLieu_GetListFileBackup = "Lấy danh sách file sao lưu";
        // Nhóm người dùng
        public static readonly string HT_NhomNguoiDung_Them = "Thêm nhóm người dùng";
        public static readonly string HT_NhomNguoiDung_Sua = "Sửa nhóm người dùng";
        public static readonly string HT_NhomNguoiDung_Xoa = "Xóa nhóm người dùng";
        public static readonly string HT_NhomNguoiDung_GetByKey = "Lọc nhóm người dùng theo tên";
        public static readonly string HT_NhomNguoiDung_GetByID = "Lấy nhóm người dùng theo ID";
        public static readonly string HT_NhomNguoiDung_GetListPaging = "Lấy danh sách phân trang nhóm người dùng";

        public static readonly string HT_NguoiDung_NhomNguoiDung_Them = "Thêm người dùng vào nhóm người dùng";
        public static readonly string HT_NguoiDung_NhomNguoiDung_Sua = "Sửa người dùng trong nhóm người dùng";
        public static readonly string HT_NguoiDung_NhomNguoiDung_Xoa = "Xóa người dùng trong nhóm người dùng";
        public static readonly string HT_NguoiDung_NhomNguoiDung_GetByKey = "Lọc người dùng trong nhóm người dùng theo tên";
        //public static readonly string HT_NguoiDung_NhomNguoiDung_GetByID = "Lấy nhóm người dùng theo ID";
        public static readonly string HT_NguoiDung_NhomNguoiDung_GetListPaging = "Lấy danh sách người dùng trong nhóm người dùng";

        public static readonly string HT_PhanQuyen_Them = "Thêm chức năng vào nhóm người dùng";
        public static readonly string HT_PhanQuyen_Sua = "Sửa chức năng trong nhóm người dùng";
        public static readonly string HT_PhanQuyen_Xoa = "Xóa chức năng trong nhóm người dùng";
        public static readonly string HT_PhanQuyen_GetByKey = "Lọc chức năng trong nhóm người dùng theo tên";
        public static readonly string HT_PhanQuyen_GetByID = "Lấy người dùng trong nhóm người dùng theo ID";
        public static readonly string HT_PhanQuyen_GetListPaging = "Lấy danh sách chức năng trong nhóm người dùng";

        public static readonly string HT_ChucNang_GetListPaging = "Lấy danh sách chức năng";


        public static readonly string HT_HuongDanSuDung_Them = "Thêm hướng dẫn sử dụng";
        public static readonly string HT_HuongDanSuDung_Sua = "Sửa hướng dẫn sử dụng";
        public static readonly string HT_HuongDanSuDung_Xoa = "Xóa hướng dẫn sử dụng";
        public static readonly string HT_HuongDanSuDung_GetByMaChucNang = "Lấy hướng dẫn sử dụng theo chức năng";
        public static readonly string HT_HuongDanSuDung_GetByID = "Lấy hướng dẫn sử dụng theo ID";
        public static readonly string HT_HuongDanSuDung_GetListPaging = "Lấy danh sách phân trang hướng dẫn sử dụng";

        #endregion
        #region Nhiệm vụ
        public static readonly string NV_ThemNhiemVu = "Thêm mới nhiệm vụ";
        public static readonly string NV_XoaNhiemVu = "Xoá nhiệm vụ";

        //public static readonly string NV_ThayDoiNgayHetHan = "Tha nhiệm vụ";
        //public static readonly string NV_ThayDoiTenNhiemVu = "Xoá nhiệm vụ";
        //public static readonly string NV_ThayDoi = "Xoá nhiệm vụ";


        #endregion
        #region Báo cáo

        #endregion
        //public string Enum(int Number)
        //{

        //}
        // Insert Success
        public static string Alert_Insert_Success(string Text)
        {
            return string.Concat("Thêm mới ", Text, " thành công!");
        }
        //Update Success
        public static string Alert_Update_Success(string Text)
        {
            return string.Concat("Cập nhật ", Text, " thành công!");
        }
        //Delete Success
        public static string Alert_Delete_Success(string Text)
        {
            return string.Concat("Xoá ", Text, " thành công!");
        }
        //Insert Error
        public static string Alert_Insert_Error(string Text)
        {
            return string.Concat("Thêm ", Text, " không thành công!");
        }
        //Update Error
        public static string Alert_Update_Error(string Text)
        {
            return string.Concat("Cập nhật ", Text, " không thành công!");
        }
        //Delete Error
        public static string Alert_Delete_Error(string Text)
        {
            return string.Concat("Xóa ", Text, " không thành công!");
        }
        //Check Not Fill
        public static string Alert_Error_NotFill(string Text)
        {
            return string.Concat(Text, " không được để trống!");
        }
        //Check Duplicate
        public static string Alert_Error_Duplicate(string Text)
        {
            return string.Concat(Text, " bị trùng lặp!");
        }
        //Check Exist
        public static string Alert_Error_Exist(string Text)
        {
            return string.Concat(Text, " đã tồn tại!");
        }
        //Check Not Exist
        public static string Alert_Error_NotExist(string Text)
        {
            return string.Concat(Text, " không tồn tại!");
        }
        //Check Lengh Too long
        public static string Alert_Error_CheckLenghTooLong(string Text)
        {
            return string.Concat(Text, " quá dài!");
        }
    }
}
