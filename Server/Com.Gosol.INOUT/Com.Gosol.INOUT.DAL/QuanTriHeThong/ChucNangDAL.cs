using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Security;
using Microsoft.Data.Sqlite;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface IChucNangDAL
    {
        List<ChucNangModel> GetListChucNangByNguoiDungID(int NguoidungID);
        public List<ChucNangModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow);
    }
    public class ChucNangDAL : IChucNangDAL
    {
        public List<ChucNangModel> GetListChucNangByNguoiDungID(int NguoidungID)
        {
            List<ChucNangModel> Result = new List<ChucNangModel>();
            List<ChucNangModel> list = new List<ChucNangModel>();
            SqliteParameter[] parameters = new SqliteParameter[] {
                new SqliteParameter(@"NguoiDungID", SqlDbType.Int),
            };
            parameters[0].Value = NguoidungID;

            try
            {
                if (UserRole.CheckAdmin(NguoidungID))
                {
                    var p = new BasePagingParams();
                    p.PageSize = 999;
                    var tt = 1;
                    list = new ChucNangDAL().GetPagingBySearch(p, ref tt);
                    //list.ForEach(x => Result.Add(new ChucNangModel(x.ChucNangID, x.TenChucNang, x.MaChucNang, 1)));
                    foreach (var item in list)
                    {
                        //if (item.ChucNangID < 200 && item.MaChucNang != "dm-trang-thai") Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 15));
                        //else Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 1));

                        if (item.MaChucNang == "checkin-out")
                        {
                            Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 3));
                        }
                        else if (item.MaChucNang == "bao-cao")
                        {
                            Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 1));
                        }
                        else
                        {
                            Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 15));
                        }
                    }
                }
                else
                {
                    using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, QueryManager.Get("v1_HT_ChucNang_GetByNguoidungID") , parameters))
                    {

                        while (dr.Read())
                        {
                            ChucNangModel item = new ChucNangModel(Utils.ConvertToInt32(dr["ChucNangID"], 0)
                                , Utils.ConvertToString(dr["TenChucNang"], String.Empty)
                                 , Utils.ConvertToString(dr["MaChucNang"], String.Empty)
                                , Utils.ConvertToInt32(dr["Quyen"], 0))
                                ;

                            list.Add(item);
                        }
                        dr.Close();
                        Result = (from m in list
                                  group m by new { m.ChucNangID, m.TenChucNang, m.MaChucNang } into chucNang
                                  select new ChucNangModel(chucNang.Key.ChucNangID, chucNang.Key.TenChucNang, chucNang.Key.MaChucNang, list.Where(x => x.ChucNangID == chucNang.Key.ChucNangID).ToList().Max(x => x.Quyen))
                                  //{
                                  //    ChucNangID = chucNang.Key.ChucNangID,
                                  //    TenChucNang = chucNang.Key.TenChucNang,
                                  //    MaChucNang = chucNang.Key.MaChucNang,
                                  //    Quyen = list.Where(x => x.ChucNangID == chucNang.Key.ChucNangID).ToList().Max(x => x.Quyen)

                                  //}
                                  ).ToList();

                    }

                }

            }
            catch
            {
                throw;
            }
            return Result;
        }
        //public List<ChucNangModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        //{
        //    List<ChucNangModel> list = new List<ChucNangModel>();
        //    SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("Keyword",SqlDbType.NVarChar,200),
        //        new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
        //        new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
        //        new SqlParameter("pLimit",SqlDbType.Int),
        //        new SqlParameter("pOffset",SqlDbType.Int),
        //        new SqlParameter("TotalRow",SqlDbType.Int),

        //    };
        //    parameters[0].Value = p.Keyword != null ? p.Keyword : "";
        //    parameters[1].Value = p.OrderByName;
        //    parameters[2].Value = p.OrderByOption;
        //    parameters[3].Value = p.Limit;
        //    parameters[4].Value = p.Offset;
        //    parameters[5].Direction = ParameterDirection.Output;
        //    parameters[5].Size = 8;
        //    try
        //    {
        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HeThong_ChucNang_GetPagingBySearch", parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                ChucNangModel item = new ChucNangModel();
        //                item.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
        //                item.TenChucNang = Utils.ConvertToString(dr["TenChucNang"], string.Empty);
        //                item.ChucNangChaID = Utils.ConvertToInt32(dr["ChucNangChaID"], 0);
        //                item.TenChucNangCha = Utils.ConvertToString(dr["TenChucNangCha"], string.Empty);
        //                item.MaChucNang = Utils.ConvertToString(dr["MaChucNang"], string.Empty);
        //                list.Add(item);
        //            }
        //            dr.Close();
        //        }
        //        TotalRow = Utils.ConvertToInt32(parameters[5].Value, 0);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return list;
        //}
        public List<ChucNangModel> GetPagingBySearch(BasePagingParams p, ref int TotalRow)
        {
            List<ChucNangModel> list = new List<ChucNangModel>();

            SqliteParameter[] parameters = new SqliteParameter[]
            {
                new SqliteParameter("@Keyword", DbType.String) { Value = p.Keyword ?? "" },
                new SqliteParameter("@OrderByName", DbType.String) { Value = p.OrderByName ?? "" },
                new SqliteParameter("@OrderByOption", DbType.String) { Value = p.OrderByOption ?? "" },
                new SqliteParameter("@pLimit", DbType.Int32) { Value = p.Limit },
                new SqliteParameter("@pOffset", DbType.Int32) { Value = p.Offset }
            };

            try
            {
                using (SqliteDataReader dr = SQLHelper.ExecuteReaderV2(
                    SQLHelper.connectionString,
                    CommandType.Text,
                    QueryManager.Get("v1_HeThong_ChucNang_GetPagingBySearch"),
                    parameters))
                {
                    while (dr.Read())
                    {
                        ChucNangModel item = new ChucNangModel();
                        item.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        item.TenChucNang = Utils.ConvertToString(dr["TenChucNang"], string.Empty);
                        item.ChucNangChaID = Utils.ConvertToInt32(dr["ChucNangChaID"], 0);
                        item.TenChucNangCha = Utils.ConvertToString(dr["TenChucNangCha"], string.Empty);
                        item.MaChucNang = Utils.ConvertToString(dr["MaChucNang"], string.Empty);

                        // lấy TotalRow từ cột trong resultset
                        TotalRow = Utils.ConvertToInt32(dr["TotalRow"], 0);

                        list.Add(item);
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

        public List<ChucNangModel> GetListChucNangByCoQuanID(int CoQuanID, int NguoiDungID)
        {
            List<ChucNangModel> Result = new List<ChucNangModel>();
            List<ChucNangModel> list = new List<ChucNangModel>();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(@"CoQuanID", SqlDbType.Int),
            };
            parameters[0].Value = CoQuanID;

            try
            {
                if (UserRole.CheckAdmin(NguoiDungID))
                {
                    var p = new BasePagingParams();
                    p.PageSize = 999;
                    var tt = 1;
                    list = new ChucNangDAL().GetPagingBySearch(p, ref tt);
                    //list.ForEach(x => Result.Add(new ChucNangModel(x.ChucNangID, x.TenChucNang, x.MaChucNang, 1)));
                    foreach (var item in list)
                    {
                        //if (item.ChucNangID < 200 && item.MaChucNang != "dm-trang-thai") Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 15));
                        //else Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 1));

                        if (item.MaChucNang == "checkin-out")
                        {
                            Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 3));
                        }
                        else if (item.MaChucNang == "bao-cao")
                        {
                            Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 1));
                        }
                        else
                        {
                            Result.Add(new ChucNangModel(item.ChucNangID, item.TenChucNang, item.MaChucNang, 15));
                        }
                    }
                }
                else
                {
                    //using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_ChucNang_GetByCoQuanID", parameters))
                    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_ChucNang_GetByCoQuanID", parameters))
                    {

                        while (dr.Read())
                        {
                            ChucNangModel item = new ChucNangModel(Utils.ConvertToInt32(dr["ChucNangID"], 0)
                                , Utils.ConvertToString(dr["TenChucNang"], String.Empty)
                                 , Utils.ConvertToString(dr["MaChucNang"], String.Empty)
                                , Utils.ConvertToInt32(dr["Quyen"], 0))
                                ;

                            list.Add(item);
                        }
                        dr.Close();
                        Result = (from m in list
                                  group m by new { m.ChucNangID, m.TenChucNang, m.MaChucNang } into chucNang
                                  select new ChucNangModel(chucNang.Key.ChucNangID, chucNang.Key.TenChucNang, chucNang.Key.MaChucNang, list.Where(x => x.ChucNangID == chucNang.Key.ChucNangID).ToList().Max(x => x.Quyen))
                                  //{
                                  //    ChucNangID = chucNang.Key.ChucNangID,
                                  //    TenChucNang = chucNang.Key.TenChucNang,
                                  //    MaChucNang = chucNang.Key.MaChucNang,
                                  //    Quyen = list.Where(x => x.ChucNangID == chucNang.Key.ChucNangID).ToList().Max(x => x.Quyen)

                                  //}
                                  ).ToList();

                    }

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
