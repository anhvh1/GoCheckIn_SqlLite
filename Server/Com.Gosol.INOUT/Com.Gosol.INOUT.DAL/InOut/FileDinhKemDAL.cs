using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.InOut;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Gosol.INOUT.DAL.FileDinhKem
{
    public interface IFileDinhKemDAL
    {
        public BaseResultModel Insert(FileDinhKemModel FileDinhKemModel);
        public BaseResultModel Insert_v4(FileDinhKemModel fileDinhKemModel);
        public List<FileDinhKemModel> GetBy_ThongTinRaVaoID(int ThongTinRaVaoID);
        public BaseResultModel UpdateFeatureID(FileDinhKemModel fileDinhKemModel);
    }
    public class FileDinhKemDAL : IFileDinhKemDAL
    {
        #region Get
        public List<FileDinhKemModel> GetBy_ThongTinRaVaoID(int ThongTinRaVaoID)
        {
            List<FileDinhKemModel> Result = new List<FileDinhKemModel>();
            //if (fileDinhKemModel == null || fileDinhKemModel.Base64File == null || fileDinhKemModel.Base64File.Length < 1)
            //{
            //    Result.Status = 0;
            //    Result.Message = "Vui lòng chọn file ảnh chân dung";
            //    return Result;
            //}
            SqlParameter[] parameters = new SqlParameter[]
            {
                  new SqlParameter("@ThongTinRaVaoID",SqlDbType.Int)
            };
            parameters[0].Value = ThongTinRaVaoID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_FileDinhKem_GetBy_ThongTinRaVaoID", parameters))
                {
                    while (dr.Read())
                    {
                        var crFile = new FileDinhKemModel();
                        crFile.ThongTinVaoRaID = Utils.ConvertToInt32(dr["ThongTinVaoRaID"], 0);
                        crFile.FileDinhKemID = Utils.ConvertToInt32(dr["FileDinhKemID"], 0);
                        crFile.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        crFile.LoaiFile = Utils.ConvertToInt32(dr["LoaiFile"], 0);
                        Result.Add(crFile);
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
        #endregion

        #region Insert
        public BaseResultModel Insert(FileDinhKemModel fileDinhKemModel)
        {
            var Result = new BaseResultModel();
            if (fileDinhKemModel == null || fileDinhKemModel.Base64File == null || fileDinhKemModel.Base64File.Length < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn file ảnh chân dung";
                return Result;
            }
            SqliteParameter[] parameters = new SqliteParameter[]
            {
                new SqliteParameter("@TenFileGoc",DbType.String),
                new SqliteParameter("@TenFileHeThong",DbType.String),
                new SqliteParameter("@LoaiFile",DbType.Int32),
                new SqliteParameter("@FileUrl",DbType.String),
                new SqliteParameter("@NgayTao",DbType.DateTime),
                new SqliteParameter("@CoBaoMat",DbType.Boolean),
                new SqliteParameter("@NguoiTaoID",DbType.Int32),
                new SqliteParameter("@ThongTinVaoRaID",DbType.Int32)

            };
            parameters[0].Value = fileDinhKemModel.TenFileGoc ?? Convert.DBNull;
            parameters[1].Value = fileDinhKemModel.TenFileHeThong ?? Convert.DBNull;
            parameters[2].Value = fileDinhKemModel.LoaiFile ?? Convert.DBNull;
            parameters[3].Value = fileDinhKemModel.FileUrl ?? Convert.DBNull;
            parameters[4].Value = fileDinhKemModel.NgayTao ?? Convert.DBNull;
            parameters[5].Value = fileDinhKemModel.CoBaoMat ?? Convert.DBNull;
            parameters[6].Value = fileDinhKemModel.NguoiTaoID ?? Convert.DBNull;
            parameters[7].Value = fileDinhKemModel.ThongTinVaoRaID ?? Convert.DBNull;

            using (SqliteConnection conn = new SqliteConnection(SQLHelper.connectionString))
            {
                conn.Open();
                using (SqliteTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        Result.Status = SQLHelper.ExecuteNonQueryV2(conn,trans, CommandType.Text, QueryManager.Get("v1_NV_FileDinhKemAnh_Insert"), parameters);
                        Result.Message = ConstantLogMessage.Alert_Insert_Success("Thêm file đính kèm");
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        Result.Message = ex.Message;
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            return Result;
        }

        public BaseResultModel Insert_v4(FileDinhKemModel fileDinhKemModel)
        {
            var Result = new BaseResultModel();
            
            SqliteParameter[] parameters = new SqliteParameter[]
            {
                new SqliteParameter("@TenFileGoc",DbType.String),
                new SqliteParameter("@TenFileHeThong",DbType.String),
                new SqliteParameter("@LoaiFile",DbType.Int32),
                new SqliteParameter("@FileUrl",DbType.String),
                new SqliteParameter("@NgayTao",DbType.DateTime),
                new SqliteParameter("@CoBaoMat",DbType.Boolean),
                new SqliteParameter("@NguoiTaoID",DbType.Int32),
                new SqliteParameter("@ThongTinVaoRaID",DbType.Int32),
                new SqliteParameter("@LaCanBo",DbType.Boolean),
            };
            parameters[0].Value = fileDinhKemModel.TenFileGoc ?? Convert.DBNull;
            parameters[1].Value = fileDinhKemModel.TenFileHeThong ?? Convert.DBNull;
            parameters[2].Value = fileDinhKemModel.LoaiFile ?? Convert.DBNull;
            parameters[3].Value = fileDinhKemModel.FileUrl ?? Convert.DBNull;
            parameters[4].Value = fileDinhKemModel.NgayTao ?? Convert.DBNull;
            parameters[5].Value = fileDinhKemModel.CoBaoMat ?? Convert.DBNull;
            parameters[6].Value = fileDinhKemModel.NguoiTaoID ?? Convert.DBNull;
            parameters[7].Value = fileDinhKemModel.ThongTinVaoRaID ?? Convert.DBNull;
            parameters[8].Value = fileDinhKemModel.LaCanBo ?? Convert.DBNull;

            using (SqliteConnection conn = new SqliteConnection(SQLHelper.connectionString))
            {
                conn.Open();
                using (SqliteTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        var val = SQLHelper.ExecuteScalarV2(conn,trans,CommandType.Text, QueryManager.Get("v1_NV_FileDinhKemAnh_Insert_New"), parameters);
                        Result.Data = Utils.ConvertToInt32(val, 0);
                        Result.Status = 1;
                        Result.Message = ConstantLogMessage.Alert_Insert_Success("Thêm file đính kèm");
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        Result.Message = ex.Message;
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return Result;
        }
        #endregion

        #region Update
        public BaseResultModel UpdateFeatureID(FileDinhKemModel fileDinhKemModel)
        {
            var result = new BaseResultModel();
            
            SqliteParameter[] parameters = new SqliteParameter[]
              {
                new SqliteParameter("FileDinhKemID", SqlDbType.Int),
                new SqliteParameter("FeatureID", SqlDbType.Int)
              };
            parameters[0].Value = fileDinhKemModel.FileDinhKemID;
            parameters[1].Value = fileDinhKemModel.FeatureID;

            using (SqliteConnection conn = new SqliteConnection(SQLHelper.connectionString))
            {
                conn.Open();
                using (SqliteTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        result.Status = SQLHelper.ExecuteNonQueryV2(conn,trans, CommandType.Text, QueryManager.Get("v4_NV_FileDinhKemAnh_UpdateFeatureID"), parameters);
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
                    result.Message = ConstantLogMessage.Alert_Update_Success("file");
                    result.Status = 1;
                }
                return result;
            }
        }
        #endregion

        #region Delete
        //public List<string> Delete(int FileDinhKemID)
        //{

        //    List<string> dic = new List<string>();

        //    SqlParameter[] parameters = new SqlParameter[]
        //                  {new SqlParameter("@FileDinhKemID", SqlDbType.Int)};
        //    parameters[0].Value = FileDinhKemID;
        //    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
        //    {
        //        conn.Open();
        //        using (SqlTransaction trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_NV_FileDinhKem_Delete", parameters), 0);
        //                trans.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                dic.Add(ex.Message);
        //                trans.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //    return dic;
        //}
        #endregion
    }
}
