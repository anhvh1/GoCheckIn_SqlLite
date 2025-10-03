using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Com.Gosol.INOUT.DAL.DanhMuc
{
    public interface IDanhMucTrangThaiDAL
    {
        public List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel);
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel, int CoQuanID);
        public BaseResultModel Delete(List<int> ListTrangThaiID);
        public DanhMucTrangThaiModel GetByID(int? TrangThaiID);
        public DanhMucTrangThaiModel GetByIDAndCoQuanID(int? TrangThaiID, int? CoQuanID);
        public List<CoQuanTrangThaiModel> GetByCoQuanTrangThai(int? TrangThaiID, int? CoQuanID);
    }
    public class DanhMucTrangThaiDAL : IDanhMucTrangThaiDAL
    {
        private const string INSERT = @"v1_DanhMuc_TrangThai_Insert";
        private const string UPDATE = @"v1_DanhMuc_TrangThai_Update";
        private const string DELETE = @"v1_DanhMuc_TrangThai_Delete";
        private const string GETBYID = @"v1_DanhMuc_TrangThai_GetByID";
        private const string GETBYNAME = @"v1_DanhMuc_TrangThai_GetByName";
        private const string GETLISTPAGING = @"v1_DanhMuc_TrangThai_GetPagingBySearch";
        private const string GETAll = @"v1_DanhMuc_TrangThai_GetAll";
        private const string GETBYCOQUANTRANGTHAI = @"v1_DanhMuc_TrangThai_GetByTrangThaiCoQuan";
        private const string UPDATECOQUANTRANGTHAI = @"v1_DanhMuc_CoQuan_TrangThai_Update";
        // param constant
        private const string PARAM_TrangThaiID = @"TrangThaiID";
        private const string PARAM_TenTrangThai = @"TenTrangThai";
        private const string PARAM_TrangThaiSuDung = @"TrangThaiSuDung";
        private const string PARAM_Role = @"Role";
        // param trạng thái công việc cơ quan
        private const string PARAM_TrangThaiCongViecCoQuanID = @"TrangThaiCongViecCoQuanID";
        private const string PARAM_CoQuanID = @"CoQuanID";
        // Insert
        public BaseResultModel Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucTrangThaiModel == null || DanhMucTrangThaiModel.TenTrangThai == null || DanhMucTrangThaiModel.TenTrangThai.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên trạng thái không được trống";
                }
                else if (DanhMucTrangThaiModel.TenTrangThai.Trim().Length > 50)
                {
                    Result.Status = 0;
                    Result.Message = "Tên trạng thái không được quá 50 ký tự";
                }
                else
                {
                    var TrangThai = GetByName(DanhMucTrangThaiModel.TenTrangThai);
                    if (TrangThai != null && TrangThai.TrangThaiID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Trạng thái đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                            new SqlParameter(PARAM_TenTrangThai, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_TrangThaiSuDung, SqlDbType.NVarChar)
                          };
                        parameters[0].Value = DanhMucTrangThaiModel.TenTrangThai.Trim();
                        parameters[1].Value = DanhMucTrangThaiModel.TrangThaiSuDung == null ? false : DanhMucTrangThaiModel.TrangThaiSuDung;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERT, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục trạng thái");
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

        // Update
        /// <summary>
        /// Dành cho người dùng update role sử dụng
        /// </summary>
        /// <param name="DanhMucTrangThaiModel"></param>
        /// <returns></returns>
        public BaseResultModel Update(DanhMucTrangThaiModel DanhMucTrangThaiModel, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                int crID;
                if (!int.TryParse(DanhMucTrangThaiModel.TrangThaiID.ToString(), out crID) || DanhMucTrangThaiModel.TrangThaiID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Trạng thái không tồn tại, hoặc TrangThaiID không đúng định dạng";
                }
                else
                {
                    //var crObj = GetByCoQuanTrangThai(DanhMucTrangThaiModel.TrangThaiID, CoQuanID);
                    //var objDouble = GetByName(DanhMucTrangThaiModel.TenTrangThai.Trim());
                    //if (crObj == null || crObj.TrangThaiCongViecCoQuanID < 1)
                    //{
                    //    Result.Status = 0;
                    //    Result.Message = "Danh mục trạng thái không tồn tại";
                    //}

                    //else
                    //{
                    var pList = new SqlParameter("@ListRoleID", SqlDbType.Structured);
                    pList.TypeName = "dbo.list_ID";
                    // var TrangThai = 400;
                    var tbRoleID = new DataTable();
                    tbRoleID.Columns.Add("ID", typeof(string));
                    SqlParameter[] parameters = new SqlParameter[]
                     {
                            new SqlParameter(PARAM_TrangThaiID, SqlDbType.Int),
                            new SqlParameter(PARAM_CoQuanID, SqlDbType.Int),
                           // new SqlParameter(PARAM_TenTrangThai, SqlDbType.NVarChar),
                            pList // List Role sử dụng
                     };
                    DanhMucTrangThaiModel.ListRoleID.ForEach(x => tbRoleID.Rows.Add(x));

                    parameters[0].Value = DanhMucTrangThaiModel.TrangThaiID;
                    parameters[1].Value = CoQuanID;
                    parameters[2].Value = tbRoleID;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                // Cập nhật Role sử dụng
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATECOQUANTRANGTHAI, parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Update_Success("danh mục trạng thái");
                            }
                            catch
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw;
                            }
                        }
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }
        public BaseResultModel Delete(List<int> ListTrangThaiID)
        {
            BaseResultModel Result = new BaseResultModel();
            if (ListTrangThaiID.Count <= 0)
            {

                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                Result.Status = 0;
                return Result;
            }
            else
            {
                for (int i = 0; i < ListTrangThaiID.Count; i++)
                {

                    int crID;
                    if (!int.TryParse(ListTrangThaiID[i].ToString(), out crID))
                    {
                        Result.Message = "Trạng thái '" + ListTrangThaiID[i].ToString() + "' không đúng định dạng";
                        Result.Status = 0;
                        return Result;
                    }
                    else
                    {
                        var crObj = GetByID(ListTrangThaiID[i]);
                        if (crObj == null || crObj.TrangThaiID < 1)
                        {
                            Result.Message = "TrangThaiID " + GetByID(ListTrangThaiID[i]).TenTrangThai + " không tồn tại";
                            Result.Status = 0;
                            return Result;
                        }
                        else if (new HeThongCanBoDAL().GetCanBoByTrangThaiID(ListTrangThaiID[i]).Count > 0)
                        {
                            Result.Message = "Trạng thái " + GetByID(ListTrangThaiID[i]).TenTrangThai + " đang được sử dụng ! Không thể xóa";
                            Result.Status = 0;
                            return Result;
                        }
                        else
                        {
                            SqlParameter[] parameters = new SqlParameter[]
                             {
                                new SqlParameter(PARAM_TrangThaiID, SqlDbType.Int)
                             };
                            parameters[0].Value = ListTrangThaiID[i];
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        int val = 0;
                                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE, parameters);
                                        trans.Commit();


                                    }
                                    catch
                                    {
                                        Result.Status = -1;
                                        Result.Message = ConstantLogMessage.API_Error_System;
                                        trans.Rollback();
                                        return Result;
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Danh mục trạng thái");
                return Result;
            }
        }

        public DanhMucTrangThaiModel GetByID(int? TrangThaiID)
        {
            DanhMucTrangThaiModel TrangThai = null;
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_TrangThaiID,SqlDbType.Int)
              };
            parameters[0].Value = TrangThaiID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETBYID, parameters))
                {
                    while (dr.Read())
                    {
                        TrangThai = new DanhMucTrangThaiModel(Utils.ConvertToInt32(dr["TrangThaiCongViecID"], 0), Utils.ConvertToString(dr["TenTrangThaiCongViec"], string.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return TrangThai;
        }

        public DanhMucTrangThaiModel GetByIDAndCoQuanID(int? TrangThaiID, int? CoQuanID)
        {
            List<DanhMucTrangThaiModel> DanhSachTrangThai = new List<DanhMucTrangThaiModel>();
            DanhMucTrangThaiModel TrangThai = new DanhMucTrangThaiModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_TrangThaiID,SqlDbType.Int),
                new SqlParameter("CoQuanID",SqlDbType.Int)
              };
            parameters[0].Value = TrangThaiID ?? Convert.DBNull;
            parameters[1].Value = CoQuanID ?? Convert.DBNull;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_DanhMuc_TrangThai_GetByID_AndCoQuanID", parameters))
                {
                    while (dr.Read())
                    {
                        var tt = new DanhMucTrangThaiModel();
                        tt.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiCongViecID"], 0);
                        tt.TenTrangThai = Utils.ConvertToString(dr["TenTrangThaiCongViec"], string.Empty);
                        tt.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
                        DanhSachTrangThai.Add(tt);
                    }
                    dr.Close();
                }
                TrangThai = (from m in DanhSachTrangThai
                             group m by new { m.TrangThaiID, m.TenTrangThai } into trangthai
                             select new DanhMucTrangThaiModel()
                             {
                                 TrangThaiID = trangthai.Key.TrangThaiID,
                                 TenTrangThai = trangthai.Key.TenTrangThai,
                                 ListRoleID = DanhSachTrangThai.Where(x => x.VaiTro > 0).Select(x => x.VaiTro.Value).ToList()
                             }
                           ).FirstOrDefault();
            }
            catch
            {
                throw;
            }
            return TrangThai;
        }

        public DanhMucTrangThaiModel GetByName(string TenTrangThai)
        {
            DanhMucTrangThaiModel TrangThai = new DanhMucTrangThaiModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenTrangThai",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenTrangThai.Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETBYNAME, parameters))
                {
                    while (dr.Read())
                    {
                        TrangThai = new DanhMucTrangThaiModel(Utils.ConvertToInt32(dr["TrangThaiCongViecID"], 0), Utils.ConvertToString(dr["TenTrangThaiCongViec"], string.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));
                        var id = TrangThai.TrangThaiID;

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return TrangThai;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="TotalRow"></param>
        /// <returns></returns>

        public List<DanhMucTrangThaiModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            List<DanhMucTrangThaiModel> list = new List<DanhMucTrangThaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("Keyword",SqlDbType.NVarChar),
                new SqlParameter("OrderByName",SqlDbType.NVarChar),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),

              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            try
            {
                var ds_tt = new SystemConfigDAL().GetByKey("TRANG_THAI_CONG_VIEC").ToString().Split(",");

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, GETLISTPAGING, parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucTrangThaiModel item = new DanhMucTrangThaiModel();
                        item.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiCongViecID"], 0);
                        item.TenTrangThai = Utils.ConvertToString(dr["TenTrangThaiCongViec"], "");
                        item.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true);
                        if (ds_tt.Any(x => x == item.TrangThaiID.ToString()))
                        {
                            item.ChoPhepSua = false;
                        }
                        list.Add(item);
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public List<DanhMucTrangThaiModel> GetAll()
        {
            List<DanhMucTrangThaiModel> list = new List<DanhMucTrangThaiModel>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, GETAll))
                {
                    while (dr.Read())
                    {
                        DanhMucTrangThaiModel item = new DanhMucTrangThaiModel();
                        item.TrangThaiID = Utils.ConvertToInt32(dr["TrangThaiCongViecID"], 0);
                        item.TenTrangThai = Utils.ConvertToString(dr["TenTrangThaiCongViec"], "");
                        item.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true);
                        list.Add(item);
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
        /// Lấy thông tin cơ quan trạng thái
        /// </summary>
        /// <param name="TrangThaiID"></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<CoQuanTrangThaiModel> GetByCoQuanTrangThai(int? TrangThaiID, int? CoQuanID)
        {
            List<CoQuanTrangThaiModel> TrangThaiCoQuan = new List<CoQuanTrangThaiModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_TrangThaiID,SqlDbType.Int),
                new SqlParameter(PARAM_CoQuanID,SqlDbType.Int),
              };
            parameters[0].Value = TrangThaiID ?? Convert.DBNull;
            parameters[1].Value = CoQuanID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETBYCOQUANTRANGTHAI, parameters))
                {
                    while (dr.Read())
                    {
                        var item = new CoQuanTrangThaiModel(Utils.ConvertToInt32(dr["TrangThaiCongViecCoQuanID"], 0), Utils.ConvertToInt32(dr["TrangThaiCongViecID"], 0), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToInt32(dr["VaiTro"], 0));
                        TrangThaiCoQuan.Add(item);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return TrangThaiCoQuan;
        }
       
    }
}
