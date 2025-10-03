using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using System.Data.SqlClient;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface ISystemLogDAL
    {
        public int Insert(SystemLogModel systemLogInfo);
        public List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow);
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay);
        public int Insert_Notify(SystemLogModel systemLogInfo, ref int LogID);
    }

    public class SystemLogDAL : ISystemLogDAL
    {
        #region Database query string

        private const string V1_SYSTEMLOG_INSERT = @"v1_SystemLog_Insert";
        private const string V1_SYSTEMLOG_GETPAGINGBYSEARCH = @"v1_SystemLog_GetPagingBySearch";
        private const string V1_SYSTEMLOG_GETPAGINGBYQUANTRIDULIEU = @"v1_SystemLog_GetPagingByQuanTriDuLieu";

        #endregion

        #region paramaters constant

        private const string PARM_SYSTEMLOGid = @"SystemLogid";
        private const string PARM_CanBoID = @"CanBoID";
        private const string PARM_LOGINFO = @"LogInfo";
        private const string PARM_LOGTIME = @"LogTime";
        private const string PARM_LOGTYPE = @"LogType";
        private const string PARAM_KEY = "@Keyword";
        private const string PARAM_START = "@Start";
        private const string PARAM_END = "@End";
        #endregion

        #region Insert, update
        private SqliteParameter[] GetInsertParms()
        {
            SqliteParameter[] parms = new SqliteParameter[] {
                new SqliteParameter(PARM_CanBoID, SqlDbType.Int),
                new SqliteParameter(PARM_LOGINFO, SqlDbType.NVarChar),
                new SqliteParameter(PARM_LOGTIME, SqlDbType.DateTime),
                new SqliteParameter(PARM_LOGTYPE, SqlDbType.Int),
                new SqliteParameter("@SystemLogType", SqlDbType.Int),
                new SqliteParameter("@NhiemVuID", SqlDbType.Int),
                new SqliteParameter("@LogID", SqlDbType.Int)
            };
            return parms;
        }

        private void SetInsertParms(SqliteParameter[] parms, SystemLogModel systemLogInfo)
        {
            parms[0].Value = systemLogInfo.CanBoID;
            parms[1].Value = systemLogInfo.LogInfo;
            parms[2].Value = systemLogInfo.LogTime;
            parms[3].Value = systemLogInfo.LogType ?? Convert.DBNull;
            parms[4].Value = systemLogInfo.SystemLogType ?? Convert.DBNull;
            parms[5].Value = systemLogInfo.NhiemVuID ?? Convert.DBNull;
            parms[6].Value = Convert.DBNull;
        }

        //public int Insert(SystemLogModel systemLogInfo)
        //{
        //    object val = null;
        //    SqliteParameter[] parameters = GetInsertParms();
        //    SetInsertParms(parameters, systemLogInfo);
        //    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
        //    {
        //        conn.Open();
        //        using (SqlTransaction trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                val = SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, V1_SYSTEMLOG_INSERT, parameters);
        //                trans.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();
        //                throw;
        //            }
        //        }
        //        conn.Close();
        //    }
        //    return Convert.ToInt32(val);
        //}
        public int Insert(SystemLogModel systemLogInfo)
        {
            object val = null;
            var parameters = GetInsertParms();
            SetInsertParms(parameters, systemLogInfo);

            using (var conn = new SqliteConnection(SQLHelper.connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var sql = QueryManager.Get("v1_SystemLog_Insert");                      
                        using (var cmd = new SqliteCommand(sql, conn, trans))
                        {
                            cmd.Parameters.AddRange(parameters);
                            val = cmd.ExecuteScalar();
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                conn.Close();
            }

            return Convert.ToInt32(val);
        }
        public int Insert_Notify(SystemLogModel systemLogInfo, ref int LogID)
        {
            object val = null;
            SqlParameter[] parms = new SqlParameter[] {
                new SqlParameter(PARM_CanBoID, SqlDbType.Int),
                new SqlParameter(PARM_LOGINFO, SqlDbType.NVarChar),
                new SqlParameter(PARM_LOGTIME, SqlDbType.DateTime),
                new SqlParameter(PARM_LOGTYPE, SqlDbType.Int),
                  new SqlParameter("@SystemLogType", SqlDbType.Int),
                    new SqlParameter("@NhiemVuID", SqlDbType.Int),
                    new SqlParameter("@LogID", SqlDbType.Int)
            };
            parms[0].Value = systemLogInfo.CanBoID;
            parms[1].Value = systemLogInfo.LogInfo;
            parms[2].Value = systemLogInfo.LogTime;
            parms[3].Value = systemLogInfo.LogType ?? Convert.DBNull;
            parms[4].Value = systemLogInfo.SystemLogType ?? Convert.DBNull;
            parms[5].Value = systemLogInfo.NhiemVuID ?? Convert.DBNull;
            parms[6].Direction = ParameterDirection.Output;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, V1_SYSTEMLOG_INSERT, parms);
                        trans.Commit();
                        LogID = Utils.ConvertToInt32(parms[6].Value, 0);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                conn.Close();
            }
            return Convert.ToInt32(val);
        }

        public List<SystemLogPartialModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {

            List<SystemLogPartialModel> list = new List<SystemLogPartialModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),

            };
            parameters[0].Value = p.Keyword != null ? p.Keyword : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, V1_SYSTEMLOG_GETPAGINGBYSEARCH, parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogPartialModel item = new SystemLogPartialModel();
                        item.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], "");
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        item.LogInfo = Utils.ConvertToString(dr["LogInfo"], "");
                        item.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.MinValue);

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


        public List<SystemLogPartialModel> GetPagingByQuanTriDuLieu(BasePagingParams p, ref int TotalRow)
        {

            List<SystemLogPartialModel> list = new List<SystemLogPartialModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),

            };
            parameters[0].Value = p.Keyword != null ? p.Keyword : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, V1_SYSTEMLOG_GETPAGINGBYQUANTRIDULIEU, parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogPartialModel item = new SystemLogPartialModel();
                        item.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        item.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], "");
                        item.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
                        item.LogInfo = Utils.ConvertToString(dr["LogInfo"], "");
                        item.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.MinValue);

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
        #endregion
        // Lấy danh sách systemlog từ ngày đến ngày
        public List<SystemLogModel> GetAllSystemLogFromTo(string TuNgay, string DenNgay)
        {
            List<SystemLogModel> ListSystemLogs = new List<SystemLogModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TuNgay",SqlDbType.VarChar),
                   new SqlParameter("@DenNgay",SqlDbType.VarChar)
              };
            parameters[0].Value = TuNgay ?? Convert.DBNull;
            parameters[1].Value = DenNgay ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_GetListSystemLogFromTo", parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogModel SystemLogModel = new SystemLogModel();
                        SystemLogModel.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        SystemLogModel.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        SystemLogModel.LogInfo = Utils.ConvertToString(dr["LogInfo"], string.Empty);
                        SystemLogModel.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.Now);
                        SystemLogModel.LogType = Utils.ConvertToInt32(dr["LogType"], 0);
                        ListSystemLogs.Add(SystemLogModel);
                    }
                    dr.Close();
                    //if (canBo != null && canBo.CanBoID > 0)
                    //{
                    //    canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(CanBoID.Value);
                    //    // lây danh sách chức vụ của cán bộ
                    //}
                }
            }
            catch
            {
                throw;
            }
            return ListSystemLogs;
        }

        // Tạo file log
        public XDocument CreateLogFile(string SystemLogPath, string TuNgay, string DenNgay)
        {
            XDocument doc = new XDocument();
            var ListSystemLog = GetAllSystemLogFromTo(TuNgay, DenNgay);
            doc =
                  new XDocument(
                  new XElement("LogConfig", ListSystemLog.Select(x =>
                  new XElement("SystemLog", new XAttribute("SystemLogID", x.SystemLogid), new XAttribute("CanBoID", x.CanBoID), new XAttribute("LogInfo", x.LogInfo), new XAttribute("LogTime", x.LogTime), new
                XAttribute("LogType", x.LogType)))));
            doc.Save(SystemLogPath);
            return doc;
        }

        // Get log by canboid and  system log info

        public List<SystemLogModel> GetListSystemLogByCanBoAndLogInfo(int? CanBoID, string LogInfo)
        {
            List<SystemLogModel> ListSystemLogs = new List<SystemLogModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@CanBoID",SqlDbType.Int),
                   new SqlParameter("@LogInfo",SqlDbType.NVarChar)
              };
            parameters[0].Value = CanBoID ?? Convert.DBNull;
            parameters[1].Value = LogInfo ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_GetListSystemLogByCanBoIDAndLogInfo", parameters))
                {
                    while (dr.Read())
                    {
                        SystemLogModel SystemLogModel = new SystemLogModel();
                        SystemLogModel.SystemLogid = Utils.ConvertToInt32(dr["SystemLogid"], 0);
                        SystemLogModel.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        SystemLogModel.LogInfo = Utils.ConvertToString(dr["LogInfo"], string.Empty);
                        SystemLogModel.LogTime = Utils.ConvertToDateTime(dr["LogTime"], DateTime.Now);
                        SystemLogModel.LogType = Utils.ConvertToInt32(dr["LogType"], 0);
                        ListSystemLogs.Add(SystemLogModel);
                    }
                    dr.Close();
                    //if (canBo != null && canBo.CanBoID > 0)
                    //{
                    //    canBo.DanhSachChucVuID = CanBoChucVu_GetBy_CanBoID(CanBoID.Value);
                    //    // lây danh sách chức vụ của cán bộ
                    //}
                }
            }
            catch
            {
                throw;
            }
            return ListSystemLogs;
        }
    }
}
