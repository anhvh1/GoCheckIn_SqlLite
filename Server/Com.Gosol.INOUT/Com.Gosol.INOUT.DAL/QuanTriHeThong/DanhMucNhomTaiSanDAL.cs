
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Com.Gosol.INOUT.Models.DanhMuc;
using MySql.Data.MySqlClient;
using Com.Gosol.INOUT.Ultilities;
using Com.Gosol.INOUT.Models;
using System.Data;
using System.Linq;
namespace Com.Gosol.INOUT.DAL.DanhMuc
{
    public interface IDanhMucNhomTaiSanDAL
    {
        public int Insert(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message);
        public int Update(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message);
        public List<string> Delete(List<int> ListNhomTaiSanID);
        public DanhMucNhomTaiSanModel GetNTSByID(int NhomTaiSanID);
        //public List<DanhMucNhomTaiSanModel> FilterByName(string TenNhomTaiSan);
        public List<DanhMucNhomTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
        public List<DanhMucNhomTaiSanModel> GetAllNhomTaiSanCha();
    }

    public class DanhMucNhomTaiSanDAL : IDanhMucNhomTaiSanDAL
    {
        public DanhMucNhomTaiSanDAL()
        {

        }


        private const string GETALL = @"v1_DanhMuc_NhomTaiSan_GetAll";
        // param constant
        private const string PARAM_NhomTaiSanID = "@NhomTaiSanID";
        private const string PARAM_NhomTaiSanChaID = "@NhomTaiSanChaID";
        private const string PARAM_TenNhomTaiSan = "@TenNhomTaiSan";
        private const string PARAM_CoQuanID = "@CoQuanID";
        private const string PARAM_TrangThaiSuDung = "@TrangThaiSuDung";
        private const string PARAM_ThuTuSapXep = "@ThuTuSapXep";
        private const string PARAM_MoTa = "@MoTa";

        // Insert
        public int Insert(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message)
        {
            int val = 0;
            if (DanhMucNhomTaiSanModel.TenNhomTaiSan.Trim().Length > 50)
            {
                Message = ConstantLogMessage.API_Error_TooLong;
                return val;

            }
            if (string.IsNullOrEmpty(DanhMucNhomTaiSanModel.TenNhomTaiSan) || DanhMucNhomTaiSanModel.TenNhomTaiSan.Trim().Length <= 0)
            {
                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            if (!Utils.CheckSpecialCharacter(DanhMucNhomTaiSanModel.TenNhomTaiSan))
            {
                Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                return val;
            }
            var NhomTaiSan = GetByName(DanhMucNhomTaiSanModel.TenNhomTaiSan);
            if (NhomTaiSan.NhomTaiSanID > 0)
            {
                Message = ConstantLogMessage.Alert_Error_Exist("Tên nhóm tài sản");
                return val;
            }
            if (DanhMucNhomTaiSanModel.NhomTaiSanChaID > 0)
            {
                var NhomTaiSan1 = GetNTSByID(DanhMucNhomTaiSanModel.NhomTaiSanChaID.Value);
                if (NhomTaiSan1 == null)
                {
                    val = 0;
                    Message = ConstantLogMessage.Alert_Error_NotExist("Nhóm tài sản");
                    return val;
                }
            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_NhomTaiSanChaID,SqlDbType.Int),
                new SqlParameter(PARAM_TenNhomTaiSan,SqlDbType.NVarChar),
                new SqlParameter(PARAM_CoQuanID,SqlDbType.Int),
                new SqlParameter(PARAM_TrangThaiSuDung,SqlDbType.Bit),
                new SqlParameter(PARAM_ThuTuSapXep,SqlDbType.Int),
                new SqlParameter(PARAM_MoTa,SqlDbType.NVarChar)
              };
            parameters[0].Value = DanhMucNhomTaiSanModel.NhomTaiSanChaID ?? Convert.DBNull;
            parameters[1].Value = DanhMucNhomTaiSanModel.TenNhomTaiSan;
            parameters[2].Value = DanhMucNhomTaiSanModel.CoQuanID ?? Convert.DBNull;
            parameters[3].Value = DanhMucNhomTaiSanModel.TrangThaiSuDung ?? Convert.DBNull;
            parameters[4].Value = DanhMucNhomTaiSanModel.ThuTuSapXep ?? Convert.DBNull;
            parameters[5].Value = DanhMucNhomTaiSanModel.MoTa;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {

                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_NhomTaiSan_Insert", parameters);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            Message = ConstantLogMessage.Alert_Insert_Success("nhóm tài sản");
            return val;
        }

        // Update
        public int Update(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel, ref string Message)
        {
            int val = 0;
            if(DanhMucNhomTaiSanModel.NhomTaiSanID == 0)
            {
                Message = "Chưa có nhóm tài sản được chọn!";
                return val;
            }
            if (DanhMucNhomTaiSanModel.TenNhomTaiSan.Trim().Length > 50)
            {
                Message = ConstantLogMessage.API_Error_TooLong;
                return val;

            }
            if (string.IsNullOrEmpty(DanhMucNhomTaiSanModel.TenNhomTaiSan) || DanhMucNhomTaiSanModel.TenNhomTaiSan.Trim().Length <= 0)
            {
                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            if (!Utils.CheckSpecialCharacter(DanhMucNhomTaiSanModel.TenNhomTaiSan))
            {
                Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                return val;
            }
            var NhomTaiSan = GetByName(DanhMucNhomTaiSanModel.TenNhomTaiSan);
            if (NhomTaiSan.NhomTaiSanID > 0 && NhomTaiSan.NhomTaiSanID != DanhMucNhomTaiSanModel.NhomTaiSanID)
            {

                Message = ConstantLogMessage.Alert_Error_Exist("Tên nhóm tài sản");
                return val;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_NhomTaiSanID,SqlDbType.Int),
                new SqlParameter(PARAM_NhomTaiSanChaID,SqlDbType.Int),
                new SqlParameter(PARAM_TenNhomTaiSan,SqlDbType.NVarChar),
                new SqlParameter(PARAM_CoQuanID,SqlDbType.Int),
                new SqlParameter(PARAM_TrangThaiSuDung,SqlDbType.Bit),
                new SqlParameter(PARAM_ThuTuSapXep,SqlDbType.Int),
                new SqlParameter(PARAM_MoTa,SqlDbType.NVarChar)
              };
            parameters[0].Value = DanhMucNhomTaiSanModel.NhomTaiSanID;
            parameters[1].Value = DanhMucNhomTaiSanModel.NhomTaiSanChaID ?? Convert.DBNull;
            parameters[2].Value = DanhMucNhomTaiSanModel.TenNhomTaiSan;
            parameters[3].Value = DanhMucNhomTaiSanModel.CoQuanID ?? Convert.DBNull;
            parameters[4].Value = DanhMucNhomTaiSanModel.TrangThaiSuDung ?? Convert.DBNull;
            parameters[5].Value = DanhMucNhomTaiSanModel.ThuTuSapXep ?? Convert.DBNull;
            parameters[6].Value = DanhMucNhomTaiSanModel.MoTa;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_NhomTaiSan_Update", parameters);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            Message = ConstantLogMessage.Alert_Update_Success("nhóm tài sản");
            return val;
        }
        // Delete
        public List<string> Delete(List<int> ListNhomTaiSanID)
        {
            List<string> dic = new List<string>();
            string message = "";
            if (ListNhomTaiSanID.Count <= 0)
            {
                message = ConstantLogMessage.API_Error_NotExist;
                dic.Add(message);           
            }
            else
            {
                for (int i = 0; i < ListNhomTaiSanID.Count; i++)
                {
                    //var GetNhomTaiSanChil = GetAllNTS().Where(x => x.NhomTaiSanChaID == ListNhomTaiSanID[i]).ToList().Count();
                    //var GetLoaiTaiSanChil = new DanhMucLoaiTaiSanDAL().GetAll().Where(x => x.NhomTaiSanID == ListNhomTaiSanID[i]).ToList().Count();
                    //if (GetNhomTaiSanChil > 0 || GetLoaiTaiSanChil > 0)
                    //{
                    //    dic.Add(0, "Nhóm tài sản đang sử dụng không thể xóa!Thử lại!");
                    //    return dic;
                    //}
                    int val = 0;
                    //List<DanhMucLoaiTaiSanModel> LoaiTaiSan = new DanhMucLoaiTaiSanDAL().GetAll();
                    //var query = LoaiTaiSan.Where(x => x.NhomTaiSanID == ListNhomTaiSanID[i]).Count();
                    if (GetNTSByID(ListNhomTaiSanID[i]).NhomTaiSanID <=0)
                    {
                        message = ConstantLogMessage.API_Error_NotExist;
                        dic.Add(message);
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
  {
                    new SqlParameter("@NhomTaiSanID", SqlDbType.Int)

  };
                        parameters[0].Value = ListNhomTaiSanID[i];
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_NhomTaiSan_Delete", parameters);
                                    trans.Commit();
                                }
                                catch
                                {
                                    trans.Rollback();
                                    throw;
                                }


                            }
                        }
                        var NhomTaiSanByParentID = GetAllNTS().Where(x => x.NhomTaiSanChaID == ListNhomTaiSanID[i]).ToList();
                        foreach (var item in NhomTaiSanByParentID)
                        {
                            item.NhomTaiSanChaID = null;
                            string Message = null;
                            Update(item, ref Message);
                        }
                    }
                }

               
            }
            return dic;
        }
        // Get all nhom tai san
        public List<DanhMucNhomTaiSanModel> GetAllNTS()
        {
            List<DanhMucNhomTaiSanModel> list = new List<DanhMucNhomTaiSanModel>();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETALL))
                {
                    while (dr.Read())
                    {

                        DanhMucNhomTaiSanModel nhomTaiSan = new DanhMucNhomTaiSanModel(Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0), Utils.ConvertToString(dr["TenNhomTaiSan"], String.Empty), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true), Utils.ConvertToInt32(dr["ThuTuSapXep"], 0), Utils.ConvertToString(dr["MoTa"], String.Empty));
                        list.Add(nhomTaiSan);
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
        // Get nhom tai san by id
        public DanhMucNhomTaiSanModel GetNTSByID(int NhomTaiSanID)
        {
            if (NhomTaiSanID <= 0)
            {
                return new DanhMucNhomTaiSanModel();
            }
            DanhMucNhomTaiSanModel nhomTaiSan = new DanhMucNhomTaiSanModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_NhomTaiSanID,SqlDbType.Int)
              };
            parameters[0].Value = NhomTaiSanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_NhomTaiSan_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        nhomTaiSan = new DanhMucNhomTaiSanModel(Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0), Utils.ConvertToString(dr["TenNhomTaiSan"], String.Empty), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true), Utils.ConvertToInt32(dr["ThuTuSapXep"], 0), Utils.ConvertToString(dr["MoTa"], String.Empty));

                    }
                    dr.Close();
                }
                return nhomTaiSan;
            }
            catch
            {
                throw;
            }
        }

        // Get By Name
        public DanhMucNhomTaiSanModel GetByName(string TenNhomTaiSan)
        {
            if (string.IsNullOrEmpty(TenNhomTaiSan))
            {
                return new DanhMucNhomTaiSanModel();
            }
            DanhMucNhomTaiSanModel nhomTaiSan = new DanhMucNhomTaiSanModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenNhomTaiSan",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenNhomTaiSan;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_NhomTaiSan_GetByName", parameters))
                {
                    while (dr.Read())
                    {
                        nhomTaiSan = new DanhMucNhomTaiSanModel(Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0), Utils.ConvertToString(dr["TenNhomTaiSan"], String.Empty), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true), Utils.ConvertToInt32(dr["ThuTuSapXep"], 0), Utils.ConvertToString(dr["MoTa"], String.Empty));

                    }
                    dr.Close();
                }
                return nhomTaiSan;
            }
            catch
            {
                throw;
            }
        }

        //Filter by name
        //public List<DanhMucNhomTaiSanModel> FilterByName(string TenNhomTaiSan)
        //{
        //    List<DanhMucNhomTaiSanModel> list = new List<DanhMucNhomTaiSanModel>();
        //  SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter(PARAM_TenNhomTaiSan,SqlDbType.NVarChar)
        //    };
        //    parameters[0].Value = TenNhomTaiSan;
        //    try
        //    {
        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILTERBYNAME, parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel = new DanhMucNhomTaiSanModel(Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0), Utils.ConvertToString(dr["TenNhomTaiSan"], String.Empty), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true), Utils.ConvertToInt32(dr["ThuTuSapXep"], 0));
        //                list.Add(DanhMucNhomTaiSanModel);
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

        // Get List Paging
        public List<DanhMucNhomTaiSanModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            List<DanhMucNhomTaiSanModel> list = new List<DanhMucNhomTaiSanModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int),

              };
            parameters[0].Value = p.Keyword != null ? p.Keyword.Trim() : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            //parameters[5].Value = 0;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_DanhMuc_NhomTaiSan_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucNhomTaiSanModel item = new DanhMucNhomTaiSanModel();
                        item.NhomTaiSanID = Utils.ConvertToInt32(dr["NhomTaiSanID"], 0);
                        item.NhomTaiSanChaID = Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0);
                        item.TenNhomTaiSan = Utils.ConvertToString(dr["TenNhomTaiSan"], "");
                        item.TrangThaiSuDung = Utils.ConvertToBoolean(dr["TrangThaiSuDung"], false);
                        item.ThuTuSapXep = Utils.ConvertToInt32(dr["ThuTuSapXep"], 0);
                        item.MoTa = Utils.ConvertToString(dr["MoTa"], "");
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


        /// <summary>
        /// lấy toàn bộ nhóm tài sản cha -- NhomTaiSanChaID=null
        /// </summary>
        /// <returns></returns>
        public List<DanhMucNhomTaiSanModel> GetAllNhomTaiSanCha()
        {
            List<DanhMucNhomTaiSanModel> list = new List<DanhMucNhomTaiSanModel>();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_DanhMuc_NhomTaiSan_GetAllNhomTaiSanCha"))
                {
                    while (dr.Read())
                    {

                        DanhMucNhomTaiSanModel nhomTaiSan = new DanhMucNhomTaiSanModel(Utils.ConvertToInt32(dr["NhomTaiSanID"], 0), Utils.ConvertToInt32(dr["NhomTaiSanChaID"], 0), Utils.ConvertToString(dr["TenNhomTaiSan"], String.Empty), Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToBoolean(dr["TrangThaiSuDung"], true), Utils.ConvertToInt32(dr["ThuTuSapXep"], 0), Utils.ConvertToString(dr["MoTa"], String.Empty));
                        list.Add(nhomTaiSan);
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
    }
}
