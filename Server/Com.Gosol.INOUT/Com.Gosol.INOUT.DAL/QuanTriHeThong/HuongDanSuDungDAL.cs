using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Com.Gosol.INOUT.Models;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface IHuongDanSuDungDAL
    {
        public List<HuongDanSuDungModel> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow);
        public HuongDanSuDungModel GetByID(int HuongDanSuDungID);
        public BaseResultModel Insert(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID);
        public BaseResultModel Update(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID);
        public BaseResultModel Delete(List<int> ListHuongDanSuDungID, int CanBoID);
        public HuongDanSuDungModel GetByMaChucNang(string MaChucNang);
    }
    public class HuongDanSuDungDAL : IHuongDanSuDungDAL
    {

        public List<HuongDanSuDungModel> GetPagingBySearch(BasePagingParamsForFilter p, ref int TotalRow)
        {
            List<HuongDanSuDungModel> list = new List<HuongDanSuDungModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                new SqlParameter("ChucNangID",SqlDbType.Int),
                new SqlParameter("ChucNangChaID",SqlDbType.Int),

            };
            parameters[0].Value = p.Keyword != null ? p.Keyword : "";
            parameters[1].Value = p.OrderByName;
            parameters[2].Value = p.OrderByOption;
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Direction = ParameterDirection.Output;
            parameters[5].Size = 8;
            parameters[6].Value = p.ChucNangID ?? Convert.DBNull;
            parameters[7].Value = p.NhomChucNang == "HeThong" ? 29 : p.NhomChucNang == "DanhMuc" ? 30 : p.NhomChucNang == "KeKhai" ? 31 : p.NhomChucNang == "BaoCao" ? 32 : Convert.DBNull;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_HuongDanSuDung_GetPagingBySearch", parameters))
                {
                    while (dr.Read())
                    {
                        HuongDanSuDungModel item = new HuongDanSuDungModel();
                        item.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        item.HuongDanSuDungID = Utils.ConvertToInt32(dr["HuongDanSuDungID"], 0);
                        item.NguoiCapNhat = Utils.ConvertToInt32(dr["NguoiCapNhat"], 0);
                        item.NgayCapNhat = Utils.ConvertToDateTime(dr["NgayCapNhat"], DateTime.Now);
                        item.TenFileGoc = Utils.ConvertToString(dr["TenFieGoc"], string.Empty);
                        item.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        item.TieuDe = Utils.ConvertToString(dr["TieuDe"], string.Empty);
                        item.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        //item.ChucNangChaID = Utils.ConvertToInt32(dr["ChucNangChaID"], 0);
                        item.MaChucNang = Utils.ConvertToString(dr["MaChucNang"], string.Empty);
                        item.TenChucNang = Utils.ConvertToString(dr["TenChucNang"], string.Empty);
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


        public BaseResultModel Insert(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (HuongDanSuDungModel == null || HuongDanSuDungModel.TieuDe == null || HuongDanSuDungModel.TieuDe.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tiêu đề không được trống";
                    return Result;
                }
                else if (HuongDanSuDungModel.MaChucNang == null || HuongDanSuDungModel.MaChucNang.Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Chức năng không được trống";
                    return Result;
                }
                else if (HuongDanSuDungModel.Base64String == null || HuongDanSuDungModel.Base64String.Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "File hướng dẫn sử dụng không được trống";
                    return Result;
                }
                else
                {
                    //var crHuongDanSuDung = GetByChucNangID(HuongDanSuDungModel.ChucNangID.Value);
                    var crHuongDanSuDung = GetByMaChucNang(HuongDanSuDungModel.MaChucNang);
                    if (crHuongDanSuDung != null && crHuongDanSuDung.HuongDanSuDungID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Hướng dẫn sử dụng cho chức năng đã tồn tại";
                        return Result;
                    }
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                    new SqlParameter("@ChucNangID", SqlDbType.Int),
                    new SqlParameter("@TieuDe", SqlDbType.NVarChar),
                    new SqlParameter("@TenFileGoc", SqlDbType.NVarChar),
                    new SqlParameter("@TenFileHeThong", SqlDbType.NVarChar),
                    new SqlParameter("@NgayCapNhat", SqlDbType.DateTime),
                    new SqlParameter("@NguoiCapNhat", SqlDbType.Int),
                    new SqlParameter("@TrangThai", SqlDbType.Int),
                    new SqlParameter("@MaChucNang", SqlDbType.NVarChar),
                    };
                    parameters[0].Value = HuongDanSuDungModel.ChucNangID ?? Convert.DBNull;
                    parameters[1].Value = HuongDanSuDungModel.TieuDe.Trim();
                    parameters[2].Value = HuongDanSuDungModel.TenFileGoc;
                    parameters[3].Value = HuongDanSuDungModel.TenFileHeThong;
                    parameters[4].Value = DateTime.Now;
                    parameters[5].Value = CanBoID;
                    parameters[6].Value = 1;
                    parameters[7].Value = HuongDanSuDungModel.MaChucNang;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, "v1_HeThong_HuongDanSuDung_Insert", parameters), 0);
                                trans.Commit();
                                if (query > 0)
                                {
                                    Result.Status = 1; Result.Data = query;
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Hướng dẫn sử dụng");
                                }
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
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }


        public BaseResultModel Update(HuongDanSuDungModel HuongDanSuDungModel, int CanBoID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (HuongDanSuDungModel == null || HuongDanSuDungModel.TieuDe == null || HuongDanSuDungModel.TieuDe.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tiêu đề không được trống";
                    return Result;
                }
                else if (HuongDanSuDungModel.MaChucNang == null || HuongDanSuDungModel.MaChucNang.Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Chức năng không được trống";
                    return Result;
                }
                //else if (HuongDanSuDungModel.Base64String == null || HuongDanSuDungModel.Base64String.Length < 1)
                //{
                //    Result.Status = 0;
                //    Result.Message = "File hướng dẫn sử dụng không được trống";
                //    return Result;
                //}
                else
                {
                    //var crHuongDanSuDung = GetByChucNangID(HuongDanSuDungModel.ChucNangID.Value);
                    var crHuongDanSuDung = GetByMaChucNang(HuongDanSuDungModel.MaChucNang);
                    if (crHuongDanSuDung != null && crHuongDanSuDung.HuongDanSuDungID > 0 && crHuongDanSuDung.HuongDanSuDungID != HuongDanSuDungModel.HuongDanSuDungID)
                    {
                        Result.Status = 0;
                        Result.Message = "Hướng dẫn sử dụng cho chức năng đã tồn tại";
                        return Result;
                    }
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                    new SqlParameter("@HuongDanSuDungID", SqlDbType.Int),
                    new SqlParameter("@ChucNangID", SqlDbType.Int),
                    new SqlParameter("@TieuDe", SqlDbType.NVarChar),
                    new SqlParameter("@TenFileGoc", SqlDbType.NVarChar),
                    new SqlParameter("@TenFileHeThong", SqlDbType.NVarChar),
                    new SqlParameter("@NgayCapNhat", SqlDbType.DateTime),
                    new SqlParameter("@NguoiCapNhat", SqlDbType.Int),
                    new SqlParameter("@TrangThai", SqlDbType.Int),
                    new SqlParameter("@MaChucNang", SqlDbType.NVarChar),
                    };
                    parameters[0].Value = HuongDanSuDungModel.HuongDanSuDungID;
                    parameters[1].Value = HuongDanSuDungModel.ChucNangID ?? Convert.DBNull;
                    parameters[2].Value = HuongDanSuDungModel.TieuDe.Trim();
                    parameters[3].Value = HuongDanSuDungModel.TenFileGoc;
                    parameters[4].Value = HuongDanSuDungModel.TenFileHeThong;
                    parameters[5].Value = DateTime.Now;
                    parameters[6].Value = CanBoID;
                    parameters[7].Value = 1;
                    parameters[8].Value = HuongDanSuDungModel.MaChucNang;

                    if (HuongDanSuDungModel.Base64String == null || HuongDanSuDungModel.Base64String.Length < 1)
                    {
                        parameters[3].Value = crHuongDanSuDung.TenFileGoc;
                        parameters[4].Value = crHuongDanSuDung.TenFileHeThong;
                    }

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                var query = Utils.ConvertToInt32(SQLHelper.ExecuteScalar(trans, CommandType.StoredProcedure, "v1_HeThong_HuongDanSuDung_Update", parameters), 0);
                                trans.Commit();
                                if (query >= 0)
                                {
                                    Result.Status = 1; Result.Data = query;
                                    Result.Message = ConstantLogMessage.Alert_Update_Success("Hướng dẫn sử dụng");
                                }
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
            catch (Exception)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw;
            }
            return Result;
        }


        public BaseResultModel Delete(List<int> ListHuongDanSuDungID, int CanBoID)
        {
            var Result = new BaseResultModel();
            if (ListHuongDanSuDungID.Count <= 0)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                for (int i = 0; i < ListHuongDanSuDungID.Count; i++)
                {
                    int crID;
                    if (!int.TryParse(ListHuongDanSuDungID[i].ToString(), out crID))
                    {
                        Result.Status = 0;
                        Result.Message = "Hướng dẫn sử dụng '" + ListHuongDanSuDungID[i].ToString() + "' không đúng định dạng";
                        return Result;
                    }
                    else
                    {
                        var crObj = GetByID(ListHuongDanSuDungID[i]);
                        if (crObj == null || crObj.HuongDanSuDungID == null || crObj.HuongDanSuDungID < 1)
                        {
                            Result.Status = 0;
                            Result.Message = "HuongDanSuDungID '" + ListHuongDanSuDungID[i].ToString() + "' không tồn tại";
                            return Result;
                        }
                    }
                }

                var pList = new SqlParameter("@DanhSachHuongDanSuDungID", SqlDbType.Structured);
                pList.TypeName = "dbo.list_ID";
                // var TrangThai = 400;
                var tbID = new DataTable();
                tbID.Columns.Add("ID", typeof(string));
                ListHuongDanSuDungID.ForEach(x => tbID.Rows.Add(x));
                SqlParameter[] parameters = new SqlParameter[]
                 {
                   pList
                 };
                parameters[0].Value = tbID;
                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            int val = 0;
                            val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_HuongDanSuDung_Delete", parameters);
                            trans.Commit();
                            if (val < 0)
                            {
                                Result.Status = 0;
                                Result.Message = "Không thể xóa Hướng dẫn sử dụng";
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

                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Hướng dẫn sử dụng");
                return Result;
            }
        }

        public HuongDanSuDungModel GetByID(int HuongDanSuDungID)
        {
            HuongDanSuDungModel Result = new HuongDanSuDungModel();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(@"HuongDanSuDungID", SqlDbType.Int),
            };
            parameters[0].Value = HuongDanSuDungID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_HuongDanSuDung_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        Result.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        Result.HuongDanSuDungID = Utils.ConvertToInt32(dr["HuongDanSuDungID"], 0);
                        Result.NguoiCapNhat = Utils.ConvertToInt32(dr["NguoiCapNhat"], 0);
                        Result.NgayCapNhat = Utils.ConvertToDateTime(dr["NgayCapNhat"], DateTime.Now);
                        Result.TenFileGoc = Utils.ConvertToString(dr["TenFieGoc"], string.Empty);
                        Result.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        Result.TieuDe = Utils.ConvertToString(dr["TieuDe"], string.Empty);
                        Result.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Result;
        }

        public HuongDanSuDungModel GetByMaChucNang(string MaChucNang)
        {
            HuongDanSuDungModel Result = new HuongDanSuDungModel();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(@"MaChucNang", SqlDbType.NVarChar),
            };
            parameters[0].Value = MaChucNang;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_HuongDanSuDung_GetByMaChucNang", parameters))
                {
                    while (dr.Read())
                    {
                        //Result.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        Result.HuongDanSuDungID = Utils.ConvertToInt32(dr["HuongDanSuDungID"], 0);
                        Result.NguoiCapNhat = Utils.ConvertToInt32(dr["NguoiCapNhat"], 0);
                        Result.NgayCapNhat = Utils.ConvertToDateTime(dr["NgayCapNhat"], DateTime.Now);
                        Result.TenFileGoc = Utils.ConvertToString(dr["TenFieGoc"], string.Empty);
                        Result.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        Result.TieuDe = Utils.ConvertToString(dr["TieuDe"], string.Empty);
                        Result.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        Result.MaChucNang = Utils.ConvertToString(dr["MaChucNang"], string.Empty);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Result;
        }
        public List<HuongDanSuDungModel> GetByChucNangID(int ChucNangID)
        {
            List<HuongDanSuDungModel> Result = new List<HuongDanSuDungModel>();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(@"ChucNangID", SqlDbType.NVarChar),
            };
            parameters[0].Value = ChucNangID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_HuongDanSuDung_GetByChucNangID", parameters))
                {
                    while (dr.Read())
                    {
                        var item = new HuongDanSuDungModel();
                        item.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        item.HuongDanSuDungID = Utils.ConvertToInt32(dr["HuongDanSuDungID"], 0);
                        item.NguoiCapNhat = Utils.ConvertToInt32(dr["NguoiCapNhat"], 0);
                        item.NgayCapNhat = Utils.ConvertToDateTime(dr["NgayCapNhat"], DateTime.Now);
                        item.TenFileGoc = Utils.ConvertToString(dr["TenFieGoc"], string.Empty);
                        item.TenFileHeThong = Utils.ConvertToString(dr["TenFileHeThong"], string.Empty);
                        item.TieuDe = Utils.ConvertToString(dr["TieuDe"], string.Empty);
                        item.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
                        Result.Add(item);
                        break;
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return Result;
        }
    }
}
