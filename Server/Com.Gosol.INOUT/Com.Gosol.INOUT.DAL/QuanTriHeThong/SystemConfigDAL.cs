using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface ISystemConfigDAL
    {
        public List<SystemConfigModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(SystemConfigModel SystemConfigModel);
        public BaseResultModel Update(SystemConfigModel SystemConfigModel);
        public BaseResultModel Delete(List<int> ListSystemConfigID);
        public SystemConfigModel GetByID(int SystemConfigID);
        public SystemConfigModel GetByKey(string ConfigKey);

    }
    public class SystemConfigDAL : ISystemConfigDAL
    {
        private const string INSERT = @"v1_HT_SystemConfig_Insert";
        private const string UPDATE = @"v1_HT_SystemConfig_Update";
        private const string DELETE = @"v1_HT_SystemConfig_Delete";
        private const string GETBYID = @"v1_HT_SystemConfig_GetByID";
        private const string GETBYNAME = @"v1_HT_SystemConfig_GetByKey";
        private const string GETLISTPAGING = @"v1_HT_SystemConfig_GetPagingBySearch";

        // param constant
        private const string PARAM_SystemConfigID = @"SystemConfigID";
        private const string PARAM_ConfigKey = @"ConfigKey";
        private const string PARAM_ConfigValue = @"ConfigValue";
        private const string PARAM_Description = @"Description";
        // Insert
        //private DbINOUTContext _DbINOUTContext;
        //public SystemConfigDAL(DbINOUTContext dbINOUTContext)
        //{
        //    _DbINOUTContext = dbINOUTContext;
        //}
        public SystemConfigDAL()
        {

        }
        public BaseResultModel Insert(SystemConfigModel SystemConfigModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (SystemConfigModel == null || SystemConfigModel.ConfigKey == null || SystemConfigModel.ConfigKey.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "ConfigKey không được trống";
                }
                else if (SystemConfigModel.ConfigKey.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Độ dài của ConfigKey không được quá 200 ký tự";
                }

                else if (SystemConfigModel.ConfigValue == null || SystemConfigModel.ConfigValue.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "ConfigValue không được trống";
                }
                else if (SystemConfigModel.ConfigValue.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Độ dài của ConfigValue không được quá 200 ký tự";
                }
                else
                {
                    var SystemConfig = GetByKey(SystemConfigModel.ConfigKey.Trim());
                    if (SystemConfig != null && SystemConfig.SystemConfigID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "ConfigKey đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                    new SqlParameter(PARAM_ConfigKey, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_ConfigValue, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_Description, SqlDbType.NVarChar)
                        };
                        parameters[0].Value = SystemConfigModel.ConfigKey.Trim();
                        parameters[1].Value = SystemConfigModel.ConfigValue.Trim();
                        parameters[2].Value = SystemConfigModel.Description.Trim();
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, INSERT, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Tham số hệ thống");
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
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }

        // Update
        public BaseResultModel Update(SystemConfigModel SystemConfigModel)
        {
            var Result = new BaseResultModel();
            try
            {
                int crID;
                if (!int.TryParse(SystemConfigModel.SystemConfigID.ToString(), out crID) || SystemConfigModel.SystemConfigID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tham số hệ thống không tồn tại, hoặc SystemConfigID không đúng định dạng";
                }
                else if (SystemConfigModel == null || SystemConfigModel.ConfigKey == null || SystemConfigModel.ConfigKey.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "ConfigKey không được trống";
                }
                else if (SystemConfigModel.ConfigKey.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Độ dài của ConfigKey không được quá 200 ký tự";
                }

                else if (SystemConfigModel.ConfigValue == null || SystemConfigModel.ConfigValue.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "ConfigValue không được trống";
                }
                else if (SystemConfigModel.ConfigValue.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "ConfigValue không được quá 200 ký tự";
                }
                else
                {
                    var crObj = GetByID(SystemConfigModel.SystemConfigID);
                    var objDouble = GetByKey(SystemConfigModel.ConfigKey.Trim());
                    if (crObj == null || crObj.SystemConfigID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Tham số hệ thống không tồn tại";
                    }
                    else if (objDouble != null && objDouble.SystemConfigID > 0 && objDouble.SystemConfigID != SystemConfigModel.SystemConfigID)
                    {
                        Result.Status = 0;
                        Result.Message = "ConfigKey đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter(PARAM_SystemConfigID, SqlDbType.Int),
                            new SqlParameter(PARAM_ConfigKey, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_ConfigValue, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_Description, SqlDbType.NVarChar)
                        };

                        parameters[0].Value = SystemConfigModel.SystemConfigID;
                        parameters[1].Value = SystemConfigModel.ConfigKey.Trim();
                        parameters[2].Value = SystemConfigModel.ConfigValue.Trim();
                        parameters[3].Value = SystemConfigModel.Description.Trim();
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, UPDATE, parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("Tham số hệ thống");
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
                    }
                }
            }
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }
        public BaseResultModel Delete(List<int> ListSystemConfigID)
        {
            var Result = new BaseResultModel();
            if (ListSystemConfigID.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                for (int i = 0; i < ListSystemConfigID.Count; i++)
                {
                    int crID;
                    if (!int.TryParse(ListSystemConfigID[i].ToString(), out crID))
                    {
                        Result.Status = 0;
                        Result.Message = "SystemConfigID '" + ListSystemConfigID[i].ToString() + "' không đúng định dạng";
                        return Result;
                    }
                    else
                    {
                        var crObj = GetByID(ListSystemConfigID[i]);
                        if (crObj == null || crObj.SystemConfigID < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "SystemConfigID '" + ListSystemConfigID[i].ToString() + "' không tồn tại";
                            return Result;
                        }
                        else
                        {
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                            new SqlParameter(PARAM_SystemConfigID, SqlDbType.Int)
                            };
                            parameters[0].Value = ListSystemConfigID[i];
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETE, parameters);
                                        trans.Commit();
                                        if (val < 0)
                                        {
                                            Result.Status = 0;
                                            Result.Message = "Không thể xóa Tham số hệ thống có SystemConfigID = " + ListSystemConfigID[i];
                                            return Result;
                                        }
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
                Result.Message = ConstantLogMessage.Alert_Delete_Success("tham số hệ thống");
                return Result;
            }
        }

        public SystemConfigModel GetByID(int SystemConfigID)
        {
            SystemConfigModel SystemConfig = null;
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(PARAM_SystemConfigID,SqlDbType.Int)
            };
            parameters[0].Value = SystemConfigID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETBYID, parameters))
                {
                    while (dr.Read())
                    {
                        SystemConfig = new SystemConfigModel(Utils.ConvertToInt32(dr["SystemConfigID"], 0), Utils.ConvertToString(dr["ConfigKey"], string.Empty), Utils.ConvertToString(dr["ConfigValue"], string.Empty), Utils.ConvertToString(dr["Description"], string.Empty));
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return SystemConfig;
        }

        public SystemConfigModel GetByKey(string ConfigKey)
        {
            SystemConfigModel SystemConfig = new SystemConfigModel();
            SqliteParameter[] parameters = new SqliteParameter[]
            {
                new SqliteParameter(@"ConfigKey",DbType.String)
            };
            parameters[0].Value = ConfigKey;
            try
            {

                using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, QueryManager.Get("v1_HT_SystemConfig_GetByKey"), parameters))
                {
                    while (dr.Read())
                    {
                        SystemConfig = new SystemConfigModel(Utils.ConvertToInt32(dr["SystemConfigID"], 0), Utils.ConvertToString(dr["ConfigKey"], string.Empty), Utils.ConvertToString(dr["ConfigValue"], string.Empty), Utils.ConvertToString(dr["Description"], string.Empty));
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return SystemConfig;
        }

        public List<SystemConfigModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            //var db = _DbINOUTContext;
            //var xxx = db.DanhMucSystemConfig.Where(t => t.ConfigKey.Contains("a")).ToList();

            List<SystemConfigModel> list = new List<SystemConfigModel>();
            SqliteParameter[] parameters = new SqliteParameter[]
            {
            new SqliteParameter("@Keyword", DbType.String) { Size = 200 },
            new SqliteParameter("@OrderByName", DbType.String) { Size = 50 },
            new SqliteParameter("@OrderByOption", DbType.String) { Size = 50 },
            new SqliteParameter("@pLimit", DbType.Int32),
            new SqliteParameter("@pOffset", DbType.Int32)
            };

            parameters[0].Value = p.Keyword ?? "";
            parameters[1].Value = p.OrderByName ?? "";
            parameters[2].Value = p.OrderByOption ?? "";
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;

            try
            {
                using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(
                    SQLHelper.connectionString,
                    CommandType.Text,
                    QueryManager.Get("v1_HT_SystemConfig_GetPagingBySearch"),
                    parameters))
                {
                    TotalRow = 0; // reset trước
                    while (dr.Read())
                    {
                        SystemConfigModel item = new SystemConfigModel();
                        item.SystemConfigID = Utils.ConvertToInt32(dr["SystemConfigID"], 0);
                        item.ConfigKey = Utils.ConvertToString(dr["ConfigKey"], string.Empty);
                        item.ConfigValue = Utils.ConvertToString(dr["ConfigValue"], string.Empty);
                        item.Description = Utils.ConvertToString(dr["Description"], string.Empty);
                        list.Add(item);

                        // lấy TotalRow từ cột trong CTE
                        if (TotalRow == 0 && dr["TotalRow"] != DBNull.Value)
                        {
                            TotalRow = Convert.ToInt32(dr["TotalRow"]);
                        }
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return list;
        }


    }
}
