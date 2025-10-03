using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Ultilities;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.Gosol.INOUT.DAL.DanhMuc
{
    public interface IDanhMucChucVuDAL
    {
        public List<DanhMucChucVuModel> GetPagingBySearch(BasePagingParams p, ref int TotalCount);
        public BaseResultModel Insert(DanhMucChucVuModel DanhMucChucVuModel);
        public BaseResultModel Update(DanhMucChucVuModel DanhMucChucVuModel);
        public List<string> Delete(List<int> ListChucVuID);
        public DanhMucChucVuModel GetChucVuByID(int? ChucVuID);
        public List<DanhMucChucVuModel> GetAll();
        public BaseResultModel ImportChucVu(string FilePath);
    }
    public class DanhMucChucVuDAL : IDanhMucChucVuDAL
    {

        // param constant
        private const string PARAM_ChucVuID = "@ChucVuID";
        private const string PARAM_TenChucVu = "@TenChucVu";
        private const string PARAM_GhiChu = "@GhiChu";
        private const string PARAM_KeKhaiHangNam = "@KeKhaiHangNam";


        #region AnhVH
        public BaseResultModel Insert(DanhMucChucVuModel DanhMucChucVuModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucChucVuModel == null || DanhMucChucVuModel.TenChucVu == null || DanhMucChucVuModel.TenChucVu.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên chức vụ không được trống";
                    return Result;
                }
                else if (DanhMucChucVuModel.TenChucVu.Trim().Length > 50)
                {
                    Result.Status = 0;
                    Result.Message = "Tên chức vụ không được quá 50 ký tự";
                    return Result;
                }
                else
                {
                    var crChucVu = GetChucVuByName(DanhMucChucVuModel.TenChucVu);
                    if (crChucVu != null && crChucVu.ChucVuID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Chức vụ đã tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                           {
                            new SqlParameter(PARAM_TenChucVu, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_GhiChu, SqlDbType.NVarChar),
                            new SqlParameter(PARAM_KeKhaiHangNam, SqlDbType.Bit)

                           };
                        parameters[0].Value = DanhMucChucVuModel.TenChucVu.Trim();
                        parameters[1].Value = DanhMucChucVuModel.GhiChu ?? Convert.DBNull;
                        parameters[2].Value = DanhMucChucVuModel.KeKhaiHangNam ?? Convert.DBNull;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_Insert", parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("danh mục chức vụ");
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



        public BaseResultModel Update(DanhMucChucVuModel DanhMucChucVuModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (DanhMucChucVuModel == null || DanhMucChucVuModel.TenChucVu == null || DanhMucChucVuModel.TenChucVu.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên chức vụ không được trống";
                    return Result;
                }
                else if (DanhMucChucVuModel.TenChucVu.Trim().Length > 50)
                {
                    Result.Status = 0;
                    Result.Message = "Tên chức vụ không được quá 50 ký tự";
                    return Result;
                }
                else
                {
                    var crObj = GetChucVuByID(DanhMucChucVuModel.ChucVuID);
                    var objDouble = GetChucVuByName(DanhMucChucVuModel.TenChucVu.Trim());
                    if (crObj == null || crObj.ChucVuID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Chức vụ không tồn tại";
                        return Result;
                    }
                    else if (objDouble != null && objDouble.ChucVuID > 0 && objDouble.ChucVuID != DanhMucChucVuModel.ChucVuID)
                    {
                        Result.Status = 0;
                        Result.Message = "Chức vụ đã tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                                new SqlParameter(PARAM_ChucVuID, SqlDbType.Int),
                                new SqlParameter(PARAM_TenChucVu, SqlDbType.NVarChar),
                                new SqlParameter(PARAM_GhiChu, SqlDbType.NVarChar),
                                 new SqlParameter(PARAM_KeKhaiHangNam, SqlDbType.Bit)

                        };

                        parameters[0].Value = DanhMucChucVuModel.ChucVuID;
                        parameters[1].Value = DanhMucChucVuModel.TenChucVu.Trim();
                        parameters[2].Value = DanhMucChucVuModel.GhiChu ?? Convert.DBNull;
                        parameters[3].Value = DanhMucChucVuModel.KeKhaiHangNam ?? Convert.DBNull;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_Update", parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("danh mục chức vụ");
                                    return Result;
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
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }
        #endregion
        public int Insert_Old(DanhMucChucVuModel DanhMucChucVuModel)
        {
            int val = 0;

            if (DanhMucChucVuModel == null)
            {
                return val;
            }
            var chucVu = GetChucVuByName(DanhMucChucVuModel.TenChucVu.Trim());
            if (chucVu.ChucVuID > 0)
            {
                val = 0;
            }
            else
            {
                SqlParameter[] parameters = new SqlParameter[]
                  {
                    new SqlParameter(PARAM_TenChucVu, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_GhiChu, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_KeKhaiHangNam, SqlDbType.Bit)

                  };
                parameters[0].Value = DanhMucChucVuModel.TenChucVu.Trim();
                parameters[1].Value = DanhMucChucVuModel.GhiChu ?? Convert.DBNull;
                parameters[2].Value = DanhMucChucVuModel.KeKhaiHangNam ?? Convert.DBNull;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_Insert", parameters);
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
            return val;

        }


        // Update
        public int Update_Old(DanhMucChucVuModel DanhMucChucVuModel)
        {
            int val = 0;
            if (DanhMucChucVuModel == null)
            {
                return val;
            }
            var objDouble = GetChucVuByName(DanhMucChucVuModel.TenChucVu);
            if (objDouble != null && objDouble.ChucVuID > 0 && objDouble.ChucVuID != DanhMucChucVuModel.ChucVuID)
            {
                val = 2;
                return val;
            }

            SqlParameter[] parameters = new SqlParameter[]
              {
                    new SqlParameter(PARAM_ChucVuID, SqlDbType.Int),
                    new SqlParameter(PARAM_TenChucVu, SqlDbType.NVarChar),
                    new SqlParameter(PARAM_GhiChu, SqlDbType.NVarChar),
                     new SqlParameter(PARAM_KeKhaiHangNam, SqlDbType.Bit)

              };

            parameters[0].Value = DanhMucChucVuModel.ChucVuID;
            parameters[1].Value = DanhMucChucVuModel.TenChucVu.Trim();
            parameters[2].Value = DanhMucChucVuModel.GhiChu ?? Convert.DBNull;
            parameters[3].Value = DanhMucChucVuModel.KeKhaiHangNam ?? Convert.DBNull;
            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        val = Utils.ConvertToInt32(SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_Update", parameters), 0);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                    return val;
                }
            }

        }



        public List<string> Delete(List<int> ListChucVuID)
        {
            List<string> dic = new List<string>();

            //if (ListChucVuID.Count <= 0)
            //{
            //    dic.Add("Chưa có chức vụ nào được chon!");
            //    return dic;
            //}
            string message = "";
            if (ListChucVuID.Count <= 0)
            {
                message = "Vui lòng chọn dữ liệu trước khi xóa!";
                dic.Add(message);
                //return dic;
            }
            else
            {
                //var Seccess = 0;
                for (int i = 0; i < ListChucVuID.Count; i++)
                {
                    var CanBo = new HeThongCanBoDAL().GetCanBoByChucVuID(ListChucVuID[i]);
                    //if (GetChucVuByID(ListChucVuID[i]) == null)
                    //{
                    //    //message = "Chức vụ không tồn tại ! Mời chọn lại !";
                    //    //dic.Add(0, message);
                    //    //return dic;
                    //}
                    if (CanBo.CanBoID > 0)
                    {
                        dic.Add(GetChucVuByID(ListChucVuID[i]).TenChucVu);
                        //return dic;
                    }
                     else if (GetChucVuByID(ListChucVuID[i]) != null)

                    {
                        SqlParameter[] parameters = new SqlParameter[]
  {
                    new SqlParameter(PARAM_ChucVuID, SqlDbType.Int)

  };
                        parameters[0].Value = ListChucVuID[i];
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_Delete", parameters);
                                    trans.Commit();
                                    //if (val == 0)
                                    //{
                                    //    message = "Không thể xóa từ Chức vụ  " + GetChucVuByID(ListChucVuID[i]).TenChucVu;
                                    //    dic.Add(0, message);
                                    //    return dic;
                                    //}

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
                        
            }
            return dic;
        }

        //Get All
        public List<DanhMucChucVuModel> GetAll()
        {
            List<DanhMucChucVuModel> list = new List<DanhMucChucVuModel>();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_DanhMuc_ChucVu_GetAll"))
                {
                    while (dr.Read())
                    {
                        DanhMucChucVuModel item = new DanhMucChucVuModel();
                        item.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        item.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], "");
                        item.GhiChu = Utils.ConvertToString(dr["GhiChu"], "");
                        item.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);

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
        //Get By ID
        public DanhMucChucVuModel GetChucVuByID(int? ChucVuID)
        {
            if (ChucVuID <= 0 || ChucVuID == null)
            {
                return new DanhMucChucVuModel();
            }
            DanhMucChucVuModel chucVu = null;
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(PARAM_ChucVuID,SqlDbType.Int)
              };
            parameters[0].Value = ChucVuID;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        chucVu = new DanhMucChucVuModel();
                        chucVu.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        chucVu.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        chucVu.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        chucVu.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return chucVu;
        }

        public DanhMucChucVuModel GetChucVuByName(string TenChucVu)
        {
            if (string.IsNullOrEmpty(TenChucVu))
            {
                return new DanhMucChucVuModel();
            }
            DanhMucChucVuModel chucVu = new DanhMucChucVuModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenChucVu",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenChucVu.Trim();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_GetByName", parameters))
                {
                    while (dr.Read())
                    {
                        chucVu = new DanhMucChucVuModel();
                        chucVu.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        chucVu.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        chucVu.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        chucVu.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return chucVu;
        }


        public List<DanhMucChucVuModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            //var db = _DbINOUTContext;
            //var xxx = db.DanhMucChucVu.Where(t => t.TenChucVu.Contains("a")).ToList();

            List<DanhMucChucVuModel> list = new List<DanhMucChucVuModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter("@Keyword",SqlDbType.NVarChar),
                new SqlParameter("@OrderByName",SqlDbType.NVarChar),
                new SqlParameter("@OrderByOption",SqlDbType.NVarChar),
                new SqlParameter("@pLimit",SqlDbType.Int),
                new SqlParameter("@pOffset",SqlDbType.Int),
                new SqlParameter("@TotalRow",SqlDbType.Int)

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

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        DanhMucChucVuModel chucVu = new DanhMucChucVuModel();
                        chucVu.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        chucVu.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        chucVu.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        chucVu.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);
                        list.Add(chucVu);
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
        public List<DanhMucChucVuModel> GetAllChucVu()
        {
            List<DanhMucChucVuModel> List = new List<DanhMucChucVuModel>();
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, @"v1_DanhMuc_ChucVu_GetAll"))
                {
                    while (dr.Read())
                    {
                        DanhMucChucVuModel chucVu = new DanhMucChucVuModel();
                        chucVu.ChucVuID = Utils.ConvertToInt32(dr["ChucVuID"], 0);
                        chucVu.TenChucVu = Utils.ConvertToString(dr["TenChucVu"], string.Empty);
                        chucVu.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        chucVu.KeKhaiHangNam = Utils.ConvertToBoolean(dr["KeKhaiHangNam"], false);
                        List.Add(chucVu);

                    }
                    dr.Close();
                }

            }
            catch
            {
                throw;
            }
            return List;
        }
        //Import Chức Vụ
        public BaseResultModel ImportChucVu(string FilePath)
        {
            BaseResultModel Result = new BaseResultModel();
            if (!File.Exists(FilePath))
            {
                return Result;
            }
            try
            {

                using (ExcelPackage package = new ExcelPackage(new FileInfo(FilePath)))
                {
                    var totalWorksheets = package.Workbook.Worksheets.Count;
                    if (totalWorksheets <= 0)
                    {
                        return Result;
                    }
                    else
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
                        List<DanhMucChucVuModel> list = new List<DanhMucChucVuModel>();
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
                                //if (j == 5)
                                //{
                                //    MyListChucVu.Add(workSheet.Cells[i, j].Value);

                                //}
                                {
                                    object cellValue = workSheet.Cells[i, j].Value;
                                    lstobj.Add(cellValue);
                                }

                            }
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                dt.Columns.Add("Column" + (dimension + 1));
                            }
                            //List<int> ListCoQuanID = new List<int>();
                            //string TenChucVu = MyListChucVu.FirstOrDefault().ToString();
                            //List<char> rackBag = new List<char>();
                            //rackBag.AddRange(TenChucVu.ToCharArray());
                            //var sum = rackBag.Count(x => x.Equals(",")) + 1;
                            //var TenChucVuSplit = TenChucVu.Split(",");
                            //for (int a = 0; a <= sum; a++)
                            //{
                            //    int ChucVuID = ConvertChucVuIDByName(TenChucVuSplit[i].ToString());
                            //    ListCoQuanID.Add(ChucVuID);
                            //}                        
                            DataRow row = dt.NewRow();
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                row["Column" + (dimension + 1)] = lstobj[dimension];
                            }
                            dt.Rows.Add(row);
                            foreach (DataRow dr in dt.Rows)
                            {
                                list.Add(new DanhMucChucVuModel
                                {
                                    ChucVuID = Utils.ConvertToInt32(dr["Column1"], 0),
                                    TenChucVu = Utils.ConvertToString(dr["Column2"], string.Empty),
                                    GhiChu = Utils.ConvertToString(dr["Column3"], string.Empty),
                                });

                            }
                            dt.Clear();
                            for (int dimension = 0; dimension < lstobj.Count; dimension++)
                            {
                                dt.Columns.Remove("Column" + (dimension + 1));
                            }
                            foreach (var item in list)
                            {
                                //Dictionary<int, int> ListCanBoChucVu = new Dictionary<int, int>();
                                //item.DanhSachChucVuID.ForEach(x => ListCanBoChucVu.Add(x, item.CanBoID));
                                //InsertCanBoChucVu(ListCanBoChucVu);
                                if (string.IsNullOrEmpty(item.TenChucVu))
                                {
                                    return Result;
                                }
                                if (GetChucVuByID(item.ChucVuID) != null)
                                {
                                    string Message = null;
                                    Result = Insert(item);

                                }
                                else
                                {
                                    var CanBoID = 0;
                                    Result = Update(item);
                                }
                            }
                        }


                    }
                }

                return Result;
            }
            catch
            {
                throw;
            }
        }

    }
}
