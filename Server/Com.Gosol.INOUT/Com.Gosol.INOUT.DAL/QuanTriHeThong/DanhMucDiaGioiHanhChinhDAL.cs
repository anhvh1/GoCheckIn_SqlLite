using Com.Gosol.INOUT.Ultilities;
using Com.Gosol.INOUT.Models.DanhMuc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Microsoft.Data.Sqlite;

namespace Com.Gosol.INOUT.DAL.DanhMuc
{
    public interface IDanhMucDiaGioiHanhChinhDAL
    {
        public Dictionary<int, int> InsertTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID);
        public int UpdateTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel);
        public Dictionary<int, string> DeleteTinh(int TinhID);
        public Dictionary<int, int> InsertHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID);
        public int UpdateHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel);
        public Dictionary<int, string> DeleteHuyen(int HuyenID);
        public Dictionary<int, int> InsertXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID);
        public int UpdateXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel);
        public Dictionary<int, string> DeleteXa(int XaID);
        public List<DanhMucDiaGioiHanhChinhModel> GetListByidAndCap();
        public List<DanhMucDiaGioiHanhChinhModel> FilterByName(string FilterName);
        public List<object> GetAllByCap(int ID, int Cap, string Keyword);
        public DanhMucDiaGioiHanhChinhModelUpdatePartial GetDGHCByIDAndCap(int id, int Cap, string Keyword);

    }
    public class DanhMucDiaGioiHanhChinhDAL : IDanhMucDiaGioiHanhChinhDAL
    {
        private const string INSERTTINH = @"v1_DanhMuc_DiaGioiHanhChinh_InsertTinh";
        private const string UPDATETINH = @"v1_DanhMuc_DiaGioiHanhChinh_UpdateTinh";
        private const string DELETETINH = @"v1_DanhMuc_DiaGioiHanhChinh_DeleteTinh";
        private const string INSERTHUYEN = @"v1_DanhMuc_DiaGioiHanhChinh_InsertHuyen";
        private const string UPDATEHUYEN = @"v1_DanhMuc_DiaGioiHanhChinh_UpdateHuyen";
        private const string DELETEHUYEN = @"v1_DanhMuc_DiaGioiHanhChinh_DeleteHuyen";
        private const string INSERTXA = @"v1_DanhMuc_DiaGioiHanhChinh_InsertXa";
        private const string UPDATEXA = @"v1_DanhMuc_DiaGioiHanhChinh_UpdateXa";
        private const string DELETEXA = @"v1_DanhMuc_DiaGioiHanhChinh_DeleteXa";
        private const string GETLISTBYidANDCAP = @"v1_DanhMuc_DiaGioiHanhChinh_GetListByidAndCap";
        private const string GetByID = @"v1_DanhMuc_DiaGioiHanhChinh_GetByID";
        private const string FILTERBYNAME = @"v1_DanhMuc_DiaGioiHanhChinh_FilterByName";
        private const string GETALLBYCAP = @"v1_DanhMuc_DiaGioiHanhChinh_GetAllByCap";

        // param constant

        private const string PARAM_TinhID = "@TinhID";
        private const string PARAM_TENTINH = "@TenTinh";
        private const string PARAM_TENDAYDU = "@TenDayDu";
        private const string PARAM_HuyenID = "@HuyenID";
        private const string PARAM_TENHUYEN = "@TenHuyen";
        private const string PARAM_TENXA = "@TenXa";
        private const string PARAM_XaID = "@XaID";
        private const string PARAM_MappingCode = "@MappingCode";
        //private const string PARAM_TENXA = "TenXa";
        //private const string PARAM_TENXADAYDU = "TenXaDayDu";

        //CRUD Tinh

        public Dictionary<int, int> InsertTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID)
        {
            int val = 0;
            Dictionary<int, int> dic = new Dictionary<int, int>();
            if (DanhMucDiaGioiHanhChinhModel == null)
            {
                dic.Add(0, 0);
                return dic;
            }
            var DiaGioi = GetTinhByName(DanhMucDiaGioiHanhChinhModel.TenTinh);
            if (DiaGioi.TinhID > 0)
            {
                dic.Add(0, 0);
                return dic;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
            new SqlParameter("@ID",SqlDbType.Int),
            new SqlParameter(PARAM_TENTINH,SqlDbType.NVarChar),
            new SqlParameter(PARAM_TENDAYDU,SqlDbType.NVarChar),
            new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar),
              };
            parameters[0].Value = ID;
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = DanhMucDiaGioiHanhChinhModel.TenTinh;
            parameters[2].Value = DanhMucDiaGioiHanhChinhModel.TenDayDu;
            parameters[3].Value = DanhMucDiaGioiHanhChinhModel.MappingCode ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERTTINH, parameters), 0);
                        ID = Utils.ConvertToInt32(parameters[0].Value, 0);
                        dic.Add(val, ID);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return dic;
        }

        //Update
        public int UpdateTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel)
        {
            int val = 0;
            if (DanhMucDiaGioiHanhChinhModel == null)
            {
                return val;
            }
            var DiaGioi = GetTinhByName(DanhMucDiaGioiHanhChinhModel.TenTinh);
            if (DiaGioi.TinhID != DanhMucDiaGioiHanhChinhModel.TinhID && DiaGioi.TenTinh == DanhMucDiaGioiHanhChinhModel.TenTinh)
            {
                return val;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_TinhID,SqlDbType.Int),
                new SqlParameter(PARAM_TENTINH,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TENDAYDU,SqlDbType.NVarChar),
                new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar),

              };
            parameters[0].Value = DanhMucDiaGioiHanhChinhModel.TinhID;
            parameters[1].Value = DanhMucDiaGioiHanhChinhModel.TenTinh;
            parameters[2].Value = DanhMucDiaGioiHanhChinhModel.TenDayDu;
            parameters[3].Value = DanhMucDiaGioiHanhChinhModel.MappingCode ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATETINH, parameters), 0);
                        trans.Commit();
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

        //Delete
        public Dictionary<int, string> DeleteTinh(int TinhID)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            int val = 0;
            if (TinhID <= 0)
            {
                dic.Add(0, "Chưa chọn địa giới hành chính");
                return dic;
            }
            var ListCanBoByCoQuanID = new HeThongCanBoDAL().GetAllByCoQuanID(TinhID);
            if (ListCanBoByCoQuanID.Count > 0)
            {
                dic.Add(0, "Địa giới hành chính đã được sử dụng, không thể xóa!");
                return dic;
            }
            List<DanhMucDiaGioiHanhChinhModel> DiaGioi = GetDGHCByID(TinhID);
            if (DiaGioi.Where(x=>x.HuyenID > 0 && x.TinhID == TinhID).ToList().Count > 0)
            {
                dic.Add(0, "Địa giới hành chính đã được sử dụng, không thể xóa!");
                return dic;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_TinhID,SqlDbType.Int)
              };
            parameters[0].Value = TinhID;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETETINH, parameters), 0);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            dic.Add(1, "Xóa thành công!");
            return dic;
        }

        //Get Tinh By Name
        public DanhMucDiaGioiHanhChinhModel GetTinhByName(string TenTinh)
        {
            if (string.IsNullOrEmpty(TenTinh))
            {
                return new DanhMucDiaGioiHanhChinhModel();
            }
            DanhMucDiaGioiHanhChinhModel Tinh = new DanhMucDiaGioiHanhChinhModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TenTinh",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenTinh;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_DiaGioiHanhChinh_GetTinhByName", parameters))
                {
                    while (dr.Read())
                    {
                        Tinh = new DanhMucDiaGioiHanhChinhModel();
                        Tinh.TinhID = Utils.ConvertToInt32(dr["TinhID"], 0);
                        Tinh.TenTinh = Utils.ConvertToString(dr["TenTinh"], string.Empty);
                        Tinh.TenDayDu = Utils.ConvertToString(dr["TenDayDu"], string.Empty);


                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Tinh;
        }
        // CRUD Huyen
        public Dictionary<int, int> InsertHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID)
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            int val = 0;
            if (DanhMucDiaGioiHanhChinhModel.TinhID != 0)
            {
                var Tinh = GetDGHCByIDAndCap(DanhMucDiaGioiHanhChinhModel.TinhID.Value, 1, null);
                if (Tinh == null)
                {
                    val = 0;
                    dic.Add(val, 0);
                    return dic;
                }
            }
            if (DanhMucDiaGioiHanhChinhModel == null)
            {
                dic.Add(0, 0);
                return dic;
            }
            var DiaGioi = GetHuyenByName(DanhMucDiaGioiHanhChinhModel.TenHuyen);
            if (DiaGioi.HuyenID > 0)
            {
                dic.Add(0, 0);
                return dic;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@ID",SqlDbType.Int),
                new SqlParameter(PARAM_TENHUYEN,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TENDAYDU,SqlDbType.NVarChar),
                //new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TinhID,SqlDbType.Int)
              };
            parameters[0].Value = ID;
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = DanhMucDiaGioiHanhChinhModel.TenHuyen;
            parameters[2].Value = DanhMucDiaGioiHanhChinhModel.TenDayDu;
            //parameters[3].Value = DanhMucDiaGioiHanhChinhModel.MappingCode ?? Convert.DBNull;
            parameters[3].Value = DanhMucDiaGioiHanhChinhModel.TinhID ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERTHUYEN, parameters);
                        ID = Utils.ConvertToInt32(parameters[0].Value, 0);
                        dic.Add(val, ID);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return dic;
        }
        public int UpdateHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel)
        {
            int val = 0;
            if (DanhMucDiaGioiHanhChinhModel.TinhID != 0)
            {
                var Tinh = GetDGHCByIDAndCap(DanhMucDiaGioiHanhChinhModel.TinhID.Value, 1, null);
                if (string.IsNullOrEmpty(Tinh.Ten))
                {
                    val = 0;
                    return val;
                }
            }
            //var DiaGioi = GetHuyenByName(DanhMucDiaGioiHanhChinhModel.TenHuyen.Trim());
            //if (DiaGioi.HuyenID != DanhMucDiaGioiHanhChinhModel.HuyenID && DiaGioi.TenHuyen == DanhMucDiaGioiHanhChinhModel.TenHuyen)
            //{

            //    return val;
            //}
            if (DanhMucDiaGioiHanhChinhModel == null)
            {
                return val;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_HuyenID,SqlDbType.Int),
                new SqlParameter(PARAM_TENHUYEN,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TinhID,SqlDbType.Int),
                new SqlParameter(PARAM_TENDAYDU,SqlDbType.NVarChar),
                //new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar)

              };
            parameters[0].Value = DanhMucDiaGioiHanhChinhModel.HuyenID;
            parameters[1].Value = DanhMucDiaGioiHanhChinhModel.TenHuyen;
            parameters[2].Value = DanhMucDiaGioiHanhChinhModel.TinhID ?? Convert.DBNull;
            parameters[3].Value = DanhMucDiaGioiHanhChinhModel.TenDayDu;
            //parameters[4].Value = DanhMucDiaGioiHanhChinhModel.MappingCode ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATEHUYEN, parameters), 0);
                        trans.Commit();
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
        public Dictionary<int, string> DeleteHuyen(int HuyenID)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            int val = 0;
            if (HuyenID <= 0)
            {
                dic.Add(0, "Chưa chọn huyện!");
                return dic;
            }
            var ListCanBoByCoQuanID = new HeThongCanBoDAL().GetAllByCoQuanID(HuyenID);
            if (ListCanBoByCoQuanID.Count > 0)
            {
                dic.Add(0, "Địa giới hành chính đã được sử dụng, không thể xóa!");
                return dic;
            }
            List<DanhMucDiaGioiHanhChinhModel> DiaGioi = GetDGHCByID(HuyenID);
            if (DiaGioi.Where(x => x.XaID > 0 && x.HuyenID == HuyenID).ToList().Count > 0)
            {
                dic.Add(0, "Địa giới hành chính đã được sử dụng, không thể xóa!");
                return dic;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_HuyenID,SqlDbType.Int)
              };
            parameters[0].Value = HuyenID;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETEHUYEN, parameters), 0);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            dic.Add(1, "Xóa thành công!");
            return dic;
        }
        //Get huyen by name
        public DanhMucDiaGioiHanhChinhModel GetHuyenByName(string TenHuyen)
        {
            if (string.IsNullOrEmpty(TenHuyen))
            {
                return new DanhMucDiaGioiHanhChinhModel();
            }
            DanhMucDiaGioiHanhChinhModel Huyen = new DanhMucDiaGioiHanhChinhModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TenHuyen",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenHuyen;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_DiaGioiHanhChinh_GetHuyenByName", parameters))
                {
                    while (dr.Read())
                    {
                        Huyen = new DanhMucDiaGioiHanhChinhModel(Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0)
                          , Utils.ConvertToString(dr["TenTinh"], string.Empty), Utils.ConvertToString(dr["TenHuyen"], string.Empty), Utils.ConvertToString(dr["TenXa"], string.Empty), null);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Huyen;
        }
        // CRUD Xa
        public Dictionary<int, int> InsertXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID)
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            int val = 0;
            if (DanhMucDiaGioiHanhChinhModel.HuyenID != 0)
            {
                var Tinh = GetDGHCByIDAndCap(DanhMucDiaGioiHanhChinhModel.HuyenID.Value, 2, null);
                if (Tinh == null)
                {
                    val = 2;
                    dic.Add(val, 0);
                    return dic;
                }
            }
            if (DanhMucDiaGioiHanhChinhModel == null)
            {
                dic.Add(0, 0);
                return dic;
            }
            var Huyen = new DanhMucDiaGioiHanhChinhDAL().GetDGHCByIDAndCap(DanhMucDiaGioiHanhChinhModel.HuyenID.Value, 2,null);
            if(string.IsNullOrEmpty(Huyen.Ten) || string.IsNullOrEmpty(Huyen.TenDayDu))
            {
                dic.Add(0, 0);
                return dic;
            }
            var DiaGioi = GetXaByName(DanhMucDiaGioiHanhChinhModel.TenXa);
            if (DiaGioi.XaID > 0 )
            {
                dic.Add(0, 0);
                return dic;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@ID",SqlDbType.Int),
                new SqlParameter(PARAM_TENXA,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TENDAYDU,SqlDbType.NVarChar),
                new SqlParameter(PARAM_HuyenID,SqlDbType.Int),
                new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar),
              };
            parameters[0].Value = ID;
            parameters[0].Direction = ParameterDirection.Output;
            parameters[1].Value = DanhMucDiaGioiHanhChinhModel.TenXa;
            parameters[2].Value = DanhMucDiaGioiHanhChinhModel.TenDayDu;
            parameters[3].Value = DanhMucDiaGioiHanhChinhModel.HuyenID;
            parameters[4].Value = DanhMucDiaGioiHanhChinhModel.MappingCode ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, INSERTXA, parameters);
                        ID = Utils.ConvertToInt32(parameters[0].Value, 0);
                        dic.Add(val, ID);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return dic;
        }
        public int UpdateXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel)
        {
            int val = 0;
            if (DanhMucDiaGioiHanhChinhModel.HuyenID != 0)
            {
                var Huyen = GetDGHCByIDAndCap(DanhMucDiaGioiHanhChinhModel.HuyenID.Value, 2, null);
                if (string.IsNullOrEmpty(Huyen.Ten))
                {
                    val = 0;
                    return val;
                }
            }
            //var DiaGioi = GetXaByName(DanhMucDiaGioiHanhChinhModel.TenXa);
            //if (DiaGioi.XaID != DanhMucDiaGioiHanhChinhModel.XaID && DiaGioi.TenXa == DanhMucDiaGioiHanhChinhModel.TenXa)
            //{
            //    return val;
            //}
            if (DanhMucDiaGioiHanhChinhModel == null)
            {
                return val;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_XaID,SqlDbType.Int),
                new SqlParameter(PARAM_HuyenID,SqlDbType.Int),
                new SqlParameter(PARAM_TENXA,SqlDbType.NVarChar),
                new SqlParameter(PARAM_TENDAYDU,SqlDbType.NVarChar),
                new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar),

              };
            parameters[0].Value = DanhMucDiaGioiHanhChinhModel.XaID;
            parameters[1].Value = DanhMucDiaGioiHanhChinhModel.HuyenID ?? Convert.DBNull;
            parameters[2].Value = DanhMucDiaGioiHanhChinhModel.TenXa;
            parameters[3].Value = DanhMucDiaGioiHanhChinhModel.TenDayDu ?? Convert.DBNull;
            parameters[4].Value = DanhMucDiaGioiHanhChinhModel.MappingCode ?? Convert.DBNull;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, UPDATEXA, parameters), 0);
                        trans.Commit();
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
        public Dictionary<int, string> DeleteXa(int XaID)
        {
            int val = 0;
            Dictionary<int, string> dic = new Dictionary<int, string>();
            if (XaID <= 0)
            {
                dic.Add(0, "Chưa chọn xã!");
                return dic;
            }
            var ListCanBoByCoQuanID = new HeThongCanBoDAL().GetAllByCoQuanID(XaID);
             if (ListCanBoByCoQuanID.Count > 0)
            {
                dic.Add(0, "Địa giới hành chính đã được sử dụng, không thể xóa");
                return dic;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_XaID,SqlDbType.Int)
              };
            parameters[0].Value = XaID;

            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, DELETEXA, parameters), 0);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            dic.Add(1, "Xóa thành công!");
            return dic;
        }
        public DanhMucDiaGioiHanhChinhModel GetXaByName(string TenXa)
        {
            if (string.IsNullOrEmpty(TenXa))
            {
                return new DanhMucDiaGioiHanhChinhModel();
            }
            DanhMucDiaGioiHanhChinhModel Xa = new DanhMucDiaGioiHanhChinhModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TenXa",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenXa;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_DiaGioiHanhChinh_GetXaByName", parameters))
                {
                    while (dr.Read())
                    {
                        Xa = new DanhMucDiaGioiHanhChinhModel(Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0)
                          , Utils.ConvertToString(dr["TenTinh"], string.Empty), Utils.ConvertToString(dr["TenHuyen"], string.Empty), Utils.ConvertToString(dr["TenXa"], string.Empty), null);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Xa;
        }

        //Get List By id And Cap
        public List<DanhMucDiaGioiHanhChinhModel> GetListByidAndCap()
        {
            List<DanhMucDiaGioiHanhChinhModel> list = new List<DanhMucDiaGioiHanhChinhModel>();
            // SqlParameter[] parameters = new SqlParameter[]
            //{
            //     new SqlParameter(PARAM_TinhID,SqlDbType.Int)
            //};
            // parameters[0].Value = TinhID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GETLISTBYidANDCAP))
                {
                    while (dr.Read())
                    {
                        DanhMucDiaGioiHanhChinhModel diaGioiHanhChinh = new DanhMucDiaGioiHanhChinhModel(Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0),
                            Utils.ConvertToInt32(dr["XaID"], 0), Utils.ConvertToString(dr["TenTinh"], string.Empty), Utils.ConvertToString(dr["TenHuyen"], string.Empty), Utils.ConvertToString(dr["TenXa"], string.Empty), null);
                        list.Add(diaGioiHanhChinh);
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

        // GetByID
        public List<DanhMucDiaGioiHanhChinhModel> GetDGHCByID(int? id)
        {
            if (id <= 0 || id == null)
            {
                return new List<DanhMucDiaGioiHanhChinhModel>();
            }
            List<DanhMucDiaGioiHanhChinhModel> list = new List<DanhMucDiaGioiHanhChinhModel>();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@ID",SqlDbType.Int)
             };


            parameters[0].Value = id;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GetByID, parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel = new DanhMucDiaGioiHanhChinhModel();
                        DanhMucDiaGioiHanhChinhModel.TinhID = Utils.ConvertToInt32(dr["TinhID"], 0);
                        DanhMucDiaGioiHanhChinhModel.HuyenID = Utils.ConvertToInt32(dr["HuyenID"], 0);
                        DanhMucDiaGioiHanhChinhModel.XaID = Utils.ConvertToInt32(dr["XaID"], 0);
                        DanhMucDiaGioiHanhChinhModel.TenTinh = Utils.ConvertToString(dr["TenTinh"], string.Empty);
                        DanhMucDiaGioiHanhChinhModel.TenHuyen = Utils.ConvertToString(dr["TenHuyen"], string.Empty);
                        DanhMucDiaGioiHanhChinhModel.TenXa = Utils.ConvertToString(dr["TenXa"], string.Empty);
                        list.Add(DanhMucDiaGioiHanhChinhModel);
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

        // Get By id and cap
        public DanhMucDiaGioiHanhChinhModelUpdatePartial GetDGHCByIDAndCap(int id, int Cap, string Keyword)
        {
            if (id <= 0)
            {
                return new DanhMucDiaGioiHanhChinhModelUpdatePartial();
            }
            DanhMucDiaGioiHanhChinhModelUpdatePartial DiaGioi = new DanhMucDiaGioiHanhChinhModelUpdatePartial();
            SqlParameter[] parameters = new SqlParameter[]
             {
                new SqlParameter("@ID",SqlDbType.Int),

                new SqlParameter("@Cap",SqlDbType.Int),
                new SqlParameter("@Keyword",SqlDbType.NVarChar)
             };


            parameters[0].Value = id;
            parameters[1].Value = Cap;
            parameters[2].Value = Keyword == null ? "" : Keyword.Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_DiaGioiHanhChinh_GetByIDAndCap", parameters))
                {
                    while (dr.Read())
                    {
                        DiaGioi = new DanhMucDiaGioiHanhChinhModelUpdatePartial(Utils.ConvertToInt32(dr["ID"], 0), Utils.ConvertToString(dr["Ten"], string.Empty), Utils.ConvertToString(dr["TenDayDu"], string.Empty), Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Cap, Utils.ConvertToInt32(dr["TotalChildren"], 0), Utils.ConvertToInt32(dr["Highlight"], 0));
                        break;

                    }

                    dr.Close();
                }

            }
            catch
            {
                throw;
            }
            return DiaGioi;

        }
        //// GetByHuyenID
        //public List<DanhMucDiaGioiHanhChinhModel> GetByHuyenID(int HuyenID)
        //{
        //    List<DanhMucDiaGioiHanhChinhModel> list = new List<DanhMucDiaGioiHanhChinhModel>();
        //  SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter(PARAM_HuyenID,SqlDbType.NVarChar)
        //    };
        //    parameters[0].Value = HuyenID;
        //    try
        //    {

        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GetByID, parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                DanhMucDiaGioiHanhChinhModel diaGioiHanhChinh = new DanhMucDiaGioiHanhChinhModel(0, Utils.ConvertToInt32(dr["HuyenID"], 0), 0
        //                    , null, Utils.ConvertToString(dr["TenHuyen"], string.Empty), null, Utils.ConvertToString(dr["MappingCode"], string.Empty));
        //                list.Add(diaGioiHanhChinh);
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

        ////// GetByIDXaID
        //public List<DanhMucDiaGioiHanhChinhModel> GetByXaID(int XaID)
        //{
        //    List<DanhMucDiaGioiHanhChinhModel> list = new List<DanhMucDiaGioiHanhChinhModel>();
        //  SqlParameter[] parameters = new SqlParameter[]
        //   {
        //        new SqlParameter(PARAM_XaID,SqlDbType.NVarChar)
        //   };
        //    parameters[0].Value = XaID;
        //    try
        //    {

        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, GetByID, parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                DanhMucDiaGioiHanhChinhModel diaGioiHanhChinh = new DanhMucDiaGioiHanhChinhModel(0, 0
        //                   , Utils.ConvertToInt32(dr["XaID"], 0), null, null, Utils.ConvertToString(dr["TenXa"], string.Empty), Utils.ConvertToString(dr["MappingCode"], string.Empty));
        //                list.Add(diaGioiHanhChinh);
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


        // Filter By Name 
        public List<DanhMucDiaGioiHanhChinhModel> FilterByName(string FilterName)
        {
            if (string.IsNullOrEmpty(FilterName))
            {
                return new List<DanhMucDiaGioiHanhChinhModel>();
            }
            List<DanhMucDiaGioiHanhChinhModel> list = new List<DanhMucDiaGioiHanhChinhModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter( "@FilterName",SqlDbType.NVarChar)
              };
            parameters[0].Value = FilterName;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILTERBYNAME, parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucDiaGioiHanhChinhModel diaGioiHanhChinh = new DanhMucDiaGioiHanhChinhModel(Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0), Utils.ConvertToString(dr["TenTinh"], string.Empty), Utils.ConvertToString(dr["TenHuyen"], string.Empty), Utils.ConvertToString(dr["TenXa"], string.Empty), null);
                        list.Add(diaGioiHanhChinh);
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


        public List<object> GetAllByCap(int ID, int Cap, string Keyword)
        {
            List<object> list = new List<object>();
            SqliteParameter[] parameters = new SqliteParameter[]
              {
                new SqliteParameter("@ID",SqlDbType.Int),
                new SqliteParameter("@Cap",SqlDbType.Int),
                new SqliteParameter("@Keyword",DbType.String)

              };
            parameters[0].Value = ID;
            parameters[1].Value = Cap;
            parameters[2].Value = Keyword == null ? "" : Keyword;

            try
            {

                using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, QueryManager.Get("v1_DanhMuc_DiaGioiHanhChinh_GetAllByCap") , parameters))
                {
                    while (dr.Read())
                    {

                        //object[] obj = new object[] { Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToString(dr["TenTinh"], "") };
                        //var json = JsonConvert.SerializeObject(new { ID = Utils.ConvertToInt32(dr["ID"], 0), Ten = Utils.ConvertToString(dr["Ten"], String.Empty) });
                        //object objnew = JsonConvert.DeserializeObject<object>(json);
                        var DiaGioiHanhChinh = new DanhMucDiaGioiHanhChinhModelPartial(Utils.ConvertToInt32(dr["ID"], 0), Utils.ConvertToString(dr["Ten"], string.Empty), Utils.ConvertToString(dr["TenDayDu"], string.Empty), Utils.ConvertToInt32(dr["TotalChildren"], 0), Cap, Utils.ConvertToInt32(dr["Highlight"], 0));
                        list.Add(DiaGioiHanhChinh);
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
    }
}
