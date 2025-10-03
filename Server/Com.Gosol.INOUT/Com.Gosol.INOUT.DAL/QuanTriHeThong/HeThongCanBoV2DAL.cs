using Com.Gosol.INOUT.DAL.DanhMuc;

using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.Ultilities;
using Com.Gosol.INOUT.Ultilities;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface IHeThongCanBoV2DAL
    {
        public int Insert(HeThongCanBoModel HeThongCanBoModel, ref int CanBoID, ref string Message);
        public int Update(HeThongCanBoModel HeThongCanBoModel, ref string Message);
        public List<string> Delete(List<int> CanBoID);
        public HeThongCanBoModel GetCanBoByID(int? CanBoID);
        //public List<HeThongCanBoModel> FilterByName(string TenCanBo, int IsStatus, int CoQuanID);
        public List<HeThongCanBoModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID);
        public List<HeThongCanBoPartialModel> ReadExcelFile(string FilePath, int? CoQuanID);
        public int ImportToExel(string FilePath, int? CoQuanID);
        public string GenerationMaCanBo(int CoQuanID);
        public List<HeThongCanBoModel> GetAllCanBoByCoQuanID(int CoQuanID, int CoQuan_ID);
        public List<HeThongCanBoModel> GetAllByCoQuanID(int CoQuanID);
        public List<HeThongCanBoModel> GetAllInCoQuanCha(int CoQuanID);
        public ThongTinDonViModel HeThongCanBo_GetThongTinCoQuan(int CanBoID, int NguoiDungID);
        public List<HeThongCanBoModel> GetDanhSachLeTan();
        public List<HeThongCanBoModel> DanhSachCanBo_TrongCoQuanSuDungPhanMem(int CoQuanID);
        public List<HeThongCanBoModel> GetDanhSachLeTan_TrongCoQuanSuDungPhanMem(int CoQuanID);
        public BaseResultModel CapNhatThongTinCanBo(HeThongCanBoV2Model canBoInfo);
        public BaseResultModel CapNhatFaceID(HeThongCanBoV2Model canBoInfo);
        public HeThongCanBoV2Model GetCanBoByID_v4(int? CanBoID);
    }
    public class HeThongCanBoV2DAL : IHeThongCanBoV2DAL


    {
        //private readonly ControllerBase


        //param constant
        private const string PARAM_CanBoID = "@CanBoID";
        private const string PARAM_MaCB = "@MaCB";
        private const string PARAM_TenCanBo = "@TenCanBo";
        private const string PARAM_NgaySinh = "@NgaySinh";
        private const string PARAM_GioiTinh = "@GioiTinh";
        private const string PARAM_DiaChi = "@DiaChi";
        private const string PARAM_ChucVuID = "@ChucVuID";
        private const string PARAM_QuyenKy = "@QuyenKy";
        private const string PARAM_Email = "@Email";
        private const string PARAM_DienThoai = "@DienThoai";
        private const string PARAM_PhongBanID = "@PhongBanID";
        private const string PARAM_CoQuanID = "@CoQuanID";
        private const string PARAM_RoleID = "@RoleID";
        private const string PARAM_QuanTridonVi = "@QuanTridonVi";
        private const string PARAM_CoQuanCuID = "@CoQuanCuID";
        private const string PARAM_CanBoCuID = "@CanBoCuID";
        private const string PARAM_XemTaiLieuMat = "@XemTaiLieuMat";
        private const string PARAM_IsStatus = "@IsStatus";
        private const string PARAM_AnhHoSo = "@AnhHoSo";
        private const string PARAM_HoKhau = "@HoKhau";
        private const string PARAM_MaCQ = "@MaCQ";
        private const string PARAM_CapQuanLy = "@CapQuanLy";
        private const string PARAM_TrangThaiID = "@TrangThaiID";
        private const string PARAM_CMND = "@CMND";
        private const string PARAM_NoiCap = "@NoiCap";
        private const string PARAM_NgayCap = "@NgayCap";


        #region Cán bộ
        public string GenerationMaCanBo(int CoQuanID)
        {
            string maCanBo = "";
            string maCanBoCurr = "";
            string maCoQuan = "";

            SqlParameter[] parameters1 = new SqlParameter[]
       {
                new SqlParameter(PARAM_CoQuanID, SqlDbType.Int)
       };
            parameters1[0].Value = CoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetByID", parameters1))
                {
                    while (dr.Read())
                    {
                        maCoQuan = Utils.ConvertToString(dr["MaCQ"], String.Empty);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (string.IsNullOrEmpty(maCoQuan))
            {
                maCoQuan = "CQ";
            }
            Random oRandom = new Random();
            int MaVach = oRandom.Next(1000, 99999);
            maCanBo = maCoQuan + MaVach;
            //}
            //else
            //{
            //    string s = maCanBoCurr.Substring(maCanBoCurr.IndexOf("_") + 1).ToString();
            //    int STT = Utils.ConvertToInt32((maCanBoCurr.Substring(maCanBoCurr.IndexOf("_") + 1).ToString()), 0);
            //    STT = STT + 1;
            //}

            return maCanBo;
        }

        // Insert Can Bo
        public int Insert(HeThongCanBoModel HeThongCanBoModel, ref int CanBoID, ref string Message)
        {
            int CoQuanID = HeThongCanBoModel.CoQuanID ?? default(int);
            int val = 0;
            if (HeThongCanBoModel.TenCanBo.Trim().Length > 500)
            {

                Message = ConstantLogMessage.API_Error_TooLong;
                return val;
            }
            if (string.IsNullOrEmpty(HeThongCanBoModel.TenCanBo) || HeThongCanBoModel.TenCanBo.Trim().Length <= 0)
            {

                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            //if (!Utils.CheckSpecialCharacter(HeThongCanBoModel.TenCanBo))
            //{

            //    Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
            //    return val;
            //}
            var CanBoByCoQuanAndChucVu = GetAllCanBoByChucVuIDAndCoQuanID(HeThongCanBoModel.ChucVuID, HeThongCanBoModel.CoQuanID);
            if (CanBoByCoQuanAndChucVu.Count > 0)
            {
                Message = "Chức vụ trong cơ quan đã có cán bộ làm việc! Thử lại!";
                return val;
            }
            var CanBo = GetByMaCB(HeThongCanBoModel.MaCB);
            if (HeThongCanBoModel.CoQuanID != 0)
            {
                var CoQuan = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
                if (CoQuan == null)
                {
                    Message = ConstantLogMessage.Alert_Error_NotExist("Cơ quan");
                    return val;
                }
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_NgaySinh, SqlDbType.DateTime),
                    new SqlParameter(PARAM_GioiTinh, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_DiaChi, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_ChucVuID, SqlDbType.Int),
                    new SqlParameter(PARAM_QuyenKy, SqlDbType.Int),
                    new SqlParameter(PARAM_Email, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_DienThoai, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_PhongBanID, SqlDbType.Int),
                    new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                    new SqlParameter(PARAM_RoleID, SqlDbType.Int),
                    new SqlParameter(PARAM_QuanTridonVi,  SqlDbType.Int),
                    new SqlParameter(PARAM_CoQuanCuID, SqlDbType.Int),
                    new SqlParameter(PARAM_CanBoCuID,  SqlDbType.Int),
                    new SqlParameter(PARAM_XemTaiLieuMat,  SqlDbType.Int),
                    new SqlParameter(PARAM_TrangThaiID,  SqlDbType.Int),
                    new SqlParameter(PARAM_AnhHoSo,  SqlDbType.NVarChar),
                    new SqlParameter(PARAM_HoKhau,  SqlDbType.NVarChar),
                    new SqlParameter(PARAM_MaCB,  SqlDbType.NVarChar),
                    new SqlParameter(PARAM_CanBoID,SqlDbType.Int),
                    new SqlParameter(PARAM_CapQuanLy,SqlDbType.Int),
                    new SqlParameter("VaiTro",SqlDbType.NVarChar),
                    new SqlParameter(PARAM_CMND, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_NgayCap, SqlDbType.DateTime),
                    new SqlParameter(PARAM_NoiCap, SqlDbType.NVarChar),
                     new SqlParameter("LaLeTan", SqlDbType.Bit),


              };
            parameters[0].Value = HeThongCanBoModel.TenCanBo;
            parameters[1].Value = HeThongCanBoModel.NgaySinh == null ? Convert.DBNull : HeThongCanBoModel.NgaySinh.Value.ToString("yyyy/MM/dd");
            parameters[2].Value = HeThongCanBoModel.GioiTinh ?? Convert.DBNull;
            parameters[3].Value = HeThongCanBoModel.DiaChi ?? Convert.DBNull;
            parameters[4].Value = HeThongCanBoModel.ChucVuID ?? Convert.DBNull;
            parameters[5].Value = HeThongCanBoModel.QuyenKy ?? Convert.DBNull;
            parameters[6].Value = HeThongCanBoModel.Email ?? Convert.DBNull;
            parameters[7].Value = HeThongCanBoModel.DienThoai ?? Convert.DBNull;
            parameters[8].Value = HeThongCanBoModel.PhongBanID ?? Convert.DBNull;
            parameters[9].Value = HeThongCanBoModel.CoQuanID ?? Convert.DBNull;
            parameters[10].Value = HeThongCanBoModel.RoleID ?? Convert.DBNull;
            parameters[11].Value = HeThongCanBoModel.QuanTridonVi ?? Convert.DBNull;
            parameters[12].Value = HeThongCanBoModel.CoQuanCuID ?? Convert.DBNull;
            parameters[13].Value = HeThongCanBoModel.CanBoCuID ?? Convert.DBNull;
            parameters[14].Value = HeThongCanBoModel.XemTaiLieuMat ?? Convert.DBNull;
            parameters[15].Value = HeThongCanBoModel.TrangThaiID ?? Convert.DBNull;
            parameters[16].Value = HeThongCanBoModel.AnhHoSo ?? Convert.DBNull;
            parameters[17].Value = HeThongCanBoModel.HoKhau ?? Convert.DBNull;

            if (string.IsNullOrEmpty(HeThongCanBoModel.MaCB) || HeThongCanBoModel.MaCB.Trim().Length <= 0)
            {
                parameters[18].Value = GenerationMaCanBo(CoQuanID);
            }
            else
            {
                var CanBoByMa = GetByMaCB(HeThongCanBoModel.MaCB);
                if (CanBoByMa.CanBoID > 0)
                {
                    Message = "Mã cán bộ đã tồn tại!";
                    val = 0;
                    return val;
                }
                parameters[18].Value = HeThongCanBoModel.MaCB;
            }
            parameters[19].Direction = ParameterDirection.Output;
            parameters[20].Value = HeThongCanBoModel.CapQuanLy ?? Convert.DBNull;
            parameters[21].Value = HeThongCanBoModel.VaiTro ?? Convert.DBNull;
            parameters[22].Value = HeThongCanBoModel.CMND ?? Convert.DBNull;
            parameters[23].Value = HeThongCanBoModel.NgayCap == null ? Convert.DBNull : HeThongCanBoModel.NgayCap.Value.ToString("yyyy/MM/dd");
            parameters[24].Value = HeThongCanBoModel.NoiCap ?? Convert.DBNull;
            parameters[25].Value = HeThongCanBoModel.LaLeTan ?? Convert.DBNull;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Insert", parameters);
                        trans.Commit();
                        CanBoID = Utils.ConvertToInt32(parameters[19].Value, 0);
                        //var CanBoByID = GetCanBoByID(CanBoID);
                        //val = Update(CanBoByID, ref Message);
                        if (val > 0)
                        {
                            var mess = "";
                            int NguoiDungID = 0;
                            HeThongNguoiDungModel HeThongNguoiDungModel = new HeThongNguoiDungModel();
                            HeThongNguoiDungModel.CanBoID = CanBoID;
                            HeThongNguoiDungModel.TenNguoiDung = HeThongCanBoModel.TenNguoiDung;
                            var matKhauMacDinh = new SystemConfigDAL().GetByKey("MatKhau_MacDinh").ConfigValue;
                            HeThongNguoiDungModel.MatKhau = Cryptor.EncryptPasswordUser(HeThongNguoiDungModel.TenNguoiDung.Trim().ToLower(), matKhauMacDinh ?? "123456");
                            HeThongNguoiDungModel.CanBoID = CanBoID;
                            HeThongNguoiDungModel.TrangThai = 1;
                            var insertNguoiDung = new HeThongNguoiDungDAL().Insert(HeThongNguoiDungModel, ref mess, ref NguoiDungID);
                            if (insertNguoiDung <= 0) { Message = mess; return insertNguoiDung; }
                            //HeThongCanBoModel.ListNhomNguoiDungID = new List<int>();
                            if (HeThongCanBoModel.ListNhomNguoiDungID == null) { }
                            else
                            {
                                foreach (var item in HeThongCanBoModel.ListNhomNguoiDungID)
                                {
                                    NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel = new NguoiDungNhomNguoiDungModel();
                                    NguoiDungNhomNguoiDungModel.NguoiDungID = NguoiDungID;
                                    NguoiDungNhomNguoiDungModel.NhomNguoiDungID = item;
                                    var insertNguoiDung_NND = new PhanQuyenDAL().NguoiDung_NhomNguoiDung_Insert(NguoiDungNhomNguoiDungModel);
                                    if (insertNguoiDung_NND.Status <= 0) { Message = mess; return insertNguoiDung_NND.Status; }
                                }
                            }

                            if (HeThongCanBoModel.DanhSachChucVuID != null && HeThongCanBoModel.DanhSachChucVuID.Count > 0 && HeThongCanBoModel.DanhSachChucVuID.FirstOrDefault() > 0)
                            {
                                var query = CanBoChucVu_Insert(HeThongCanBoModel.DanhSachChucVuID, CanBoID, ref mess);
                                if (query <= 0) { Message = mess; return query; }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    Message = ConstantLogMessage.Alert_Insert_Success("cán bộ");
                    return val;
                }
            }

        }

        // Update 
        public int Update(HeThongCanBoModel HeThongCanBoModel, ref string Message)
        {

            int val = 0;
            //var CanBoByCoQuanAndChucVu = GetAllCanBoByChucVuIDAndCoQuanID(HeThongCanBoModel.ChucVuID, HeThongCanBoModel.CoQuanID);
            //if (CanBoByCoQuanAndChucVu.Count > 0)
            //{
            //    Message = "Chức vụ trong cơ quan đã có cán bộ làm việc! Thử lại!";
            //    return val;
            //}
            if (HeThongCanBoModel.CanBoID <= 0)
            {
                Message = "Chưa có cán bộ được chọn!";
                return val;
            }
            if (HeThongCanBoModel.TenCanBo.Trim().Length > 500)
            {

                Message = ConstantLogMessage.API_Error_TooLong;
                return val;
            }
            if (string.IsNullOrEmpty(HeThongCanBoModel.TenCanBo) || HeThongCanBoModel.TenCanBo.Trim().Length <= 0)
            {

                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            //if (!Utils.CheckSpecialCharacter(HeThongCanBoModel.TenCanBo))
            //{

            //    Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
            //    return val;
            //}
            var crCanBo = GetCanBoByID(HeThongCanBoModel.CanBoID);
            if (crCanBo == null || crCanBo.CanBoID < 1)
            {
                Message = ConstantLogMessage.Alert_Error_NotExist("Cán bộ");
                return val;
            }
            // Check Mã Cán bộ
            //if (!string.IsNullOrEmpty(HeThongCanBoModel.MaCB))
            //{
            //    var CanBoByMaCb = GetByMaCB(HeThongCanBoModel.MaCB.Trim()); ;
            //    if (CanBoByMaCb.CanBoID != HeThongCanBoModel.CanBoID && CanBoByMaCb.MaCB == HeThongCanBoModel.MaCB)
            //    {
            //        Message = "Mã cán bộ đã tồn tại";
            //        return val;
            //    }
            //}
            else if (string.IsNullOrEmpty(HeThongCanBoModel.MaCB))
            {
                HeThongCanBoModel.MaCB = GenerationMaCanBo(HeThongCanBoModel.CoQuanID.Value);
            }
            //  Check Cấp quản lý
            //if (HeThongCanBoModel.CapQuanLy == null || HeThongCanBoModel.CapQuanLy < 1)
            //{
            //    Message = "cấp quản lý không được trống";
            //    return val;
            //}
            if (HeThongCanBoModel.CoQuanID != null)
            {
                var CoQuan = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
                if (CoQuan == null)
                {
                    Message = ConstantLogMessage.Alert_Error_NotExist("Cơ quan");
                    return val;
                }
                else
                {
                    //var CoQuanByID = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
                    //HeThongCanBoModel.MaCB = string.Concat(CoQuanByID.MaCQ, HeThongCanBoModel.CanBoID);
                }
            }
            var NguoiDungByCanBoID = new HeThongNguoiDungDAL().GetByCanBoID(HeThongCanBoModel.CanBoID);
            string MatKhauMoi = NguoiDungByCanBoID.MatKhau;
            if (NguoiDungByCanBoID.TenNguoiDung != HeThongCanBoModel.TenNguoiDung)
            {
                string MatKhauCuSauDecript = Cryptor.DecryptPasswordUser(NguoiDungByCanBoID.MatKhauCu);
                //string[] MatKhauCuSauDecript = DecriptND.Split(NguoiDungByCanBoID.TenNguoiDung);
                MatKhauMoi = Cryptor.EncryptPasswordUser(HeThongCanBoModel.TenNguoiDung.Trim().ToLower(), MatKhauCuSauDecript);
            }
            else
            {
                //MatKhauMoi = Cryptor.EncryptPasswordUser(HeThongCanBoModel.TenNguoiDung.Trim().ToLower(), "123456");
            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                            new SqlParameter(PARAM_CanBoID, SqlDbType.Int),
                            new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_NgaySinh, SqlDbType.DateTime),
                            new SqlParameter(PARAM_GioiTinh, SqlDbType.Int),
                            new SqlParameter(PARAM_DiaChi, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_ChucVuID, SqlDbType.Int),
                            new SqlParameter(PARAM_QuyenKy, SqlDbType.Int),
                            new SqlParameter(PARAM_Email, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_DienThoai, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_PhongBanID, SqlDbType.Int),
                            new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                            new SqlParameter(PARAM_RoleID, SqlDbType.Int),
                            new SqlParameter(PARAM_QuanTridonVi,  SqlDbType.Int),
                            new SqlParameter(PARAM_CoQuanCuID, SqlDbType.Int),
                            new SqlParameter(PARAM_CanBoCuID,  SqlDbType.Int),
                            new SqlParameter(PARAM_XemTaiLieuMat,  SqlDbType.Int),
                            new SqlParameter(PARAM_TrangThaiID,  SqlDbType.Int),
                            new SqlParameter(PARAM_AnhHoSo,  SqlDbType.NVarChar),
                            new SqlParameter(PARAM_HoKhau,  SqlDbType.NVarChar),
                            new SqlParameter(PARAM_MaCB,  SqlDbType.NVarChar),
                            new SqlParameter(PARAM_CapQuanLy,SqlDbType.Int),
                            new SqlParameter("VaiTro",SqlDbType.NVarChar),
                            new SqlParameter(PARAM_CMND, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_NgayCap, SqlDbType.DateTime),
                            new SqlParameter(PARAM_NoiCap, SqlDbType.NVarChar),
                              new SqlParameter("TenNguoiDung", SqlDbType.NVarChar),
                               new SqlParameter("MatKhau", SqlDbType.NVarChar),
                               new SqlParameter("LaLeTan", SqlDbType.NVarChar)
              };
            parameters[0].Value = HeThongCanBoModel.CanBoID;
            parameters[1].Value = HeThongCanBoModel.TenCanBo ?? Convert.DBNull;
            parameters[2].Value = HeThongCanBoModel.NgaySinh == null ? Convert.DBNull : HeThongCanBoModel.NgaySinh.Value.ToString("yyyy/MM/dd");
            parameters[3].Value = HeThongCanBoModel.GioiTinh ?? Convert.DBNull;
            parameters[4].Value = HeThongCanBoModel.DiaChi ?? Convert.DBNull;
            parameters[5].Value = HeThongCanBoModel.ChucVuID ?? Convert.DBNull;
            parameters[6].Value = HeThongCanBoModel.QuyenKy ?? Convert.DBNull;
            parameters[7].Value = HeThongCanBoModel.Email ?? Convert.DBNull;
            parameters[8].Value = HeThongCanBoModel.DienThoai ?? Convert.DBNull;
            parameters[9].Value = HeThongCanBoModel.PhongBanID ?? Convert.DBNull;
            parameters[10].Value = HeThongCanBoModel.CoQuanID ?? Convert.DBNull;
            parameters[11].Value = HeThongCanBoModel.RoleID ?? Convert.DBNull;
            parameters[12].Value = HeThongCanBoModel.QuanTridonVi ?? Convert.DBNull;
            parameters[13].Value = HeThongCanBoModel.CoQuanCuID ?? Convert.DBNull;
            parameters[14].Value = HeThongCanBoModel.CanBoCuID ?? Convert.DBNull;
            parameters[15].Value = HeThongCanBoModel.XemTaiLieuMat ?? Convert.DBNull;
            parameters[16].Value = HeThongCanBoModel.TrangThaiID ?? Convert.DBNull;
            parameters[17].Value = HeThongCanBoModel.AnhHoSo ?? Convert.DBNull;
            parameters[18].Value = HeThongCanBoModel.HoKhau ?? Convert.DBNull;
            parameters[19].Value = HeThongCanBoModel.MaCB;
            parameters[20].Value = HeThongCanBoModel.CapQuanLy ?? Convert.DBNull;
            parameters[21].Value = HeThongCanBoModel.VaiTro ?? Convert.DBNull;
            parameters[22].Value = HeThongCanBoModel.CMND ?? Convert.DBNull;
            parameters[23].Value = HeThongCanBoModel.NgayCap == null ? Convert.DBNull : HeThongCanBoModel.NgayCap.Value.ToString("yyyy/MM/dd");
            parameters[24].Value = HeThongCanBoModel.NoiCap ?? Convert.DBNull;
            parameters[25].Value = HeThongCanBoModel.TenNguoiDung ?? Convert.DBNull;
            parameters[26].Value = MatKhauMoi ?? Convert.DBNull;
            parameters[27].Value = HeThongCanBoModel.LaLeTan ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string MessageNew = null;
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Update", parameters);
                        trans.Commit();
                        //else if(HeThongCanBoModel.TrangThaiID == 3) // Nghỉ sinh đẻ
                        //{
                        //    crNguoiDung.TrangThai = 2;
                        //}
                        if (val > 0)
                        {
                            var mess = "";
                            // xóa các chức vụ của  Cán bộ đã có
                            var NguoiDungID = new HeThongNguoiDungDAL().GetByCanBoID(HeThongCanBoModel.CanBoID).NguoiDungID;
                            var qrDelete = CanBoChucVu_Delete_By_CanBoID(HeThongCanBoModel.CanBoID, ref mess);
                            var qrDeleteNDNND = NguoiDungNhomNguoiDung_Delete_By_CanBoID(NguoiDungID, ref mess);
                            if (qrDelete < 0 || qrDeleteNDNND < 0) { Message = mess; return qrDelete; }
                            // thêm lại cán bộ - chức vụ
                            if (HeThongCanBoModel.ListNhomNguoiDungID != null)
                            {
                                foreach (var item in HeThongCanBoModel.ListNhomNguoiDungID)
                                {
                                    NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel = new NguoiDungNhomNguoiDungModel();
                                    NguoiDungNhomNguoiDungModel.NguoiDungID = NguoiDungID;
                                    NguoiDungNhomNguoiDungModel.NhomNguoiDungID = item;
                                    var insertNguoiDung_NND = new PhanQuyenDAL().NguoiDung_NhomNguoiDung_Insert(NguoiDungNhomNguoiDungModel);
                                    if (insertNguoiDung_NND.Status < 0) { Message = mess; return insertNguoiDung_NND.Status; }
                                }
                            }
                            var query = CanBoChucVu_Insert(HeThongCanBoModel.DanhSachChucVuID, HeThongCanBoModel.CanBoID, ref mess);
                            if (query < 0) { Message = mess; return query; }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                    Message = ConstantLogMessage.Alert_Update_Success("cán bộ - chức vụ");
                }
                //var crNguoiDung = new HeThongNguoiDungDAL().GetByCanBoID(HeThongCanBoModel.CanBoID);
                //if (HeThongCanBoModel.TrangThaiID == 1) // Đang hoạt động
                //{
                //    new HeThongNguoiDungDAL().UpdateTrangThai(new List<int>() { HeThongCanBoModel.CanBoID }, 1);
                //}
                //else if (HeThongCanBoModel.TrangThaiID == 2) // Nghỉ hưu
                //{
                //    new HeThongNguoiDungDAL().UpdateTrangThai(new List<int>() { HeThongCanBoModel.CanBoID }, 0);
                //}
                return val;
            }
        }

        //public int Update(HeThongCanBoModel HeThongCanBoModel, ref string Message)
        //{

        //    int val = 0;
        //    var CanBoByCoQuanAndChucVu = GetAllCanBoByChucVuIDAndCoQuanID(HeThongCanBoModel.ChucVuID, HeThongCanBoModel.CoQuanID);
        //    //if (CanBoByCoQuanAndChucVu.Count > 0)
        //    //{
        //    //    Message = "Chức vụ trong cơ quan đã có cán bộ làm việc! Thử lại!";
        //    //    return val;
        //    //}
        //    if (HeThongCanBoModel.CanBoID <= 0)
        //    {
        //        Message = "Chưa có cán bộ được chọn!";
        //        return val;
        //    }
        //    if (HeThongCanBoModel.TenCanBo.Trim().Length > 50)
        //    {

        //        Message = ConstantLogMessage.API_Error_TooLong;
        //        return val;
        //    }
        //    if (string.IsNullOrEmpty(HeThongCanBoModel.TenCanBo) || HeThongCanBoModel.TenCanBo.Trim().Length <= 0)
        //    {

        //        Message = ConstantLogMessage.API_Error_NotFill;
        //        return val;
        //    }
        //    //if (!Utils.CheckSpecialCharacter(HeThongCanBoModel.TenCanBo))
        //    //{

        //    //    Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
        //    //    return val;
        //    //}
        //    var crCanBo = GetCanBoByID(HeThongCanBoModel.CanBoID);
        //    if (crCanBo == null || crCanBo.CanBoID < 1)
        //    {
        //        Message = ConstantLogMessage.Alert_Error_NotExist("Cán bộ");
        //        return val;
        //    }
        //    // Check Mã Cán bộ
        //    //if (!string.IsNullOrEmpty(HeThongCanBoModel.MaCB))
        //    //{
        //    //    var CanBoByMaCb = GetByMaCB(HeThongCanBoModel.MaCB.Trim()); ;
        //    //    if (CanBoByMaCb.CanBoID != HeThongCanBoModel.CanBoID && CanBoByMaCb.MaCB == HeThongCanBoModel.MaCB)
        //    //    {
        //    //        Message = "Mã cán bộ đã tồn tại";
        //    //        return val;
        //    //    }
        //    //}
        //    else if (string.IsNullOrEmpty(HeThongCanBoModel.MaCB))
        //    {
        //        HeThongCanBoModel.MaCB = GenerationMaCanBo(HeThongCanBoModel.CoQuanID.Value);
        //    }
        //    //  Check Cấp quản lý
        //    //if (HeThongCanBoModel.CapQuanLy == null || HeThongCanBoModel.CapQuanLy < 1)
        //    //{
        //    //    Message = "cấp quản lý không được trống";
        //    //    return val;
        //    //}
        //    if (HeThongCanBoModel.CoQuanID != null)
        //    {
        //        var CoQuan = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
        //        if (CoQuan == null)
        //        {
        //            Message = ConstantLogMessage.Alert_Error_NotExist("Cơ quan");
        //            return val;
        //        }
        //        else
        //        {
        //            var CoQuanByID = new DanhMucCoQuanDonViDAL().GetByID(HeThongCanBoModel.CoQuanID);
        //            //HeThongCanBoModel.MaCB = string.Concat(CoQuanByID.MaCQ, HeThongCanBoModel.CanBoID);
        //        }
        //    }
        //    //if (CanBo.CanBoID > 0 && CanBo.CanBoID != HeThongCanBoModel.CanBoID)
        //    //{
        //    //    val = 2;
        //    //    Message = ConstantLogMessage.Alert_Error_Exist("Tên cán bộ");
        //    //    return val;
        //    //}
        //    SqlParameter[] parameters = new SqlParameter[]
        //      {
        //                    new SqlParameter(PARAM_CanBoID, SqlDbType.Int),
        //                    new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_NgaySinh, SqlDbType.DateTime),
        //                    new SqlParameter(PARAM_GioiTinh, SqlDbType.Int),
        //                    new SqlParameter(PARAM_DiaChi, SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_ChucVuID, SqlDbType.Int),
        //                    new SqlParameter(PARAM_QuyenKy, SqlDbType.Int),
        //                    new SqlParameter(PARAM_Email, SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_DienThoai, SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_PhongBanID, SqlDbType.Int),
        //                    new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
        //                    new SqlParameter(PARAM_RoleID, SqlDbType.Int),
        //                    new SqlParameter(PARAM_QuanTridonVi,  SqlDbType.Int),
        //                    new SqlParameter(PARAM_CoQuanCuID, SqlDbType.Int),
        //                    new SqlParameter(PARAM_CanBoCuID,  SqlDbType.Int),
        //                    new SqlParameter(PARAM_XemTaiLieuMat,  SqlDbType.Int),
        //                    new SqlParameter(PARAM_TrangThaiID,  SqlDbType.Int),
        //                    new SqlParameter(PARAM_AnhHoSo,  SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_HoKhau,  SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_MaCB,  SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_CapQuanLy,SqlDbType.Int),
        //                    new SqlParameter("VaiTro",SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_CMND, SqlDbType.NVarChar),
        //                    new SqlParameter(PARAM_NgayCap, SqlDbType.DateTime),
        //                    new SqlParameter(PARAM_NoiCap, SqlDbType.NVarChar),
        //                    new SqlParameter("TenNguoiDung", SqlDbType.NVarChar),
        //                    new SqlParameter("LaLeTan", SqlDbType.NVarChar)
        //      };
        //    parameters[0].Value = HeThongCanBoModel.CanBoID;
        //    parameters[1].Value = HeThongCanBoModel.TenCanBo ?? Convert.DBNull;
        //    parameters[2].Value = HeThongCanBoModel.NgaySinh == null ? Convert.DBNull : HeThongCanBoModel.NgaySinh.Value.ToString("yyyy/MM/dd");
        //    parameters[3].Value = HeThongCanBoModel.GioiTinh ?? Convert.DBNull;
        //    parameters[4].Value = HeThongCanBoModel.DiaChi ?? Convert.DBNull;
        //    parameters[5].Value = HeThongCanBoModel.ChucVuID ?? Convert.DBNull;
        //    parameters[6].Value = HeThongCanBoModel.QuyenKy ?? Convert.DBNull;
        //    parameters[7].Value = HeThongCanBoModel.Email ?? Convert.DBNull;
        //    parameters[8].Value = HeThongCanBoModel.DienThoai ?? Convert.DBNull;
        //    parameters[9].Value = HeThongCanBoModel.PhongBanID ?? Convert.DBNull;
        //    parameters[10].Value = HeThongCanBoModel.CoQuanID ?? Convert.DBNull;
        //    parameters[11].Value = HeThongCanBoModel.RoleID ?? Convert.DBNull;
        //    parameters[12].Value = HeThongCanBoModel.QuanTridonVi ?? Convert.DBNull;
        //    parameters[13].Value = HeThongCanBoModel.CoQuanCuID ?? Convert.DBNull;
        //    parameters[14].Value = HeThongCanBoModel.CanBoCuID ?? Convert.DBNull;
        //    parameters[15].Value = HeThongCanBoModel.XemTaiLieuMat ?? Convert.DBNull;
        //    parameters[16].Value = HeThongCanBoModel.TrangThaiID ?? Convert.DBNull;
        //    parameters[17].Value = HeThongCanBoModel.AnhHoSo ?? Convert.DBNull;
        //    parameters[18].Value = HeThongCanBoModel.HoKhau ?? Convert.DBNull;
        //    parameters[19].Value = HeThongCanBoModel.MaCB;
        //    parameters[20].Value = HeThongCanBoModel.CapQuanLy ?? Convert.DBNull;
        //    parameters[21].Value = HeThongCanBoModel.VaiTro ?? Convert.DBNull;
        //    parameters[22].Value = HeThongCanBoModel.CMND ?? Convert.DBNull;
        //    parameters[23].Value = HeThongCanBoModel.NgayCap == null ? Convert.DBNull : HeThongCanBoModel.NgayCap.Value.ToString("yyyy/MM/dd");
        //    parameters[24].Value = HeThongCanBoModel.NoiCap ?? Convert.DBNull;
        //    parameters[25].Value = HeThongCanBoModel.TenNguoiDung ?? Convert.DBNull;
        //    parameters[26].Value = HeThongCanBoModel.LaLeTan ?? Convert.DBNull;
        //    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
        //    {
        //        conn.Open();
        //        using (SqlTransaction trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                string MessageNew = null;
        //                val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Update", parameters);
        //                trans.Commit();
        //                //else if(HeThongCanBoModel.TrangThaiID == 3) // Nghỉ sinh đẻ
        //                //{
        //                //    crNguoiDung.TrangThai = 2;
        //                //}
        //                if (val > 0)
        //                {
        //                    var mess = "";
        //                    // xóa các chức vụ của  Cán bộ đã có
        //                    var NguoiDungID = new HeThongNguoiDungDAL().GetByCanBoID(HeThongCanBoModel.CanBoID).NguoiDungID;
        //                    var qrDelete = CanBoChucVu_Delete_By_CanBoID(HeThongCanBoModel.CanBoID, ref mess);
        //                    var qrDeleteNDNND = NguoiDungNhomNguoiDung_Delete_By_CanBoID(NguoiDungID, ref mess);
        //                    if (qrDelete < 0 || qrDeleteNDNND < 0) { Message = mess; return qrDelete; }
        //                    // thêm lại cán bộ - chức vụ
        //                    if (HeThongCanBoModel.ListNhomNguoiDungID != null)
        //                    {
        //                        foreach (var item in HeThongCanBoModel.ListNhomNguoiDungID)
        //                        {
        //                            NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel = new NguoiDungNhomNguoiDungModel();
        //                            NguoiDungNhomNguoiDungModel.NguoiDungID = NguoiDungID;
        //                            NguoiDungNhomNguoiDungModel.NhomNguoiDungID = item;
        //                            var insertNguoiDung_NND = new PhanQuyenDAL().NguoiDung_NhomNguoiDung_Insert(NguoiDungNhomNguoiDungModel);
        //                            if (insertNguoiDung_NND.Status < 0) { Message = mess; return insertNguoiDung_NND.Status; }
        //                        }
        //                    }
        //                    var query = CanBoChucVu_Insert(HeThongCanBoModel.DanhSachChucVuID, HeThongCanBoModel.CanBoID, ref mess);
        //                    if (query < 0) { Message = mess; return query; }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();
        //                throw;
        //            }
        //            Message = ConstantLogMessage.Alert_Update_Success("cán bộ - chức vụ");
        //        }
        //        //var crNguoiDung = new HeThongNguoiDungDAL().GetByCanBoID(HeThongCanBoModel.CanBoID);
        //        //if (HeThongCanBoModel.TrangThaiID == 1) // Đang hoạt động
        //        //{
        //        //    new HeThongNguoiDungDAL().UpdateTrangThai(new List<int>() { HeThongCanBoModel.CanBoID }, 1);
        //        //}
        //        //else if (HeThongCanBoModel.TrangThaiID == 2) // Nghỉ hưu
        //        //{
        //        //    new HeThongNguoiDungDAL().UpdateTrangThai(new List<int>() { HeThongCanBoModel.CanBoID }, 0);
        //        //}
        //        return val;
        //    }
        //}

        // Delete
        public List<string> Delete(List<int> ListCanBoID)
        {

            List<string> dic = new List<string>();
            string message = "";
            if (ListCanBoID.Count <= 0)
            {
                message = ConstantLogMessage.API_Error_NotExist;
                dic.Add(message);
                return dic;
            }
            else
            {
                int val = 0;
                for (int i = 0; i < ListCanBoID.Count; i++)
                {
                    var CanBoByID = new HeThongCanBoDAL().GetCanBoByID(ListCanBoID[i]);
                    if (string.IsNullOrEmpty(CanBoByID.TenCanBo))
                    {
                        message = "Vui lòng chọn cán bộ để xóa!";
                        dic.Add(message);
                    }
                    //var CanBoByNguoiDungID = new HeThongCanBoDAL().GetCanBoByNguoiDungID(ListNguoiDungID[i]);

                    //var KeKhaiByCanBoID = new KeKhaiDAL().GetKeKhaiByCanBoID(ListCanBoID[i]).ToList();
                    //else if (CheckRef(ListCanBoID[i]))
                    //{
                    //    dic.Add("Cán bộ " + GetCanBoByID(ListCanBoID[i]).TenCanBo + " đang được sử dụng. Không thể xóa!");
                    //    //return dic;
                    //}
                    //var NguoiDung = new HeThongNguoidungDAL().GetAll();
                    //var query = NguoiDung.Where(x => x.CanBoID == ListCanBoID[i]).Count();
                    else if (GetCanBoByID(ListCanBoID[i]) == null)
                    {
                        message = "Cán bộ " + GetCanBoByID(ListCanBoID[i]).TenCanBo + " không tồn tại!";
                        dic.Add(message);
                        //return dic;
                    }
                    //else if (KeKhaiByCanBoID.Count > 0)
                    //{
                    //    dic.Add("Cán bộ " + GetCanBoByID(ListCanBoID[i]).TenCanBo + " đã có kê khai. Không thể xóa!");
                    //    //return dic;
                    //}
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                        new SqlParameter(@"CanBoID", SqlDbType.Int)

                          };
                        parameters[0].Value = ListCanBoID[i];
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_Delete", parameters), 0);
                                    trans.Commit();
                                    //if (val == 0)
                                    //{
                                    //    message = "Không thể xóa từ Cán bộ thứ " + ListCanBoID[i];
                                    //    dic.Add(0, message);
                                    //    return dic;
                                    //}
                                    //message = "Xóa dữ liệu thành công!";
                                    //dic.Add(1, message);
                                    //return dic;
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    throw;
                                }


                            }
                        }


                    }
                }
                //message = message;
                //dic.Add(1, message);
                return dic;
            }

        }

        //public List<DanhMucChucVuModel> GetAll()

        // Get By id
        public HeThongCanBoModel GetCanBoByID(int? CanBoID)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            if (CanBoID <= 0)
            {
                return canBo;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        //canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        //canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.IsStatus = Utils.ConvertToInt32(dr["IsStatus"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        canBo.CMND = Utils.ConvertToString(dr["CMND"], string.Empty);
                        canBo.NgayCap = Utils.ConvertToDateTime(dr["NgayCap"], DateTime.Now);
                        canBo.NoiCap = Utils.ConvertToString(dr["NoiCap"], string.Empty);
                        canBo.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                        canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.LaLeTan = Utils.ConvertToBoolean(dr["LaLeTan"], false);
                        break;
                    }
                    dr.Close();
                    if (canBo != null && canBo.CanBoID > 0)
                    {
                        canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(CanBoID.Value);
                        canBo.ListNhomNguoiDungID = NguoiDungNhomNguoiDung_GetBy_NguoiDungID(new HeThongNguoiDungDAL().GetByCanBoID(CanBoID.Value).NguoiDungID);
                        // lây danh sách chức vụ của cán bộ
                    }
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }


        /// <summary>
        ///  lấy thông tin tên cơ quan và cơ quan cha của cán bộ đang đăng nhập
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <param name="NguoiDungID"></param>
        /// <returns></returns>
        public ThongTinDonViModel HeThongCanBo_GetThongTinCoQuan(int CanBoID, int NguoiDungID)
        {
            ThongTinDonViModel canBo = new ThongTinDonViModel();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                canBo.TenCoQuan = string.Empty;
                canBo.TenCoQuanCha = string.Empty;
                return canBo;
            }


            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThongCanBo_GetThongTinCoQuan_GetBy_CanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new ThongTinDonViModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                        break;
                    }
                    dr.Close();

                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }
        //GetAll
        public List<HeThongCanBoModel> GetAll()
        {
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAll"))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(canBo.CanBoID);
                        canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.LaLeTan = Utils.ConvertToBoolean(dr["LaLeTan"], false);

                        list.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return list;
        }


        // Get All without nguoidungid
        public List<HeThongCanBoModel> GetAllCanBoWithoutNguoiDung()
        {
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_GetAllCanBoWithoutNguoiDung"))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        list.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return list;
        }
        // Get By Name
        public HeThongCanBoModel GetByMaCB(string MaCB)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"MaCB",SqlDbType.NVarChar)
              };
            parameters[0].Value = MaCB ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetByMaCB", parameters))
                {
                    while (dr.Read())
                    {
                        // canBo = new HeThongCanBoModel(Utils.ConvertToInt32(dr["CanBoID"], 0), Utils.ConvertToString(dr["TenCanBo"], string.Empty), Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now), Utils.ConvertToInt32(dr["GioiTinh"], 0), Utils.ConvertToString(dr["DiaChi"], string.Empty), Utils.ConvertToInt32(dr["ChucVuID"], 0), Utils.ConvertToInt32(dr["QuyenKy"], 0), Utils.ConvertToString(dr["Email"], string.Empty), Utils.ConvertToString(dr["DienThoai"], string.Empty), Utils.ConvertToInt32(dr["PhongBanID"], 0), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToInt32(dr["RoleID"], 0), Utils.ConvertToInt32(dr["QuanTridonVi"], 0), Utils.ConvertToInt32(dr["CoQuanCuID"], 0), Utils.ConvertToInt32(dr["CanBoCuID"], 0), Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0), Utils.ConvertToString(dr["AnhHoSo"], string.Empty),
                        //   Utils.ConvertToString(dr["HoKhau"], string.Empty), Utils.ConvertToString(dr["MaCB"], string.Empty), Utils.ConvertToInt32(dr["CapQuanLy"], 0), Utils.ConvertToInt32(dr["TrangThaiID"], 0), Utils.ConvertToInt32(dr["NguoiDungID"], 0));
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        break;

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }
        // Filter By Name
        //public List<HeThongCanBoModel> FilterByName(string TenCanBo, int IsStatus, int CoQuanID)
        //{
        //    List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
        //  SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter(PARAM_TenCanBo,SqlDbType.NVarChar),
        //        new SqlParameter(PARAM_CoQuanID,SqlDbType.Int),
        //        new SqlParameter(PARAM_IsStatus,SqlDbType.Int)
        //};
        //    parameters[0].Value = TenCanBo;
        //    parameters[1].Value = CoQuanID == 0 ? (int?)null : CoQuanID;
        //    parameters[2].Value = IsStatus == 0 ? (int?)null : IsStatus;
        //    try
        //    {

        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILTERBYNAME, parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                HeThongCanBoModel canBo = new HeThongCanBoModel(Utils.ConvertToInt32(dr["CanBoID"], 0), Utils.ConvertToString(dr["TenCanBo"], string.Empty), Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now), Utils.ConvertToInt32(dr["GioiTinh"], 0), Utils.ConvertToString(dr["DiaChi"], string.Empty), Utils.ConvertToInt32(dr["ChucVuID"], 0), Utils.ConvertToInt32(dr["QuyenKy"], 0), Utils.ConvertToString(dr["Email"], string.Empty), Utils.ConvertToString(dr["DienThoai"], string.Empty), Utils.ConvertToInt32(dr["PhongBanID"], 0), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToInt32(dr["RoleID"], 0), Utils.ConvertToInt32(dr["QuanTridonVi"], 0), Utils.ConvertToInt32(dr["CoQuanCuID"], 0), Utils.ConvertToInt32(dr["CanBoCuID"], 0), Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0), Utils.ConvertToInt32(dr["IsStatus"], 0));

        //                list.Add(canBo);
        //            }
        //            dr.Close();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return list;
        //}

        // Get list Paging
        public List<HeThongCanBoModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow, int? CoQuanID, int? TrangThaiID, int CoQuan_ID, int NguoiDungID)
        {
            var pList = new SqlParameter("@DanhSachCoQuan", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            //tbCoQuanID.Rows.Add(1);
            //CoQuanID.ForEach(x => table.Rows.Add(x));
            // check cấp đang thao tác để lấy ra danh sách CoQuanID và trạng thái 
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuan_ID);
            //if (p.CoQuanID != null && p.CoQuanID > 0)
            //{
            //    tbCoQuanID.Rows.Add(p.CoQuanID);
            //} 
            List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapConByCapCoQuan(crCoQuan.CoQuanID).ToList();
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapConByCapCoQuan(0).ToList();
            }
            listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@CoQuanID",SqlDbType.Int),
                new SqlParameter("@TrangThaiID",SqlDbType.Int),
                  pList
              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            //parameters[5].Value = 0;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = CoQuanID ?? Convert.DBNull;
            parameters[7].Value = TrangThaiID ?? Convert.DBNull;
            parameters[8].Value = tbCoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_CanBo_GetPagingBySearch_New", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        canBo.CapCoQuanID = Utils.ConvertToInt32(dr["CapID"], 0);
                        canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(canBo.CanBoID);
                        canBo.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                        canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        //int NguoiDungID = new HeThongNguoiDungDAL().GetByCanBoID(canBo.CanBoID).NguoiDungID;
                        canBo.ListNhomNguoiDungID = NguoiDungNhomNguoiDung_GetBy_NguoiDungID(canBo.NguoiDungID);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.LaLeTan = Utils.ConvertToBoolean(dr["LaLeTan"], false);
                        if (canBo.CanBoID != 1)
                        {
                            list.Add(canBo);
                        }

                    }

                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //List<int> List = new List<int>();
            //listCoQuanCon.ForEach(x => List.Add(x.CoQuanID));
            //list.Where(x => List.Contains(x.CoQuanID.Value)).ToList();
            list = list.OrderBy(x => x.CapCoQuanID).ToList();
            return list;
        }

        // Import exel file to db
        public List<HeThongCanBoPartialModel> ReadExcelFile(string FilePath, int? CoQuanID)
        {
            var ListCoQuanID_New = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID.Value).Select(x => x.CoQuanID);
            List<HeThongCanBoPartialModel> ListCanBoPar = new List<HeThongCanBoPartialModel>();
            HeThongCanBoPartialModel HeThongCanBoPartialModel = new HeThongCanBoPartialModel();
            List<string> ListMaCB = new List<string>();
            int val = 0;
            if (!File.Exists(FilePath))
            {
                return ListCanBoPar;
            }
            try
            {

                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var totalWorksheets = package.Workbook.Worksheets.Count;
                    if (totalWorksheets <= 0)
                    {
                        return ListCanBoPar;
                    }
                    else
                    {
                        List<string> ListNguyenNhan = new List<string>();
                        List<HeThongCanBoModel> listCanBo = new List<HeThongCanBoModel>();
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];

                        //Range usedRange = workSheet.Get();                        
                        DataTable dt = new DataTable(typeof(object).Name);
                        //var RowStart = 0;
                        int endRow = 0;
                        int RowNoData = 0;
                        int? GioiTinh = 0;
                        int VaiTro = 0;
                        //int CapID = 0;
                        string LoiNgaySinh = null;
                        DateTime? Ngaysinhnhat = null;
                        for (int a = 4;
                                 a <= workSheet.Dimension.End.Row;
                                 a++)
                        {

                            if (Utils.ConvertToInt32(workSheet.Cells[a, 1].Value, 0) <= 0 && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 2].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 3].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 4].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 5].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 6].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 7].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 8].Value, string.Empty)) && Utils.ConvertToInt32(workSheet.Cells[a, 9].Value, 0) <= 0 &&
                                 string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 10].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 11].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 12].Value, string.Empty)) && string.IsNullOrEmpty(Utils.ConvertToString(workSheet.Cells[a, 13].Value, string.Empty)))
                            {
                                RowNoData = a;
                                break;
                            }
                            endRow = a;
                        }
                        if (RowNoData == 4)
                        {
                            ListNguyenNhan.Add("Files không có dữ liệu!");
                            HeThongCanBoPartialModel.NguyenNhan = ListNguyenNhan;
                            ListCanBoPar.Add(HeThongCanBoPartialModel);
                            return ListCanBoPar;
                        }
                        for (int i = 4;
                             i < endRow + 1;
                             i++)
                        {
                            HeThongCanBoPartialModel = new HeThongCanBoPartialModel();
                            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
                            List<string> ListStringChucVu = new List<string>();
                            ListNguyenNhan = new List<string>();
                            List<object> lstobj = new List<object>();
                            List<object> MyListChucVu = new List<object>();

                            for (int j = workSheet.Dimension.Start.Column;
                                     j <= workSheet.Dimension.End.Column;
                                     j++)
                            {
                                if (j == 4)
                                {

                                    string NgaySinh = Utils.ConvertToString(workSheet.Cells[i, 4].Value, string.Empty).Trim().ToLower();
                                    if (string.IsNullOrEmpty(NgaySinh))
                                    {
                                        Ngaysinhnhat = (DateTime?)null;
                                        LoiNgaySinh = "Ngày sinh đang để trống";
                                    }
                                    else
                                    if (Utils.CheckCharacter(NgaySinh.ToString().Trim()))
                                    {
                                        LoiNgaySinh = "Ngày sinh không đúng định dạng";
                                        Ngaysinhnhat = Utils.ConvertToDateTime(NgaySinh, DateTime.Now);
                                    }
                                    else
                                    {
                                        LoiNgaySinh = null;
                                        Ngaysinhnhat = Utils.ConvertToDateTime(NgaySinh, DateTime.Now);
                                    }
                                }
                                if (j == 5)
                                {
                                    string TenGioiTinh = Utils.ConvertToString(workSheet.Cells[i, 5].Value, string.Empty).Trim().ToLower();
                                    if (TenGioiTinh == "nam")
                                    {
                                        GioiTinh = 1;
                                    }
                                    else if (TenGioiTinh == "nữ")
                                    {
                                        GioiTinh = 0;
                                    }
                                    else
                                    {
                                        GioiTinh = (int?)null;
                                    }
                                }
                                if (j == 6)
                                {
                                    MyListChucVu.Add(workSheet.Cells[i, j].Value);
                                    ListStringChucVu = MyListChucVu.Select(s => (string)s).ToList();
                                    ListStringChucVu.RemoveAll(x => string.IsNullOrEmpty(x) == true);
                                }
                                if (j == 14)
                                {
                                    string TenVaiTro = Utils.ConvertToString(workSheet.Cells[i, 14].Value, string.Empty).Trim().ToLower();
                                    if (TenVaiTro == "lãnh đạo")
                                    {
                                        VaiTro = 2;
                                    }
                                    else if (TenVaiTro == "cán bộ/chuyên viên")
                                    {
                                        VaiTro = 3;
                                    }
                                    else
                                    {
                                        VaiTro = 0;
                                    }
                                }
                                {
                                    object cellValue = workSheet.Cells[i, j].Value;
                                    lstobj.Add(cellValue);
                                }

                            }
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                dt.Columns.Add("Column" + (dimension + 1));
                            }
                            List<int> ListCoQuanID = new List<int>();
                            string[] SplitListChucVu = new string[] { };
                            if (ListStringChucVu.Count > 0)
                            {
                                SplitListChucVu = ListStringChucVu.FirstOrDefault().ToString().Split(",");
                            }
                            for (int a = 0; a < SplitListChucVu.Length; a++)
                            {
                                int ChucVuID = ConvertChucVuIDByName(SplitListChucVu[a].ToString());
                                if (ChucVuID > 0)
                                {
                                    ListCoQuanID.Add(ChucVuID);
                                }
                            }
                            //for (int dimension = 0; dimension < MyListChucVu.Count; dimension++)
                            //{

                            //    ListCoQuanID.Add(Utils.ConvertToInt32(MyListChucVu[dimension], 0));
                            //}
                            DataRow row = dt.NewRow();
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                //Console.Write("{0} ", lstobj[element, dimension]);
                                //if(dimension == 5)
                                //{
                                //    MyList.Add(WS.Cells[Row, dimension+1].Value);
                                //}
                                row["Column" + (dimension + 1)] = lstobj[dimension];
                            }
                            dt.Rows.Add(row);
                            foreach (DataRow dr in dt.Rows)
                            {
                                list.Add(new HeThongCanBoModel
                                {
                                    CanBoID = Utils.ConvertToInt32(dr["Column1"], 0),
                                    TenCanBo = Utils.ConvertToString(dr["Column3"], string.Empty),
                                    NgaySinh = Ngaysinhnhat,
                                    GioiTinh = GioiTinh,
                                    DanhSachChucVuID = ListCoQuanID,
                                    CoQuanID = Utils.ConvertToInt32(ConvertCoQuanIDByName(dr["Column7"].ToString()), 0),
                                    Email = Utils.ConvertToString(dr["Column8"], string.Empty),
                                    DienThoai = Utils.ConvertToString(dr["Column9"], string.Empty),
                                    HoKhau = Utils.ConvertToString(dr["Column10"], string.Empty),
                                    DiaChi = Utils.ConvertToString(dr["Column11"], string.Empty),
                                    TrangThaiID = Utils.ConvertToInt32(ConvertTrangThaiIDByName(dr["Column12"].ToString()), 0),
                                    QuyenKy = (int?)null,
                                    PhongBanID = (int?)null,
                                    RoleID = (int?)null,
                                    QuanTridonVi = (int?)null,
                                    CoQuanCuID = (int?)null,
                                    CanBoCuID = (int?)null,
                                    XemTaiLieuMat = (int?)null,
                                    AnhHoSo = null,
                                    MaCB = Utils.ConvertToString(dr["Column2"], string.Empty),
                                    CapQuanLy = GetCapQuanLyID(Utils.ConvertToString(dr["Column13"], string.Empty)),
                                    VaiTro = VaiTro
                                });
                            }
                            dt.Clear();
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                dt.Columns.Remove("Column" + (dimension + 1));
                            }
                            foreach (var item in list)
                            {
                                listCanBo.Add(item);

                                if (GetByMaCB(item.MaCB).CanBoID > 0)
                                {
                                    ListNguyenNhan.Add("Mã cán bộ này đã tồn tại");
                                }
                                if (ListMaCB.Contains(item.MaCB.ToString().Trim().ToLower()) && !string.IsNullOrEmpty(item.MaCB.ToString().Trim().ToLower()))
                                {
                                    ListNguyenNhan.Add("Mã cán bộ trùng");
                                }
                                if (string.IsNullOrEmpty(item.TenCanBo) || string.IsNullOrEmpty(item.NgaySinh.ToString()) || item.GioiTinh == null || item.GioiTinh == null || item.DanhSachChucVuID.Count <= 0
                                    || item.CoQuanID <= 0 || string.IsNullOrEmpty(item.HoKhau) || string.IsNullOrEmpty(item.DiaChi) || /*item.TrangThaiID <= 0 ||*/ item.CapQuanLy <= 0 || !ListCoQuanID_New.Contains(item.CoQuanID.Value) || item.VaiTro <= 0)
                                {

                                    //HeThongCanBoPartialModel.CapQuanLy = item.VaiTro;
                                    //if (item.TrangThaiID > 0)
                                    //{
                                    //    HeThongCanBoPartialModel.TenTrangThai = new DanhMucTrangThaiDAL().GetByID(item.TrangThaiID.Value).TenTrangThai;
                                    //}
                                    if (!ListCoQuanID_New.Contains(item.CoQuanID.Value) && item.CoQuanID.Value > 0)
                                    {
                                        ListNguyenNhan.Add("Nơi công tác không thuộc phạm vi quản lý");
                                    }
                                    if (string.IsNullOrEmpty(item.TenCanBo))
                                    {
                                        ListNguyenNhan.Add("Chưa điền tên cán bộ");
                                    }
                                    //if (string.IsNullOrEmpty(item.NgaySinh.ToString()))
                                    //{
                                    //    ListNguyenNhan.Add("Ngày sinh đang để trống");
                                    //}
                                    if (!string.IsNullOrEmpty(LoiNgaySinh))
                                    {
                                        ListNguyenNhan.Add(LoiNgaySinh);
                                    }
                                    if (item.GioiTinh < 0 || item.GioiTinh == null)
                                    {
                                        ListNguyenNhan.Add("Thiếu giới tính");
                                    }
                                    //if (item.DanhSachChucVuID.Count <= 0)
                                    //{
                                    //    ListNguyenNhan.Add("Chưa chọn chức vụ từng làm");
                                    //}
                                    if (item.CoQuanID <= 0)
                                    {
                                        ListNguyenNhan.Add("Nơi công tác đang để trống hoặc không đúng");
                                    }
                                    if (Utils.CheckPhoneNumber(item.DienThoai.ToString()) && !string.IsNullOrEmpty(item.DienThoai.ToString()))
                                    {
                                        ListNguyenNhan.Add("Điện thoại không đúng định dạng");
                                    }
                                    if (!Utils.CheckEmail(item.Email.ToString()) && !string.IsNullOrEmpty(item.Email.ToString()))
                                    {
                                        ListNguyenNhan.Add("Email không đúng định dạng");
                                    }
                                    if (item.DanhSachChucVuID.Count <= 0)
                                    {
                                        ListNguyenNhan.Add("Chưa chọn chức vụ");
                                    }
                                    if (string.IsNullOrEmpty(item.HoKhau))
                                    {
                                        ListNguyenNhan.Add("Hộ khẩu thường trú đang để trống");
                                    }
                                    if (string.IsNullOrEmpty(item.DiaChi))
                                    {
                                        ListNguyenNhan.Add("Chỗ ở hiện nay đang để trống");
                                    }
                                    //if (item.TrangThaiID <= 0)
                                    //{
                                    //    ListNguyenNhan.Add("Trạng thái đang để trống");
                                    //}
                                    if (item.CapQuanLy <= 0)
                                    {
                                        ListNguyenNhan.Add("Chưa chọn cấp quản lý");
                                    }
                                    if (item.VaiTro <= 0)
                                    {
                                        ListNguyenNhan.Add("Vai trò đang để trống hoặc vai trò không đúng");
                                    }
                                    if (item.CapQuanLy == EnumCapQuanLyCanBo.CapTinh.GetHashCode())
                                    {
                                        HeThongCanBoPartialModel.TenCapQuanLy = "Cấp tỉnh";
                                    }
                                    else if (item.CapQuanLy == EnumCapQuanLyCanBo.CapHuyen.GetHashCode())
                                    {
                                        HeThongCanBoPartialModel.TenCapQuanLy = "Cấp huyện";
                                    }

                                }
                                HeThongCanBoPartialModel.NguyenNhan = ListNguyenNhan;
                                HeThongCanBoPartialModel.TenCanBo = item.TenCanBo;
                                HeThongCanBoPartialModel.DanhSachTenChucVu = ListStringChucVu;
                                HeThongCanBoPartialModel.NgaySinh = item.NgaySinh;
                                HeThongCanBoPartialModel.GioiTinh = item.GioiTinh;
                                HeThongCanBoPartialModel.TenCoQuan = new DanhMucCoQuanDonViDAL().GetByID(item.CoQuanID).TenCoQuan;
                                HeThongCanBoPartialModel.Email = item.Email;
                                HeThongCanBoPartialModel.DienThoai = item.DienThoai;
                                HeThongCanBoPartialModel.HoKhau = item.HoKhau;
                                HeThongCanBoPartialModel.DiaChi = item.DiaChi;
                                HeThongCanBoPartialModel.VaiTro = item.VaiTro;
                                HeThongCanBoPartialModel.MaCB = item.MaCB;
                                if (ListNguyenNhan.Count > 0)
                                {
                                    ListCanBoPar.Add(HeThongCanBoPartialModel);
                                }
                                //else
                                //{
                                //    listCanBo.Add()
                                //}
                                ListMaCB.AddRange(list.Where(x => !ListMaCB.Contains(x.MaCB.ToString().Trim().ToLower()) && string.IsNullOrEmpty(x.MaCB.ToString()) == false).Select(y => y.MaCB.ToString().Trim().ToLower()));
                            }


                        }
                        if (ListCanBoPar.Where(x => x.NguyenNhan.Count > 0).ToList().Count <= 0)
                        {
                            foreach (var item in listCanBo)
                            {
                                Dictionary<int, int> ListCanBoChucVu = new Dictionary<int, int>();
                                item.DanhSachChucVuID.ForEach(x => ListCanBoChucVu.Add(x, item.CanBoID));
                                var CanBoID = 0;
                                string Message = null;
                                val = Insert(item, ref CanBoID, ref Message);
                                InsertCanBoChucVu(ListCanBoChucVu);

                            }
                        }
                    }


                }
                return ListCanBoPar;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Import data to exel 
        public int ImportToExel(string FilePath, int? CoQuanID)
        {
            int val = 0;
            if (!File.Exists(FilePath))
            {
                return val;
            }
            try
            {
                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var TotalWorksheet = package.Workbook.Worksheets.Count;
                    if (TotalWorksheet <= 0)
                    {
                        return val;
                    }
                    else
                    {

                        for (int i = 0; i < TotalWorksheet; i++)
                        {
                            DataTable dt = new DataTable();
                            if (package.Workbook.Worksheets[i].Name.ToString() == "Chuc_Vu")
                            {
                                var ListChucVu = new DanhMucChucVuDAL().GetAllChucVu();
                                dt.Columns.Add("STT");
                                dt.Columns.Add("TenChucVu");
                                foreach (var item in ListChucVu)
                                {

                                    dt.Rows.Add(item.ChucVuID, item.TenChucVu);
                                }
                                //XLWorkbook wbook = new ClosedXML.Excel.XLWorkbook(FilePath);
                                //wbook.Worksheet(package.Workbook.Worksheets[i].Name.ToString()).Clear();
                                package.Workbook.Worksheets[i].Cells.Clear();
                                int rowIndex = 0;
                                foreach (DataRow dr in dt.Rows)
                                {
                                    rowIndex++;
                                    int colIndex = 0;
                                    package.Workbook.Worksheets[i].Cells[1, 1].Value = "ID";
                                    package.Workbook.Worksheets[i].Cells[1, 2].Value = "Chức Vụ";
                                    foreach (DataColumn dc in dt.Columns)
                                    {
                                        colIndex++;
                                        package.Workbook.Worksheets[i].SetValue(rowIndex + 1, colIndex, dr[dc.ColumnName]);
                                    }
                                }
                                package.Save();
                                dt.Clear();

                            }
                            if (package.Workbook.Worksheets[i].Name.ToString() == "Noi_Cong_Tac")
                            {
                                var ListCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID.Value);
                                dt.Columns.Add("STT");
                                dt.Columns.Add("NoiCongTac");
                                foreach (var item in ListCoQuan)
                                {

                                    dt.Rows.Add(item.CoQuanID, item.TenCoQuan);
                                }
                                package.Workbook.Worksheets[i].Cells.Clear();
                                int rowIndex = 0;
                                foreach (DataRow dr in dt.Rows)
                                {
                                    rowIndex++;
                                    int colIndex = 0;
                                    package.Workbook.Worksheets[i].Cells[1, 1].Value = "ID";
                                    package.Workbook.Worksheets[i].Cells[1, 2].Value = "Nơi công tác";
                                    foreach (DataColumn dc in dt.Columns)
                                    {
                                        colIndex++;
                                        package.Workbook.Worksheets[i].SetValue(rowIndex + 1, colIndex, dr[dc.ColumnName]);
                                    }
                                }
                                package.Save();
                                dt.Clear();
                            }
                            if (package.Workbook.Worksheets[i].Name.ToString() == "Trang_Thai")
                            {
                                var ListTrangThai = new DanhMucTrangThaiDAL().GetAll().Where(x => x.TrangThaiSuDung == true).ToList();
                                dt.Columns.Add("STT");
                                dt.Columns.Add("TenTrangThai");
                                foreach (var item in ListTrangThai)
                                {

                                    dt.Rows.Add(item.TrangThaiID, item.TenTrangThai);
                                }
                                package.Workbook.Worksheets[i].Cells.Clear();
                                int rowIndex = 0;
                                foreach (DataRow dr in dt.Rows)
                                {
                                    rowIndex++;
                                    int colIndex = 0;
                                    package.Workbook.Worksheets[i].Cells[1, 1].Value = "ID";
                                    package.Workbook.Worksheets[i].Cells[1, 2].Value = "Trạng Thái";
                                    foreach (DataColumn dc in dt.Columns)
                                    {
                                        colIndex++;
                                        package.Workbook.Worksheets[i].SetValue(rowIndex + 1, colIndex, dr[dc.ColumnName]);
                                    }
                                }
                                package.Save();
                                dt.Clear();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 1;
        }

        //Chek Reference
        public bool CheckRef(int CanBoID)
        {
            try
            {
                //var CanBoByDotKeKhai = new KeKhaiTaiSanDAL().GetAll().Where(x => x.CanBoID == CanBoID).ToList().Count();
                //var checkThanNhan = new KeKhaiThanNhanDAL().GetByCanBoID(CanBoID);
                HeThongNguoiDungModel nguoiDungInfo = new HeThongNguoiDungDAL().GetByCanBoID(CanBoID);
                NguoiDungNhomNguoiDungModel nguoiDung_nhomNguoiDungInfo = new PhanQuyenDAL().NguoiDung_NhomNguoiDung_GetByNguoiDungID_ForCheckRef(nguoiDungInfo.NguoiDungID);
                ////var checkDotKeKhai = new DotKeKhaiDAL().GetDotKeKhaiByCanBoID(CanBoID);
                if (nguoiDung_nhomNguoiDungInfo.NguoiDungID > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return true;
                throw ex;
            }
        }

        // Convert sang chức vụ id by tên chức vụ
        public int ConvertChucVuIDByName(string TenChucVu)
        {
            return new DanhMucChucVuDAL().GetChucVuByName(TenChucVu).ChucVuID;
        }
        // Convert sang chức vụ id by tên chức vụ
        public int ConvertCoQuanIDByName(string TenCoQuan)
        {
            return new DanhMucCoQuanDonViDAL().GetByName(TenCoQuan).CoQuanID;
        }

        // Convert sang trạng thái id by tên chức vụ
        public int ConvertTrangThaiIDByName(string TenTrangThai)
        {
            return new DanhMucTrangThaiDAL().GetByName(TenTrangThai).TrangThaiID;
        }

        public List<HeThongCanBoModel> GetCanBoByTrangThaiID(int? CanBoID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TrangThaiID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetByTrangThaiID", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        Result.Add(canBo);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Result;
        }

        /// <summary>
        /// lấy toàn bộ cán bộ thuộc danh sách cơ quan
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAllByListCoQuanID(List<int> CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            var table = new DataTable();
            table.Columns.Add("CoQuanID", typeof(string));
            CoQuanID.ForEach(x => table.Rows.Add(x));

            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.id_list";
            SqlParameter[] parameters = new SqlParameter[]
            {
                pList
            };
            parameters[0].Value = table;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInListCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        Result.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }


        /// <summary>
        ///  lấy tất cả cán bộ trong cơ quan và cơ quan con
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAllByCoQuanID(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            var DanhSachCoQuanID = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID).Select(x => x.CoQuanID).ToList();
            var table = new DataTable();
            table.Columns.Add("CoQuanID", typeof(string));
            DanhSachCoQuanID.ForEach(x => table.Rows.Add(x));

            var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            pList.TypeName = "dbo.id_list";
            SqlParameter[] parameters = new SqlParameter[]
            {
                pList
            };
            parameters[0].Value = table;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInListCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        if (canBo.CanBoID != 1)
                            Result.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        public List<HeThongCanBoModel> GetAllByNhiemVuID(int NhiemVuID)
        {
            var Result = new List<HeThongCanBoModel>();
            //var DanhSachCoQuanID = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID).Select(x => x.CoQuanID).ToList();

            //var table = new DataTable();
            //table.Columns.Add("CoQuanID", typeof(string));
            //DanhSachCoQuanID.ForEach(x => table.Rows.Add(x));

            //var pList = new SqlParameter("@ListCoQuanID", SqlDbType.Structured);
            //pList.TypeName = "dbo.id_list";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NhiemVuID",SqlDbType.Int)
            };
            parameters[0].Value = NhiemVuID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllByNhiemVuID", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        //canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        if (canBo.CanBoID != 1)
                            Result.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        /// <summary>
        /// lấy toàn bộ cán bộ trong 1 cơ quan 
        /// lấy cả danh sách chức vụ, tên chức vụ
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAllInCoQuanID(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CoQuanID", SqlDbType.Int)
        };
            parameters[0].Value = CoQuanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        //if (Utils.ConvertToInt32(dr["TrangThaiID"], 0) == EnumTrangThaiCanBo.DangLamViec.GetHashCode())
                        //{
                        HeThongCanBoPartialModel canBo = new HeThongCanBoPartialModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        Result.Add(canBo);

                        //}
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public List<HeThongCanBoShortModel> GetThanNhanByCanBoID(int CanBoID)
        {
            List<HeThongCanBoShortModel> list = new List<HeThongCanBoShortModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CanBoID",SqlDbType.Int)
            };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetThanNhan_ByCanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoShortModel canBo = new HeThongCanBoShortModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.ThanNhanID = Utils.ConvertToInt32(dr["ThanNhanID"], 0);
                        canBo.HoTenThanNhan = Utils.ConvertToString(dr["HoTen"], string.Empty);
                        list.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        /// <summary>
        /// Lấy chi tiết thông tin cán bộ cho chi tiết bản kê khai  
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <returns></returns>
        public HeThongCanBoPartialModel GetChiTietCanBoByID(int CanBoID)
        {
            HeThongCanBoPartialModel canBo = new HeThongCanBoPartialModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetChiTiet_ByID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoPartialModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        var DanhSachChucVu = CanBoChucVu_GetChucVuCuaCanBo(CanBoID);
                        canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }

        public List<HeThongCanBoModel> GetAllInCoQuanCha(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@CoQuanID",SqlDbType.Int)
             };
            parameters[0].Value = CoQuanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInCoQuanCha", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        Result.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }


        /// <summary>
        /// lấy tất cả cán bộ thuộc cấp quản lý và là cán bộ của đơn vị hiện tại và các đơn vị con
        /// </summary>
        /// <param name="CapQuanLy"></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(int? CapQuanLy, int? CoQuanID)
        {
            List<HeThongCanBoModel> Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CapQuanLy",SqlDbType.Int),
                new SqlParameter("@CoQuanID",SqlDbType.Int)
            };
            parameters[0].Value = CapQuanLy ?? Convert.DBNull;
            parameters[1].Value = CoQuanID ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        Result.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var canbo = Result.Where(x => x.CanBoID == 10).ToList().FirstOrDefault();
            return Result;
        }


        /// <summary>
        /// lấy danh sách cán bộ có quyền Checkin-out (quyền >1 - thêm mới trở lên), là cán bộ trong cơ quan sử dụng phần mềm
        ///
        /// </summary>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetDanhSachLeTan_TrongCoQuanSuDungPhanMem(int CoQuanID)
        {
            List<HeThongCanBoModel> Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@CoQuanID",SqlDbType.Int) // id đơn vị sử dụng phần mềm
           };
            parameters[0].Value = CoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_HeThong_CanBo_DanhSachLeTan_TrongCoQuanSuDungPhanMem", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        //canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        //canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        //canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        //canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        //canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        //canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        //canBo.CapCoQuanID = Utils.ConvertToInt32(dr["CapID"], 0);
                        //canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(canBo.CanBoID);
                        //canBo.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        //canBo.LaLeTan = Utils.ConvertToBoolean(dr["LaLeTan"], false);
                        if (canBo.CanBoID != 1)
                        {
                            Result.Add(canBo);
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //list = list.OrderBy(x => x.CapCoQuanID).ToList();
            return Result;
        }


        /// <summary>
        /// lấy danh sách cán bộ có quyền Checkin-out (quyền >1 - thêm mới trở lên) trong toàn bộ hệ thống
        /// </summary>
        /// <returns></returns>
        public List<HeThongCanBoModel> GetDanhSachLeTan()
        {
            List<HeThongCanBoModel> Result = new List<HeThongCanBoModel>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_CanBo_GetDanhSachLeTan"))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        //canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        //canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        //canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        //canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        //canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        //canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        //canBo.CapCoQuanID = Utils.ConvertToInt32(dr["CapID"], 0);
                        //canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(canBo.CanBoID);
                        //canBo.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        //canBo.LaLeTan = Utils.ConvertToBoolean(dr["LaLeTan"], false);
                        if (canBo.CanBoID != 1)
                        {
                            Result.Add(canBo);
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //list = list.OrderBy(x => x.CapCoQuanID).ToList();
            return Result;
        }


        /// <summary>
        /// lấy toàn bộ cán bộ thuộc hệ thống cơ quan của cơ quan sử dụng phần mềm theo cơ quan đăng nhập
        /// </summary>
        /// <param name="CoQuanID"> là cơ quan đăng nhập hiện tại</param>
        /// <returns></returns>
        public List<HeThongCanBoModel> DanhSachCanBo_TrongCoQuanSuDungPhanMem(int CoQuanID)
        {

            var Result = new List<HeThongCanBoModel>();

            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@CoQuanID",SqlDbType.Int)
             };
            parameters[0].Value = CoQuanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_DanhSachCanBo_TrongCoQuanSuDungPhanMem", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        //canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        //canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        //canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        //canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        //canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(canBo.CanBoID);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        //canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        //canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        canBo.LaLeTan = Utils.ConvertToBoolean(dr["LaLeTan"], false);
                        Result.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }





        #endregion


        #region HT_CanBo_Chuc_Vu

        public int CanBoChucVu_Insert(List<int> ListChucVu, int CanBoID, ref string Message)
        {
            int val = 0;
            if (ListChucVu == null)
            {
                ListChucVu = new List<int>();
            }
            var table = new DataTable();
            table.Columns.Add("ID1", typeof(string));
            table.Columns.Add("ID2", typeof(string));
            for (int i = 0; i < ListChucVu.Count; i++)
            {
                var nrow = table.NewRow();
                nrow["ID1"] = CanBoID;
                nrow["ID2"] = ListChucVu[i];
                table.Rows.Add(nrow);
            }

            var pList = new SqlParameter("@list_idCanBo_idChucVu", SqlDbType.Structured);
            pList.TypeName = "dbo.ID1_ID2_list";
            SqlParameter[] parameters = new SqlParameter[]
            {
                pList
            };
            parameters[0].Value = table;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_Insert", parameters), 0);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                    Message = ConstantLogMessage.Alert_Insert_Success("Cán bộ - chức vụ");
                    return val;
                }
            }

        }

        public List<int> CanBoChucVu_GetBy_CanBoID(int CanBoID)
        {
            List<int> Result = new List<int>();
            SqlParameter[] parameters = new SqlParameter[]
           {
                 new SqlParameter("@CanBoID", SqlDbType.Int)
           };
            parameters[0].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_GetBy_CanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        Result.Add(Utils.ConvertToInt32(dr["ChucVuID"], 0));
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        public List<int> NguoiDungNhomNguoiDung_GetBy_NguoiDungID(int? NguoiDungID)
        {
            List<int> Result = new List<int>();
            SqlParameter[] parameters = new SqlParameter[]
           {
                 new SqlParameter("@NguoiDungID", SqlDbType.Int)
           };
            parameters[0].Value = NguoiDungID ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_NguoiDung_NhomNguoiDung_GetBy_NguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        Result.Add(Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0));
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        public List<DanhMucChucVuModel> CanBoChucVu_GetChucVuCuaCanBo(int CanBoID)
        {
            List<DanhMucChucVuModel> Result = new List<DanhMucChucVuModel>();
            SqlParameter[] parameters = new SqlParameter[]
           {
                 new SqlParameter("@CanBoID", SqlDbType.Int)
           };
            parameters[0].Value = CanBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_GetBy_CanBoID", parameters))
                {
                    while (dr.Read())
                    {
                        var item = new DanhMucChucVuModel();
                        item.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        item.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        Result.Add(item);
                    }
                    dr.Close();
                }
                Result.GroupBy(x => x.ChucVuID).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public int CanBoChucVu_Delete_By_CanBoID(int CanBoID, ref string message)
        {
            message = "";
            var val = 0;

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter(@"CanBoID", SqlDbType.Int)
            };
            parameters[0].Value = CanBoID;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_Delete_ByCanBoID", parameters);
                        trans.Commit();
                        if (val < 0)
                        {
                            message = "Không thể xóa chức vụ của cán bộ";
                        }
                        else message = ConstantLogMessage.Alert_Delete_Success("Cán bộ - chức vụ");
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return val;

        }

        public int NguoiDungNhomNguoiDung_Delete_By_CanBoID(int NguoiDungID, ref string message)
        {
            message = "";
            var val = 0;

            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter(@"NguoiDungID", SqlDbType.Int)
            };
            parameters[0].Value = NguoiDungID;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_NguoiDung_NhomNguoiDung_Delete_ByCanBoID", parameters);
                        trans.Commit();
                        if (val < 0)
                        {
                            message = "Không thể xóa chức vụ của cán bộ";
                        }
                        else message = ConstantLogMessage.Alert_Delete_Success("Người dùng - nhóm người dùng");
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return val;

        }

        public int GetCapQuanLyID(string TenCapQuanLy)
        {
            if (TenCapQuanLy == "Toàn tỉnh")
            {
                return 0;
            }
            else if (TenCapQuanLy == "Cấp tỉnh")
            {
                return 1;

            }

            else if (TenCapQuanLy == "Cấp huyện")
            {
                return 2;
            }

            return 0;
        }
        public List<CanBoChuVu> CanBoChucVu_GetAll()
        {
            List<CanBoChuVu> list = new List<CanBoChuVu>();

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_GetAll"))
                {
                    while (dr.Read())
                    {
                        CanBoChuVu canBo = new CanBoChuVu();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        list.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return list;
        }

        public int InsertCanBoChucVu(Dictionary<int, int> ListCanBoChucVu)
        {
            var val = 0;
            var pList = new SqlParameter("@list_idCanBo_idChucVu", SqlDbType.Structured);
            pList.TypeName = "dbo.id_id_list";
            var tbChucVuCanBo = new DataTable();
            tbChucVuCanBo.Columns.Add("ChucVuID");
            tbChucVuCanBo.Columns.Add("CanBoID");
            foreach (var item in ListCanBoChucVu)
            {
                var newrow = tbChucVuCanBo.NewRow();
                newrow["ChucVuID"] = item.Key;
                newrow["CanBoID"] = item.Value;
                tbChucVuCanBo.Rows.Add(newrow);
            }
            SqlParameter[] parameters = new SqlParameter[]
           {
           pList
           };
            parameters[0].Value = tbChucVuCanBo;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_ChucVu_Insert", parameters);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    return val;
                }
            }
        }

        // Get all cán bộ by coquanid
        public List<HeThongCanBoModel> GetAllCanBoByCoQuanID(int CoQuanID, int CoQuan_ID)
        {
            List<DanhMucCoQuanDonViModel> ListCoQuanCon = new List<DanhMucCoQuanDonViModel>();
            if (CoQuanID <= 0)
            {
                ListCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuan_ID);
            }
            else
            {
                ListCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
            }
            List<int> list = new List<int>();
            ListCoQuanCon.ForEach(x => list.Add(x.CoQuanID));
            List<HeThongCanBoModel> ListCanBoByCoQuanID = new HeThongCanBoDAL().GetAllCanBoWithoutNguoiDung().Where(x => list.Contains(x.CoQuanID.Value)).ToList();
            return ListCanBoByCoQuanID;
        }

        // Get All Cán bộ by CoQuanID and ChucVuID
        public List<HeThongCanBoModel> GetAllCanBoByChucVuIDAndCoQuanID(int? ChucVuID, int? CoQuanID)
        {
            List<HeThongCanBoModel> list = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@ChucVuID",SqlDbType.Int),
            new SqlParameter("@CoQuanID",SqlDbType.Int)
            };
            parameters[0].Value = ChucVuID ?? Convert.DBNull;
            parameters[1].Value = CoQuanID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllCanBoByChucVuAndCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        HeThongCanBoModel canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        // canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        list.Add(canBo);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return list;

        }

        //Get Cán bộ by nguoidungid
        public HeThongCanBoModel GetCanBoByNguoiDungID(int? NguoiDungID)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@NguoiDungID",SqlDbType.Int)

            };
            parameters[0].Value = NguoiDungID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetCanBoByNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        // canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }

        // get cán bộ by chức vụ id
        public HeThongCanBoModel GetCanBoByChucVuID(int? ChucVuID)
        {
            HeThongCanBoModel canBo = new HeThongCanBoModel();
            SqlParameter[] parameters = new SqlParameter[]
            {
            new SqlParameter("@ChucVuID",SqlDbType.Int)

            };
            parameters[0].Value = ChucVuID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetCanBoByChucVuID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        //canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        //canBo.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.Now);
                        //canBo.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
                        //canBo.DiaChi = Utils.ConvertToString(dr["DiaChi"], string.Empty);
                        canBo.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        //canBo.QuyenKy = Utils.ConvertToInt32(dr["QuyenKy"], 0);
                        //canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        //canBo.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        //canBo.PhongBanID = Utils.ConvertToInt32(dr["PhongBanID"], 0);
                        //canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        // canBo.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
                        //canBo.QuanTridonVi = Utils.ConvertToInt32(dr["QuanTridonVi"], 0);
                        //canBo.CoQuanCuID = Utils.ConvertToInt32(dr["CoQuanCuID"], 0);
                        //canBo.CanBoCuID = Utils.ConvertToInt32(dr["CanBoCuID"], 0);
                        //canBo.XemTaiLieuMat = Utils.ConvertToInt32(dr["XemTaiLieuMat"], 0);
                        //canBo.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        //canBo.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        //canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.CapQuanLy = Utils.ConvertToInt32(dr["CapQuanLy"], 0);
                        //canBo.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiID"], 0);
                        //canBo.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        //canBo.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;

        }

        public List<HeThongCanBoModel> GetListByCoQuanID(int CoQuanID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CoQuanID", SqlDbType.Int)
        };
            parameters[0].Value = CoQuanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllInCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        if (Utils.ConvertToInt32(dr["TrangThaiID"], 0) == EnumTrangThaiCanBo.DangLamViec.GetHashCode())
                        {
                            HeThongCanBoPartialModel canBo = new HeThongCanBoPartialModel();
                            canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                            canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                            canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                            canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                            canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                            //var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                            //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                            //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                            Result.Add(canBo);

                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        public List<HeThongCanBoModel> GetListByNhiemVuChaID(int NhiemVuChaID)
        {
            var Result = new List<HeThongCanBoModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NhiemVuChaID", SqlDbType.Int)
            };
            parameters[0].Value = NhiemVuChaID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_HeThong_CanBo_GetAllByNhiemVuChaID", parameters))
                {
                    while (dr.Read())
                    {
                        //if (Utils.ConvertToInt32(dr["TrangThaiID"], 0) == EnumTrangThaiCanBo.DangLamViec.GetHashCode())
                        //{
                        HeThongCanBoPartialModel canBo = new HeThongCanBoPartialModel();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.MaCB = Utils.ConvertToString(dr["MaCB"], string.Empty);
                        //canBo.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        //var DanhSachChucVu = new HeThongCanBoDAL().CanBoChucVu_GetChucVuCuaCanBo(canBo.CanBoID);
                        //canBo.DanhSachChucVuID = DanhSachChucVu.Select(x => x.ChucVuID).ToList();
                        //canBo.DanhSachTenChucVu = DanhSachChucVu.Select(x => x.TenChucVu).ToList();
                        Result.Add(canBo);

                        //}
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        // Lấy danh sách cán bộ đã nghỉ hưu hoặc nghỉ việc
        public int GetListCanBo_Expire(int CanBoID, int CoQuanID)
        {
            var pList = new SqlParameter("@ListCanBoID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
            var CapQuanLy = 2;
            var CoQuanQuanLy = 0;
            if (UserRole.CheckAdmin(CanBoID))
            {
                // TrangThai = 200;
                CapQuanLy = 0;
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
            {

            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
            {
                //  TrangThai = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
            {
                // TrangThai = 300;
                CapQuanLy = EnumCapQuanLyCanBo.CapTinh.GetHashCode();
                CoQuanQuanLy = 0;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
            {
                // TrangThai = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanID;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
            {

                //TrangThai = 300;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanChaID.Value;
            }
            else if (crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode())      // cấp xã
            {
                //  TrangThai = 200;
                CapQuanLy = EnumCapQuanLyCanBo.CapHuyen.GetHashCode();
                CoQuanQuanLy = crCoQuan.CoQuanID;

            }
            int val = 0;
            var listCanBoAll = new HeThongCanBoDAL().GetAll_By_CapQuanLy_And_DonViID_And_DonViChaID(CapQuanLy, CoQuanQuanLy);
            var list = listCanBoAll.Where(x => x.TrangThaiID == 2).ToList();
            list.ForEach(x => tbCanBoID.Rows.Add(x.CanBoID));
            SqlParameter[] parameters = new SqlParameter[]
           {
                new SqlParameter("@TrangThai",SqlDbType.Int),
              pList

           };
            parameters[0].Value = 0;
            parameters[1].Value = tbCanBoID;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HeThongNguoiDung_UpdateTrangThai", parameters);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                    return val;
                }
            }
        }


        #region V2 - anhvh - 07/08/2021
        /// <summary>
        /// thêm admin cho đơn vị sử dụng phần mền,
        /// thêm cán bộ, thêm người dùng
        /// </summary>
        /// <param name="canBoInfo"></param>
        /// <returns></returns>
        public BaseResultModel Insert_AdminDonVi(HeThongCanBoModel canBoInfo)
        {
            var result = new BaseResultModel();
            var CanBoID = 0;
            var NguoiDungID = 0;
            if (string.IsNullOrEmpty(canBoInfo.TenCanBo) || canBoInfo.TenCanBo.Trim().Length <= 0)
            {
                result.Message = ConstantLogMessage.API_Error_NotFill;
                result.Status = 0;
                return result;
            }
            if (canBoInfo.TenCanBo.Trim().Length > 100)
            {
                result.Message = ConstantLogMessage.API_Error_TooLong;
                result.Status = 0;
                return result;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                    new SqlParameter("DienThoai",  SqlDbType.VarChar),
                    new SqlParameter("Email",  SqlDbType.NVarChar)
              };
            parameters[0].Value = canBoInfo.TenCanBo;
            parameters[1].Value = canBoInfo.CoQuanID ?? Convert.DBNull;
            parameters[2].Value = canBoInfo.DienThoai ?? Convert.DBNull;
            parameters[3].Value = canBoInfo.Email ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        CanBoID = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, @"v2_HeThong_CanBo_InsertAdminDonVi", parameters), 0);
                        trans.Commit();
                        if (CanBoID > 0)
                        {
                            var mess = "";
                            HeThongNguoiDungModel nguoiDungInfo = new HeThongNguoiDungModel();
                            nguoiDungInfo.CanBoID = CanBoID;
                            nguoiDungInfo.TenNguoiDung = canBoInfo.TenNguoiDung;
                            var matKhauMacDinh = new SystemConfigDAL().GetByKey("MatKhau_MacDinh").ConfigValue;
                            nguoiDungInfo.MatKhau = Cryptor.EncryptPasswordUser(nguoiDungInfo.TenNguoiDung.Trim().ToLower(), matKhauMacDinh ?? "123456");
                            nguoiDungInfo.TrangThai = 1;
                            var insertNguoiDung = new HeThongNguoiDungDAL().Insert(nguoiDungInfo, ref mess, ref NguoiDungID);
                            if (insertNguoiDung <= 0) { result.Message = mess; result.Status = 0; return result; }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Message = ConstantLogMessage.API_Error;
                        result.Status = 0;
                        return result;
                    }
                    result.Message = ConstantLogMessage.Alert_Insert_Success("cán bộ");
                    result.Status = 1;
                    result.Data = NguoiDungID;
                    return result;
                }
            }

        }

        /// <summary>
        /// thêm mới cán bộ vào phòng ban,
        /// thêm cán bộ, thêm người dùng, thêm vào nhóm người dùng
        /// </summary>
        /// <param name="canBoInfo"></param>
        /// <returns></returns>
        public BaseResultModel ThemMoiCanBoChoPhongBan(HeThongCanBoV2Model canBoInfo, Boolean ThemMoiCoQuan)
        {
            var result = new BaseResultModel();
            var NguoiDungID = 0;
            if (string.IsNullOrEmpty(canBoInfo.TenCanBo) || canBoInfo.TenCanBo.Trim().Length <= 0)
            {
                result.Message = ConstantLogMessage.API_Error_NotFill;
                result.Status = 0;
                return result;
            }
            if (canBoInfo.TenCanBo.Trim().Length > 500)
            {
                result.Message = ConstantLogMessage.API_Error_TooLong;
                result.Status = 0;
                return result;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                    new SqlParameter("Email",  SqlDbType.NVarChar)
              };
            parameters[0].Value = canBoInfo.TenCanBo;
            parameters[1].Value = canBoInfo.CoQuanID;
            parameters[2].Value = canBoInfo.Email ?? Convert.DBNull;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // thêm cán bộ
                        result.Status = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, @"v2_HeThong_CanBo_ThemMoiCanBoVaoPhongBan", parameters), 0);
                        trans.Commit();
                        if (result.Status < 10)
                        {
                            trans.Rollback();
                            result.Message = ConstantLogMessage.API_Error;
                            result.Status = 0;
                            return result;
                        }
                        // thêm người dùng
                        var mess = "";
                        HeThongNguoiDungModel nguoiDungInfo = new HeThongNguoiDungModel();
                        nguoiDungInfo.CanBoID = result.Status;
                        nguoiDungInfo.TenNguoiDung = canBoInfo.Email;
                        var matKhauMacDinh = new SystemConfigDAL().GetByKey("MatKhau_MacDinh").ConfigValue;
                        nguoiDungInfo.MatKhau = Cryptor.EncryptPasswordUser(nguoiDungInfo.TenNguoiDung.Trim().ToLower(), matKhauMacDinh ?? "123456");
                        nguoiDungInfo.TrangThai = 1;
                        var insertNguoiDung = new HeThongNguoiDungDAL().Insert(nguoiDungInfo, ref mess, ref NguoiDungID);
                        if (insertNguoiDung <= 0) { result.Message = mess; result.Status = 0; return result; }
                        // thêm người dùng vào nhóm
                        if (canBoInfo.DanhSachNhomNguoiDung != null && canBoInfo.DanhSachNhomNguoiDung.Count > 0)
                        {
                            var phanQuyen = new PhanQuyenDAL();
                            foreach (var item in canBoInfo.DanhSachNhomNguoiDung)
                            {
                                var nhomNguoiDungDung = item.NhomNguoiDungID;
                                if (ThemMoiCoQuan)
                                {
                                    var dataNhomNguoiDung = new PhanQuyenDAL().NhomNguoiDungCuaCoQuan(item.NhomNguoiDungID, canBoInfo.CoQuanID);
                                    if (dataNhomNguoiDung != null && dataNhomNguoiDung.NhomNguoiDungID > 0)
                                        nhomNguoiDungDung = dataNhomNguoiDung.NhomNguoiDungID;
                                }
                                var isNguoiDungVaoNhom = phanQuyen.PhanQuyen_ThemNguoiDungVaoNhonm(new NguoiDungNhomNguoiDungModel() { NguoiDungID = NguoiDungID, NhomNguoiDungID = nhomNguoiDungDung });
                                if (isNguoiDungVaoNhom.Status < 1)
                                {
                                    return isNguoiDungVaoNhom;
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Message = ConstantLogMessage.API_Error;
                        result.Status = 0;
                        return result;
                    }
                    result.Message = ConstantLogMessage.Alert_Insert_Success("cán bộ");
                    result.Status = 1;
                    result.Data = NguoiDungID;
                    return result;
                }
            }

        }


        public BaseResultModel CapNhatThongTinCanBo(HeThongCanBoV2Model canBoInfo)
        {
            var result = new BaseResultModel();
            if (canBoInfo.CanBoID <= 0)
            {

                result.Message = "Chưa có cán bộ được chọn!";
                result.Status = 0;
                return result;
            }
            if (canBoInfo.TenCanBo.Trim().Length > 500)
            {
                result.Message = ConstantLogMessage.API_Error_TooLong;
                result.Status = 0;
                return result;
            }

            var crCanBo = GetCanBoByID(canBoInfo.CanBoID);
            if (crCanBo == null || crCanBo.CanBoID < 1)
            {
                result.Message = ConstantLogMessage.Alert_Error_NotExist("Cán bộ");
                result.Status = 0;
                return result;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_CanBoID, SqlDbType.Int),
                new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                new SqlParameter(PARAM_Email, SqlDbType.NVarChar),
                new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                new SqlParameter("NguoiDungID", SqlDbType.Int)
              };
            parameters[0].Value = canBoInfo.CanBoID;
            parameters[1].Value = canBoInfo.TenCanBo ?? Convert.DBNull;
            parameters[2].Value = canBoInfo.Email ?? Convert.DBNull;
            parameters[3].Value = canBoInfo.CoQuanID;
            parameters[4].Value = canBoInfo.NguoiDungID;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v2_CapNhatThongTinCanBo", parameters);
                        trans.Commit();
                        if (result.Status < 1)
                        {
                            result.Message = ConstantLogMessage.API_Error;
                            result.Status = -1;
                            return result;
                        }
                        if (canBoInfo.DanhSachNhomNguoiDung != null && canBoInfo.DanhSachNhomNguoiDung.Count > 0)
                        {
                            var phanQuyen = new PhanQuyenDAL();
                            foreach (var item in canBoInfo.DanhSachNhomNguoiDung)
                            {
                                var isNguoiDungVaoNhom = phanQuyen.PhanQuyen_ThemNguoiDungVaoNhonm(new NguoiDungNhomNguoiDungModel() { NguoiDungID = canBoInfo.NguoiDungID, NhomNguoiDungID = item.NhomNguoiDungID });
                                if (isNguoiDungVaoNhom.Status < 1)
                                {
                                    return isNguoiDungVaoNhom;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Message = ConstantLogMessage.API_Error;
                        result.Status = -1;
                        return result;
                    }
                    result.Message = ConstantLogMessage.Alert_Update_Success("cán bộ");
                    result.Status = 1;
                }
                return result;
            }
        }

        #endregion

        #region V4
        public BaseResultModel CapNhatFaceID(HeThongCanBoV2Model canBoInfo)
        {
            var result = new BaseResultModel();
            if (canBoInfo.CanBoID <= 0)
            {

                result.Message = "Chưa có cán bộ được chọn!";
                result.Status = 0;
                return result;
            }

            var crCanBo = GetCanBoByID(canBoInfo.CanBoID);
            if (crCanBo == null || crCanBo.CanBoID < 1)
            {
                result.Message = ConstantLogMessage.Alert_Error_NotExist("Cán bộ");
                result.Status = 0;
                return result;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_CanBoID, SqlDbType.Int),
                new SqlParameter("FaceID", SqlDbType.Int)
              };
            parameters[0].Value = canBoInfo.CanBoID;
            parameters[1].Value = canBoInfo.FaceID;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v4_CapNhatFaceID", parameters);
                        trans.Commit();
                        if (result.Status < 1)
                        {
                            result.Message = ConstantLogMessage.API_Error;
                            result.Status = -1;
                            return result;
                        } 
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Message = ConstantLogMessage.API_Error;
                        result.Status = -1;
                        return result;
                    }
                    result.Message = ConstantLogMessage.Alert_Update_Success("cán bộ");
                    result.Status = 1;
                }
                return result;
            }
        }

        public HeThongCanBoV2Model GetCanBoByID_v4(int? CanBoID)
        {
            HeThongCanBoV2Model canBo = new HeThongCanBoV2Model();
            if (CanBoID <= 0)
            {
                return canBo;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int)
              };
            parameters[0].Value = CanBoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v4_HeThong_CanBo_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        canBo = new HeThongCanBoV2Model();
                        canBo.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        canBo.FaceID = Utils.ConvertToInt32(dr["FaceID"], 0);
                        canBo.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
                        canBo.Email = Utils.ConvertToString(dr["Email"], string.Empty);
                        canBo.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        canBo.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                      
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return canBo;
        }

        public BaseResultModel ThemMoiCanBoChoPhongBan_v4(HeThongCanBoV2Model canBoInfo, Boolean ThemMoiCoQuan)
        {
            var result = new BaseResultModel();
            var NguoiDungID = 0;
            if (string.IsNullOrEmpty(canBoInfo.TenCanBo) || canBoInfo.TenCanBo.Trim().Length <= 0)
            {
                result.Message = ConstantLogMessage.API_Error_NotFill;
                result.Status = 0;
                return result;
            }
            if (canBoInfo.TenCanBo.Trim().Length > 500)
            {
                result.Message = ConstantLogMessage.API_Error_TooLong;
                result.Status = 0;
                return result;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_TenCanBo, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                    new SqlParameter("Email",  SqlDbType.NVarChar)
              };
            parameters[0].Value = canBoInfo.TenCanBo;
            parameters[1].Value = canBoInfo.CoQuanID;
            parameters[2].Value = canBoInfo.Email ?? Convert.DBNull;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // thêm cán bộ
                        result.Status = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, @"v2_HeThong_CanBo_ThemMoiCanBoVaoPhongBan", parameters), 0);
                        trans.Commit();
                        if (result.Status < 10)
                        {
                            trans.Rollback();
                            result.Message = ConstantLogMessage.API_Error;
                            result.Status = 0;
                            return result;
                        }
                        // thêm người dùng
                        var mess = "";
                        HeThongNguoiDungModel nguoiDungInfo = new HeThongNguoiDungModel();
                        nguoiDungInfo.CanBoID = result.Status;
                        nguoiDungInfo.TenNguoiDung = canBoInfo.Email;
                        if(canBoInfo.Email == null || string.IsNullOrEmpty(canBoInfo.Email))
                        {
                            nguoiDungInfo.TenNguoiDung = canBoInfo.TenTaiKhoan + nguoiDungInfo.CanBoID;
                        }
                        var matKhauMacDinh = new SystemConfigDAL().GetByKey("MatKhau_MacDinh").ConfigValue;
                        nguoiDungInfo.MatKhau = Cryptor.EncryptPasswordUser(nguoiDungInfo.TenNguoiDung.Trim().ToLower(), matKhauMacDinh ?? "123456");
                        nguoiDungInfo.TrangThai = 1;
                        var insertNguoiDung = new HeThongNguoiDungDAL().Insert(nguoiDungInfo, ref mess, ref NguoiDungID);
                        if (insertNguoiDung <= 0) { result.Message = mess; result.Status = 0; return result; }
                        // thêm người dùng vào nhóm
                        if (canBoInfo.DanhSachNhomNguoiDung != null && canBoInfo.DanhSachNhomNguoiDung.Count > 0)
                        {
                            var phanQuyen = new PhanQuyenDAL();
                            foreach (var item in canBoInfo.DanhSachNhomNguoiDung)
                            {
                                var nhomNguoiDungDung = item.NhomNguoiDungID;
                                if (ThemMoiCoQuan)
                                {
                                    var dataNhomNguoiDung = new PhanQuyenDAL().NhomNguoiDungCuaCoQuan(item.NhomNguoiDungID, canBoInfo.CoQuanID);
                                    if (dataNhomNguoiDung != null && dataNhomNguoiDung.NhomNguoiDungID > 0)
                                        nhomNguoiDungDung = dataNhomNguoiDung.NhomNguoiDungID;
                                }
                                var isNguoiDungVaoNhom = phanQuyen.PhanQuyen_ThemNguoiDungVaoNhonm(new NguoiDungNhomNguoiDungModel() { NguoiDungID = NguoiDungID, NhomNguoiDungID = nhomNguoiDungDung });
                                if (isNguoiDungVaoNhom.Status < 1)
                                {
                                    return isNguoiDungVaoNhom;
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        result.Message = ConstantLogMessage.API_Error;
                        result.Status = 0;
                        return result;
                    }
                    result.Message = ConstantLogMessage.Alert_Insert_Success("cán bộ");
                    result.Status = 1;
                    result.Data = NguoiDungID;
                    return result;
                }
            }

        }
        #endregion
    }

    #endregion

}
