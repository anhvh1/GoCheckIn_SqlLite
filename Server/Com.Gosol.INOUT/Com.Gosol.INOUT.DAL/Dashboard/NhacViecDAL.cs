
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models.Dashboard;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Com.Gosol.INOUT.DAL.Dashboard
{
    public interface INhacViecDAL
    {
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID, Models.BasePagingParams p, ref int TotalRow);
        public int InsertNotify(int? CanBoDangNhap, int? LoaiThongBao, List<int> ListCanBoNhanThongBao, string ThongBaoChoChinhBan, string ThongBaoChoNguoiKhac, int? ChinhBanHayNguoiKhac);
        public int UpdateNotify(int? ThongBaoID, int? LoaiUpdate, int? CanBoNhanThongBao);
        public void UpdateLog(int? LogID);
    }
    public class NhacViecDAL : INhacViecDAL
    {
        // Lấy danh sách thông báo việc cần làm
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID, Models.BasePagingParams p, ref int TotalRow)
        {
            // Trang thái xem  : 11- Thông báo của chính bạn , 10: Thông báo của người khác , 00 : Đã xem
            List<NhacViecModel> ListNhacViec = new List<NhacViecModel>();
            //string message = "";
            var PageSize = new SystemConfigDAL().GetByKey("Page_Size_Notify").ConfigValue;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CanBoID",SqlDbType.Int),
                new SqlParameter("@Start",SqlDbType.Int),
                new SqlParameter("@End",SqlDbType.Int),    new SqlParameter("@TotalRow",SqlDbType.Int)
            };
            parameters[0].Value = CanBoID;
            parameters[1].Value = p.Offset;
            parameters[2].Value = PageSize;
            parameters[3].Direction = ParameterDirection.Output;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_GetDSNhacViec", parameters))
                {
                    while (dr.Read())
                    {
                        NhacViecModel NhacViecModel = new NhacViecModel();
                        NhacViecModel.CanBoNhanThongBao = Utils.ConvertToInt32(dr["CanBoNhanThongBao"], 0);
                        NhacViecModel.TrangThaiXem = Utils.ConvertToInt32(dr["TrangThaiXem"], 0);
                        NhacViecModel.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], string.Empty);
                        NhacViecModel.Name = NhacViecModel.TrangThaiXem == 11 ? Utils.ConvertToString(dr["ThongBaoChoChinhBan"], string.Empty)
                            : Utils.ConvertToString(dr["ThongBaoChoNguoiKhac"], string.Empty);
                        NhacViecModel.Key = Utils.ConvertToString(dr["MaChucNang"], string.Empty);
                        ListNhacViec.Add(NhacViecModel);
                    }
                    dr.Close();
                    TotalRow = Utils.ConvertToInt32(parameters[3].Value, 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ListNhacViec;
        }
        public int UpdateNotify(int? ThongBaoID, int? LoaiUpdate, int? CanBoNhanThongBao)
        {
            int val = 0;
            SqlParameter[] parameters = new SqlParameter[]
                      {
                                new SqlParameter("@ThongBaoID", SqlDbType.Int),
                                new SqlParameter("@LoaiUpdate", SqlDbType.NVarChar),
                                new SqlParameter("@CanBoNhanThongBao", SqlDbType.NVarChar)
                      };

            parameters[0].Value = ThongBaoID ?? Convert.DBNull;
            parameters[1].Value = LoaiUpdate ?? Convert.DBNull;
            parameters[2].Value = CanBoNhanThongBao ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_Notify_Update", parameters);
                        trans.Commit();
                        //Result.Message = ConstantLogMessage.Alert_Update_Success("danh mục chức vụ");
                        return val;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// Hàm Không dùng nữa  By HoangNH
        /// </summary>
        /// <param name="CanBoDangNhap"></param>
        /// <param name="LoaiThongBao"></param>
        /// <param name="ListCanBoNhanThongBao"></param>
        /// <param name="ThongBaoChoChinhBan"></param>
        /// <param name="ThongBaoChoNguoiKhac"></param>
        /// <param name="ChinhBanHayNguoiKhac"></param>
        /// <returns></returns>
        public int InsertNotify(int? CanBoDangNhap, int? LoaiThongBao, List<int> ListCanBoNhanThongBao, string ThongBaoChoChinhBan, string ThongBaoChoNguoiKhac, int? ChinhBanHayNguoiKhac)
        {
            int val = 0;
            var list = new SqlParameter("@ListCanBoID", SqlDbType.Structured);
            list.TypeName = "dbo.list_ID";
            var tbCanBoID = new DataTable();
            tbCanBoID.Columns.Add("ID", typeof(string));
            ListCanBoNhanThongBao.ForEach(x => tbCanBoID.Rows.Add(x));
            string MaChucNang = "";
            if (LoaiThongBao == 1) { MaChucNang = "nhiem-vu"; } else { MaChucNang = "nhiem-vu/chi-tiet"; }
            try
            {

                SqlParameter[] parameters = new SqlParameter[]
                   {
                            new SqlParameter("@ThongBaoChoChinhBan", SqlDbType.NVarChar),
                          list,
                            new SqlParameter("@TrangThaiXem", SqlDbType.Int),
                             new SqlParameter("@CanBoTaoThongBao", SqlDbType.Int),
                              new SqlParameter("@MaChucNang", SqlDbType.NVarChar),
                               new SqlParameter("@ThongBaoChoNguoiKhac", SqlDbType.NVarChar),
                               new SqlParameter("@ChinhBanHayNguoiKhac",SqlDbType.Int)

                   };
                parameters[0].Value = ThongBaoChoChinhBan ?? Convert.DBNull;
                parameters[1].Value = tbCanBoID;
                parameters[2].Value = 10;
                parameters[3].Value = CanBoDangNhap ?? Convert.DBNull;
                parameters[4].Value = MaChucNang ?? Convert.DBNull;
                parameters[5].Value = ThongBaoChoNguoiKhac ?? Convert.DBNull;
                parameters[6].Value = ChinhBanHayNguoiKhac ?? Convert.DBNull;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_Notify_Insert", parameters);
                            trans.Commit();
                            //Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục chức vụ");
                        }
                        catch (Exception ex)
                        {
                            //Result.Status = -1;
                            //Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Result.Status = -1;
                //Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return val;
        }

        public void UpdateLog(int? LogID)
        {
            int val = 0;
            //var list = new SqlParameter("@ListCanBoID", SqlDbType.Structured);
            //list.TypeName = "dbo.list_ID";
            //var tbCanBoID = new DataTable();
            //tbCanBoID.Columns.Add("ID", typeof(string));
            //ListCanBoNhanThongBao.ForEach(x => tbCanBoID.Rows.Add(x));
            //string MaChucNang = "";
            //if (LoaiThongBao == 1) { MaChucNang = "nhiem-vu"; } else { MaChucNang = "nhiem-vu/chi-tiet"; }
            try
            {

                SqlParameter[] parameters = new SqlParameter[]
                   {
                            new SqlParameter("@LogID", SqlDbType.NVarChar)
                   };
                parameters[0].Value = LogID ?? Convert.DBNull;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_HT_UpdateLog", parameters);
                            trans.Commit();
                            //Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục chức vụ");
                        }
                        catch (Exception ex)
                        {
                            //Result.Status = -1;
                            //Result.Message = ConstantLogMessage.API_Error_System;
                            trans.Rollback();
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Result.Status = -1;
                //Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            //return val;
        }
    }
}

