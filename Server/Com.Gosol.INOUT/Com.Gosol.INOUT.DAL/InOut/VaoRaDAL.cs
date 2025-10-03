using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Com.Gosol.INOUT.Models.InOut;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.DAL.DanhMuc;
using System.Linq;
using Com.Gosol.INOUT.Security;
using Microsoft.Data.Sqlite;

namespace Com.Gosol.INOUT.DAL.InOut
{
    public interface IVaoRaDAL
    {
        public BaseResultModel Vao(ThongTinVaoRaModel VaoRaModel, ref string MaThe, ref string SoCMND, int CoQuanID);
        public BaseResultModel Ra(ThongTinVaoRaModel TTKhachModel);
        public List<ThongTinVaoRaModel> GetListPageBySearch(BasePagingParams p, int? Type, ref int TotalRow, int CoQuanID, int NguoidungID, string serverPath);
        public ThongTinVaoRaModel Get_By_ThongTinVaoRaID(int ThongTinVaoRaID, string serverPath);
        public List<ThongTinVaoRaModel> Get_By_MaThe(string MaThe, int LoaiCheckOut, int CoQuanID, string serverPath);
        public List<ThongTinVaoRaModel> Get_For_CheckOut_By_AnhChanDung(int CoQuanID);
        public TongHopTheoNgay ThongTinVaoRa_TongHopTheoNgay(int CoQuanID, int NguoidungID);
        public List<ThongTinVaoRaModel> ThongKe_KhachVaoRa(BasePagingParamsFilter p, ref int TotalRow, int? DonViSuDungID);
        public DoiTuongGap DanhSachDoiTuongGap(int CoQuanID);
    }
    public class VaoRaDAL : IVaoRaDAL
    {
        #region Get
        /// <summary>
        /// lấy danh sách phân trang có điều kiện lọc cho danh sách khác,
        /// danh sách khách độc lập cho từng cơ quan sử dụng phần mềm
        /// </summary>
        /// <param name="p"></param>
        /// <param name="Type"></param>
        /// <param name="TotalRow"></param>
        /// <param name="CoQuanID"></param>
        /// <param name="serverPath"></param>
        /// <returns></returns>
        public List<ThongTinVaoRaModel> GetListPageBySearch(BasePagingParams p, int? Type, ref int TotalRow, int CoQuanID, int NguoidungID, string serverPath)
        {
            var result = new List<ThongTinVaoRaModel>();
            //var danhSachCoQuan = new DanhMucCoQuanDonViDAL().DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap(CoQuanID);
            //if (UserRole.CheckAdmin(NguoidungID))
            //    danhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAll();
            //var pListCoQuan = new SqlParameter("ListCoQuan", SqlDbType.Structured);
            //pListCoQuan.TypeName = "dbo.id_list";
            //var tbCoQuanID = new DataTable();
            //tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            //danhSachCoQuan.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Type",SqlDbType.Int),
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("NgayCheckIn",SqlDbType.Int),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("DonViSuDungID",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int)
              };
            parameters[0].Value = Type ?? Convert.DBNull;
            parameters[1].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[2].Value = p.NgayCheckIn ?? Convert.DBNull;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = CoQuanID;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[6].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinVaoRa_GetListPageBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        ThongTinVaoRaModel ttVaoRa = new ThongTinVaoRaModel();
                        ttVaoRa = new ThongTinVaoRaModel();
                        // thông tin khách
                        ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                        //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                        ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                        ttVaoRa.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                        ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                        ttVaoRa.NgayCapCMND = Utils.ConvertToNullableDateTime(dr["NgayCapCMND"], null);
                        // thông tin vào ra
                        ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                        ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                        if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                        ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                        ttVaoRa.DonViCaNhan = Utils.ConvertToInt32(dr["DonViCaNhan"], 0);
                        ttVaoRa.TenCanBoGap = Utils.ConvertToString(dr["TenDonViCaNhan"], string.Empty);
                        ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                        ttVaoRa.LeTan_Ra = Utils.ConvertToInt32(dr["LeTan_Ra"], 0);
                        ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                        ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                        ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                        ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                        ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                        ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                        // thông tin file
                        ttVaoRa.AnhChanDungBase64 = Utils.ConvertToString(dr["AnhChanDung"], string.Empty);
                        ttVaoRa.AnhCMND_MTBase64 = Utils.ConvertToString(dr["AnhCMND_MT"], string.Empty);
                        ttVaoRa.AnhCMND_MSBase64 = Utils.ConvertToString(dr["AnhCMND_MS"], string.Empty);
                        if (ttVaoRa.AnhChanDungBase64 != null && ttVaoRa.AnhChanDungBase64 != string.Empty)
                            ttVaoRa.AnhChanDungBase64 = serverPath + ttVaoRa.AnhChanDungBase64;
                        if (ttVaoRa.AnhCMND_MSBase64 != null && ttVaoRa.AnhCMND_MSBase64 != string.Empty)
                            ttVaoRa.AnhCMND_MSBase64 = serverPath + ttVaoRa.AnhCMND_MSBase64;
                        if (ttVaoRa.AnhCMND_MTBase64 != null && ttVaoRa.AnhCMND_MTBase64 != string.Empty)
                            ttVaoRa.AnhCMND_MTBase64 = serverPath + ttVaoRa.AnhCMND_MTBase64;

                        result.Add(ttVaoRa);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[6].Value, 0);
                return result;
            }
            catch (Exception ex)
            {
                return result;
                throw ex;
            }
        }

        /// <summary>
        /// Lấy thông tin ra vào bằng id
        /// </summary>
        /// <param name="ThongTinVaoRaID"></param>
        /// <returns></returns>
        public ThongTinVaoRaModel Get_By_ThongTinVaoRaID(int ThongTinVaoRaID, string serverPath)
        {
            ThongTinVaoRaModel ttVaoRa = new ThongTinVaoRaModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"ThongTinVaoRaID",SqlDbType.Int)
              };
            parameters[0].Value = ThongTinVaoRaID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinVaoRa_GetBy_ID", parameters))
                {
                    while (dr.Read())
                    {
                        // thông tin khách
                        ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                        //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                        ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                        ttVaoRa.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                        ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                        ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                        ttVaoRa.NgayCapCMND = Utils.ConvertToNullableDateTime(dr["NgayCapCMND"], null);
                        // thông tin vào ra
                        ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                        ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                        if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                        ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                        ttVaoRa.DonViCaNhan = Utils.ConvertToInt32(dr["DonViCaNhan"], 0);
                        ttVaoRa.TenCanBoGap = Utils.ConvertToString(dr["TenDonViCaNhan"], string.Empty);
                        ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                        ttVaoRa.LeTan_Ra = Utils.ConvertToInt32(dr["LeTan_Ra"], 0);
                        ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                        ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                        ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                        ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                        ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                        ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                        // thông tin file
                        ttVaoRa.AnhChanDungBase64 = Utils.ConvertToString(dr["AnhChanDung"], string.Empty);
                        ttVaoRa.AnhCMND_MTBase64 = Utils.ConvertToString(dr["AnhCMND_MT"], string.Empty);
                        ttVaoRa.AnhCMND_MSBase64 = Utils.ConvertToString(dr["AnhCMND_MS"], string.Empty);
                        if (ttVaoRa.AnhChanDungBase64 != null && ttVaoRa.AnhChanDungBase64 != string.Empty)
                            ttVaoRa.AnhChanDungBase64 = serverPath + ttVaoRa.AnhChanDungBase64;
                        if (ttVaoRa.AnhCMND_MSBase64 != null && ttVaoRa.AnhCMND_MSBase64 != string.Empty)
                            ttVaoRa.AnhCMND_MSBase64 = serverPath + ttVaoRa.AnhCMND_MSBase64;
                        if (ttVaoRa.AnhCMND_MTBase64 != null && ttVaoRa.AnhCMND_MTBase64 != string.Empty)
                            ttVaoRa.AnhCMND_MTBase64 = serverPath + ttVaoRa.AnhCMND_MTBase64;
                        break;
                    }
                    dr.Close();
                }
                return ttVaoRa;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin checkin bằng mã thẻ
        /// </summary>
        /// <param name="MaThe"></param>
        /// <returns></returns>
        /// <summary>
        /// lấy thông tin vào ra theo mã thẻ
        /// </summary>
        /// <param name="MaThe"></param>
        /// <returns></returns>
        public List<ThongTinVaoRaModel> Get_By_MaThe(string MaThe, int LoaiCheckOut, int CoQuanID, string serverPath)
        {
            var datas = new List<ThongTinVaoRaModel>();
            var result = new List<ThongTinVaoRaModel>();
            if (string.IsNullOrEmpty(MaThe))
            {
                return new List<ThongTinVaoRaModel>();
            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"MaThe",SqlDbType.NVarChar),
                new SqlParameter(@"DonViSuDungID",SqlDbType.Int)
              };
            parameters[0].Value = MaThe;
            parameters[1].Value = CoQuanID;
            try
            {
                // mã thẻ
                if (LoaiCheckOut == EnumLoaiCheckOut.MaThe.GetHashCode())
                {
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinVaoRa_GetBy_MaThe", parameters))
                    {
                        while (dr.Read())
                        {
                            ThongTinVaoRaModel ttVaoRa = new ThongTinVaoRaModel();
                            ttVaoRa = new ThongTinVaoRaModel();
                            // thông tin khách
                            ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                            //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                            ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                            ttVaoRa.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                            ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                            ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                            ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                            ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                            ttVaoRa.NgayCapCMND = Utils.ConvertToNullableDateTime(dr["NgayCapCMND"], null);
                            // thông tin vào ra
                            ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                            ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                            if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                            ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                            ttVaoRa.DonViCaNhan = Utils.ConvertToInt32(dr["DonViCaNhan"], 0);
                            ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                            ttVaoRa.LeTan_Ra = Utils.ConvertToInt32(dr["LeTan_Ra"], 0);
                            ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                            ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                            ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                            ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                            ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                            ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                            // thông tin file
                            ttVaoRa.LoaiFile = Utils.ConvertToInt32(dr["LoaiFile"], 0);
                            ttVaoRa.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);

                            datas.Add(ttVaoRa);
                        }
                        dr.Close();
                    }
                }
                // CMND
                else if (LoaiCheckOut == EnumLoaiCheckOut.CMND.GetHashCode())
                {
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinVaoRa_GetBy_CMND", parameters))
                    {
                        while (dr.Read())
                        {
                            ThongTinVaoRaModel ttVaoRa = new ThongTinVaoRaModel();
                            ttVaoRa = new ThongTinVaoRaModel();
                            // thông tin khách
                            ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                            //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                            ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                            ttVaoRa.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                            ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                            ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                            ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                            ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                            ttVaoRa.NgayCapCMND = Utils.ConvertToNullableDateTime(dr["NgayCapCMND"], null);
                            // thông tin vào ra
                            ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                            ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                            if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                            ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                            ttVaoRa.DonViCaNhan = Utils.ConvertToInt32(dr["DonViCaNhan"], 0);
                            ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                            ttVaoRa.LeTan_Ra = Utils.ConvertToInt32(dr["LeTan_Ra"], 0);
                            ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                            ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                            ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                            ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                            ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                            ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                            // thông tin file
                            ttVaoRa.LoaiFile = Utils.ConvertToInt32(dr["LoaiFile"], 0);
                            ttVaoRa.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);

                            datas.Add(ttVaoRa);
                        }
                        dr.Close();
                    }
                }
                // Họ tên
                else if (LoaiCheckOut == EnumLoaiCheckOut.HoTen.GetHashCode()) // Họ tên
                {
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinVaoRa_GetBy_HoTen", parameters))
                    {
                        while (dr.Read())
                        {
                            ThongTinVaoRaModel ttVaoRa = new ThongTinVaoRaModel();
                            ttVaoRa = new ThongTinVaoRaModel();
                            // thông tin khách
                            ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                            //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                            ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                            ttVaoRa.NgaySinh = Utils.ConvertToNullableDateTime(dr["NgaySinh"], null);
                            ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                            ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                            ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                            ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                            ttVaoRa.NgayCapCMND = Utils.ConvertToNullableDateTime(dr["NgayCapCMND"], null);
                            // thông tin vào ra
                            ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                            ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                            if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                            ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                            ttVaoRa.DonViCaNhan = Utils.ConvertToInt32(dr["DonViCaNhan"], 0);
                            ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                            ttVaoRa.LeTan_Ra = Utils.ConvertToInt32(dr["LeTan_Ra"], 0);
                            ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                            ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                            ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                            ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                            ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                            ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                            // thông tin file
                            ttVaoRa.LoaiFile = Utils.ConvertToInt32(dr["LoaiFile"], 0);
                            ttVaoRa.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);

                            datas.Add(ttVaoRa);
                        }
                        dr.Close();
                    }
                }

                if (datas != null && datas.Count > 0)
                {
                    result = (from m in datas
                              group m by new
                              {
                                  m.ThongTinVaoRaID,
                                  m.HoVaTen,
                                  m.HoKhau,
                                  m.DienThoai,
                                  m.SoCMND,
                                  m.NoiCapCMND,
                                  m.NgayCapCMND,
                                  m.GioVao,
                                  m.GioRa,
                                  m.GapCanBo,
                                  m.DonViCaNhan,
                                  m.LeTan_Vao,
                                  m.LeTan_Ra,
                                  m.MaThe,
                                  m.TenCoQuan,
                                  m.ThanNhiet,
                                  m.LyDoGap
                              } into vr
                              select new ThongTinVaoRaModel()
                              {
                                  ThongTinVaoRaID = vr.Key.ThongTinVaoRaID,
                                  HoVaTen = vr.Key.HoVaTen,
                                  HoKhau = vr.Key.HoKhau,
                                  DienThoai = vr.Key.DienThoai,
                                  SoCMND = vr.Key.SoCMND,
                                  NoiCapCMND = vr.Key.NoiCapCMND,
                                  NgayCapCMND = vr.Key.NgayCapCMND,
                                  GioVao = vr.Key.GioVao,
                                  GioRa = vr.Key.GioRa == DateTime.MinValue ? null : vr.Key.GioRa,
                                  GapCanBo = vr.Key.GapCanBo,
                                  DonViCaNhan = vr.Key.DonViCaNhan,
                                  LeTan_Vao = vr.Key.LeTan_Vao,
                                  LeTan_Ra = vr.Key.LeTan_Ra == 0 ? null : vr.Key.LeTan_Ra,
                                  MaThe = vr.Key.MaThe,
                                  TenCoQuan = vr.Key.TenCoQuan,
                                  ThanNhiet = vr.Key.ThanNhiet,
                                  LyDoGap = vr.Key.LyDoGap,
                                  AnhChanDungBase64 = datas.FirstOrDefault(x => x.ThongTinVaoRaID == vr.Key.ThongTinVaoRaID && x.LoaiFile == EnumLoaiFile.AnhChanDung.GetHashCode()) != null ? serverPath + datas.FirstOrDefault(x => x.ThongTinVaoRaID == vr.Key.ThongTinVaoRaID && x.LoaiFile == EnumLoaiFile.AnhChanDung.GetHashCode()).TenFileHeThong : string.Empty,
                                  AnhCMND_MTBase64 = datas.FirstOrDefault(x => x.ThongTinVaoRaID == vr.Key.ThongTinVaoRaID && x.LoaiFile == EnumLoaiFile.MatTruocCMND.GetHashCode()) != null ? serverPath + datas.FirstOrDefault(x => x.ThongTinVaoRaID == vr.Key.ThongTinVaoRaID && x.LoaiFile == EnumLoaiFile.MatTruocCMND.GetHashCode()).TenFileHeThong : string.Empty,
                                  AnhCMND_MSBase64 = datas.FirstOrDefault(x => x.ThongTinVaoRaID == vr.Key.ThongTinVaoRaID && x.LoaiFile == EnumLoaiFile.MatSauCMND.GetHashCode()) != null ? serverPath + datas.FirstOrDefault(x => x.ThongTinVaoRaID == vr.Key.ThongTinVaoRaID && x.LoaiFile == EnumLoaiFile.MatSauCMND.GetHashCode()).TenFileHeThong : string.Empty,
                              }).ToList();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// lấy thông tin vào ra theo số cmnd
        /// </summary>
        /// <param name="SoCMND"></param>
        /// <returns></returns>
        public ThongTinVaoRaModel Get_By_CMND(string SoCMND, int CoQuanID)
        {
            ThongTinVaoRaModel ttVaoRa = new ThongTinVaoRaModel();

            if (string.IsNullOrEmpty(SoCMND))
            {
                return new ThongTinVaoRaModel();
            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"SoCMND",SqlDbType.NVarChar),
                new SqlParameter(@"DonViSuDungID",SqlDbType.Int),
              };
            parameters[0].Value = SoCMND;
            parameters[1].Value = CoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinVaoRa_GetBy_SoCMND", parameters))
                {
                    while (dr.Read())
                    {
                        ttVaoRa = new ThongTinVaoRaModel();
                        // thông tin khách
                        //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                        ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                        ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                        ttVaoRa.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.MinValue);
                        ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                        ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                        ttVaoRa.NgayCapCMND = Utils.ConvertToDateTime(dr["NgayCapCMND"], DateTime.MinValue);
                        // thông tin vào ra
                        ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                        ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                        if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                        ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                        ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                        ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                        ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                        ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                        ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                        ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                        ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                        break;
                    }
                    dr.Close();
                }
                return ttVaoRa;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// lấy thông tin khách chưa check out
        /// </summary>
        /// <returns></returns>
        public List<ThongTinVaoRaModel> Get_For_CheckOut_By_AnhChanDung(int CoQuanID)
        {
            List<ThongTinVaoRaModel> Result = new List<ThongTinVaoRaModel>();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]
                  {
                   new SqlParameter(@"DonViSuDungID",SqlDbType.Int)
                  };
                parameters[0].Value = CoQuanID;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinVaoRa_Get_For_CheckOut_By_AnhChanDung", parameters))
                {
                    while (dr.Read())
                    {
                        var ttVaoRa = new ThongTinVaoRaModel();
                        // thông tin khách
                        ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                        //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                        ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                        ttVaoRa.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.MinValue);
                        ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                        ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                        ttVaoRa.NgayCapCMND = Utils.ConvertToDateTime(dr["NgayCapCMND"], DateTime.MinValue);
                        // thông tin vào ra
                        ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                        ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                        if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                        ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                        ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["DonViCaNhan"], 0);
                        ttVaoRa.TenCanBoGap = Utils.ConvertToString(dr["TenDonViCaNhan"], string.Empty);
                        ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                        ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                        ttVaoRa.FileDinhKemID = Utils.ConvertToInt32(dr["FileDinhKemID"], 0);
                        ttVaoRa.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                        ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                        ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                        ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                        ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                        Result.Add(ttVaoRa);
                    }
                    dr.Close();
                }
                return Result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// tổng hợp khách vào ra theo ngày của từng cơ quan sử dụng phần mềm
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public TongHopTheoNgay ThongTinVaoRa_TongHopTheoNgay(int CoQuanID, int NguoidungID)
        {
            var result = new TongHopTheoNgay();
            //var danhSachCoQuan = new DanhMucCoQuanDonViDAL().DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap(CoQuanID);
            //if (UserRole.CheckAdmin(NguoidungID))
            //    danhSachCoQuan = new DanhMucCoQuanDonViDAL().GetAll();
            //var pListCoQuan = new SqlParameter("ListCoQuan", SqlDbType.Structured);
            //pListCoQuan.TypeName = "dbo.id_list";
            //var tbCoQuanID = new DataTable();
            //tbCoQuanID.Columns.Add("CoQuanID", typeof(string));
            //danhSachCoQuan.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            SqliteParameter[] parameters = new SqliteParameter[]
              {
                  new SqliteParameter(@"DonViSuDungID",SqlDbType.Int)
              };
            parameters[0].Value = CoQuanID;
            try
            {
                using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, QueryManager.Get("v1_NV_ThongTinVaoRa_TongHopTheoNgay"), parameters))
                {
                    while (dr.Read())
                    {
                        result.Tong = Utils.ConvertToInt32(dr["Tong"], 0);
                        result.DangGap = Utils.ConvertToInt32(dr["DangGap"], 0);
                        result.DaVe = Utils.ConvertToInt32(dr["DaVe"], 0);
                    }
                    dr.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
                throw ex;
            }
        }

        #endregion

        #region check in

        /// <summary>
        /// thêm thông tin vào ra - checkin
        /// </summary>
        /// <param name="TTKhachModel"></param>
        /// <param name="MaThe"></param>
        /// <returns></returns>
        public BaseResultModel Insert(ThongTinVaoRaModel TTKhachModel, ref string MaThe)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                  {
                            new SqlParameter("GioVao", SqlDbType.DateTime2),
                            new SqlParameter("GapCanBo", SqlDbType.Int),
                            new SqlParameter("LeTan_Vao", SqlDbType.Int),
                            new SqlParameter("MaThe", SqlDbType.NVarChar),
                            new SqlParameter("MaTheTuNhap", SqlDbType.NVarChar),

                            new SqlParameter("@HoVaTen", SqlDbType.NVarChar),
                            new SqlParameter("@NgaySinh", SqlDbType.DateTime),
                            new SqlParameter("@HoKhau", SqlDbType.NVarChar),
                            new SqlParameter("@DienThoai", SqlDbType.VarChar),
                            new SqlParameter("@SoCMND", SqlDbType.VarChar),
                            new SqlParameter("@NoiCapCMND", SqlDbType.NVarChar),
                            new SqlParameter("@NgayCapCMND", SqlDbType.DateTime),
                            new SqlParameter("@TenCoQuan", SqlDbType.NVarChar),
                            new SqlParameter("@DonViSuDungID", SqlDbType.Int),
                            new SqlParameter("@ThanNhiet", SqlDbType.NVarChar),
                            new SqlParameter("@DonViCaNhan", SqlDbType.Int),
                            new SqlParameter("@GioiTinh", SqlDbType.NVarChar),
                            new SqlParameter("@LoaiGiayTo", SqlDbType.NVarChar),
                            new SqlParameter("@LyDoGap", SqlDbType.Int)
                  };
                parameters[0].Value = DateTime.Now;
                parameters[1].Value = TTKhachModel.GapCanBo;
                parameters[2].Value = TTKhachModel.LeTan_Vao;
                parameters[3].Direction = ParameterDirection.Output;
                parameters[3].Size = 20;
                parameters[4].Value = (TTKhachModel.MaThe == null || TTKhachModel.MaThe.Length < 1 || string.IsNullOrEmpty(TTKhachModel.MaThe)) ? Convert.DBNull : TTKhachModel.MaThe;

                parameters[5].Value = TTKhachModel.HoVaTen ?? Convert.DBNull;
                parameters[6].Value = TTKhachModel.NgaySinh != null ? Utils.ConvertToDateTime(TTKhachModel.NgaySinh.ToString(), DateTime.MinValue) : Convert.DBNull;
                parameters[7].Value = TTKhachModel.HoKhau ?? Convert.DBNull; ;
                parameters[8].Value = TTKhachModel.DienThoai ?? Convert.DBNull;
                parameters[9].Value = TTKhachModel.SoCMND ?? Convert.DBNull; ;
                parameters[10].Value = TTKhachModel.NoiCapCMND ?? Convert.DBNull;
                parameters[11].Value = TTKhachModel.NgayCapCMND != null ? Utils.ConvertToDateTime(TTKhachModel.NgayCapCMND.ToString(), DateTime.MinValue) : Convert.DBNull;
                parameters[12].Value = TTKhachModel.TenCoQuan ?? Convert.DBNull;
                parameters[13].Value = TTKhachModel.DonViSuDungID ?? Convert.DBNull;
                parameters[14].Value = TTKhachModel.ThanNhiet ?? Convert.DBNull;
                parameters[15].Value = TTKhachModel.DonViCaNhan;
                parameters[16].Value = TTKhachModel.GioiTinh ?? Convert.DBNull;
                parameters[17].Value = TTKhachModel.LoaiGiayTo ?? Convert.DBNull;
                parameters[18].Value = TTKhachModel.LyDoGap;


                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, "v1_NV_ThongTinVaoRa_Insert", parameters), 0);
                            trans.Commit();
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("Check In");
                        }
                        catch (Exception ex)
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw ex;
                        }

                    }
                    MaThe = Utils.ConvertToString(parameters[4].Value, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        /// <summary>
        /// CheckIn
        /// </summary>
        /// <param name="VaoRaModel"></param>
        /// <param name="MaThe"></param>
        /// <returns></returns>
        public BaseResultModel Vao(ThongTinVaoRaModel VaoRaModel, ref string MaThe, ref string SoCMND, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (VaoRaModel == null || VaoRaModel.SoCMND == null)
                {
                    Result.Status = 0;
                    Result.Message = "Số CMND không được trống";
                    return Result;
                }
                else if (VaoRaModel.HoVaTen == null || VaoRaModel.HoVaTen.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Họ và tên không được trống";
                    return Result;
                }
                else if (VaoRaModel.LyDoGap == null || VaoRaModel.LyDoGap < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Lý do gặp không được trống";
                    return Result;
                }
                //else if (VaoRaModel.LeTan_Vao == null || VaoRaModel.LeTan_Vao < 1)
                //{
                //    Result.Status = 0;
                //    Result.Message = "Lễ tân không được trống";
                //    return Result;
                //}
                //else if (VaoRaModel.DonViCaNhan == null || VaoRaModel.DonViCaNhan < 1)
                //{
                //    Result.Status = 0;
                //    Result.Message = "Loại đối tượng gặp không được trống";
                //    return Result;
                //}
                else
                {
                    //
                    if (VaoRaModel.LyDoGap == EnumLyDoGap.GapCanBo.GetHashCode())
                    {
                        if (VaoRaModel.GapCanBo == null || VaoRaModel.GapCanBo < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "Cán bộ gặp không được trống";
                            return Result;
                        }
                        else if (VaoRaModel.DonViCaNhan == null || VaoRaModel.DonViCaNhan < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "Loại đối tượng gặp không được trống";
                            return Result;
                        }
                    }
                    else
                    {
                        VaoRaModel.GapCanBo = CoQuanID;
                        VaoRaModel.DonViCaNhan = 1;
                    }
                    //
                    // check khách đã check in chưa
                    // nếu check in rồi mà chưa checkout thì không cho check in nữa
                    // bước 1. chech bằng ảnh chân dung - check trên controller
                    // bước 2. check bằng số CMND
                    var ttKhacDaCheckInBangCMND = Get_By_CMND(VaoRaModel.SoCMND, CoQuanID);
                    if (ttKhacDaCheckInBangCMND != null && ttKhacDaCheckInBangCMND.ThongTinVaoRaID > 0 && (ttKhacDaCheckInBangCMND.GioRa == DateTime.MinValue || ttKhacDaCheckInBangCMND.GioRa == null))
                    {
                        Result.Status = 0;
                        Result.Message = "Khách chưa checkout. Vui lòng kiểm tra lại.";
                        return Result;
                    }
                    if (VaoRaModel.MaThe != null && string.IsNullOrEmpty(VaoRaModel.MaThe))
                    {
                        var ttKhacDaCheckIn = Get_By_MaThe(VaoRaModel.MaThe, EnumLoaiCheckOut.MaThe.GetHashCode(), CoQuanID, "serverPath");
                        if (ttKhacDaCheckIn != null && ttKhacDaCheckIn.Count > 0)
                        {
                            Result.Status = 0;
                            Result.Message = "Mã thẻ đã được sử dụng. Vui lòng kiểm tra lại.";
                            return Result;
                        }
                    }


                    Result = Insert(VaoRaModel, ref MaThe);
                    SoCMND = VaoRaModel.SoCMND;
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        #endregion


        #region check out

        /// <summary>
        /// check out
        /// </summary>
        /// <param name="TTKhachModel"></param>
        /// <returns></returns>
        public BaseResultModel Update_GioRa(ThongTinVaoRaModel TTKhachModel)
        {
            var Result = new BaseResultModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                  {
                     new SqlParameter("ThongTinVaoRaID", SqlDbType.Int),
                     new SqlParameter("LeTan_Ra", SqlDbType.Int),
                      new SqlParameter("GioRa", SqlDbType.DateTime2),
                  };
                parameters[0].Value = TTKhachModel.ThongTinVaoRaID;
                parameters[1].Value = TTKhachModel.LeTan_Ra;
                parameters[2].Value = DateTime.Now;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_NV_ThongTinVaoRa_Update_GioRa", parameters);
                            trans.Commit();
                            Result.Message = ConstantLogMessage.Alert_Update_Success("Check Out");
                        }
                        catch (Exception ex)
                        {
                            Result.Status = -1;
                            Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        /// <summary>
        /// check out
        /// </summary>
        /// <param name="TTKhachModel"></param>
        /// <returns></returns>
        public BaseResultModel Ra(ThongTinVaoRaModel TTKhachModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (TTKhachModel == null || TTKhachModel.ThongTinVaoRaID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập mã thẻ hợp lệ";
                    return Result;
                }
                else
                {
                    var TTVaoRa = Get_By_ThongTinVaoRaID(TTKhachModel.ThongTinVaoRaID, "ServerPath");
                    if (TTVaoRa == null || TTVaoRa.ThongTinVaoRaID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Không tìm thấy thông tin khách cần checkout. Vui lòng kiểm tra lại.";
                    }
                    else
                    {
                        Result = Update_GioRa(TTKhachModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        #endregion


        #region báo cáo

        public List<ThongTinVaoRaModel> ThongKe_KhachVaoRa(BasePagingParamsFilter p, ref int TotalRow, int? DonViSuDungID)
        {
            List<ThongTinVaoRaModel> Result = new List<ThongTinVaoRaModel>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@TuNgay",SqlDbType.NVarChar),
                new SqlParameter("@DenNgay",SqlDbType.NVarChar),
                new SqlParameter("@CanBoGapID",SqlDbType.Int),
                new SqlParameter("@LeTanID",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),
                new SqlParameter("@CoQuanID",SqlDbType.Int),
                new SqlParameter("@DonViCaNhan",SqlDbType.Int)
                };
                //parameters[0].Value = p.StartDate == DateTime.MinValue ? Convert.DBNull : p.StartDate;
                //parameters[1].Value = p.EndDate == DateTime.MinValue ? Convert.DBNull : p.EndDate;
                parameters[0].Value = p.TuNgay ?? Convert.DBNull;
                parameters[1].Value = p.DenNgay ?? Convert.DBNull;

                parameters[2].Value = p.CanBoGapID ?? Convert.DBNull;
                parameters[3].Value = p.LeTanID ?? Convert.DBNull;
                parameters[4].Direction = ParameterDirection.Output;
                parameters[4].Size = 20;
                parameters[5].Value = DonViSuDungID ?? Convert.DBNull;
                parameters[6].Value = p.DonViCaNhan ?? Convert.DBNull;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongKe_KhachVaoRa", parameters))
                {
                    while (dr.Read())
                    {
                        var ttVaoRa = new ThongTinVaoRaModel();
                        // thông tin khách
                        ttVaoRa.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                        //ttVaoRa.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                        ttVaoRa.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                        ttVaoRa.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.MinValue);
                        ttVaoRa.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        ttVaoRa.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        ttVaoRa.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                        ttVaoRa.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                        ttVaoRa.NgayCapCMND = Utils.ConvertToDateTime(dr["NgayCapCMND"], DateTime.MinValue);
                        // thông tin vào ra
                        ttVaoRa.GioVao = Utils.ConvertToDateTime_Hour(dr["GioVao"], DateTime.MinValue);
                        ttVaoRa.GioRa = Utils.ConvertToDateTime_Hour(dr["GioRa"], DateTime.MinValue);
                        if (ttVaoRa.GioRa == DateTime.MinValue) ttVaoRa.GioRa = null;
                        ttVaoRa.GapCanBo = Utils.ConvertToInt32(dr["GapCanBo"], 0);
                        ttVaoRa.LeTan_Vao = Utils.ConvertToInt32(dr["LeTan_Vao"], 0);
                        ttVaoRa.MaThe = Utils.ConvertToString(dr["MaThe"], string.Empty);
                        //ttVaoRa.FileDinhKemID = Utils.ConvertToInt32(dr["FileDinhKemID"], 0);
                        //ttVaoRa.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);

                        ttVaoRa.TenCanBoGap = Utils.ConvertToString(dr["TenCanBoGap"], string.Empty);
                        ttVaoRa.GapBoPhanID = Utils.ConvertToInt32(dr["GapBoPhanID"], 0);
                        ttVaoRa.TenBoPhanGap = Utils.ConvertToString(dr["TenBoPhanGap"], string.Empty);
                        ttVaoRa.TenCoQuan = Utils.ConvertToString(dr["NoiCongTac"], string.Empty);
                        ttVaoRa.ThanNhiet = Utils.ConvertToString(dr["ThanNhiet"], string.Empty);
                        ttVaoRa.GioiTinh = Utils.ConvertToString(dr["GioiTinh"], string.Empty);
                        ttVaoRa.LoaiGiayTo = Utils.ConvertToString(dr["LoaiGiayTo"], string.Empty);
                        ttVaoRa.LyDoGap = Utils.ConvertToInt32(dr["LyDoGap"], 0);
                        Result.Add(ttVaoRa);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[4].Value, 0);
                return Result;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        /// <summary>
        /// lấy danh sách đối tượng gặp trong cơ quan sử dụng phần mềm theo hình cây, 
        /// ghép cơ quan và cán bộ
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public DoiTuongGap DanhSachDoiTuongGap(int CoQuanID)
        {
            var Result = new DoiTuongGap();
            try
            {
                var danhSachDoiTuong = new List<DoiTuongGap>();

                var danhSachCoQuanTrongPhamVi = new DanhMucCoQuanDonViDAL().DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap(CoQuanID);
                var danhSachCanBoTrongPhamVi = new HeThongCanBoDAL().DanhSachCanBo_TrongCoQuanSuDungPhanMem(CoQuanID);

                danhSachCoQuanTrongPhamVi.ForEach(x =>
                    danhSachDoiTuong.Add(
                        new DoiTuongGap()
                        {
                            ID = x.CoQuanID,
                            ParentId = x.CoQuanChaID,
                            Name = x.TenCoQuan,
                            Type = 1, // cơ quan
                            Active = x.SuDungPM
                        }));
                danhSachCanBoTrongPhamVi.ForEach(x =>
                  danhSachDoiTuong.Add(
                      new DoiTuongGap()
                      {
                          ID = x.CanBoID,
                          ParentId = x.CoQuanID,
                          Name = x.TenCanBo,
                          Type = 2 // cán bộ
                      }));
                danhSachDoiTuong.OrderBy(x => x.ParentId);
                danhSachDoiTuong.ForEach(x => x.Children = danhSachDoiTuong.Where(i => i.ParentId == x.ID).ToList());
                danhSachDoiTuong.RemoveAll(x => x.ParentId != null && x.Active == false);
                Result = danhSachDoiTuong.FirstOrDefault();
                return Result;
            }
            catch (Exception)
            {
                return Result;
                throw;
            }
        }
    }
}
