using Com.Gosol.INOUT.Model.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public class CanBoDAL
    {
        #region contructor
        private static readonly Lazy<CanBoDAL> _instance = new Lazy<CanBoDAL>(() =>
        {
            return new CanBoDAL();
        });
        private CanBoDAL()
        {

        }
        public static CanBoDAL Instance { get => _instance.Value; }
        #endregion

        #region Database query string
        private const string V1_DM_CANBO_GETUSERINFOBYCANBOID = "v1_DM_CanBo_GetUserInfoByCanBoID";
        private const string V1_DM_CANBO_GETCANBOBYNHATHUOCID = "v1_DM_CanBo_GetCanBoByNhaThuocID";
        private const string V1_DM_CANBO_GETBYID = "v1_DM_CanBo_GetByID";
        #endregion

        #region paramaters constant
        private const string PARAM_CANBOID = @"CanBoID";
        private const string PARAM_TENCANBO = @"TenCanBo";
        private const string PARAM_NGAYSINH = @"NgaySinh";
        private const string PARAM_GIOITINH = @"GioiTinh";
        private const string PARAM_DIACHI = @"DiaChi";
        private const string PARAM_CHUOINHATHUOCID = @"ChuoiNhaThuocID";
        private const string PARAM_NHATHUOCID = @"NhaThuocID";
        private const string PARAM_KHOID = @"KhoID";
        private const string PARAM_EMAIL = @"Email";
        private const string PARAM_DIENTHOAI = @"DienThoai";
        private const string PARAM_ROLEID = @"RoleID";

        private const string PARAM_START = @"Start";
        private const string PARAM_END = @"End";
        private const string PARAM_KEYWORD = @"Keyword";
        #endregion

        #region insert , update
        #endregion

        #region get data
        private CanBoInfo GetData(SqlDataReader dr)
        {
            CanBoInfo info = new CanBoInfo();
            //info.CanBoID = Utils.GetInt32(dr["CanBoID"], 0);
            //info.TenCanBo = Utils.GetString(dr["TenCanBo"], String.Empty);
            //info.Email = Utils.GetString(dr["Email"], String.Empty);
            //info.DiaChi = Utils.GetString(dr["DiaChi"], String.Empty);
            //info.DienThoai = Utils.GetString(dr["DienThoai"], String.Empty);
            //info.ChuoiNhaThuocID = Utils.GetInt32(dr["ChuoiNhaThuocID"], 0);
            //info.RoleID = Utils.GetInt32(dr["RoleID"], 0);
            //info.KhoID = Utils.GetInt32(dr["KhoID"], 0);
            //info.NhaThuocID = Utils.GetInt32(dr["NhaThuocID"], 0);
            //info.NgaySinh = Utils.ConvertToDateTime(dr["NgaySinh"], DateTime.MinValue);
            //info.GioiTinh = Utils.ConvertToInt32(dr["GioiTinh"], 0);
            return info;
        }
        public CanBoInfo GetByID(int canBoID)
        {
            CanBoInfo info = null;
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(PARAM_CANBOID, SqlDbType.Int)
            };
            parameters[0].Value = canBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, V1_DM_CANBO_GETBYID, parameters))
                {
                    if (dr.Read())
                    {
                        info = GetData(dr);
                        info.NgaySinhStr = Format.FormatDate(info.NgaySinh);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return info;
        }
        public CanBoInfo GetUserInfoByCanBoID(int canBoID)
        {
            CanBoInfo info = null;
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(PARAM_CANBOID, SqlDbType.Int)
            };
            parameters[0].Value = canBoID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, V1_DM_CANBO_GETUSERINFOBYCANBOID, parameters))
                {
                    if (dr.Read())
                    {
                        info = GetData(dr);
                        info.TenChuoiNhaThuoc = Utils.GetString(dr["TenChuoiNhaThuoc"], String.Empty);
                        info.TenNhaThuoc = Utils.GetString(dr["TenNhaThuoc"], String.Empty);
                        info.RoleName = Utils.GetString(dr["RoleName"], String.Empty);
                        info.LoaiHinhThuc = Utils.GetInt32(dr["LoaiHinhThuc"], 0);
                        //info.NhaThuocParentID = Utils.GetInt32(dr["NhaThuocParentID"], 0);
                        info.NgaySinhStr = Format.FormatDate(info.NgaySinh);

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return info;
        }

        public List<NhanVienDTO> GetListCanBoByNhaThuocID(int nhaThuocID)
        {
            List<NhanVienDTO> lst = new List<NhanVienDTO>();
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter(PARAM_NHATHUOCID, SqlDbType.Int)
            };
            parameters[0].Value = nhaThuocID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, V1_DM_CANBO_GETCANBOBYNHATHUOCID, parameters))
                {
                    while (dr.Read())
                    {
                        NhanVienDTO info = new NhanVienDTO();
                        info.TenNhanVien = Utils.ConvertToString(dr["TenCanBo"], String.Empty);
                        //info.NhanVienID = Utils.ConvertToInt32(dr["CanBoID"], 0);
                        lst.Add(info);

                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return lst;
        }
        #endregion
    }
}
