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

namespace Com.Gosol.INOUT.DAL.InOut
{
    public interface IThongTinKhachDAL
    {
        //public int Insert(Thong ThongTinKhachModel);
        //public ThongTinKhachModel GetThongTinKhachBySoHieuThongTinKhach(string SoHieuThongTinKhach);
        //public int Update(ThongTinKhachModel ThongTinKhachModel, ref string Message, int CanBoDangNhap);
    }
    public class ThongTinKhachDAL : IThongTinKhachDAL
    {
        #region Get

        /// <summary>
        /// lấy thông tin khách theo số cmnd
        /// </summary>
        /// <param name="SoCMND"></param>
        /// <returns></returns>
        public ThongTinKhachModel Get_By_CMND(string SoCMND)
        {
            if (string.IsNullOrEmpty(SoCMND))
            {
                return new ThongTinKhachModel();
            }
            ThongTinKhachModel ttKhach = new ThongTinKhachModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"SoCMND",SqlDbType.NVarChar)
              };
            parameters[0].Value = SoCMND;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_NV_ThongTinKhach_GetBy_SoCMND", parameters))
                {
                    while (dr.Read())
                    {
                        ttKhach = new ThongTinKhachModel();
                        ttKhach.ThongTinKhachID = Utils.ConvertToInt32(dr["ThongTinKhachID"], 0);
                        ttKhach.HoVaTen = Utils.ConvertToString(dr["HoVaTen"], string.Empty);
                        ttKhach.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.MinValue);
                        ttKhach.HoKhau = Utils.ConvertToString(dr["HoKhau"], string.Empty);
                        ttKhach.DienThoai = Utils.ConvertToString(dr["DienThoai"], string.Empty);
                        ttKhach.SoCMND = Utils.ConvertToString(dr["SoCMND"], string.Empty);
                        ttKhach.NoiCapCMND = Utils.ConvertToString(dr["NoiCapCMND"], String.Empty);
                        ttKhach.NgayCapCMND = Utils.ConvertToDateTime(dr["NgayCapCMND"], DateTime.MinValue);
                        break;
                    }
                    dr.Close();
                }
                return ttKhach;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region insert

        /// <summary>
        /// thêm thông tin khách
        /// </summary>
        /// <param name="TTKhachModel"></param>
        /// <returns></returns>
        public BaseResultModel Insert(ThongTinVaoRaModel TTKhachModel, ref string SoCMND)
        {
            var Result = new BaseResultModel();
            try
            {
                if (TTKhachModel == null || TTKhachModel.SoCMND == null || TTKhachModel.SoCMND.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Số CMND không được trống";
                }
                else if (TTKhachModel.HoVaTen == null || TTKhachModel.HoVaTen.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Họ và tên không được trống";
                }
                else
                {
                    var crKhach = Get_By_CMND(TTKhachModel.SoCMND);
                    if (crKhach != null && crKhach.ThongTinKhachID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Thông tin khách đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                            new SqlParameter("HoVaTen", SqlDbType.NVarChar),
                            new SqlParameter("NgaySinh", SqlDbType.DateTime),
                            new SqlParameter("HoKhau", SqlDbType.NVarChar),
                            new SqlParameter("DienThoai", SqlDbType.VarChar),
                            new SqlParameter("SoCMND", SqlDbType.VarChar),
                            new SqlParameter("NoiCapCMND", SqlDbType.NVarChar),
                            new SqlParameter("NgayCapCMND", SqlDbType.DateTime)
                          };
                        parameters[0].Value = TTKhachModel.HoVaTen;
                        parameters[1].Value = TTKhachModel.NgaySinh ?? Convert.DBNull;
                        parameters[2].Value = TTKhachModel.HoKhau ?? Convert.DBNull;
                        parameters[3].Value = TTKhachModel.DienThoai ?? Convert.DBNull;
                        parameters[4].Value = TTKhachModel.SoCMND ?? Convert.DBNull;
                        parameters[5].Value = TTKhachModel.NoiCapCMND ?? Convert.DBNull;
                        parameters[6].Value = TTKhachModel.NgayCapCMND ?? Convert.DBNull;
                        SoCMND = TTKhachModel.SoCMND;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, System.Data.CommandType.StoredProcedure, "v1_NV_ThongTinKhach_Insert", parameters), 0);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Thông tin khách");
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
        #endregion


    }
}
