using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface INguoiDungDAL
    {
        NguoiDungModel GetInfoByLogin(string UserName, string Password);
    }
    public class NguoiDungDAL : INguoiDungDAL
    {
        //public NguoiDungModel GetInfoByLogin(string UserName, string Password)
        //{
        //    var sql = QueryManager.Get("v1_HT_NguoiDung_GetByLogin");
        //    NguoiDungModel user = null;
        //    if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
        //    {
        //        return user;
        //    }
        //    SqlParameter[] parameters = new SqlParameter[]
        //    {
        //        new SqlParameter(@"TenNguoiDung",SqlDbType.NVarChar),
        //        new SqlParameter(@"MatKhau",SqlDbType.NVarChar),
        //    };

        //    parameters[0].Value = UserName.Trim();
        //    parameters[1].Value = Password.Trim();
        //    try
        //    {

        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_NguoiDung_GetByLogin", parameters))
        //        {
        //            if (dr.Read())
        //            {
        //                user = new NguoiDungModel();
        //                user.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], "");
        //                user.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], "");
        //                user.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
        //                user.CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0);
        //                //user.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], "");
        //                user.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
        //                user.TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0);
        //                user.CapCoQuan = Utils.ConvertToInt32(dr["CapID"], 0);
        //                user.VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0);
        //                user.AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], "");
        //                user.QuanLyThanNhan = user.VaiTro;
        //                // nếu người dùng có quyền quản lý cán bộ thì có quyền quản lý thân nhân
        //                var QuyenCuaCanBo = new ChucNangDAL().GetListChucNangByNguoiDungID(user.NguoiDungID);
        //                //if (QuyenCuaCanBo.Any(x => x.ChucNangID == ChucNangEnum.HeThong_QuanLy_CanBo.GetHashCode())) user.QuanLyThanNhan = EnumVaiTroCanBo.LanhDao.GetHashCode();
        //                user.TinhID= Utils.ConvertToInt32(dr["TinhID"], 0);
        //                user.RoleID = Utils.ConvertToInt32(dr["RoleID"], 0);
        //            }
        //            dr.Close();
        //        }

        //        if (user != null)
        //        {
        //            var CoQuanSuDungPhanMem = new DanhMucCoQuanDonViDAL().GetCoQuanSuDungPhanMem_By_CoQuanDangNhap(user.CoQuanID);
        //            user.CoQuanSuDungPhanMem = CoQuanSuDungPhanMem.CoQuanID;
        //            user.TenCoQuanSuDungPhanMem = CoQuanSuDungPhanMem.TenCoQuan;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return user;
        //}

        public NguoiDungModel GetInfoByLogin(string UserName, string Password)
        {
            var sql = QueryManager.Get("v1_HT_NguoiDung_GetByLogin");
            NguoiDungModel user = null;

            var parameters = new SqliteParameter[]
            {
                new SqliteParameter("@TenNguoiDung", UserName.Trim()),
                new SqliteParameter("@MatKhau", Password.Trim())
            };

            using (var dr = SQLHelper.ExecuteReaderV2(SQLHelper.connectionString, CommandType.Text, sql, parameters))
            {
                if (dr.Read())
                {
                    user = new NguoiDungModel
                    {
                        TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], ""),
                        TenCanBo = Utils.ConvertToString(dr["TenCanBo"], ""),
                        NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0),
                        CanBoID = Utils.ConvertToInt32(dr["CanBoID"], 0),
                        CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0),
                        TrangThai = Utils.ConvertToInt32(dr["TrangThai"], 0),
                        CapCoQuan = Utils.ConvertToInt32(dr["CapID"], 0),
                        VaiTro = Utils.ConvertToInt32(dr["VaiTro"], 0),
                        AnhHoSo = Utils.ConvertToString(dr["AnhHoSo"], ""),
                        QuanLyThanNhan = Utils.ConvertToInt32(dr["VaiTro"], 0),
                        TinhID = Utils.ConvertToInt32(dr["TinhID"], 0),
                        RoleID = Utils.ConvertToInt32(dr["RoleID"], 0)
                    };
                }
               
            }
            if (user != null)
            {
                var CoQuanSuDungPhanMem = new DanhMucCoQuanDonViDAL().GetCoQuanSuDungPhanMem_By_CoQuanDangNhap(user.CoQuanID);
                user.CoQuanSuDungPhanMem = CoQuanSuDungPhanMem.CoQuanID;
                user.TenCoQuanSuDungPhanMem = CoQuanSuDungPhanMem.TenCoQuan;
            }

            return user;
        }

    }
}
