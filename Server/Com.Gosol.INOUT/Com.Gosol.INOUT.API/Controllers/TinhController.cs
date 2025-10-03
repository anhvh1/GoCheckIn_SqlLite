using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Com.Gosol.INOUT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TinhController : ControllerBase
    {
        private readonly string _connString;

        public TinhController()
        {
            _connString = SQLHelper.connectionString; // connection string đến file gocheckin.db
        }

        [HttpPost("HT_SystemConfig")]
        public IActionResult InsertHT_SystemConfig()
        {
            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();

                string sql = @"INSERT INTO HT_SystemConfig
                       (SystemConfigID, ConfigKey, ConfigValue, Description)
                       VALUES (@SystemConfigID, @ConfigKey, @ConfigValue, @Description)";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    // Insert từng bản ghi
                    InsertSystemConfigRecord(cmd, 1, "MatKhau_MacDinh", "123456", null);
                    InsertSystemConfigRecord(cmd, 2, "TRANG_THAI_CONG_VIEC", "1", "DS trạng thái công việc không được phép sửa");
                    InsertSystemConfigRecord(cmd, 3, "SoLuong_NhiemVu_XemGanDay", "5", "Số lượng nhiệm vụ  đã xem gần đây được hiển thị");
                    InsertSystemConfigRecord(cmd, 4, "NAM_TRIEN_KHAI_PHAN_MEM", "2020", null);
                    InsertSystemConfigRecord(cmd, 5, "Page_Size", "20", null);
                    InsertSystemConfigRecord(cmd, 6, "Alert_Exdate", "5", null);
                    InsertSystemConfigRecord(cmd, 7, "BackUp_Path", @"C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\Backup", null);
                    InsertSystemConfigRecord(cmd, 8, "Ten_Don_Vi", "Công ty cổ phần giải pháp công nghệ Go", null);
                    InsertSystemConfigRecord(cmd, 9, "Thong_Tin_Ho_Tro", "0948.860.868;contact@gosol.com.vn", null);
                    InsertSystemConfigRecord(cmd, 10, "Backup_Date", "1", null);
                    InsertSystemConfigRecord(cmd, 11, "Mail_QuanTri", "kekhaitaisan.team@gmail.com;gosol@123", null);
                    InsertSystemConfigRecord(cmd, 12, "Exp_Mail", "5", null);
                    InsertSystemConfigRecord(cmd, 13, "Link_LosePassword", "https://gocheckin.vn/quen-mat-khau?Token=", null);
                    InsertSystemConfigRecord(cmd, 14, "Exp_LogFile", "1", null);
                    InsertSystemConfigRecord(cmd, 15, "UploadFile_Path", @"D:\ImageGoCheckIn", null);
                    InsertSystemConfigRecord(cmd, 16, "TrangThaiID_MacDinh", "1", null);
                    InsertSystemConfigRecord(cmd, 17, "Page_Size_Notify", "2", "Phân trang Notify");
                    InsertSystemConfigRecord(cmd, 18, "Camera_Path", "http://192.168.100.157/mjpg/video.mjpg?timestamp=1592292116838", null);
                    InsertSystemConfigRecord(cmd, 19, "ChucVu_LeTan", null, null);
                    InsertSystemConfigRecord(cmd, 20, "NhomPhanQuyen_Admin", "98", "Nhóm quyền của admin (Phân cách bởi dấu ,)");
                    InsertSystemConfigRecord(cmd, 21, "NhomNguoiDungMacDinhThemMoiChoCoQuan", "98,110,432,483", "nhóm người dùng mặc định thêm vào khi thêm mới cơ quan sử dụng phần mềm (cách nhau bởi dấu ,)");
                    InsertSystemConfigRecord(cmd, 22, "NhomNguoiDungMacDinhChoAdminDonVi", "98,432,483", "nhóm người dùng mặc định cho admin đơn vị");
                    InsertSystemConfigRecord(cmd, 23, "Than_Nhiet", "37.5", null);
                    InsertSystemConfigRecord(cmd, 24, "FileLimit", "20", null);
                    InsertSystemConfigRecord(cmd, 25, "KhoangThoiGianChekIn", "60", "Khoảng thời gian giữa 2 lần checkin (giây)");
                }
            }

            return Ok(new { message = "Đã insert tất cả dữ liệu vào bảng HT_SystemConfig thành công!" });
        }

        private void InsertSystemConfigRecord(SQLiteCommand cmd, int id, string key, string value, string description)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@SystemConfigID", id);
            cmd.Parameters.AddWithValue("@ConfigKey", key ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ConfigValue", value ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        [HttpPost("delete")]
        public IActionResult DeleteAllThongTinKhach()
        {
            int rowsAffected = 0;

            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();

                string sql = @"
                            DELETE FROM NV_FileDinhKem;
                            DELETE FROM NV_ThongTinVaoRa;
                            DELETE FROM NV_ThongTinKhach;";


                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    rowsAffected = cmd.ExecuteNonQuery(); // Trả về số dòng bị xóa
                }
            }

            return Ok(new { DeletedRows = rowsAffected });
        }



        // Lấy tất cả
        [HttpGet("get-all")]
        public IActionResult GetAll()
        {
            var result = new List<object>();

            using (var conn = new SQLiteConnection(_connString))
            {
                conn.Open();

                string sql = @"SELECT *
                           FROM NV_ThongTinVaoRa
                           ORDER BY GioVao DESC";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new
                        {
                            ThongTinVaoRaID = reader["ThongTinVaoRaID"],
                            GioVao = reader["GioVao"],
                            GioRa = reader["GioRa"],
                            HoVaTen = reader["HoVaTen"],
                            SoCMND = reader["SoCMND"],
                            MaThe = reader["MaThe"],
                            TenCoQuan = reader["TenCoQuan"],
                            GioRa_Int = reader["GioRa_Int"],
                            GioVao_Int = reader["GioVao_Int"],
                        };

                        result.Add(item);
                    }
                }
            }

            return Ok(result);
        }

    }
}