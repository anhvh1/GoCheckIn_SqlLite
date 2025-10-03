using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Ultilities;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System.Linq;
using System.Data.SqlClient;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.Models;
using System.IO;
using OfficeOpenXml;
using Microsoft.Data.Sqlite;


namespace Com.Gosol.INOUT.DAL.DanhMuc
{
    public interface IDanhMucCoQuanDonViDAL
    {
        public int Insert(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref int CoQuanID, int NguoiDungID, ref string Message);
        public int Update(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref string Message);
        public Dictionary<int, string> Delete(List<int> ListCoQuanID);
        public List<DanhMucCoQuanDonViModel> FilterByName(string TenCoQuan);
        public DanhMucCoQuanDonViPartialNew GetByID(int? CoQuanID);
        public List<DanhMucCoQuanDonViModel> GetListByidAndCap();
        public List<DanhMucCoQuanDonViModelPartial> GetAllByCap(int ID, int Cap, string Keyword);
        public List<DanhMucCoQuanDonViModelPartial> GetALL(int ID, int CapCoQuanID, string Keyword);
        public List<DanhMucCoQuanDonViModel> GetListByUser(int CoQuanID, int NguoiDungID);
        public int ImportFile(string FilePath, ref string Message, int NguoiDungID);
        public List<DanhMucCoQuanDonViModel> GetListByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID);
        public List<DanhMucCoQuanDonViModel> GetByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID, string KeyWord);
        public BaseResultModel CheckMaCQ(int? CoQuanID, string MaCQ);
        public DanhMucCoQuanDonViModel GetCoQuanSuDungPhanMem_By_CoQuanDangNhap(int? CoQuanID);
    }
    public class DanhMucCoQuanDonViDAL : IDanhMucCoQuanDonViDAL
    {

        //private const string GETLISTBYidANDCAP = @"v1_DanhMuc_CoQuanDonVi_AddNhomTaiSan";
        private const string FILTERBYNAME = @"v1_DanhMuc_CoQuanDonVi_FilterByName";

        // param constant
        private const string PARAM_CoQuanID = "@CoQuanID";
        private const string PARAM_TenCoQuan = "@TenCoQuan";
        private const string PARAM_CoQuanChaID = "@CoQuanChaID";
        private const string PARAM_CapID = "@CapID";
        private const string PARAM_ThamQuyenID = "@ThamQuyenID";
        private const string PARAM_TinhID = "@TinhID";
        private const string PARAM_HuyenID = "@HuyenID";
        private const string PARAM_XaID = "@XaID";
        private const string PARAM_CapUBND = "@CapUBND";
        private const string PARAM_CapThanhTra = "@CapThanhTra";
        private const string PARAM_CQCoHieuLuc = "@CQCoHieuLuc";
        private const string PARAM_SuDungPM = "@SuDungPM";
        private const string PARAM_MaCQ = "@MaCQ";
        private const string PARAM_SuDungQuyTrinh = "@SuDungQuyTrinh";
        private const string PARAM_MappingCode = "@MappingCode";
        private const string PARAM_IsDisable = "@IsDisable";
        private const string PARAM_TTChiaTachSapNhap = "@TTChiaTachSapNhap";
        private const string PARAM_ChiaTachSapNhapDenCQID = "@ChiaTachSapNhapDenCQID";
        private const string PARAM_IsStatus = "@IsStatus";
        //private const string PARAM_ID = @"ChiaTachSapNhapCQid";

        // Insert
        public int Insert(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref int CoQuanID, int NguoiDungID, ref string Message)
        {
            int val = 0;

            if (DanhMucCoQuanDonViModel.TenCoQuan.Trim().Length > 100)
            {
                Message = ConstantLogMessage.API_Error_TooLong;
                return val;
            }
            if (string.IsNullOrEmpty(DanhMucCoQuanDonViModel.TenCoQuan) || DanhMucCoQuanDonViModel.TenCoQuan.Trim().Length <= 0)
            {
                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }

            else
            {
                SqlParameter[] parameters = new SqlParameter[]
                  {
                        new SqlParameter(PARAM_TenCoQuan,SqlDbType.NVarChar),
                        new SqlParameter(PARAM_CoQuanChaID,SqlDbType.Int),
                        new SqlParameter(PARAM_CapID,SqlDbType.Int),
                        new SqlParameter(PARAM_ThamQuyenID,SqlDbType.Int),
                        new SqlParameter(PARAM_TinhID,SqlDbType.Int),
                        new SqlParameter(PARAM_HuyenID,SqlDbType.Int),
                        new SqlParameter(PARAM_XaID,SqlDbType.Int),
                        new SqlParameter(PARAM_CapUBND,SqlDbType.Bit),
                        new SqlParameter(PARAM_CapThanhTra,SqlDbType.Bit),
                        new SqlParameter(PARAM_CQCoHieuLuc,SqlDbType.Bit),
                        new SqlParameter(PARAM_SuDungPM,SqlDbType.Bit),
                        new SqlParameter(PARAM_MaCQ,SqlDbType.NVarChar),
                        new SqlParameter(PARAM_SuDungQuyTrinh,SqlDbType.Bit),
                        new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar),
                        new SqlParameter(PARAM_IsDisable,SqlDbType.Bit),
                        new SqlParameter(PARAM_TTChiaTachSapNhap,SqlDbType.Int),
                        new SqlParameter(PARAM_ChiaTachSapNhapDenCQID,SqlDbType.Int),
                        new SqlParameter(PARAM_CoQuanID,SqlDbType.Int),
                          new SqlParameter(PARAM_IsStatus,SqlDbType.Bit),
                          new SqlParameter("CoQuanSuDungPhanMenID",SqlDbType.Int)
                  };
                parameters[0].Value = DanhMucCoQuanDonViModel.TenCoQuan.Trim();
                parameters[1].Value = DanhMucCoQuanDonViModel.CoQuanChaID ?? Convert.DBNull;
                parameters[2].Value = DanhMucCoQuanDonViModel.CapID ?? Convert.DBNull;
                parameters[3].Value = DanhMucCoQuanDonViModel.ThamQuyenID ?? Convert.DBNull;
                parameters[4].Value = DanhMucCoQuanDonViModel.TinhID ?? Convert.DBNull;
                parameters[5].Value = DanhMucCoQuanDonViModel.HuyenID ?? Convert.DBNull;
                parameters[6].Value = DanhMucCoQuanDonViModel.XaID ?? Convert.DBNull;
                parameters[7].Value = DanhMucCoQuanDonViModel.CapUBND ?? Convert.DBNull;
                parameters[8].Value = DanhMucCoQuanDonViModel.CapThanhTra ?? Convert.DBNull;
                parameters[9].Value = DanhMucCoQuanDonViModel.CQCoHieuLuc ?? Convert.DBNull;
                parameters[10].Value = DanhMucCoQuanDonViModel.SuDungPM ?? Convert.DBNull;
                parameters[11].Value = DanhMucCoQuanDonViModel.MaCQ ?? Convert.DBNull;
                parameters[12].Value = DanhMucCoQuanDonViModel.SuDungQuyTrinh ?? Convert.DBNull;
                parameters[13].Value = DanhMucCoQuanDonViModel.MappingCode ?? Convert.DBNull;
                parameters[14].Value = DanhMucCoQuanDonViModel.IsDisable ?? Convert.DBNull;
                parameters[15].Value = DanhMucCoQuanDonViModel.TTChiaTachSapNhap ?? Convert.DBNull;
                parameters[16].Value = DanhMucCoQuanDonViModel.ChiaTachSapNhapDenCQID ?? Convert.DBNull;
                parameters[17].Value = CoQuanID;
                parameters[17].Direction = ParameterDirection.Output;
                parameters[18].Value = DanhMucCoQuanDonViModel.IsStatus ?? Convert.DBNull;
                parameters[19].Value = DanhMucCoQuanDonViModel.CoQuanSuDungPhanMenID ?? Convert.DBNull;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            val = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_Insert", parameters);
                            CoQuanID = Utils.ConvertToInt32(parameters[17].Value, 0);
                            trans.Commit();

                            // Tự động insert trạng thái cho cơ quan mới tạo
                            var query = new PhanQuyenDAL().NhomNguoiDung_IsertNhomChoCoQuanMoi(CoQuanID, DanhMucCoQuanDonViModel.CoQuanChaID, NguoiDungID);
                            if (query.Status < 1)
                            {
                                return query.Status;
                            }
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }

                    }
                }
            }
            Message = ConstantLogMessage.Alert_Insert_Success("cơ quan");
            return CoQuanID;
        }
        // Update
        public int Update(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel, ref string Message)
        {
            int val = 0;
            if (DanhMucCoQuanDonViModel.CoQuanID <= 0)
            {
                Message = "Bạn chưa chọn cơ quan!";
                return val;
            }
            if (DanhMucCoQuanDonViModel.TenCoQuan.Trim().Length > 10)
            {
                Message = ConstantLogMessage.API_Error_TooLong;
                return val;
            }
            if (string.IsNullOrEmpty(DanhMucCoQuanDonViModel.TenCoQuan) || DanhMucCoQuanDonViModel.TenCoQuan.Trim().Length <= 0)
            {
                Message = ConstantLogMessage.API_Error_NotFill;
                return val;
            }
            var coquan = GetByName(DanhMucCoQuanDonViModel.TenCoQuan);
            if (coquan.CoQuanID > 0 && coquan.CoQuanID != DanhMucCoQuanDonViModel.CoQuanID)
            {
                Message = ConstantLogMessage.Alert_Error_Exist("Tên cơ quan");
                return val;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                        new SqlParameter(PARAM_CoQuanID,SqlDbType.Int),
                        new SqlParameter(PARAM_TenCoQuan,SqlDbType.NVarChar),
                        new SqlParameter(PARAM_CoQuanChaID,SqlDbType.Int),
                        new SqlParameter(PARAM_CapID,SqlDbType.Int),
                        new SqlParameter(PARAM_ThamQuyenID,SqlDbType.Int),
                        new SqlParameter(PARAM_TinhID,SqlDbType.Int),
                        new SqlParameter(PARAM_HuyenID,SqlDbType.Int),
                        new SqlParameter(PARAM_XaID,SqlDbType.Int),
                        new SqlParameter(PARAM_CapUBND,SqlDbType.Bit),
                        new SqlParameter(PARAM_CapThanhTra,SqlDbType.Bit),
                        new SqlParameter(PARAM_CQCoHieuLuc,SqlDbType.Bit),
                        new SqlParameter(PARAM_SuDungPM,SqlDbType.Bit),
                        new SqlParameter(PARAM_MaCQ,SqlDbType.NVarChar),
                        new SqlParameter(PARAM_SuDungQuyTrinh,SqlDbType.Bit),
                        new SqlParameter(PARAM_MappingCode,SqlDbType.NVarChar),
                        new SqlParameter(PARAM_IsDisable,SqlDbType.Bit),
                        new SqlParameter(PARAM_TTChiaTachSapNhap,SqlDbType.Int),
                        new SqlParameter(PARAM_ChiaTachSapNhapDenCQID,SqlDbType.Int),
                        new SqlParameter(PARAM_IsStatus,SqlDbType.Bit)
              };
            parameters[0].Value = DanhMucCoQuanDonViModel.CoQuanID;
            parameters[1].Value = DanhMucCoQuanDonViModel.TenCoQuan.Trim();
            parameters[2].Value = DanhMucCoQuanDonViModel.CoQuanChaID ?? Convert.DBNull;
            parameters[3].Value = DanhMucCoQuanDonViModel.CapID ?? Convert.DBNull;
            parameters[4].Value = DanhMucCoQuanDonViModel.ThamQuyenID ?? Convert.DBNull;
            parameters[5].Value = DanhMucCoQuanDonViModel.TinhID ?? Convert.DBNull;
            parameters[6].Value = DanhMucCoQuanDonViModel.HuyenID ?? Convert.DBNull;
            parameters[7].Value = DanhMucCoQuanDonViModel.XaID ?? Convert.DBNull;
            parameters[8].Value = DanhMucCoQuanDonViModel.CapUBND ?? Convert.DBNull;
            parameters[9].Value = DanhMucCoQuanDonViModel.CapThanhTra ?? Convert.DBNull;
            parameters[10].Value = DanhMucCoQuanDonViModel.CQCoHieuLuc ?? Convert.DBNull;
            parameters[11].Value = DanhMucCoQuanDonViModel.SuDungPM ?? Convert.DBNull;
            parameters[12].Value = DanhMucCoQuanDonViModel.MaCQ ?? Convert.DBNull;
            parameters[13].Value = DanhMucCoQuanDonViModel.SuDungQuyTrinh ?? Convert.DBNull;
            parameters[14].Value = DanhMucCoQuanDonViModel.MappingCode ?? Convert.DBNull;
            parameters[15].Value = DanhMucCoQuanDonViModel.IsDisable ?? Convert.DBNull;
            parameters[16].Value = DanhMucCoQuanDonViModel.TTChiaTachSapNhap ?? Convert.DBNull;
            parameters[17].Value = DanhMucCoQuanDonViModel.ChiaTachSapNhapDenCQID ?? Convert.DBNull;
            parameters[18].Value = DanhMucCoQuanDonViModel.IsStatus ?? Convert.DBNull;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_Update", parameters);
                        trans.Commit();
                    }
                    catch
                    {
                        throw;
                    }
                    Message = ConstantLogMessage.Alert_Update_Success("cơ quan");
                    return val;
                }
            }
        }

        // Delete
        public Dictionary<int, string> Delete(List<int> ListCoQuanID)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            string message = "";

            if (ListCoQuanID.Count <= 0)
            {
                message = "Bạn chưa chọn cơ quan!";
                dic.Add(0, message);
                return dic;
            }
            else
            {
                for (int i = 0; i < ListCoQuanID.Count; i++)
                {
                    if (checkCoQuanCon(ListCoQuanID[i]))
                    {
                        dic.Add(0, "Cơ quan đang chứa cơ quan con. không thể xóa!");
                        return dic;
                    }
                    if (checkCanBoTrongCoQuan(ListCoQuanID[i]))
                    {
                        dic.Add(0, "Cơ quan đang đã có cán bộ. không thể xóa!");
                        return dic;
                    }
                    if (checkNhiemVuByCoQuan(ListCoQuanID[i]))
                    {
                        dic.Add(0, "Cơ quan đang đã có nhiệm vụ. không thể xóa!");
                        return dic;
                    }
                    int val = 0;
                    //List<HeThongCanBoModel> CanBo = new HeThongCanBoDAL().GetAll();
                    //List<DanhMucCoQuanDonViModel> CoQuan = new DanhMucCoQuanDonViDAL().GetAll();
                    //var Coquanbyparentid = CoQuan.Where(x => x.CoQuanChaID == ListCoQuanID[i]).Count();
                    //if (Coquanbyparentid > 0)
                    //{
                    //    dic.Add(0, "Cơ quan đang chứa cơ quan con! không thể xóa!");
                    //    return dic;
                    //}
                    //var i = from c in CanBo where c.CoQuanID = ListCoQuanID[i] select c)
                    //var query = CanBo.Where(x => x.CoQuanID == ListCoQuanID[i]).Count(); 
                    if (GetByID(ListCoQuanID[i]) == null)
                    {
                        message = ConstantLogMessage.API_Error_NotSelected;
                        dic.Add(0, message);
                        return dic;
                    }
                    //else if (query > 0)
                    //{
                    //    dic.Add(0, "Không thể xóa Cơ Quan!");
                    //    return dic;
                    //}
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                              new SqlParameter(@"CoQuanID", SqlDbType.Int)
                         };
                        parameters[0].Value = ListCoQuanID[i];
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_Delete", parameters);
                                    trans.Commit();
                                    if (val == 0)
                                    {
                                        message = "Không thể xóa từ Cơ Quan thứ " + ListCoQuanID[i];
                                        dic.Add(0, message);
                                        return dic;
                                    }
                                    //message = ConstantLogMessage.API_Delete_Success;
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
                message = ConstantLogMessage.API_Delete_Success;
                dic.Add(1, message);
                return dic;
            }

        }

        //Filter By Name
        public List<DanhMucCoQuanDonViModel> FilterByName(string TenCoQuan)
        {
            List<DanhMucCoQuanDonViModel> list = new List<DanhMucCoQuanDonViModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_TenCoQuan,SqlDbType.NVarChar)
              };
            parameters[0].Value = TenCoQuan;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, FILTERBYNAME, parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucCoQuanDonViModel coQuanDonVi = new DanhMucCoQuanDonViModel(Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToString(dr["TenCoQuan"], string.Empty), Utils.ConvertToInt32(dr["CoQuanChaID"], 0), Utils.ConvertToInt32(dr["CapID"], 0), Utils.ConvertToInt32(dr["ThamQuyenID"], 0), Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0), Utils.ConvertToBoolean(dr["CapUBND"], true), Utils.ConvertToBoolean(dr["CapThanhTra"], true), Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false), Utils.ConvertToBoolean(dr["SuDungPM"], false), Utils.ConvertToString(dr["MaCQ"], string.Empty), Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false), Utils.ConvertToString(dr["MappingCode"], string.Empty), Utils.ConvertToBoolean(dr["IsDisable"], false), Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0), Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0), Utils.ConvertToBoolean(dr["IsStatus"], false));
                        list.Add(coQuanDonVi);
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

        // Get By id
        public DanhMucCoQuanDonViPartialNew GetByID(int? CoQuanID)
        {
            DanhMucCoQuanDonViPartialNew coQuanDonVi = new DanhMucCoQuanDonViPartialNew();
            SqliteParameter[] parameters = new SqliteParameter[]
              {
                new SqliteParameter(PARAM_CoQuanID,SqlDbType.Int)
              };
            parameters[0].Value = CoQuanID ?? Convert.DBNull;
            try
            {

                using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, QueryManager.Get("v1_DanhMuc_CoQuanDonVi_GetByID"), parameters))
                {
                    while (dr.Read())
                    {
                        //Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToString(dr["TenCoQuan"], string.Empty), Utils.ConvertToInt32(dr["CoQuanChaID"], 0), Utils.ConvertToInt32(dr["CapID"], 0), Utils.ConvertToInt32(dr["ThamQuyenID"], 0), Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0), Utils.ConvertToBoolean(dr["CapUBND"], true), Utils.ConvertToBoolean(dr["CapThanhTra"], true), Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false), Utils.ConvertToBoolean(dr["SuDungPM"], true), Utils.ConvertToString(dr["MaCQ"], string.Empty), Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false), Utils.ConvertToString(dr["MappingCode"], string.Empty), Utils.ConvertToBoolean(dr["IsDisable"], false), Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0), Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0), Utils.ConvertToBoolean(dr["IsStatus"], false)
                        coQuanDonVi = new DanhMucCoQuanDonViPartialNew();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        coQuanDonVi.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        coQuanDonVi.ThamQuyenID = Utils.ConvertToInt32(dr["ThamQuyenID"], 0);
                        coQuanDonVi.TinhID = Utils.ConvertToInt32(dr["TinhID"], 0);
                        coQuanDonVi.HuyenID = Utils.ConvertToInt32(dr["HuyenID"], 0);
                        coQuanDonVi.XaID = Utils.ConvertToInt32(dr["XaID"], 0);
                        coQuanDonVi.CapUBND = Utils.ConvertToBoolean(dr["CapUBND"], true);
                        coQuanDonVi.CapThanhTra = Utils.ConvertToBoolean(dr["CapThanhTra"], true);
                        coQuanDonVi.CQCoHieuLuc = Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false);
                        coQuanDonVi.SuDungPM = Utils.ConvertToBoolean(dr["SuDungPM"], false);
                        coQuanDonVi.MaCQ = Utils.ConvertToString(dr["MaCQ"], string.Empty);
                        coQuanDonVi.SuDungQuyTrinh = Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false);
                        coQuanDonVi.MappingCode = Utils.ConvertToString(dr["MappingCode"], string.Empty);
                        coQuanDonVi.IsDisable = Utils.ConvertToBoolean(dr["IsDisable"], false);
                        coQuanDonVi.TTChiaTachSapNhap = Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0);
                        coQuanDonVi.ChiaTachSapNhapDenCQID = Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0);
                        coQuanDonVi.IsStatus = Utils.ConvertToBoolean(dr["IsStatus"], false);
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        coQuanDonVi.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                        if (coQuanDonVi.XaID == 0)
                        {
                            coQuanDonVi.DiaChi = "";
                        }
                        else
                        {
                            //var CoQuanByID = new DanhMucDiaGioiHanhChinhDAL().GetDGHCByID(Utils.ConvertToInt32(dr["XaID"], 0)).ToList().Where(x => x.XaID == Utils.ConvertToInt32(dr["XaID"], 0));
                            //coQuanDonVi.TenCoQuanCha = GetAll().Where(x => x.CoQuanChaID == coQuanDonVi.CoQuanChaID).ToList().FirstOrDefault().TenCoQuan.ToString();
                            //coQuanDonVi.DiaChi = string.Concat(CoQuanByID.FirstOrDefault().TenXa, "-", CoQuanByID.FirstOrDefault().TenHuyen, "-", CoQuanByID.FirstOrDefault().TenTinh).ToString();
                        }
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return coQuanDonVi;
        }
        //Get by Mã CQ
        public DanhMucCoQuanDonViPartialNew GetByMCQ(string MaCQ)
        {
            DanhMucCoQuanDonViPartialNew coQuanDonVi = new DanhMucCoQuanDonViPartialNew();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@MaCQ",SqlDbType.NVarChar)
              };
            parameters[0].Value = MaCQ ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetByMaCQ", parameters))
                {
                    while (dr.Read())
                    {
                        coQuanDonVi = new DanhMucCoQuanDonViPartialNew();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        coQuanDonVi.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        coQuanDonVi.ThamQuyenID = Utils.ConvertToInt32(dr["ThamQuyenID"], 0);
                        coQuanDonVi.TinhID = Utils.ConvertToInt32(dr["TinhID"], 0);
                        coQuanDonVi.HuyenID = Utils.ConvertToInt32(dr["HuyenID"], 0);
                        coQuanDonVi.XaID = Utils.ConvertToInt32(dr["XaID"], 0);
                        coQuanDonVi.CapUBND = Utils.ConvertToBoolean(dr["CapUBND"], true);
                        coQuanDonVi.CapThanhTra = Utils.ConvertToBoolean(dr["CapThanhTra"], true);
                        coQuanDonVi.CQCoHieuLuc = Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false);
                        coQuanDonVi.SuDungPM = Utils.ConvertToBoolean(dr["SuDungPM"], false);
                        coQuanDonVi.MaCQ = Utils.ConvertToString(dr["MaCQ"], string.Empty);
                        coQuanDonVi.SuDungQuyTrinh = Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false);
                        coQuanDonVi.MappingCode = Utils.ConvertToString(dr["MappingCode"], string.Empty);
                        coQuanDonVi.IsDisable = Utils.ConvertToBoolean(dr["IsDisable"], false);
                        coQuanDonVi.TTChiaTachSapNhap = Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0);
                        coQuanDonVi.ChiaTachSapNhapDenCQID = Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0);
                        coQuanDonVi.IsStatus = Utils.ConvertToBoolean(dr["IsStatus"], false);
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        //coQuanDonVi.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return coQuanDonVi;
        }
        public DanhMucCoQuanDonViModel GetByID1(int CoQuanID)
        {
            DanhMucCoQuanDonViModel coQuanDonVi = new DanhMucCoQuanDonViModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_CoQuanID,SqlDbType.Int)
              };
            parameters[0].Value = CoQuanID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        //Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToString(dr["TenCoQuan"], string.Empty), Utils.ConvertToInt32(dr["CoQuanChaID"], 0), Utils.ConvertToInt32(dr["CapID"], 0), Utils.ConvertToInt32(dr["ThamQuyenID"], 0), Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0), Utils.ConvertToBoolean(dr["CapUBND"], true), Utils.ConvertToBoolean(dr["CapThanhTra"], true), Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false), Utils.ConvertToBoolean(dr["SuDungPM"], true), Utils.ConvertToString(dr["MaCQ"], string.Empty), Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false), Utils.ConvertToString(dr["MappingCode"], string.Empty), Utils.ConvertToBoolean(dr["IsDisable"], false), Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0), Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0), Utils.ConvertToBoolean(dr["IsStatus"], false)
                        coQuanDonVi = new DanhMucCoQuanDonViModel();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        var coQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        if (coQuanChaID == 0) coQuanDonVi.CoQuanChaID = null;
                        else coQuanDonVi.CoQuanChaID = coQuanChaID;
                        //coQuanDonVi.CoQuanChaID = coQuanChaID <1 ? null : coQuanChaID;
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        coQuanDonVi.ID = coQuanDonVi.CoQuanID;
                        coQuanDonVi.Ten = coQuanDonVi.TenCoQuan;
                        coQuanDonVi.Cap = coQuanDonVi.CapID;
                        coQuanDonVi.ParentID = coQuanDonVi.CoQuanChaID;
                        coQuanDonVi.CQCoHieuLuc = Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return coQuanDonVi;
        }

        public DanhMucCoQuanDonViModel GetByName(string TenCoQuan)
        {
            DanhMucCoQuanDonViModel coQuanDonVi = new DanhMucCoQuanDonViModel();
            if (string.IsNullOrEmpty(TenCoQuan))
            {
                return new DanhMucCoQuanDonViModel();
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@TenCoQuan",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenCoQuan;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetByName", parameters))
                {
                    while (dr.Read())
                    {
                        coQuanDonVi = new DanhMucCoQuanDonViModel(Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToString(dr["TenCoQuan"], string.Empty), Utils.ConvertToInt32(dr["CoQuanChaID"], 0), Utils.ConvertToInt32(dr["CapID"], 0), Utils.ConvertToInt32(dr["ThamQuyenID"], 0), Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0), Utils.ConvertToBoolean(dr["CapUBND"], true), Utils.ConvertToBoolean(dr["CapThanhTra"], true), Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false), Utils.ConvertToBoolean(dr["SuDungPM"], false), Utils.ConvertToString(dr["MaCQ"], string.Empty), Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false), Utils.ConvertToString(dr["MappingCode"], string.Empty), Utils.ConvertToBoolean(dr["IsDisable"], false), Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0), Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0), Utils.ConvertToBoolean(dr["IsStatus"], false));
                        break;

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return coQuanDonVi;
        }

        // Get list by id and Cap
        public List<DanhMucCoQuanDonViModel> GetListByidAndCap()
        {
            List<DanhMucCoQuanDonViModel> list = new List<DanhMucCoQuanDonViModel>();
            //SqlParameter[] parameters = new SqlParameter[]
            //{
            //    new SqlParameter(PARAM_CoQuanID,SqlDbType.Int)
            //};
            //parameters[0].Value = CoQuanID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetListByid"))
                {
                    while (dr.Read())
                    {
                        DanhMucCoQuanDonViModel coQuanDonVi = new DanhMucCoQuanDonViModel(Utils.ConvertToInt32(dr["CoQuanID"], 0), Utils.ConvertToString(dr["TenCoQuan"], string.Empty), Utils.ConvertToInt32(dr["CoQuanChaID"], 0), Utils.ConvertToInt32(dr["CapID"], 0), Utils.ConvertToInt32(dr["ThamQuyenID"], 0), Utils.ConvertToInt32(dr["TinhID"], 0), Utils.ConvertToInt32(dr["HuyenID"], 0), Utils.ConvertToInt32(dr["XaID"], 0), Utils.ConvertToBoolean(dr["CapUBND"], true), Utils.ConvertToBoolean(dr["CapThanhTra"], true), Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false), Utils.ConvertToBoolean(dr["SuDungPM"], false), Utils.ConvertToString(dr["MaCQ"], string.Empty), Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false), Utils.ConvertToString(dr["MappingCode"], string.Empty), Utils.ConvertToBoolean(dr["IsDisable"], false), Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0), Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0), Utils.ConvertToBoolean(dr["IsStatus"], false));
                        list.Add(coQuanDonVi);
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
        // GetAllByCap
        public List<DanhMucCoQuanDonViModelPartial> GetAllByCap(int ID, int Cap, string Keyword)
        {
            try
            {
                if (!string.IsNullOrEmpty(Keyword))
                {
                    Keyword = Keyword.Trim();
                }
                List<DanhMucCoQuanDonViModelPartial> List = new List<DanhMucCoQuanDonViModelPartial>();
                //List<DanhMucCoQuanDonViModelPartial> List1 = new List<DanhMucCoQuanDonViModelPartial>();
                List<object> new_List = new List<object>();
                SqlParameter[] parameters = new SqlParameter[]
                  {

                    new SqlParameter("@Keyword",SqlDbType.NVarChar)
                  };
                parameters[0].Value = Keyword ?? Convert.DBNull;
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetAllByCap", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucCoQuanDonViModelPartial CoQuanDonVi = new DanhMucCoQuanDonViModelPartial();
                        CoQuanDonVi.ID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        CoQuanDonVi.Ten = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        CoQuanDonVi.MaCQ = Utils.ConvertToInt32(dr["MaCQ"], 0);
                        CoQuanDonVi.Highlight = Utils.ConvertToInt32(dr["Highlight"], 0);
                        CoQuanDonVi.ParentID = Utils.ConvertToInt32(dr["ParentID"], 0);

                        List.Add(CoQuanDonVi);

                    }
                    DanhMucCoQuanDonViModelPartial new_CoQuanDonVi = new DanhMucCoQuanDonViModelPartial();

                    //List<int> ListparentId = new List<int>();
                    //foreach (var item in List)
                    //{
                    //    ListparentId.Add(item.ParentID.Value);
                    //}

                    ////new_CoQuanDonVi.Children =;
                    //List1 = List.Where(x => x.ParentID == new_CoQuanDonVi.ID || x.ParentID != null || x.ParentID == null || x.ParentID != new_CoQuanDonVi.ID).ToList();
                    //GetChild(List, List1);
                    ////new_List.Add(new_CoQuanDonVi.Children);
                    dr.Close();
                }
                List.ForEach(x => x.Children = List.Where(y => y.ParentID == x.ID).ToList());
                List.RemoveAll(x => x.ParentID > 0);
                return List;
            }
            catch (Exception ex)
            {
                return new List<DanhMucCoQuanDonViModelPartial>();
                throw ex;
            }
        }
        // Get All cơ quan cha
        public List<DanhMucCoQuanDonViModel> GetAllByCapCha(int CoQuanID)
        {
            List<DanhMucCoQuanDonViModel> List = new List<DanhMucCoQuanDonViModel>();
            try
            {
                var CoQuan = GetByID1(CoQuanID);
                var CoQuanChaID = CoQuan.CoQuanChaID;
                while (CoQuanChaID > 0)
                {
                    CoQuan = GetByID(CoQuanChaID);
                    CoQuanChaID = CoQuan.CoQuanChaID;
                    List.Add(CoQuan);
                }
                //var List = GetAll();
                //List<DanhMucCoQuanPar> ListDanhMucCoQuanPar = new List<DanhMucCoQuanPar>();
                //List.ForEach(x => x.Parent = List.Where(y => y.ID  == x.ParentID).ToList());
                //List.RemoveAll(x => x.ParentID > 0);
                return List;
            }
            catch (Exception ex)
            {
                return new List<DanhMucCoQuanDonViModel>();
                throw ex;
            }
        }
        public List<DanhMucCoQuanDonViModelPartial> GetALL(int ID, int CapCoQuanID, string Keyword)
        {
            try
            {
                List<DanhMucCoQuanDonViModelPartial> List = new List<DanhMucCoQuanDonViModelPartial>();
                List<object> new_List = new List<object>();
                SqlParameter[] parameters = new SqlParameter[]
                  {

                    new SqlParameter("@Keyword",SqlDbType.NVarChar)
                  };
                parameters[0].Value = Keyword == null ? "" : Keyword.Trim();
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetAllByCap", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucCoQuanDonViModelPartial CoQuanDonVi = new DanhMucCoQuanDonViModelPartial();
                        CoQuanDonVi.ID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        CoQuanDonVi.Ten = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        CoQuanDonVi.MaCQ = Utils.ConvertToInt32(dr["MaCQ"], 0);
                        CoQuanDonVi.Highlight = Utils.ConvertToInt32(dr["Highlight"], 0);
                        CoQuanDonVi.ParentID = Utils.ConvertToInt32(dr["ParentID"], 0);
                        List.Add(CoQuanDonVi);
                    }
                    DanhMucCoQuanDonViModelPartial new_CoQuanDonVi = new DanhMucCoQuanDonViModelPartial();
                    dr.Close();
                }
                //List.ForEach(x => x.Children = List.Where(y => y.ParentID == x.ID).ToList());
                //List.RemoveAll(x => x.ParentID > 0);
                return List;
            }
            catch (Exception ex)
            {
                return new List<DanhMucCoQuanDonViModelPartial>();
                throw ex;
            }
        }


        public int GetChild(List<DanhMucCoQuanDonViModelPartial> list, List<DanhMucCoQuanDonViModelPartial> list1)
        {
            List<DanhMucCoQuanDonViModelPartial> new_List = new List<DanhMucCoQuanDonViModelPartial>();
            foreach (var item in list1)
            {
                DanhMucCoQuanDonViModelPartial new_CoQuanDonVi = new DanhMucCoQuanDonViModelPartial();
                new_List = (from x in list where x.ParentID == item.ID select x).ToList();
                if (new_List.Count == 0)
                {
                    item.Children = null;
                }
                else
                {
                    item.Children = new_List;
                }
                //if (new_List.Count <= 0)
                //{
                //    return 0;
                //}
            }
            if (new_List.Count <= 0)
            {
                return 0;
            }
            GetChild(list, new_List);
            return 1;

        }
        // Get All
        public List<DanhMucCoQuanDonViModel> GetAll()
        {
            List<DanhMucCoQuanDonViModel> list = new List<DanhMucCoQuanDonViModel>();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetAll"))
                {
                    while (dr.Read())
                    {
                        DanhMucCoQuanDonViModel coQuanDonVi = new DanhMucCoQuanDonViModel();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        coQuanDonVi.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        coQuanDonVi.ThamQuyenID = Utils.ConvertToInt32(dr["ThamQuyenID"], 0);
                        coQuanDonVi.TinhID = Utils.ConvertToInt32(dr["TinhID"], 0);
                        coQuanDonVi.HuyenID = Utils.ConvertToInt32(dr["HuyenID"], 0);
                        coQuanDonVi.XaID = Utils.ConvertToInt32(dr["XaID"], 0);
                        coQuanDonVi.CapUBND = Utils.ConvertToBoolean(dr["CapUBND"], true);
                        coQuanDonVi.CapThanhTra = Utils.ConvertToBoolean(dr["CapThanhTra"], true);
                        coQuanDonVi.CQCoHieuLuc = Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false);
                        coQuanDonVi.SuDungPM = Utils.ConvertToBoolean(dr["SuDungPM"], false);
                        coQuanDonVi.MaCQ = Utils.ConvertToString(dr["MaCQ"], string.Empty);
                        coQuanDonVi.SuDungQuyTrinh = Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false);
                        coQuanDonVi.IsDisable = Utils.ConvertToBoolean(dr["IsDisable"], false);

                        list.Add(coQuanDonVi);
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

        /// <summary>
        /// kiểm tra cơ quan đang xóa có cơ quan con hay ko
        /// nếu có trả về true, ngược lại trả về false
        /// </summary>
        /// <param name="CoQuanDonViID"></param>
        /// <returns></returns>
        private bool checkCoQuanCon(int CoQuanDonViID)
        {
            var result = false;
            try
            {
                if (CoQuanDonViID <= 0)
                {
                    return false;
                }
                //var CanBoByCoQuanID = new HeThongCanBoDAL().GetAllCanBoByCoQuanID(CoQuanDonViID, 0).ToList();
                //if (CanBoByCoQuanID.Count > 0)
                //{
                //    result = true;
                //}
                var listCoQuanCon = GetAllIDByCoQuanChaID(CoQuanDonViID);
                if (listCoQuanCon != null && listCoQuanCon.Count > 0) result = true;
                return result;
            }
            catch (Exception)
            {
                return true;
                throw;
            }

        }

        /// <summary>
        /// Kiểm tra trong có quan đang xóa có cán bộ hay ko
        /// nếu có cán bộ trả về true, ngược lại trả về false
        /// </summary>
        /// <param name="CoQuanDonViID"></param>
        /// <returns></returns>
        private bool checkCanBoTrongCoQuan(int CoQuanDonViID)
        {
            var result = false;
            try
            {
                if (CoQuanDonViID <= 0)
                {
                    return false;
                }
                var listCanBoTrongCoQuan = new HeThongCanBoDAL().GetAllByListCoQuanID(new List<int>() { CoQuanDonViID });
                if (listCanBoTrongCoQuan != null && listCanBoTrongCoQuan.Count > 0) result = true;
                return result;
            }
            catch (Exception)
            {
                return true;
                throw;
            }

        }
        /// <summary>
        /// Check co quan da tạo nhiem vu hoac da nhan nhiem vu
        /// </summary>
        /// <param name="CoQuanDonViID"></param>
        /// <returns></returns>
        private bool checkNhiemVuByCoQuan(int CoQuanDonViID)
        {
            var result = false;
            try
            {
                if (CoQuanDonViID <= 0)
                {
                    return false;
                }
                return result;
            }
            catch (Exception)
            {
                return true;
                throw;
            }

        }
        public int GetByIDCha(int CoQuanChaID)
        {
            var result = 0;
            try
            {

                return result;
            }
            catch (Exception)
            {
                return 1;

                throw;
            }
        }

        public DanhMucCoQuanDonViPartialNew GetByIDForCheckRef(int? CoQuanID)
        {
            DanhMucCoQuanDonViPartialNew coQuanDonVi = new DanhMucCoQuanDonViPartialNew();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_CoQuanID,SqlDbType.Int)
              };
            parameters[0].Value = CoQuanID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        coQuanDonVi = new DanhMucCoQuanDonViPartialNew();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        coQuanDonVi.XaID = Utils.ConvertToInt32(dr["XaID"], 0);
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return coQuanDonVi;
        }

        /// <summary>
        /// lấy tất cả cơ quan con (cấp liền kề) của CoQuanID
        /// </summary>
        /// <param name="CoQuanChaID"></param>
        /// <returns></returns>
        public List<DanhMucCoQuanDonViModel> GetAllIDByCoQuanChaID(int CoQuanChaID)
        {
            List<DanhMucCoQuanDonViModel> list = new List<DanhMucCoQuanDonViModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_CoQuanChaID,SqlDbType.Int)
              };
            parameters[0].Value = CoQuanChaID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetAllIDByCoQuanChaID", parameters))
                {
                    while (dr.Read())
                    {
                        var coQuanDonVi = new DanhMucCoQuanDonViPartialNew();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        //coQuanDonVi.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        //coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["CoQuanChaID"], string.Empty);
                        list.Add(coQuanDonVi);
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

        // Get cơ quan by canbologin
        //public List<DanhMucCoQuanDonViModelPartial> GetCoQuanbyCanBoLogin(int CanBoID, int CoQuanID)
        //{
        //    try
        //    {
        //        List<DanhMucCoQuanDonViModel> listCoQuanCon = new List<DanhMucCoQuanDonViModel>();
        //        var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID(CoQuanID);
        //        if (UserRole.CheckAdmin(CanBoID))
        //        {
        //            listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(0).ToList();

        //        }
        //        else if (crCoQuan.CapID == EnumCapCoQuan.CapTrungUong.GetHashCode())         // cấp trung ương
        //        {

        //        }
        //        else if (crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode())    // cấp tỉnh   
        //        {

        //            listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID).Where(x => x.CapID != EnumCapCoQuan.CapHuyen.GetHashCode()).ToList();


        //        }
        //        else if (crCoQuan.CapID == EnumCapCoQuan.CapSo.GetHashCode())    // cấp sở   
        //        {
        //            listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(crCoQuan.CoQuanChaID.Value).Where(x => x.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()).ToList();

        //        }
        //        else if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode())    // cấp huyện   
        //        {

        //            listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID).Where(x => x.CapID != EnumCapCoQuan.CapXa.GetHashCode()).ToList();


        //        }
        //        else if (crCoQuan.CapID == EnumCapCoQuan.CapPhong.GetHashCode())    // cấp phòng  
        //        {

        //            listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID).Where(x => x.CapID == EnumCapCoQuan.CapXa.GetHashCode()).ToList();
        //        }
        //        List<DanhMucCoQuanPar> List1 = new List<DanhMucCoQuanPar>();
        //        foreach (var item in listCoQuanCon)
        //        {
        //            DanhMucCoQuanPar danhMucCoQuanDonViModelPartial = new DanhMucCoQuanPar();
        //            danhMucCoQuanDonViModelPartial.CoQuanID = item.CoQuanID;
        //            danhMucCoQuanDonViModelPartial.TenCoQuan = item.TenCoQuan;
        //            danhMucCoQuanDonViModelPartial.CoQuanChaID = item.CoQuanChaID;
        //            danhMucCoQuanDonViModelPartial.CapID = item.CapID;
        //            danhMucCoQuanDonViModelPartial.ThamQuyenID = item.ThamQuyenID;
        //            danhMucCoQuanDonViModelPartial.MaCQ = item.MaCQ;
        //            //danhMucCoQuanDonViModelPartial.CoQuanID = item.CoQuanID;
        //            var List = new DanhMucCoQuanDonViDAL().GetAll();
        //            List1.Add(danhMucCoQuanDonViModelPartial);              
        //             List1.ForEach(x => x.Children = List.Where(y => y.CoQuanChaID == x.CoQuanID).ToList());
        //        }


        //    }
        //    catch
        //    {

        //    }

        //}
        /// <summary>
        /// lấy toàn bộ cơ quan con (bao gồm cả cơ quan hiện tại)
        /// nếu là admin tổng thì lấy tất cả cơ quan trong hệ thống (CoQuanID=0)
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<DanhMucCoQuanDonViModel> GetAllCapCon(int? CoQuanID)
        {
            List<DanhMucCoQuanDonViModel> list = new List<DanhMucCoQuanDonViModel>();
            //if (GetByID1(CoQuanID).CapID == EnumCapCoQuan.CapSo.GetHashCode() || GetByID1(CoQuanID).CapID == EnumCapCoQuan.CapPhong.GetHashCode())
            //{
            //    CoQuanID = GetByID1(CoQuanID).CoQuanChaID.Value;
            //}
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_CoQuanID,SqlDbType.Int)
              };
            parameters[0].Value = CoQuanID ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetAllCapCon", parameters))
                {
                    while (dr.Read())
                    {
                        var coQuanDonVi = new DanhMucCoQuanDonViModel();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        var coQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        if (coQuanChaID == 0) coQuanDonVi.CoQuanChaID = null;
                        else coQuanDonVi.CoQuanChaID = coQuanChaID;
                        //coQuanDonVi.CoQuanChaID = coQuanChaID <1 ? null : coQuanChaID;
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        coQuanDonVi.ID = coQuanDonVi.CoQuanID;
                        coQuanDonVi.Ten = coQuanDonVi.TenCoQuan;
                        coQuanDonVi.Cap = coQuanDonVi.CapID;
                        coQuanDonVi.ParentID = coQuanDonVi.CoQuanChaID;
                        coQuanDonVi.CQCoHieuLuc = Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false);
                        coQuanDonVi.SuDungPM = Utils.ConvertToBoolean(dr["SuDungPM"], false);
                        list.Add(coQuanDonVi);
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


        // Get All cấp con và cấp cơ quan
        public List<DanhMucCoQuanDonViModel> GetAllCapConByCapCoQuan(int? CoQuanID)
        {
            List<DanhMucCoQuanDonViModel> list = new List<DanhMucCoQuanDonViModel>();
            if (GetByID1(CoQuanID.Value).CapID == EnumCapCoQuan.CapSo.GetHashCode() || GetByID1(CoQuanID.Value).CapID == EnumCapCoQuan.CapPhong.GetHashCode())
            {
                CoQuanID = GetByID1(CoQuanID.Value).CoQuanChaID.Value;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_CoQuanID,SqlDbType.Int)
              };
            parameters[0].Value = CoQuanID ?? Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_CoQuanDonVi_GetAllCapCon", parameters))
                {
                    while (dr.Read())
                    {
                        var coQuanDonVi = new DanhMucCoQuanDonViModel();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        var coQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        if (coQuanChaID == 0) coQuanDonVi.CoQuanChaID = null;
                        else coQuanDonVi.CoQuanChaID = coQuanChaID;
                        //coQuanDonVi.CoQuanChaID = coQuanChaID <1 ? null : coQuanChaID;
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        coQuanDonVi.ID = coQuanDonVi.CoQuanID;
                        coQuanDonVi.Ten = coQuanDonVi.TenCoQuan;
                        coQuanDonVi.Cap = coQuanDonVi.CapID;
                        coQuanDonVi.ParentID = coQuanDonVi.CoQuanChaID;
                        list.Add(coQuanDonVi);
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
        public List<DanhMucCoQuanDonViModel> GetListByUser(int CoQuanID, int NguoiDungID)
        {
            List<DanhMucCoQuanDonViModel> Result = new List<DanhMucCoQuanDonViModel>();
            try
            {
                var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID1(CoQuanID);
                var listCoQuan = new List<DanhMucCoQuanDonViModel>();
                //var listThanhTraTinh = new SystemConfigDAL().GetByKey("Thanh_Tra_Tinh_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
                //var listThanhTraHuyen = new SystemConfigDAL().GetByKey("Thanh_Tra_Huyen_ID").ConfigValue.Split(',').ToList().Select(x => Utils.ConvertToInt32(x.ToString(), 0)).ToList();
                if (UserRole.CheckAdmin(NguoiDungID) /*|| listThanhTraTinh.Contains(CoQuanID)*/)
                {
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
                }
                //else if (listThanhTraHuyen.Contains(CoQuanID))
                //{
                //    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(new DanhMucCoQuanDonViDAL().GetByID(CoQuanID).CoQuanChaID);
                //}
                else listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
                listCoQuan = listCoQuan.OrderBy(x => x.CapID).ThenByDescending(x => x.CoQuanID).ToList();
                if (listCoQuan.Count > 1)
                {
                    listCoQuan.ForEach(x => x.Children = listCoQuan.Where(y => y.CoQuanChaID == x.CoQuanID).ToList());
                    if (crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode() || crCoQuan.CapID == EnumCapCoQuan.CapTinh.GetHashCode() || crCoQuan.CapID == EnumCapCoQuan.CapXa.GetHashCode()
                        /*|| crCoQuan.CapID == EnumCapCoQuan.CapHuyen.GetHashCode()*/)
                    {
                        listCoQuan.RemoveAll(x => x.CoQuanChaID != null && x.CoQuanID != crCoQuan.CoQuanID);
                    }
                    else
                    {
                        listCoQuan.RemoveAll(x => x.CoQuanID != crCoQuan.CoQuanChaID /*&& x.CoQuanID != crCoQuan.CoQuanID*/);
                    }

                }
                //listCoQuan.OrderBy(x => x.CapID);
                return listCoQuan;
            }
            catch
            {
                return Result;
                throw;
            }
        }
        public List<DanhMucCoQuanDonViModel> GetListByUser_Old(int CoQuanID, int NguoiDungID)
        {
            List<DanhMucCoQuanDonViModel> Result = new List<DanhMucCoQuanDonViModel>();

            try
            {
                var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID1(CoQuanID);
                var listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);    // xã
                var coQuanHuyen = new DanhMucCoQuanDonViModel();
                if (crCoQuan.CoQuanChaID != null && crCoQuan.CoQuanChaID > 0) coQuanHuyen = new DanhMucCoQuanDonViDAL().GetByID1(crCoQuan.CoQuanChaID.Value);

                if (crCoQuan != null && crCoQuan.CoQuanID > 0) listCoQuan.Add(crCoQuan);
                if (UserRole.CheckAdmin(NguoiDungID) || crCoQuan.CapID < 3) // tỉnh và sở
                {
                    listCoQuan = new List<DanhMucCoQuanDonViModel>();
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
                }
                else if (crCoQuan.CapID == 3)   // huyện
                {
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanID);
                    listCoQuan.Add(crCoQuan);
                }
                else if (crCoQuan.CapID == 4)   // phòng
                {
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID.Value);
                    // var coQuanHuyen = new DanhMucCoQuanDonViDAL().GetByID1(crCoQuan.CoQuanChaID.Value);
                    listCoQuan.Add(coQuanHuyen);
                }

                listCoQuan.ForEach(x => x.Children = listCoQuan.Where(y => y.CoQuanChaID == x.CoQuanID).ToList());
                listCoQuan.RemoveAll(x => x.CoQuanChaID != null);
                if (!UserRole.CheckAdmin(NguoiDungID))
                {
                    if (crCoQuan.CapID == 3 || crCoQuan.CapID == 5) listCoQuan.Add(crCoQuan);
                    else if (crCoQuan.CapID == 4) listCoQuan.Add(coQuanHuyen);
                }
                return listCoQuan;
            }
            catch
            {
                return Result;
                throw;
            }
        }


        /// <summary>
        /// không dùng tới
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <param name="NguoiDungID"></param>
        /// <returns></returns>
        public List<DanhMucCoQuanDonViModel> GetListByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID)
        {
            List<DanhMucCoQuanDonViModel> Result = new List<DanhMucCoQuanDonViModel>();
            try
            {
                var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID1(CoQuanID);
                var listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
                if (UserRole.CheckAdmin(NguoiDungID))
                {
                    listCoQuan = new List<DanhMucCoQuanDonViModel>();
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
                }
                if (listCoQuan.Count > 1)
                {
                    listCoQuan.ForEach(x => x.Children = listCoQuan.Where(y => y.CoQuanChaID == x.CoQuanID).ToList());
                    listCoQuan.RemoveAll(x => x.CoQuanChaID != null && x.CoQuanID != crCoQuan.CoQuanID);
                }
                return listCoQuan;
            }
            catch
            {
                return Result;
                throw;
            }
        }
        public List<DanhMucCoQuanDonViModel> GetByUser_FoPhanQuyen(int CoQuanID, int NguoiDungID, string KeyWord)
        {
            List<DanhMucCoQuanDonViModel> Result = new List<DanhMucCoQuanDonViModel>();
            try
            {
                var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID1(CoQuanID);
                var listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
                if (UserRole.CheckAdmin(NguoiDungID))
                {
                    listCoQuan = new List<DanhMucCoQuanDonViModel>();
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
                }
                if (KeyWord != null && KeyWord.Length > 0)
                {
                    for (int i = 0; i < listCoQuan.Count; i++)
                    {
                        if (listCoQuan[i].TenCoQuan.ToLower().Contains(KeyWord.ToLower()))
                        {
                            listCoQuan[i].Highlight = 1;
                        }
                        else listCoQuan[i].Highlight = 0;
                    }
                }

                listCoQuan = listCoQuan.OrderBy(x => x.CapID).ThenByDescending(x => x.CoQuanID).ToList();

                if (listCoQuan.Count > 1)
                {
                    listCoQuan.ForEach(x => x.Children = listCoQuan.Where(y => y.CoQuanChaID == x.CoQuanID).ToList());
                    listCoQuan.RemoveAll(x => x.CoQuanChaID != null && x.CoQuanID != crCoQuan.CoQuanID);
                }
                return listCoQuan;
            }
            catch (Exception ex)
            {
                return Result;
                throw;
            }
        }
        public List<DanhMucCoQuanDonViModel> GetListByUser_Phang(int CoQuanID, int NguoiDungID)
        {
            List<DanhMucCoQuanDonViModel> Result = new List<DanhMucCoQuanDonViModel>();

            try
            {
                var crCoQuan = new DanhMucCoQuanDonViDAL().GetByID1(CoQuanID);
                var listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);    // xã
                var coQuanHuyen = new DanhMucCoQuanDonViModel();
                if (crCoQuan.CoQuanChaID != null && crCoQuan.CoQuanChaID > 0) coQuanHuyen = new DanhMucCoQuanDonViDAL().GetByID1(crCoQuan.CoQuanChaID.Value);

                //if (crCoQuan != null && crCoQuan.CoQuanID > 0) listCoQuan.Add(crCoQuan);
                if (UserRole.CheckAdmin(NguoiDungID) || crCoQuan.CapID < 3) // tỉnh và sở
                {
                    listCoQuan = new List<DanhMucCoQuanDonViModel>();
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
                }
                else if (crCoQuan.CapID == 3)   // huyện
                {
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanID);
                    //listCoQuan.Add(crCoQuan);
                }
                else if (crCoQuan.CapID == 4)   // phòng
                {
                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(crCoQuan.CoQuanChaID.Value);
                    // var coQuanHuyen = new DanhMucCoQuanDonViDAL().GetByID1(crCoQuan.CoQuanChaID.Value);
                    //listCoQuan.Add(coQuanHuyen);
                }

                //listCoQuan.ForEach(x => x.Children = listCoQuan.Where(y => y.CoQuanChaID == x.CoQuanID).ToList());
                //listCoQuan.RemoveAll(x => x.CoQuanChaID != null);
                if (UserRole.CheckAdmin(NguoiDungID))
                {
                    //if (crCoQuan.CapID == 3 || crCoQuan.CapID == 5) listCoQuan.Add(crCoQuan);
                    //else if (crCoQuan.CapID == 4) listCoQuan.Add(coQuanHuyen);
                }
                return listCoQuan;
            }
            catch
            {
                return Result;
                throw;
            }
        }
        //Import file
        public int ImportFile(string FilePath, ref string Message, int NguoiDungID)
        {
            int val = 0;
            if (!File.Exists(FilePath))
            {
                return val;
            }
            try
            {

                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var totalWorksheets = package.Workbook.Worksheets.Count;
                    if (totalWorksheets <= 0)
                    {
                        return val;
                    }
                    else
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        List<DanhMucCoQuanDonViModel> list = new List<DanhMucCoQuanDonViModel>();
                        DataTable dt = new DataTable(typeof(object).Name);
                        for (int i = 4;
                                 i <= workSheet.Dimension.End.Row;
                                 i++)
                        {
                            List<object> lstobj = new List<object>();
                            List<object> MyListChucVu = new List<object>();
                            for (int j = workSheet.Dimension.Start.Column;
                                     j <= workSheet.Dimension.End.Column;
                                     j++)
                            {
                                if (j == 5)
                                {
                                    MyListChucVu.Add(workSheet.Cells[i, j].Value);

                                }
                                {
                                    object cellValue = workSheet.Cells[i, j].Value;
                                    lstobj.Add(cellValue);
                                }

                            }
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                dt.Columns.Add("Column" + (dimension + 1));
                            }
                            DataRow row = dt.NewRow();
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                //Console.Write("{0} ", lstobj[element, dimension]);
                                //if(dimension == 5)
                                //{
                                //    MyList.Add(WS.Cells[Row, dimension+1].Value);
                                //}
                                row["Column" + (dimension + 1)] = lstobj[dimension];
                            }
                            dt.Rows.Add(row);
                            foreach (DataRow dr in dt.Rows)
                            {
                                list.Add(new DanhMucCoQuanDonViModel
                                {
                                    CoQuanID = Utils.ConvertToInt32(dr["Column1"], 0),
                                    TenCoQuan = Utils.ConvertToString(dr["Column2"], string.Empty),
                                    MaCQ = Utils.ConvertToString(dr["Column3"], string.Empty),
                                    CapID = Utils.ConvertToInt32(dr["Column4"], 0)

                                });

                            }
                            dt.Clear();
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                dt.Columns.Remove("Column" + (dimension + 1));
                            }
                            foreach (var item in list)
                            {
                                if (string.IsNullOrEmpty(item.TenCoQuan) || string.IsNullOrEmpty(item.MaCQ) || item.CapID <= 0)
                                {
                                    return 0;
                                }
                                if (GetByID(item.CoQuanID) != null)
                                {
                                    val = Update(item, ref Message);
                                }
                                else
                                {
                                    var CanBoID = 0;

                                    val = Insert(item, ref CanBoID, NguoiDungID, ref Message);
                                }
                            }
                        }


                    }
                }

                return val;
            }
            catch
            {
                throw;
            }
        }
        // Check mã cơ quan
        public BaseResultModel CheckMaCQ(int? CoQuanID, string MaCQ)
        {

            BaseResultModel BaseResultModel = new BaseResultModel();
            try
            {
                if (string.IsNullOrEmpty(MaCQ))
                {
                    BaseResultModel.Message = "Mã cơ quan đã đang để trống";
                    BaseResultModel.Status = 0;
                    return BaseResultModel;
                }
                var CoQuanByMaCQ = new DanhMucCoQuanDonViDAL().GetByMCQ(MaCQ.Trim());
                if (CoQuanID == null || CoQuanID < 1)
                {
                    if (CoQuanByMaCQ.CoQuanID > 0)
                    {
                        BaseResultModel.Message = "Mã cơ quan đã tồn tại";
                        BaseResultModel.Status = 0;
                        return BaseResultModel;
                    }
                    else
                    {
                        BaseResultModel.Status = 1;
                        return BaseResultModel;
                    }
                }
                else
                {
                    if (CoQuanByMaCQ.CoQuanID != CoQuanID && CoQuanByMaCQ.MaCQ == MaCQ.Trim())
                    {
                        BaseResultModel.Message = "Mã cơ quan đã tồn tại";
                        BaseResultModel.Status = 0;
                        return BaseResultModel;
                    }
                    else
                    {

                        BaseResultModel.Status = 1;
                        return BaseResultModel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// lấy thông tin về cơ quan đang sử dụng phần mềm từ cơ quan đang đăng nhập
        /// , (cơ quan đang đăng nhập chính là cơ quan sdpm hoặc là cơ quan trực thuộc)
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public DanhMucCoQuanDonViModel GetCoQuanSuDungPhanMem_By_CoQuanDangNhap(int? CoQuanID)
        {
            DanhMucCoQuanDonViModel coQuanDonVi = new DanhMucCoQuanDonViModel();
            SqliteParameter[] parameters = new SqliteParameter[]
              {
                new SqliteParameter(PARAM_CoQuanID,SqlDbType.Int)
              };
            parameters[0].Value = CoQuanID ?? Convert.DBNull;
            try
            {
                using (var dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, QueryManager.Get("v1_DanhMuc_CoQuanDonVi_GetCoQuanSuDungPhanMem_By_CoQuanDangNhap"), parameters))
                {
                    while (dr.Read())
                    {
                        coQuanDonVi = new DanhMucCoQuanDonViModel();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        coQuanDonVi.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        //coQuanDonVi.ThamQuyenID = Utils.ConvertToInt32(dr["ThamQuyenID"], 0);
                        //coQuanDonVi.TinhID = Utils.ConvertToInt32(dr["TinhID"], 0);
                        //coQuanDonVi.HuyenID = Utils.ConvertToInt32(dr["HuyenID"], 0);
                        //coQuanDonVi.XaID = Utils.ConvertToInt32(dr["XaID"], 0);
                        //coQuanDonVi.CapUBND = Utils.ConvertToBoolean(dr["CapUBND"], true);
                        //coQuanDonVi.CapThanhTra = Utils.ConvertToBoolean(dr["CapThanhTra"], true);
                        coQuanDonVi.CQCoHieuLuc = Utils.ConvertToBoolean(dr["CQCoHieuLuc"], false);
                        coQuanDonVi.SuDungPM = Utils.ConvertToBoolean(dr["SuDungPM"], false);
                        coQuanDonVi.MaCQ = Utils.ConvertToString(dr["MaCQ"], string.Empty);
                        //coQuanDonVi.SuDungQuyTrinh = Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false);
                        //coQuanDonVi.MappingCode = Utils.ConvertToString(dr["MappingCode"], string.Empty);
                        //coQuanDonVi.IsDisable = Utils.ConvertToBoolean(dr["IsDisable"], false);
                        //coQuanDonVi.TTChiaTachSapNhap = Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0);
                        //coQuanDonVi.ChiaTachSapNhapDenCQID = Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0);
                        //coQuanDonVi.IsStatus = Utils.ConvertToBoolean(dr["IsStatus"], false);
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        //coQuanDonVi.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return coQuanDonVi;
        }


        /// <summary>
        /// Lấy tất cả cơ quan trong cây của đơn vị sử dụng chương trình
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<DanhMucCoQuanDonViModel> DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap(int? CoQuanID)
        {
            List<DanhMucCoQuanDonViModel> Result = new List<DanhMucCoQuanDonViModel>();
            SqliteParameter[] parameters = new SqliteParameter[]
              {
                new SqliteParameter(PARAM_CoQuanID,SqlDbType.Int)
              };
            parameters[0].Value = (CoQuanID != null && CoQuanID > 0) ? CoQuanID : Convert.DBNull;
            try
            {
                using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, QueryManager.Get("v1_DanhMuc_CoQuanDonVi_DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap"), parameters))
                {
                    while (dr.Read())
                    {
                        var coQuanDonVi = new DanhMucCoQuanDonViModel();
                        coQuanDonVi.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        coQuanDonVi.CoQuanChaID = Utils.ConvertToInt32(dr["CoQuanChaID"], 0);
                        coQuanDonVi.CapID = Utils.ConvertToInt32(dr["CapID"], 0);
                        //coQuanDonVi.ThamQuyenID = Utils.ConvertToInt32(dr["ThamQuyenID"], 0);
                        //coQuanDonVi.TinhID = Utils.ConvertToInt32(dr["TinhID"], 0);
                        //coQuanDonVi.HuyenID = Utils.ConvertToInt32(dr["HuyenID"], 0);
                        //coQuanDonVi.XaID = Utils.ConvertToInt32(dr["XaID"], 0);
                        //coQuanDonVi.CapUBND = Utils.ConvertToBoolean(dr["CapUBND"], true);
                        //coQuanDonVi.CapThanhTra = Utils.ConvertToBoolean(dr["CapThanhTra"], true);
                        var cqCoHieuLucValue = dr["CQCoHieuLuc"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CQCoHieuLuc"]);
                        var cqSuDungPMValue = dr["SuDungPM"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SuDungPM"]);

                        coQuanDonVi.CQCoHieuLuc = (cqCoHieuLucValue == 1);
                        coQuanDonVi.SuDungPM = (cqSuDungPMValue == 1);

                        coQuanDonVi.MaCQ = Utils.ConvertToString(dr["MaCQ"], string.Empty);
                        //coQuanDonVi.SuDungQuyTrinh = Utils.ConvertToBoolean(dr["SuDungQuyTrinh"], false);
                        //coQuanDonVi.MappingCode = Utils.ConvertToString(dr["MappingCode"], string.Empty);
                        //coQuanDonVi.IsDisable = Utils.ConvertToBoolean(dr["IsDisable"], false);
                        //coQuanDonVi.TTChiaTachSapNhap = Utils.ConvertToInt32(dr["TTChiaTachSapNhap"], 0);
                        //coQuanDonVi.ChiaTachSapNhapDenCQID = Utils.ConvertToInt32(dr["ChiaTachSapNhapDenCQID"], 0);
                        //coQuanDonVi.IsStatus = Utils.ConvertToBoolean(dr["IsStatus"], false);
                        coQuanDonVi.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        //coQuanDonVi.TenCoQuanCha = Utils.ConvertToString(dr["TenCoQuanCha"], string.Empty);
                        if (coQuanDonVi.CQCoHieuLuc == true)
                        {
                            Result.Add(coQuanDonVi);
                        }
                    }
                    dr.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Result;
        }

    }
}
