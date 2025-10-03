using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Ultilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Com.Gosol.INOUT.DAL.DanhMuc
{
    public interface IDanhMucLoaiTaiSanDAL
    {
        public int Insert(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message);
        public int Update(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message);
        public List<string> Delete(List<int> LoaiTaiSanID);
        public DanhMucLoaiTaiSanModel GetLTSByID(int LoaiTaiSanID);
        //public List<DanhMucLoaiTaiSanModel> FilterByName(string tenLoaiTaiSan);
        public List<DanhMucLoaiTaiSanModel> GetAll();
        public List<DanhMucLoaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
    }
    public class DanhMucLoaiTaiSanDAL : IDanhMucLoaiTaiSanDAL
    {

        public DanhMucLoaiTaiSanDAL() { }
        // param constant 
        private const string PARAM_LoaiTaiSanID = "@LoaiTaiSanID";
        private const string PARAM_NhomTaiSanID = "@NhomTaiSanID";
        private const string PARAM_TENLOAITAISAN = "@TenLoaiTaiSan";
        private const string PARAM_TrangThaiSuDung = "@TrangThaiSuDung";


        // Insert loai tai san

        public int Insert(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message)
        {

            int val = 0;
            if (!Boolean.TryParse(DanhMucLoaiTaiSanModel.TrangThaiSuDung.ToString(),out bool check))
            {

                Message = ConstantLogMessage.API_Error_Status;
                val = 0;
                return val;
            }
            if (DanhMucLoaiTaiSanModel.TenLoaiTaiSan.Trim().Length > 50)
            {
                Message = ConstantLogMessage.API_Error_TooLong;
                val = 0;
                return val;
            }
            if (string.IsNullOrEmpty(DanhMucLoaiTaiSanModel.TenLoaiTaiSan) || DanhMucLoaiTaiSanModel.TenLoaiTaiSan.Trim().Length <= 0)
            {
                Message = ConstantLogMessage.Alert_Error_NotFill("Tên loại tài sản");
                val = 0;
                return val;
            }
            if (!Utils.CheckSpecialCharacter(DanhMucLoaiTaiSanModel.TenLoaiTaiSan))
            {
                Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                val = 0;
                return val;
            }
            if (DanhMucLoaiTaiSanModel.NhomTaiSanID != null && DanhMucLoaiTaiSanModel.NhomTaiSanID.Value != 0)
            {
                var NhomTaiSan = new DanhMucNhomTaiSanDAL().GetNTSByID(DanhMucLoaiTaiSanModel.NhomTaiSanID.Value);
                if (NhomTaiSan == null || NhomTaiSan.NhomTaiSanID <= 0)
                {
                    val = 0;
                    Message = ConstantLogMessage.Alert_Error_NotExist("Nhóm tài sản");
                    return val;
                }
            }
            if (DanhMucLoaiTaiSanModel == null)
            {
                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            var loaiTaiSan = GetByName(DanhMucLoaiTaiSanModel.TenLoaiTaiSan);
            if (loaiTaiSan.LoaiTaiSanID > 0)
            {
                Message = ConstantLogMessage.Alert_Error_Exist("Tên loại tài sản");
                return val = 0;
            }
            else
            {
                SqlParameter[] parameters = new SqlParameter[]
                  {
                new SqlParameter(PARAM_NhomTaiSanID,SqlDbType.Int),
                new SqlParameter(PARAM_TENLOAITAISAN,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TrangThaiSuDung,SqlDbType.Bit)
                  };
                parameters[0].Value = DanhMucLoaiTaiSanModel.NhomTaiSanID ?? Convert.DBNull;
                parameters[1].Value = DanhMucLoaiTaiSanModel.TenLoaiTaiSan;
                parameters[2].Value = DanhMucLoaiTaiSanModel.TrangThaiSuDung ?? Convert.DBNull;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_LoaiTaiSan_Insert", parameters);
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw;
                        }

                    }
                }
            }
            Message = ConstantLogMessage.Alert_Insert_Success("loại tài sản");
            return val;
        }

        // Update loai tai san
        public int Update(DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel, ref string Message)
        {
            int val = 0;
            if (DanhMucLoaiTaiSanModel.LoaiTaiSanID == 0)
            {
                Message = "Chưa có loại tài sản được chọn!";
                return val;
            }
            if (DanhMucLoaiTaiSanModel.TenLoaiTaiSan.Trim().Length > 50)
            {
                Message = ConstantLogMessage.API_Error_TooLong;
                val = 0;
                return val;
            }
            if (string.IsNullOrEmpty(DanhMucLoaiTaiSanModel.TenLoaiTaiSan) || DanhMucLoaiTaiSanModel.TenLoaiTaiSan.Trim().Length <= 0)
            {
                Message = ConstantLogMessage.Alert_Error_NotFill("Tên loại tài sản");
                val = 0;
                return val;
            }
            if (!Utils.CheckSpecialCharacter(DanhMucLoaiTaiSanModel.TenLoaiTaiSan))
            {
                Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                val = 0;
                return val;
            }
            if (DanhMucLoaiTaiSanModel.NhomTaiSanID != 0)
            {
                var NhomTaiSan = new DanhMucNhomTaiSanDAL().GetNTSByID(DanhMucLoaiTaiSanModel.NhomTaiSanID.Value);
                if (NhomTaiSan == null || NhomTaiSan.NhomTaiSanID <= 0)
                {
                    val = 0;
                    Message = ConstantLogMessage.Alert_Error_NotExist("Nhóm tài sản");
                    return val;
                }
            }
            if (DanhMucLoaiTaiSanModel == null)
            {
                Message = ConstantLogMessage.Alert_Error_NotFill("Loại tài sản");
                return val;
            }
            var loaiTaiSan = GetByName(DanhMucLoaiTaiSanModel.TenLoaiTaiSan);
            if (loaiTaiSan.LoaiTaiSanID > 0 && loaiTaiSan.LoaiTaiSanID != DanhMucLoaiTaiSanModel.LoaiTaiSanID)
            {
                Message = ConstantLogMessage.Alert_Error_Exist("Tên loại tài sản");
                return val;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_LoaiTaiSanID,SqlDbType.Int),
                new SqlParameter(PARAM_NhomTaiSanID,SqlDbType.Int),
                new SqlParameter(PARAM_TENLOAITAISAN,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TrangThaiSuDung,SqlDbType.Bit)
              };
            parameters[0].Value = DanhMucLoaiTaiSanModel.LoaiTaiSanID;
            parameters[1].Value = DanhMucLoaiTaiSanModel.NhomTaiSanID ?? Convert.DBNull;
            parameters[2].Value = DanhMucLoaiTaiSanModel.TenLoaiTaiSan;
            parameters[3].Value = DanhMucLoaiTaiSanModel.TrangThaiSuDung ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_LoaiTaiSan_Update", parameters), 0);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                    Message = ConstantLogMessage.Alert_Update_Success("loại tài sản");
                    return val;
                }
            }

        }

        // Delete loai tai san
        public List<string> Delete(List<int> ListLoaiTaiSanID)
        {

            List<string> dic = new List<string>();
            string message = "";
            if (ListLoaiTaiSanID.Count <= 0)
            {
                dic.Add(ConstantLogMessage.API_Error_NotSelected);
                return dic;
            }
            //if (ListLoaiTaiSanID.Count <= 0)
            //{
            //    message = "Loại Tài Sản không tồn tại";
            //    dic.Add(0, message);
            //    return dic;
            //}
            else
            {
                for (int i = 0; i < ListLoaiTaiSanID.Count; i++)
                {
                    int val = 0;
                    if (GetLTSByID(ListLoaiTaiSanID[i]).NhomTaiSanID <= 0)
                    {
                        message = ConstantLogMessage.API_Error_NotExist;
                        dic.Add(message);
                        //return dic;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                          new SqlParameter(@"LoaiTaiSanID", SqlDbType.Int)

                        };
                        parameters[0].Value = ListLoaiTaiSanID[i];
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_LoaiTaiSan_Delete", parameters), 0);
                                    trans.Commit();
                                    //if (val == 0)
                                    //{
                                    //    message = "Không thể xóa từ Loại tài sản thứ " + ListLoaiTaiSanID[i];
                                    //    dic.Add(0, message);
                                    //    return dic;
                                    //}
                                    //message = "Xóa dữ liệu thành công!";
                                    //dic.Add(1, message);
                                    //return dic;
                                }
                                catch
                                {
                                    trans.Rollback();
                                    throw;
                                }


                            }
                        }


                    }
                }
                //message = ConstantLogMessage.API_Delete_Success;
                //dic.Add(1, message);
                return dic;
            }

        }

        // Get by id
        public DanhMucLoaiTaiSanModel GetLTSByID(int LoaiTaiSanID)
        {
            if (LoaiTaiSanID <= 0)
            {
                return new DanhMucLoaiTaiSanModel();
            }
            DanhMucLoaiTaiSanModel loaiTaiSan = new DanhMucLoaiTaiSanModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_LoaiTaiSanID,SqlDbType.Int)
              };
            parameters[0].Value = LoaiTaiSanID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_LoaiTaiSan_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        loaiTaiSan = new DanhMucLoaiTaiSanModel(Utils.ConvertToInt32(dr["LoaiTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToString(dr["TenLoaiTaiSan"], String.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));

                    }
                    dr.Close();
                }
                return loaiTaiSan;
            }
            catch
            {
                throw;
            }
        }

        // Get By Name
        public DanhMucLoaiTaiSanModel GetByName(string TenLoaiTaiSan)
        {
            if (string.IsNullOrEmpty(TenLoaiTaiSan))
            {
                return new DanhMucLoaiTaiSanModel();
            }
            DanhMucLoaiTaiSanModel loaiTaiSan = new DanhMucLoaiTaiSanModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenLoaiTaiSan",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenLoaiTaiSan;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_LoaiTaiSan_GetByName", parameters))
                {
                    while (dr.Read())
                    {
                        loaiTaiSan = new DanhMucLoaiTaiSanModel(Utils.ConvertToInt32(dr["LoaiTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToString(dr["TenLoaiTaiSan"], String.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));

                    }
                    dr.Close();
                }
                return loaiTaiSan;
            }
            catch
            {
                throw;
            }
        }

        // Filter by name
        //public List<DanhMucLoaiTaiSanModel> FilterByName(string tenLoaiTaiSan)
        //{
        //    List<DanhMucLoaiTaiSanModel> list = new List<DanhMucLoaiTaiSanModel>();
        //  SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter(PARAM_TENLOAITAISAN,SqlDbType.NVarChar)
        //    };
        //    parameters[0].Value = tenLoaiTaiSan;
        //    try
        //    {
        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILTERBYNAME, parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel = new DanhMucLoaiTaiSanModel(Utils.ConvertToInt32(dr["LoaiTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToString(dr["TenLoaiTaiSan"], String.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));
        //                list.Add(DanhMucLoaiTaiSanModel);
        //            }
        //            dr.Close();
        //        }
        //        return list;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        // Get All
        public List<DanhMucLoaiTaiSanModel> GetAll()
        {
            List<DanhMucLoaiTaiSanModel> list = new List<DanhMucLoaiTaiSanModel>();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_LoaiTaiSan_GetAll"))
                {
                    while (dr.Read())
                    {
                        DanhMucLoaiTaiSanModel DanhMucLoaiTaiSanModel = new DanhMucLoaiTaiSanModel(Utils.ConvertToInt32(dr["LoaiTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToString(dr["TenLoaiTaiSan"], String.Empty), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true));
                        list.Add(DanhMucLoaiTaiSanModel);
                    }
                    dr.Close();
                }
                return list;
            }
            catch
            {
                throw;
            }
        }

        // Get Paging by search
        public List<DanhMucLoaiTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            List<DanhMucLoaiTaiSanModel> list = new List<DanhMucLoaiTaiSanModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),

              };
            parameters[0].Value = p.Keyword == null ? "" : p.Keyword.Trim();
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            //parameters[5].Value = 0;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_DanhMuc_LoaiTaiSan_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucLoaiTaiSanModel item = new DanhMucLoaiTaiSanModel();
                        item.LoaiTaiSanID = Utils.ConvertToInt32(dr["LoaiTaiSanID"], 0);
                        item.NhomTaiSanID = Utils.ConvertToInt32(dr["NhomTaiSanID"], 0);
                        item.TenLoaiTaiSan = Utils.ConvertToString(dr["TenLoaiTaiSan"], "");
                        item.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], false);
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
    }
}
