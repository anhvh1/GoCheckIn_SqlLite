using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Security;

namespace Com.Gosol.INOUT.DAL.QuanTriHeThong
{
    public interface IPhanQuyenDAL
    {
        public List<NhomNguoiDungModel> NhomNguoiDung_GetPagingBySearch(BasePagingParamsForFilter p, int? CoQuanID, int NguoiDungID, ref int TotalCount);
        public BaseResultModel NhomNguoiDung_Insert(NhomNguoiDungModel NhomNguoiDungModel, int NguoiDungID, int CoQuanID);
        public BaseResultModel NhomNguoiDung_GetForUpdate(int? NhomNguoiDungID, int NguoiDungID, int CoQuanID);
        public BaseResultModel NhomNguoiDung_GetChiTietByID(int? NhomNguoiDungID, int NguoiDungID, int CoQuanID);
        public BaseResultModel NhomNguoiDung_Delete(int? NhomNguoiDungID, int CoQuanID, int NguoiDungID);
        public BaseResultModel NhomNguoiDung_Update(NhomNguoiDungModel NhomNguoiDungModel, int NguoiDungID, int CoQuanID);
        public BaseResultModel NguoiDung_NhomNguoiDung_Insert(NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel);
        public BaseResultModel NguoiDung_NhomNguoiDung_Delete(int? NguoiDungID, int? NhomNguoiDungID);
        public BaseResultModel PhanQuyen_Insert(PhanQuyenModel PhanQuyenModel);
        public BaseResultModel PhanQuyen_InsertMulti(List<PhanQuyenModel> PhanQuyenModel);
        public BaseResultModel PhanQuyen_Delete(int? PhanQuyenID);
        public BaseResultModel PhanQuyen_UpdateMulti(List<PhanQuyenModel> PhanQuyenModel);

        public List<DanhMucCoQuanDonViModel> DanhMucCoQuan_GetAllFoPhanQuyen(int CoQuanID, int NguoiDungID);
        public List<HeThongCanBoModel> HeThongCanBo_GetAllByListCoQuanID(int NhomNguoiDungID, int CoQuanID, int NguoiDungID);
        public List<HeThongNguoiDungModel> HeThongNguoiDung_GetAllByListCoQuanID(int NhomNguoiDungID);
        public List<HeThongNguoiDungModel> HeThong_NguoiDung_GetListBy_NhomNguoiDungID(int NhomNguoiDungID, int NguoiDungID);
        public List<NhomNguoiDungModel> NhomNguoiDung_GetAll(BasePagingParamsForFilter p, int? CoQuanID, int NguoiDungID, ref int TotalRow);

        public List<ChucNangModel> ChucNang_GetQuyenDuocThaoTacTrongNhom(int NhomNguoiDungID, int CoQuanID, int NguoiDungID);
        public List<NhomNguoiDungModel> GetListNhomNguoiDung_ByCoQuanID(int? CoQuanID);
    }
    public class PhanQuyenDAL : IPhanQuyenDAL
    {

        // param constant
        private const string PARAM_PhanQuyenID = @"PhanQuyenID";
        private const string PARAM_ConfigKey = @"ConfigKey";
        private const string PARAM_ConfigValue = @"ConfigValue";
        private const string PARAM_Description = @"Description";

        //private DbINOUTContext _DbINOUTContext;
        //public PhanQuyenDAL(DbINOUTContext dbINOUTContext)
        //{
        //    _DbINOUTContext = dbINOUTContext;
        //}

        #region NhomNguoidung
        // đang ko dùng
        public List<NhomNguoiDungModel> NhomNguoiDung_GetPagingBySearch(BasePagingParamsForFilter p, int? CoQuanID, int NguoiDungID, ref int TotalRow)
        {
            List<NhomNguoiDungModel> list = new List<NhomNguoiDungModel>();
            var pList = new SqlParameter("@DanhSachCoQuan", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("ID", typeof(string));

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("OrderByName",SqlDbType.NVarChar,50),
                new SqlParameter("OrderByOption",SqlDbType.NVarChar,50),
                new SqlParameter("pLimit",SqlDbType.Int),
                new SqlParameter("pOffset",SqlDbType.Int),
                new SqlParameter("CoQuanID",SqlDbType.Int),
                new SqlParameter("TotalRow",SqlDbType.Int),
                  pList,
                new SqlParameter("CoQuanFilter",SqlDbType.Int),
                new SqlParameter("CanBoID",SqlDbType.Int),
            };
            parameters[0].Value = (p.Keyword != null && p.Keyword.Trim().Length > 0) ? p.Keyword.Trim() : "";
            parameters[1].Value = p.OrderByName.Trim();
            parameters[2].Value = p.OrderByOption.Trim();
            parameters[3].Value = p.Limit;
            parameters[4].Value = p.Offset;
            parameters[5].Value = CoQuanID;
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                parameters[5].Value = Convert.DBNull;
            }
            parameters[6].Direction = ParameterDirection.Output;
            parameters[6].Size = 8;

            var storeName = "v1_HeThong_PhanQuyen_NhomNguoidung_GetPagingBySearch_New";
            if (UserRole.CheckAdmin(NguoiDungID))
            {
                storeName = "v1_HeThong_PhanQuyen_NhomNguoidung_GetPagingBySearch_FoAdmin";
            }
            else
            {
                var listCoQuan = new DanhMucCoQuanDonViDAL().DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap(CoQuanID);
                listCoQuan.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            }
            parameters[7].Value = tbCoQuanID;
            parameters[8].Value = p.CoQuanID ?? Convert.DBNull;
            parameters[9].Value = p.CanBoID ?? Convert.DBNull;

            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, storeName, parameters))
                {
                    while (dr.Read())
                    {
                        list.Add(new NhomNguoiDungModel(Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0), Utils.ConvertToString(dr["TenNhom"], string.Empty), Utils.ConvertToString(dr["GhiChu"], string.Empty), Utils.ConvertToInt32(dr["ApDungCho"], 0)));
                    }
                    dr.Close();
                }
                TotalRow = Utils.ConvertToInt32(parameters[6].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }


        /// <summary>
        /// lấy danh sách nhóm người dùng
        /// tìm kiếm, lọc dữ liệu
        /// dùng thay cho hàm getpagingbysearch
        /// </summary>
        /// <param name="p"></param>
        /// <param name="CoQuanID"></param>
        /// <param name="NguoiDungID"></param>
        /// <param name="TotalRow"></param>
        /// <returns></returns>
        public List<NhomNguoiDungModel> NhomNguoiDung_GetAll(BasePagingParamsForFilter p, int? CoQuanID, int NguoiDungID, ref int TotalRow)
        {
            List<NhomNguoiDungModel> listAll = new List<NhomNguoiDungModel>();
            List<NhomNguoiDungModel> Result = new List<NhomNguoiDungModel>();
            var pList = new SqlParameter("@DanhSachCoQuan", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("ID", typeof(string));

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("Keyword",SqlDbType.NVarChar,200),
                new SqlParameter("CoQuanFilter",SqlDbType.Int),
                new SqlParameter("CanBoID",SqlDbType.Int),
            };
            parameters[0].Value = (p.Keyword != null && p.Keyword.Trim().Length > 0) ? p.Keyword.Trim() : "";

            var storeName = "v1_HeThong_PhanQuyen_NhomNguoidung_GetAll";
            parameters[1].Value = p.CoQuanID ?? Convert.DBNull;
            parameters[2].Value = p.CanBoID ?? Convert.DBNull;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, storeName, parameters))
                {
                    while (dr.Read())
                    {
                        var item = new NhomNguoiDungModel();
                        item.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        item.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        item.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        item.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        item.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        item.NhomTongID = Utils.ConvertToInt32(dr["NhomTongID"], 0);
                        item.CoQuanTao = Utils.ConvertToInt32(dr["CoQuanTao"], 0);
                        listAll.Add(item);
                    }
                    dr.Close();
                }
                //TotalRow = Utils.ConvertToInt32(parameters[6].Value, 0);

                if (UserRole.CheckAdmin(NguoiDungID))
                {
                    if ((p.CoQuanID == null || p.CoQuanID < 1) && (p.CanBoID == null || p.CanBoID < 1))
                    {
                        Result = listAll.Where(x => x.NhomTongID == 0).ToList();
                    }
                    else
                        Result = listAll;
                }
                else
                {
                    var listCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID.Value);
                    var listCoQuanID = listCoQuan.Where(x => x.CoQuanID != CoQuanID).Select(x => x.CoQuanID).ToList();
                    // add danh sách nhóm người dùng do chính cơ quan của người dùng đang đăng nhập tạo
                    Result.AddRange(listAll.Where(x => x.CoQuanTao == CoQuanID));
                    // danh sách nhóm người dùng còn lại
                    var listConLai = listAll.Where(x => x.CoQuanTao != CoQuanID && !Result.Select(y => y.NhomNguoiDungID).ToList().Contains(x.NhomTongID.Value)).ToList();
                    // add danh sách nhóm người dùng mà cơ quan cấp trên tạo cho đơn vị của người dùng đang đăng nhập
                    Result.AddRange(listConLai.Where(x => x.CoQuanID == CoQuanID));

                    // add danh sách nhóm người dùng của cơ quan cấp con
                    //Result.AddRange(listConLai.Where(x => x.CoQuanID != CoQuanID && listCoQuanID.Contains(x.CoQuanID)).ToList());
                    var listNhomCuaDonViCon = listConLai.Where(x => x.CoQuanID != CoQuanID).ToList();
                    for (int i = 0; i < listNhomCuaDonViCon.Count; i++)
                    {
                        var listDaCo = Result.Select(x => new { x.NhomNguoiDungID, x.NhomTongID }).ToList();
                        if (listCoQuanID.Contains(listNhomCuaDonViCon[i].CoQuanID)
                            && !listDaCo.Select(x => x.NhomNguoiDungID).ToList().Contains(listNhomCuaDonViCon[i].NhomNguoiDungID)
                            && !listDaCo.Select(x => x.NhomTongID).ToList().Contains(listNhomCuaDonViCon[i].NhomNguoiDungID)
                             && !listDaCo.Select(x => x.NhomTongID).ToList().Contains(listNhomCuaDonViCon[i].NhomTongID)
                            && !listDaCo.Select(x => x.NhomNguoiDungID).ToList().Contains(listNhomCuaDonViCon[i].NhomTongID.Value)
                            )
                        {
                            Result.Add(listNhomCuaDonViCon[i]);
                        }

                    }

                    //for (int i = 0; i < listConLai.Count; i++)
                    //{
                    //    var listNhomCon = listConLai.Where(x => x.NhomTongID == listConLai[i].NhomNguoiDungID && listCoQuan.Select(y => y.CoQuanID).ToList().Contains(x.CoQuanID) && x.CoQuanID != CoQuanID).ToList();
                    //    if (listNhomCon.Count > 0)
                    //    {
                    //        Result.Add(listConLai[i]);
                    //    }
                    //    else if (listConLai[i].CoQuanID == CoQuanID)
                    //    {
                    //        if (!Result.Any(x => x.NhomNguoiDungID == listConLai[i].NhomTongID || x.NhomTongID == listConLai[i].NhomNguoiDungID))
                    //        {
                    //            Result.Add(listConLai[i]);
                    //        }
                    //    }
                    //}
                    if ((p.CoQuanID != null && p.CoQuanID > 0) || p.CanBoID != null && p.CanBoID > 0)
                    {
                        Result.AddRange(listConLai.Where(x => x.CoQuanID == p.CoQuanID));
                    }
                }
                Result = (from m in Result
                          group m by new { m.NhomNguoiDungID, m.ApDungCho, m.CoQuanID, m.GhiChu, m.NhomTongID, m.TenNhom } into nhom
                          select new NhomNguoiDungModel()
                          {
                              NhomNguoiDungID = nhom.Key.NhomNguoiDungID,
                              ApDungCho = nhom.Key.ApDungCho,
                              CoQuanID = nhom.Key.CoQuanID,
                              GhiChu = nhom.Key.GhiChu,
                              NhomTongID = nhom.Key.NhomTongID,
                              TenNhom = nhom.Key.TenNhom,
                          }
                        ).ToList();
                Result.GroupBy(x => x.NhomNguoiDungID).ToList();
                if (Result.Count > 0)
                {
                    TotalRow = Result.Count;
                    Result = Result.OrderByDescending(x => x.NhomNguoiDungID).ToList();
                    Result = Result.Skip((p.PageNumber - 1) * p.PageSize).Take(p.PageSize).ToList();
                }
                else
                {
                    TotalRow = 0;
                }
            }


            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }


        //public NhomNguoiDungModel NhomNguoiDung_GetList_ByListCoQuan(List<int> DanhSachCoQuan)
        //{
        //    NhomNguoiDungModel Result = new NhomNguoiDungModel();
        //    SqlParameter[] parameters = new SqlParameter[]
        //      {
        //        new SqlParameter(@"NhomNguoiDungID",SqlDbType.NVarChar)
        //      };
        //    parameters[0].Value = NhomNguoiDungID;
        //    try
        //    {
        //        using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_GetByID", parameters))
        //        {
        //            while (dr.Read())
        //            {
        //                Result.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
        //                Result.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
        //                Result.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
        //                Result.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
        //                Result.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
        //                Result.NhomTongID = Utils.ConvertToInt32(dr["NhomTongID"], 0);
        //                break;
        //            }
        //            dr.Close();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return Result;
        //}

        public BaseResultModel NhomNguoiDung_Insert(NhomNguoiDungModel NhomNguoiDungModel, int NguoiDungID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NhomNguoiDungModel == null || NhomNguoiDungModel.TenNhom == null || NhomNguoiDungModel.TenNhom.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được trống";
                }
                else if (NhomNguoiDungModel.TenNhom.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được quá 200 ký tự";
                }
                else if (NhomNguoiDungModel.GhiChu != null && NhomNguoiDungModel.GhiChu.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Ghi chú không được quá 200 ký tự";
                }
                else
                {
                    var crObj = NhomNguoiDung_GetByName(NhomNguoiDungModel.TenNhom);
                    if (crObj != null && crObj.NhomNguoiDungID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Tên nhóm người dùng đã tồn tại";
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                            new SqlParameter("TenNhom", SqlDbType.NVarChar),
                            new SqlParameter("GhiChu", SqlDbType.NVarChar),
                            new SqlParameter("CoQuanID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongOut",SqlDbType.Int),
                            new SqlParameter("ApDungCho", SqlDbType.TinyInt),
                            new SqlParameter("CoQuanTao", SqlDbType.TinyInt)
                           ,
                          };
                        parameters[0].Value = NhomNguoiDungModel.TenNhom.Trim();
                        parameters[1].Value = NhomNguoiDungModel.GhiChu.Trim();
                        parameters[2].Value = Convert.DBNull; //NhomNguoiDungModel.CoQuanID;
                        parameters[3].Value = 0;
                        parameters[4].Direction = ParameterDirection.Output;
                        parameters[4].Size = 8;
                        if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
                            parameters[5].Value = 1;
                        else parameters[5].Value = Convert.DBNull;
                        parameters[6].Value = CoQuanID;
                        if (UserRole.CheckAdmin(NguoiDungID))
                        {
                            parameters[6].Value = Convert.DBNull;
                        }

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_Insert", parameters);
                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    throw ex;
                                }
                            }
                            if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
                            {
                                NhomNguoiDungModel.DanhSachCoQuanID = new List<int>();
                                var listCoQuan = new List<DanhMucCoQuanDonViModel>();
                                // nếu là Admin hệ thống thì tạo cho toàn bộ hệ thống
                                if (UserRole.CheckAdmin(NguoiDungID))
                                {
                                    listCoQuan = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(0).Where(x => x.CoQuanID != CoQuanID).ToList();
                                }
                                // nếu ko thì tạo cho toàn bộ đơn vị trực thuộc đơn vị
                                else
                                {
                                    //listCoQuan = new DanhMucCoQuanDonViDAL().GetListByUser_Phang(CoQuanID, NguoiDungID);
                                    listCoQuan = new DanhMucCoQuanDonViDAL().DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap(CoQuanID);

                                }
                                if (listCoQuan.Count > 0)
                                {
                                    listCoQuan.ForEach(x => NhomNguoiDungModel.DanhSachCoQuanID.Add(x.CoQuanID));
                                }
                                //if (UserRole.CheckAdmin(NguoiDungID))
                                //{
                                //    NhomNguoiDungModel.DanhSachCoQuanID.Add(CoQuanID);

                                //}
                            }
                            //else if (NhomNguoiDungModel.DanhSachCoQuanID.Count > 0)
                            //{
                            for (int i = 0; i < NhomNguoiDungModel.DanhSachCoQuanID.Count; i++)
                            {
                                NhomNguoiDungModel.CoQuanID = NhomNguoiDungModel.DanhSachCoQuanID[i];
                                NhomNguoiDungModel.NhomTongID = Utils.ConvertToInt32(parameters[4].Value, 0); ;
                                var query = NhomNguoiDung_InsertOne(NhomNguoiDungModel);
                                if (query.Status < 1)
                                {
                                    return query;
                                }
                            }
                            //}
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("Nhóm người dùng");
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
            if (Result.Status > 0)
            {
                Result.Status = 1;
            }
            return Result;
        }

        /// <summary>
        /// Lấy thông tin nhóm người dùng khi sửa nhóm người dùng
        /// </summary>
        /// <param name="NhomNguoiDungID"></param>
        /// <param name="NguoiDungID"></param>
        /// <returns></returns>
        public BaseResultModel NhomNguoiDung_GetForUpdate(int? NhomNguoiDungID, int NguoiDungID, int CoQuanID)
        {
            NhomNguoiDungModel Data = new NhomNguoiDungModel();
            BaseResultModel Result = new BaseResultModel();
            try
            {
                if (NhomNguoiDungID == null || NhomNguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng chọn Nhóm người dùng trức khi thực hiện thao tác!";
                }
                else
                {
                    Data = NhomNguoiDung_GetByID(NhomNguoiDungID.Value);
                    if (Data == null || Data.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Nhóm người dùng không tồn tại!";
                    }
                    else
                    {
                        var listDonViCoTrongNhomNguoiDung = new List<NhomNguoiDungModel>();
                        if (UserRole.CheckAdmin(NguoiDungID))
                            listDonViCoTrongNhomNguoiDung = NhomNguoiDung_GetByNhomTongID(Data.NhomTongID == 0 ? Data.NhomNguoiDungID : Data.NhomTongID.Value);
                        else
                            listDonViCoTrongNhomNguoiDung = NhomNguoiDung_GetListNhomCuaDonViCon(Data.NhomNguoiDungID, CoQuanID);

                        var listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(Data.CoQuanID);
                        Data.DanhSachCoQuan = new List<CoQuanModel>();
                        if (UserRole.CheckAdmin(NguoiDungID))
                        {
                            listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).Where(x => x.CoQuanID != Data.CoQuanID).ToList();
                        }
                        //if (listDonViCoTrongNhomNguoiDung.Count != listDonViTrucThuoc.Count)
                        //{

                        //    if (listDonViCoTrongNhomNguoiDung.Count > 0)
                        //    {

                        //        listDonViCoTrongNhomNguoiDung.ForEach(x => Data.DanhSachCoQuan.Add(new CoQuanModel(x.CoQuanID, x.TenCoQuan)));
                        //    }
                        //}

                        if (listDonViCoTrongNhomNguoiDung.Count < listDonViTrucThuoc.Count)
                        {
                            //if (listDonViTrucThuoc.Any(x => !listDonViCoTrongNhomNguoiDung.Select(y => y.CoQuanID).ToList().Contains(x.CoQuanID)))
                            //{
                            //    listDonViCoTrongNhomNguoiDung.ForEach(x => Data.DanhSachCoQuan.Add(new CoQuanModel(x.CoQuanID, x.TenCoQuan)));
                            //} 
                            if (listDonViCoTrongNhomNguoiDung.Count > 0)
                            {
                                listDonViCoTrongNhomNguoiDung.ForEach(x => Data.DanhSachCoQuan.Add(new CoQuanModel(x.CoQuanID, x.TenCoQuan)));
                            }
                        }

                    }
                }
                Result.Status = 1;
                Result.Data = Data;
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                Result.Data = null;
                throw ex;
            }
            return Result;
        }

        public BaseResultModel NhomNguoiDung_GetChiTietByID(int? NhomNguoiDungID, int NguoiDungID, int CoQuanID)
        {
            NhomNguoiDungDetailModel Data = new NhomNguoiDungDetailModel();
            BaseResultModel Result = new BaseResultModel();
            try
            {
                if (NhomNguoiDungID == null || NhomNguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng chọn Nhóm người dùng trước khi thực hiện thao tác!";
                    return Result;
                }

                else
                {
                    var crNhomNguoiDung = NhomNguoiDung_GetByID(NhomNguoiDungID.Value);
                    if (crNhomNguoiDung == null || crNhomNguoiDung.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Nhóm người dùng không tồn tại!";
                        return Result;
                    }
                    else
                    {
                        Data.NhomNguoiDungID = crNhomNguoiDung.NhomNguoiDungID;
                        Data.TenNhom = crNhomNguoiDung.TenNhom;
                        Data.ApDungCho = crNhomNguoiDung.ApDungCho;
                        Data.CoQuanID = crNhomNguoiDung.CoQuanID;
                        //   var lstNguodung = NguoiDung_NhomNguoiDung_GetByNhomNguoiDungID(NhomNguoiDungID.Value);
                        //if (UserRole.CheckAdmin(NguoiDungID))
                        //{
                        var listDonViCoTrongNhomNguoiDungAll = NhomNguoiDung_GetByNhomTongID(crNhomNguoiDung.NhomTongID == 0 ? crNhomNguoiDung.NhomNguoiDungID : crNhomNguoiDung.NhomTongID.Value);
                        var listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
                        //.GetListByUser_Phang(CoQuanID, NguoiDungID);
                        var DanhSachCoQuanID = new List<int>();
                        if (UserRole.CheckAdmin(NguoiDungID))
                        {
                            listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(0);
                        }
                        var listDonViCoTrongNhomNguoiDung = listDonViCoTrongNhomNguoiDungAll.Where(x => listDonViTrucThuoc.Select(y => y.CoQuanID).ToList().Contains(x.CoQuanID)).ToList();
                        var lstNguodung = NguoiDung_NhomNguoiDung_GetByListNhomNguoiDungID(listDonViCoTrongNhomNguoiDung.Select(x => x.NhomNguoiDungID).ToList())
                                         .Select(x => new NguoiDungModel(x.NguoiDungID, x.TenNguoiDung)).ToList();
                        //}

                        if (lstNguodung != null && lstNguodung.Count > 0)
                        {
                            Data.DanhSachNguoiDung = lstNguodung;
                        }
                        var lstChucNang = ChucNang_GetQuyenByNhomNguoiDungID(NhomNguoiDungID.Value);
                        if (lstChucNang != null && lstChucNang.Count > 0)
                        {
                            Data.DanhSachChucNang = lstChucNang;
                        }
                    }
                }
                Result.Status = 1;
                Result.Data = Data;
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                Result.Data = null;
                throw ex;
            }
            return Result;
        }

        public BaseResultModel NhomNguoiDung_Delete(int? NhomNguoiDungID, int CoQuanID, int NguoiDungID)
        {
            var Result = new BaseResultModel();
            if (NhomNguoiDungID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(NhomNguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = NhomNguoiDung_GetByID(NhomNguoiDungID.Value);
                    if (crObj == null || crObj.NhomNguoiDungID == null || crObj.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không tồn tại";
                        return Result;
                    }
                    //var listNguoiDung = NguoiDung_NhomNguoiDung_GetByNhomNguoiDungID(NhomNguoiDungID.Value);
                    // lấy danh sách người dùng đã có trong nhom, nhóm con
                    //var listDonViCoTrongNhomNguoiDungAll = NhomNguoiDung_GetByNhomTongID(crObj.NhomTongID == 0 ? crObj.NhomNguoiDungID : crObj.NhomTongID.Value);
                    var listDonViCoTrongNhomNguoiDungAll = NhomNguoiDung_GetListNhomCuaDonViCon(crObj.NhomTongID == 0 ? crObj.NhomNguoiDungID : crObj.NhomTongID.Value, CoQuanID);
                    //var listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetListByUser_Phang(CoQuanID, NguoiDungID);
                    var listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().DanhSachCoQuan_TrongCoQuanSuDungPhanMem_ByCoQuanDangNhap(CoQuanID);
                    //var DanhSachCoQuanID = new List<int>();
                    if (UserRole.CheckAdmin(NguoiDungID))
                    {
                        listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(0);
                    }
                    var listDonViCoTrongNhomNguoiDung = listDonViCoTrongNhomNguoiDungAll.Where(x => listDonViTrucThuoc.Select(y => y.CoQuanID).ToList().Contains(x.CoQuanID)).ToList();
                    var lstNguodung = NguoiDung_NhomNguoiDung_GetByListNhomNguoiDungID(listDonViCoTrongNhomNguoiDung.Select(x => x.NhomNguoiDungID).ToList())
                                     .Select(x => new NguoiDungModel(x.NguoiDungID, x.TenNguoiDung)).ToList();

                    ///////
                    //var listChucNang = ChucNang_GetQuyenByNhomNguoiDungID(NhomNguoiDungID.Value);
                    if (lstNguodung != null && lstNguodung.Count > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Nhóm người dùng đã được sử dụng không thể xóa";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                        };
                        parameters[0].Value = NhomNguoiDungID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_DeleteByID", parameters);
                                    trans.Commit();
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
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Nhóm người dùng");
                return Result;
            }
        }

        public BaseResultModel NhomNguoiDung_Update(NhomNguoiDungModel NhomNguoiDungModel, int NguoiDungID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NhomNguoiDungModel == null || NhomNguoiDungModel.TenNhom == null || NhomNguoiDungModel.TenNhom.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được trống";
                }
                else if (NhomNguoiDungModel.TenNhom.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được quá 200 ký tự";
                }
                else if (NhomNguoiDungModel.GhiChu != null && NhomNguoiDungModel.GhiChu.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Ghi chú không được quá 200 ký tự";
                }
                else
                {
                    var crObj = NhomNguoiDung_GetByID(NhomNguoiDungModel.NhomNguoiDungID);
                    if (crObj == null || crObj.NhomNguoiDungID == null || crObj.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Nhóm người dùng không tồn tại";
                    }
                    else
                    {
                        var listTrungTen = NhomNguoiDung_SearchByName(NhomNguoiDungModel.TenNhom).Where(x => x.NhomNguoiDungID != NhomNguoiDungModel.NhomNguoiDungID && x.NhomTongID != NhomNguoiDungModel.NhomNguoiDungID && x.NhomTongID.Value != 0).ToList();
                        if (listTrungTen != null && listTrungTen.Count > 0)
                        {
                            Result.Status = 0;
                            Result.Message = "Tên Nhóm người dùng đã tồn tại";
                            //return Result;
                        }
                        else
                        {
                            SqlParameter[] parameters = new SqlParameter[]
                            {
                                new SqlParameter("NhomNguoiDungID", SqlDbType.Int),
                                new SqlParameter("TenNhom", SqlDbType.NVarChar),
                                new SqlParameter("GhiChu", SqlDbType.NVarChar),
                                new SqlParameter("ApDungCho", SqlDbType.TinyInt),
                            };
                            parameters[0].Value = NhomNguoiDungModel.NhomNguoiDungID;
                            parameters[1].Value = NhomNguoiDungModel.TenNhom.Trim();
                            parameters[2].Value = NhomNguoiDungModel.GhiChu.Trim();
                            parameters[3].Value = (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1) ? 1 : Convert.DBNull;
                            using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                            {
                                conn.Open();
                                var listQuyen = ChucNang_GetQuyenByNhomNguoiDungID(NhomNguoiDungModel.NhomNguoiDungID);
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    try
                                    {
                                        Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_UpdateNhomTong", parameters);
                                        //if (UserRole.CheckAdmin(NguoiDungID))
                                        //{
                                        //    Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_UpdateNhomTong", parameters);
                                        //}
                                        //else
                                        //{
                                        //    Result.Status = SQLHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_Update", parameters);
                                        //}
                                        trans.Commit();
                                        Result.Status = 1;
                                        Result.Message = ConstantLogMessage.Alert_Update_Success("Nhóm người dùng");
                                    }
                                    catch
                                    {
                                        Result.Status = -1;
                                        Result.Message = ConstantLogMessage.API_Error_System;
                                        trans.Rollback();
                                        throw;
                                    }
                                }
                                if (UserRole.CheckAdmin(NguoiDungID)) // nếu là admin thì sửa như này
                                {
                                    if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
                                    {
                                        //if (crObj.ApDungCho != 1)
                                        //{
                                        var listAllCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).ToList();
                                        var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetByNhomTongID(NhomNguoiDungModel.NhomNguoiDungID);
                                        //var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetListNhomCuaDonViCon(NhomNguoiDungModel.NhomNguoiDungID, CoQuanID);
                                        var listCoQuanConThieu = listAllCoQuan.Where(x => !listCoQuanDangCoNhomNguoiDung.Select(a => a.CoQuanID).Contains(x.CoQuanID)).ToList();
                                        if (listCoQuanConThieu.Count > 0)
                                        {
                                            for (int i = 0; i < listCoQuanConThieu.Count; i++)
                                            {
                                                var crNhomNguoiDung = new NhomNguoiDungModel();
                                                crNhomNguoiDung.CoQuanID = listCoQuanConThieu[i].CoQuanID;
                                                crNhomNguoiDung.NhomTongID = NhomNguoiDungModel.NhomNguoiDungID;
                                                crNhomNguoiDung.TenNhom = NhomNguoiDungModel.TenNhom;
                                                crNhomNguoiDung.GhiChu = NhomNguoiDungModel.GhiChu;

                                                var isNhomNguoiDung = NhomNguoiDung_InsertOne(crNhomNguoiDung);
                                                if (isNhomNguoiDung.Status < 1)
                                                {
                                                    return isNhomNguoiDung;
                                                }
                                                // insert quyền cho nhóm người dùng mới tương ứng với quyền của nhóm tổng
                                                for (int j = 0; j < listQuyen.Count; j++)
                                                {
                                                    var crPhanQuyen = new PhanQuyenModel();
                                                    crPhanQuyen.ChucNangID = listQuyen[j].ChucNangID;
                                                    crPhanQuyen.Quyen = listQuyen[j].Quyen;
                                                    crPhanQuyen.NhomNguoiDungID = Utils.ConvertToInt32(isNhomNguoiDung.Data, 0);
                                                    var isPhanQuyen = PhanQuyen_InsertOne(crPhanQuyen);
                                                }
                                            }
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetByNhomTongID(NhomNguoiDungModel.NhomNguoiDungID);
                                        //var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetListNhomCuaDonViCon(NhomNguoiDungModel.NhomNguoiDungID, CoQuanID);
                                        var listCoQuanConThieu = NhomNguoiDungModel.DanhSachCoQuanID.Where(x => !listCoQuanDangCoNhomNguoiDung.Select(a => a.CoQuanID).Contains(x)).ToList();
                                        var listCoQuanBiXoa = listCoQuanDangCoNhomNguoiDung.Where(x => !NhomNguoiDungModel.DanhSachCoQuanID.Contains(x.CoQuanID)).ToList();
                                        if (listCoQuanConThieu.Count > 0)
                                        {
                                            // add them nhomnguoidung cho cơ quan còn thiếu
                                            for (int i = 0; i < listCoQuanConThieu.Count; i++)
                                            {
                                                var crNhomNguoiDung = new NhomNguoiDungModel();
                                                crNhomNguoiDung.CoQuanID = listCoQuanConThieu[i];
                                                crNhomNguoiDung.NhomTongID = NhomNguoiDungModel.NhomNguoiDungID;
                                                crNhomNguoiDung.TenNhom = NhomNguoiDungModel.TenNhom;
                                                crNhomNguoiDung.GhiChu = NhomNguoiDungModel.GhiChu;
                                                var isNhomNguoiDung = NhomNguoiDung_InsertOne(crNhomNguoiDung);
                                                if (isNhomNguoiDung.Status < 1)
                                                {
                                                    return isNhomNguoiDung;
                                                }
                                                // insert quyền cho nhóm người dùng mới tương ứng với quyền của nhóm tổng
                                                for (int j = 0; j < listQuyen.Count; j++)
                                                {
                                                    var crPhanQuyen = new PhanQuyenModel();
                                                    crPhanQuyen.ChucNangID = listQuyen[j].ChucNangID;
                                                    crPhanQuyen.Quyen = listQuyen[j].Quyen;
                                                    crPhanQuyen.NhomNguoiDungID = Utils.ConvertToInt32(isNhomNguoiDung.Data, 0);
                                                    var isPhanQuyen = PhanQuyen_InsertOne(crPhanQuyen);
                                                }
                                            }
                                        }
                                        if (listCoQuanBiXoa.Count > 0)
                                        {
                                            // xóa nhóm người dùng của những cơ quan bị loại ra
                                            for (int i = 0; i < listCoQuanBiXoa.Count; i++)
                                            {
                                                var dlPhanQuyen = PhanQuyen_DeleteByNhomNguoiDungID(listCoQuanBiXoa[i].NhomNguoiDungID);
                                                var dlNguoiDung_NhomNguoiDung = NguoiDung_NhomNguoiDung_DeleteByNhomNguoiDungID(listCoQuanBiXoa[i].NhomNguoiDungID);
                                                var dlNhomNguoiDung = NhomNguoiDung_DeleteOne(listCoQuanBiXoa[i].NhomNguoiDungID);
                                            }
                                        }

                                    }
                                }
                                else // nếu ko phải admin thì sửa như thế nào
                                {
                                    if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
                                    {
                                        //if (crObj.ApDungCho != 1)
                                        //{
                                        var listAllCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID).ToList();
                                        //var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetByNhomTongID(NhomNguoiDungModel.NhomNguoiDungID);
                                        var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetListNhomCuaDonViCon(NhomNguoiDungModel.NhomNguoiDungID, CoQuanID);
                                        var listCoQuanConThieu = listAllCoQuan.Where(x => !listCoQuanDangCoNhomNguoiDung.Select(a => a.CoQuanID).Contains(x.CoQuanID)).ToList();
                                        if (listCoQuanConThieu.Count > 0)
                                        {
                                            for (int i = 0; i < listCoQuanConThieu.Count; i++)
                                            {
                                                var crNhomNguoiDung = new NhomNguoiDungModel();
                                                crNhomNguoiDung.CoQuanID = listCoQuanConThieu[i].CoQuanID;
                                                crNhomNguoiDung.NhomTongID = NhomNguoiDungModel.NhomNguoiDungID;
                                                crNhomNguoiDung.TenNhom = NhomNguoiDungModel.TenNhom;
                                                crNhomNguoiDung.GhiChu = NhomNguoiDungModel.GhiChu;

                                                var isNhomNguoiDung = NhomNguoiDung_InsertOne(crNhomNguoiDung);
                                                if (isNhomNguoiDung.Status < 1)
                                                {
                                                    return isNhomNguoiDung;
                                                }
                                                // insert quyền cho nhóm người dùng mới tương ứng với quyền của nhóm tổng
                                                for (int j = 0; j < listQuyen.Count; j++)
                                                {
                                                    var crPhanQuyen = new PhanQuyenModel();
                                                    crPhanQuyen.ChucNangID = listQuyen[j].ChucNangID;
                                                    crPhanQuyen.Quyen = listQuyen[j].Quyen;
                                                    crPhanQuyen.NhomNguoiDungID = Utils.ConvertToInt32(isNhomNguoiDung.Data, 0);
                                                    var isPhanQuyen = PhanQuyen_InsertOne(crPhanQuyen);
                                                }
                                            }
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        //var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetByNhomTongID(NhomNguoiDungModel.NhomNguoiDungID);
                                        var listCoQuanDangCoNhomNguoiDung = NhomNguoiDung_GetListNhomCuaDonViCon(NhomNguoiDungModel.NhomNguoiDungID, CoQuanID);
                                        var listCoQuanConThieu = NhomNguoiDungModel.DanhSachCoQuanID.Where(x => !listCoQuanDangCoNhomNguoiDung.Select(a => a.CoQuanID).Contains(x)).ToList();
                                        var listCoQuanBiXoa = listCoQuanDangCoNhomNguoiDung.Where(x => !NhomNguoiDungModel.DanhSachCoQuanID.Contains(x.CoQuanID)).ToList();
                                        if (listCoQuanConThieu.Count > 0)
                                        {
                                            // add them nhomnguoidung cho cơ quan còn thiếu
                                            for (int i = 0; i < listCoQuanConThieu.Count; i++)
                                            {
                                                var crNhomNguoiDung = new NhomNguoiDungModel();
                                                crNhomNguoiDung.CoQuanID = listCoQuanConThieu[i];
                                                crNhomNguoiDung.NhomTongID = NhomNguoiDungModel.NhomNguoiDungID;
                                                crNhomNguoiDung.TenNhom = NhomNguoiDungModel.TenNhom;
                                                crNhomNguoiDung.GhiChu = NhomNguoiDungModel.GhiChu;
                                                var isNhomNguoiDung = NhomNguoiDung_InsertOne(crNhomNguoiDung);
                                                if (isNhomNguoiDung.Status < 1)
                                                {
                                                    return isNhomNguoiDung;
                                                }
                                                // insert quyền cho nhóm người dùng mới tương ứng với quyền của nhóm tổng
                                                for (int j = 0; j < listQuyen.Count; j++)
                                                {
                                                    var crPhanQuyen = new PhanQuyenModel();
                                                    crPhanQuyen.ChucNangID = listQuyen[j].ChucNangID;
                                                    crPhanQuyen.Quyen = listQuyen[j].Quyen;
                                                    crPhanQuyen.NhomNguoiDungID = Utils.ConvertToInt32(isNhomNguoiDung.Data, 0);
                                                    var isPhanQuyen = PhanQuyen_InsertOne(crPhanQuyen);
                                                }
                                            }
                                        }
                                        if (listCoQuanBiXoa.Count > 0)
                                        {
                                            // xóa nhóm người dùng của những cơ quan bị loại ra
                                            for (int i = 0; i < listCoQuanBiXoa.Count; i++)
                                            {
                                                var dlPhanQuyen = PhanQuyen_DeleteByNhomNguoiDungID(listCoQuanBiXoa[i].NhomNguoiDungID);
                                                var dlNguoiDung_NhomNguoiDung = NguoiDung_NhomNguoiDung_DeleteByNhomNguoiDungID(listCoQuanBiXoa[i].NhomNguoiDungID);
                                                var dlNhomNguoiDung = NhomNguoiDung_DeleteOne(listCoQuanBiXoa[i].NhomNguoiDungID);
                                            }
                                        }

                                    }

                                }

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


        public NhomNguoiDungModel NhomNguoiDung_GetByID(int NhomNguoiDungID)
        {
            NhomNguoiDungModel Result = new NhomNguoiDungModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomNguoiDungID",SqlDbType.NVarChar)
              };
            parameters[0].Value = NhomNguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        Result.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        Result.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        Result.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        Result.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        Result.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        Result.NhomTongID = Utils.ConvertToInt32(dr["NhomTongID"], 0);
                        Result.CoQuanTao = Utils.ConvertToInt32(dr["CoQuanTao"], 0);
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

        public List<NhomNguoiDungModel> NhomNguoiDung_GetByNhomTongID(int NhomTongID)
        {
            List<NhomNguoiDungModel> Result = new List<NhomNguoiDungModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomTongID",SqlDbType.NVarChar)
              };
            parameters[0].Value = NhomTongID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_GetByNhomTongID", parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new NhomNguoiDungModel();
                        crObj.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        crObj.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        crObj.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        crObj.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        crObj.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        crObj.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        Result.Add(crObj);
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

        /// <summary>
        /// Lấy danh sách nhóm người dùng của các cơ quan con có trong nhóm hiện tại
        /// </summary>
        /// <param name="NhomNguoiDungID"></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<NhomNguoiDungModel> NhomNguoiDung_GetListNhomCuaDonViCon(int NhomNguoiDungID, int CoQuanID)
        {
            var listCoQuanCon = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
            List<NhomNguoiDungModel> Result = new List<NhomNguoiDungModel>();
            var pList = new SqlParameter("@DanhSachCoQuan", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            // var TrangThai = 400;
            var tbCoQuanID = new DataTable();
            tbCoQuanID.Columns.Add("ID", typeof(string));
            listCoQuanCon.ForEach(x => tbCoQuanID.Rows.Add(x.CoQuanID));
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomNguoiDungID",SqlDbType.NVarChar),
                 pList
              };
            parameters[0].Value = NhomNguoiDungID;
            parameters[1].Value = tbCoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_GetListNhomCuaDonViCon", parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new NhomNguoiDungModel();
                        crObj.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        crObj.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        crObj.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        crObj.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        crObj.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        crObj.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        Result.Add(crObj);
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

        public BaseResultModel NhomNguoiDung_InsertOne(NhomNguoiDungModel NhomNguoiDungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NhomNguoiDungModel == null || NhomNguoiDungModel.TenNhom == null || NhomNguoiDungModel.TenNhom.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được trống";
                }
                else if (NhomNguoiDungModel.TenNhom.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được quá 200 ký tự";
                }
                else if (NhomNguoiDungModel.CoQuanID == null || NhomNguoiDungModel.CoQuanID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "CoQuanID không được trống";
                }
                else if (!CheckTonTaiCoQuan(NhomNguoiDungModel.CoQuanID))
                {
                    Result.Status = 0;
                    Result.Message = "CoQuanID " + NhomNguoiDungModel.CoQuanID + " không tồn tại";
                }

                //else if (NhomNguoiDungModel.NhomTongID == null && NhomNguoiDungModel.NhomTongID < 0)
                //{
                //    Result.Status = 0;
                //    Result.Message = "Nhóm tổng không được trống";
                //}
                //else
                //{
                //var crObj = NhomNguoiDung_GetByName(NhomNguoiDungModel.TenNhom);
                //if (crObj != null && crObj.NhomNguoiDungID > 0)
                //{
                //    Result.Status = 0;
                //    Result.Message = "Tên nhóm người dùng đã tồn tại";
                //}
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("TenNhom", SqlDbType.NVarChar),
                            new SqlParameter("GhiChu", SqlDbType.NVarChar),
                            new SqlParameter("CoQuanID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongOut",SqlDbType.Int),
                            new SqlParameter("ApDungCho", SqlDbType.TinyInt)
                            , new SqlParameter("CoQuanTao", SqlDbType.TinyInt)
                      };
                    parameters[0].Value = NhomNguoiDungModel.TenNhom.Trim();
                    parameters[1].Value = NhomNguoiDungModel.GhiChu ?? Convert.DBNull;
                    parameters[2].Value = NhomNguoiDungModel.CoQuanID;
                    parameters[3].Value = NhomNguoiDungModel.NhomTongID ?? 0;
                    parameters[4].Direction = ParameterDirection.Output;
                    parameters[4].Size = 8;
                    parameters[5].Value = Convert.DBNull;
                    parameters[6].Value = Convert.DBNull;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_Insert", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("Nhóm người dùng");
                                Result.Data = Utils.ConvertToInt32(parameters[4].Value, 0);
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
                //}
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        public NhomNguoiDungModel NhomNguoiDung_GetByName(string TenTrangThai)
        {
            NhomNguoiDungModel Result = new NhomNguoiDungModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenNhom",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenTrangThai.Trim();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_GetByName", parameters))
                {
                    while (dr.Read())
                    {
                        Result = new NhomNguoiDungModel(Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0), Utils.ConvertToString(dr["TenNhom"], string.Empty), Utils.ConvertToString(dr["GhiChu"], string.Empty), Utils.ConvertToInt32(dr["ApDungCho"], 0));
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

        public List<NhomNguoiDungModel> NhomNguoiDung_SearchByName(string TenTrangThai)
        {
            List<NhomNguoiDungModel> Result = new List<NhomNguoiDungModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"TenNhom",SqlDbType.NVarChar)
              };
            parameters[0].Value = TenTrangThai.Trim();
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_GetByName", parameters))
                {
                    while (dr.Read())
                    {

                        var item = new NhomNguoiDungModel();
                        item.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        item.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        item.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        item.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        item.NhomTongID = Utils.ConvertToInt32(dr["NhomTongID"], 0);
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

        private bool CheckTonTaiCoQuan(int CoQuanID)
        {
            try
            {
                var CoQuan = new DanhMucCoQuanDonViDAL().GetByIDForCheckRef(CoQuanID);
                if (CoQuan != null && CoQuan.CoQuanID > 0)
                {
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }

        }


        /// <summary>
        /// lấy tất cả nhóm người dùng áp dụng cho tất cả cơ quan
        /// </summary>
        /// <returns></returns>
        public List<NhomNguoiDungModel> NhomNguoiDung_GetListNhomApDungChoTatCa()
        {
            List<NhomNguoiDungModel> Result = new List<NhomNguoiDungModel>();

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_NhomNguoiDung_GetListNhomApDungChoTatCa"))
                {
                    while (dr.Read())
                    {
                        var item = new NhomNguoiDungModel();
                        item.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        item.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        item.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        item.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        item.NhomTongID = Utils.ConvertToInt32(dr["NhomTongID"], 0);
                        Result.Add(item);
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

        public BaseResultModel NhomNguoiDung_IsertNhomChoCoQuanMoi(int CoQuanID, int? CoQuanChaID, int NguoiDungID)
        {
            var Result = new BaseResultModel();
            try
            {
                var listCoQuanDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID).Where(x => x.CoQuanID != CoQuanID).ToList();
                if (UserRole.CheckAdmin(NguoiDungID))
                {
                    listCoQuanDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).Where(x => x.CoQuanID != CoQuanID).ToList();
                }
                var listNhomNguoiDungApDungChoTatCa = NhomNguoiDung_GetListNhomApDungChoTatCa();
                //var listNhomNguoiDungCanThem = new List<NhomNguoiDungModel>();
                for (int i = 0; i < listNhomNguoiDungApDungChoTatCa.Count; i++)
                {
                    var listNhomCon = new List<NhomNguoiDungModel>();
                    if (UserRole.CheckAdmin(NguoiDungID))
                        listNhomCon = NhomNguoiDung_GetByNhomTongID(listNhomNguoiDungApDungChoTatCa[i].NhomNguoiDungID);
                    else listNhomCon = NhomNguoiDung_GetListNhomCuaDonViCon(listNhomNguoiDungApDungChoTatCa[i].NhomNguoiDungID, CoQuanID);
                    var listNhomNguoiDungCuaListDonVi = listNhomCon.Where(x => listCoQuanDonViTrucThuoc.Select(y => y.CoQuanID).Contains(x.CoQuanID)).ToList();
                    if (listNhomNguoiDungCuaListDonVi.Count == listCoQuanDonViTrucThuoc.Count)
                    {
                        //listNhomNguoiDungCanThem.Add(listNhomNguoiDungApDungChoTatCa[i]);
                        var itemInSert = new NhomNguoiDungModel();
                        itemInSert.TenNhom = listNhomNguoiDungApDungChoTatCa[i].TenNhom;
                        itemInSert.CoQuanID = CoQuanID;
                        itemInSert.NhomTongID = listNhomNguoiDungApDungChoTatCa[i].NhomNguoiDungID;
                        itemInSert.GhiChu = listNhomNguoiDungApDungChoTatCa[i].GhiChu;
                        var qeury = NhomNguoiDung_InsertOne(itemInSert);
                        if (qeury.Status < 1)
                        {
                            return Result;
                        }
                        // lấy danh sách chức năng của nhóm tổng
                        var listChucNang = ChucNang_GetQuyenByNhomNguoiDungID(listNhomNguoiDungApDungChoTatCa[i].NhomNguoiDungID);
                        //ChucNang_GetQuyenDuocThaoTacTrongNhom(listNhomNguoiDungApDungChoTatCa[i].NhomNguoiDungID);
                        // insert chức năng cho nhóm mới được tạo
                        if (listChucNang != null && listChucNang.Count > 0)
                        {
                            for (int j = 0; j < listChucNang.Count; j++)
                            {
                                var itemPhanQuyen = new PhanQuyenModel();
                                itemPhanQuyen.ChucNangID = listChucNang[j].ChucNangID;
                                itemPhanQuyen.Quyen = listChucNang[j].Quyen;
                                itemPhanQuyen.NhomNguoiDungID = Utils.ConvertToInt32(qeury.Data, 0);
                                var insertChucNang = PhanQuyen_InsertOne(itemPhanQuyen);
                            }
                        }

                    }
                }
                //if (listNhomNguoiDungCanThem.Count > 0)
                //{
                //    for (int i = 0; i < listNhomNguoiDungCanThem.Count; i++)
                //    {
                //        var itemInSert = new NhomNguoiDungModel();
                //        itemInSert.TenNhom = listNhomNguoiDungCanThem[i].TenNhom;
                //        itemInSert.CoQuanID = CoQuanID;
                //        itemInSert.NhomTongID = listNhomNguoiDungCanThem[i].NhomNguoiDungID;
                //        itemInSert.GhiChu = listNhomNguoiDungCanThem[i].GhiChu;
                //      var qeury=   NhomNguoiDung_InsertOne(itemInSert);
                //        if (qeury.Status < 1)
                //        {
                //            return Result;
                //        }
                //    }
                //}
                Result.Status = 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }
        public BaseResultModel NhomNguoiDung_DeleteByNhomTongID(int? NhomNguoiDungID)
        {
            var Result = new BaseResultModel();
            if (NhomNguoiDungID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(NhomNguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = NhomNguoiDung_GetByID(NhomNguoiDungID.Value);
                    if (crObj == null || crObj.NhomNguoiDungID == null || crObj.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không tồn tại";
                        return Result;
                    }
                    var listNguoiDung = NguoiDung_NhomNguoiDung_GetByNhomNguoiDungID(NhomNguoiDungID.Value);
                    var listChucNang = ChucNang_GetQuyenByNhomNguoiDungID(NhomNguoiDungID.Value);
                    if ((listNguoiDung != null && listNguoiDung.Count > 0) || (listChucNang != null && listChucNang.Count > 0))
                    {
                        Result.Status = 0;
                        Result.Message = "Nhóm người dùng đã được sử dụng không thể xóa";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                        };
                        parameters[0].Value = NhomNguoiDungID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_DeleteByNhomTongID", parameters);
                                    trans.Commit();
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
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Nhóm người dùng");
                return Result;
            }
        }

        public BaseResultModel NhomNguoiDung_DeleteOne(int? NhomNguoiDungID)
        {
            var Result = new BaseResultModel();
            if (NhomNguoiDungID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(NhomNguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = NhomNguoiDung_GetByID(NhomNguoiDungID.Value);
                    if (crObj == null || crObj.NhomNguoiDungID == null || crObj.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không tồn tại";
                        return Result;
                    }
                    //var listNguoiDung = NguoiDung_NhomNguoiDung_GetByNhomNguoiDungID(NhomNguoiDungID.Value);
                    //var listChucNang = ChucNang_GetQuyenByNhomNguoiDungID(NhomNguoiDungID.Value);
                    //if ((listNguoiDung != null && listNguoiDung.Count > 0) || (listChucNang != null && listChucNang.Count > 0))
                    //{
                    //    Result.Status = 0;
                    //    Result.Message = "Nhóm người dùng đã được sử dụng không thể xóa";
                    //    return Result;
                    //}
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                        };
                        parameters[0].Value = NhomNguoiDungID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_Delete", parameters);
                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    return Result;
                                    throw ex;
                                }
                            }
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Nhóm người dùng");
                return Result;
            }
        }


        /// <summary>
        /// lấy toàn bộ nhóm người dùng trong hệ thống của nhóm tổng (bao gồm cả nhóm tổng)
        /// </summary>
        /// <param name="NhomNguoiDungID"></param>
        /// <returns></returns>
        public List<NhomNguoiDungModel> NhomNguoiDung_DanhSachNhomTrongNhomTong(int NhomNguoiDungID)
        {
            List<NhomNguoiDungModel> Result = new List<NhomNguoiDungModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomNguoiDungID",SqlDbType.NVarChar)
              };
            parameters[0].Value = NhomNguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_DanhSachNhomTrongNhomTong", parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new NhomNguoiDungModel();
                        crObj.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        crObj.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        crObj.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        crObj.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        crObj.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        crObj.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
                        Result.Add(crObj);
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



        #endregion


        #region NguoiDung_NhomNguoiDung
        public List<NguoiDungModel> NguoiDung_NhomNguoiDung_GetByNhomNguoiDungID(int NhomNguoiDungID)
        {
            List<NguoiDungModel> Result = new List<NguoiDungModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomNguoiDungID",SqlDbType.NVarChar)
              };
            parameters[0].Value = NhomNguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_GetByNhomNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new NguoiDungModel();
                        crObj.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        crObj.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                        Result.Add(crObj);
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

        public BaseResultModel NguoiDung_NhomNguoiDung_DeleteByNhomNguoiDungID(int? NhomNguoiDungID)
        {
            var Result = new BaseResultModel();
            if (NhomNguoiDungID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(NhomNguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var listNguoiDung_NhomNguoiDung = NguoiDung_NhomNguoiDung_GetByNhomNguoiDungID(NhomNguoiDungID.Value);
                    if (listNguoiDung_NhomNguoiDung == null || listNguoiDung_NhomNguoiDung.Count < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                        };
                        parameters[0].Value = NhomNguoiDungID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_NguoiDung_NhomNguoiDung_DeleteByNhomNguoiDungID", parameters);
                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    return Result;
                                    throw ex;
                                }
                            }
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Người dùng - Nhóm người dùng");
                return Result;
            }
        }


        /// <summary>
        /// thêm người dùng vào nhóm người dùng
        /// </summary>
        /// <param name="NguoiDungNhomNguoiDungModel"></param>
        /// <returns></returns>
        public BaseResultModel NguoiDung_NhomNguoiDung_Insert(NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NguoiDungNhomNguoiDungModel.NhomNguoiDungID == null || NguoiDungNhomNguoiDungModel.NhomNguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID không được trống";
                }
                else if (NguoiDungNhomNguoiDungModel.NguoiDungID == null || NguoiDungNhomNguoiDungModel.NguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "NguoiDungID không được trống";
                }
                else
                {
                    var checkNguoiDungTrongHeThong = NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID(NguoiDungNhomNguoiDungModel.NguoiDungID, NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
                    if (checkNguoiDungTrongHeThong != null && checkNguoiDungTrongHeThong.NguoiDungID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Người dùng đã được thêm";
                        return Result;
                    }
                    // lấy đúng nhóm người dùng của cơ quan cho người dùng được thêm mới vào nhóm
                    var crNguoiDung = new HeThongNguoiDungDAL().GetByID(NguoiDungNhomNguoiDungModel.NguoiDungID);
                    var listNhomNguoiDungDangThaoTac = NhomNguoiDung_DanhSachNhomTrongNhomTong(NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
                    var nhomNguoiDungDung = 0;
                    nhomNguoiDungDung = listNhomNguoiDungDangThaoTac.FirstOrDefault(x => x.CoQuanID == crNguoiDung.CoQuanID).NhomNguoiDungID;
                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("NguoiDungID",SqlDbType.Int),
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                      };
                    parameters[0].Value = NguoiDungNhomNguoiDungModel.NguoiDungID;
                    parameters[1].Value = nhomNguoiDungDung > 0 ? nhomNguoiDungDung : NguoiDungNhomNguoiDungModel.NhomNguoiDungID;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_Insert", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("Người dùng - Nhóm người dùng");
                            }
                            catch (Exception ex)
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw ex;
                            }

                        }
                        //var listNhomTrucThuoc = NhomNguoiDung_GetByNhomTongID(NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
                        //if (listNhomTrucThuoc != null && listNhomTrucThuoc.Count > 0)
                        //{
                        //    for (int i = 0; i < listNhomTrucThuoc.Count; i++)
                        //    {
                        //        var crObj = new NguoiDungNhomNguoiDungModel();
                        //        crObj.NguoiDungID = NguoiDungNhomNguoiDungModel.NguoiDungID;
                        //        crObj.NhomNguoiDungID = listNhomTrucThuoc[i].NhomNguoiDungID;
                        //        var isNguoiDungNhomNguoiDung = NguoiDung_NhomNguoiDung_InsertOne(crObj);
                        //        if (isNguoiDungNhomNguoiDung.Status < 1)
                        //        {
                        //            return isNguoiDungNhomNguoiDung;
                        //        }
                        //    }
                        //}

                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        public BaseResultModel NguoiDung_NhomNguoiDung_InsertMulti_New(Dictionary<int, int> NguoiDungNhomNguoiDungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                //if (NguoiDungNhomNguoiDungModel.NhomNguoiDungID == null || NguoiDungNhomNguoiDungModel.NhomNguoiDungID < 1)
                //{
                //    Result.Status = 0;
                //    Result.Message = "NhomNguoiDungID không được trống";
                //}
                //else if (NguoiDungNhomNguoiDungModel.NguoiDungID == null || NguoiDungNhomNguoiDungModel.NguoiDungID < 1)
                //{
                //    Result.Status = 0;
                //    Result.Message = "NguoiDungID không được trống";
                //}
                //else
                //{
                //var checkNguoiDungTrongHeThong = NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID(NguoiDungNhomNguoiDungModel.FirstOrDefault().Key, NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
                //if (checkNguoiDungTrongHeThong != null && checkNguoiDungTrongHeThong.NguoiDungID > 0)
                //{
                //    Result.Status = 0;
                //    Result.Message = "Người dùng đã được thêm";
                //    return Result;
                //}
                var pList = new SqlParameter("@ListNguoiDungNhomNguoiDung", SqlDbType.Structured);
                pList.TypeName = "dbo.ID1_ID2_list";
                var tbNguoiDungNNDID = new DataTable();
                tbNguoiDungNNDID.Columns.Add("NguoiDungID", typeof(string));
                tbNguoiDungNNDID.Columns.Add("NhomNguoiDungID", typeof(string));
                foreach (var item in NguoiDungNhomNguoiDungModel)
                {
                    tbNguoiDungNNDID.Rows.Add(item.Key, item.Value);
                }
                SqlParameter[] parameters = new SqlParameter[]
                  {
                            pList
                  };
                parameters[0].Value = tbNguoiDungNNDID;
                //parameters[1].Value = NguoiDungNhomNguoiDungModel.NhomNguoiDungID;

                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_Insert_One", parameters);
                            trans.Commit();
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("Người dùng - Nhóm người dùng");
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
            //var listNhomTrucThuoc = NhomNguoiDung_GetByNhomTongID(NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
            //if (listNhomTrucThuoc != null && listNhomTrucThuoc.Count > 0)
            //{
            //    for (int i = 0; i < listNhomTrucThuoc.Count; i++)
            //    {
            //        var crObj = new NguoiDungNhomNguoiDungModel();
            //        crObj.NguoiDungID = NguoiDungNhomNguoiDungModel.NguoiDungID;
            //        crObj.NhomNguoiDungID = listNhomTrucThuoc[i].NhomNguoiDungID;
            //        var isNguoiDungNhomNguoiDung = NguoiDung_NhomNguoiDung_InsertOne(crObj);
            //        if (isNguoiDungNhomNguoiDung.Status < 1)
            //        {
            //            return isNguoiDungNhomNguoiDung;
            //        }
            //    }
            //}

            //}
            //    }
            //    //}
            //}
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }


        public BaseResultModel NguoiDung_NhomNguoiDung_InsertOne(NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NguoiDungNhomNguoiDungModel.NhomNguoiDungID == null || NguoiDungNhomNguoiDungModel.NhomNguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID không được trống";
                }
                else if (NguoiDungNhomNguoiDungModel.NguoiDungID == null || NguoiDungNhomNguoiDungModel.NguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "NguoiDungID không được trống";
                }
                else
                {
                    var checkNguoiDungTrongHeThong = NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID(NguoiDungNhomNguoiDungModel.NguoiDungID, NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
                    if (checkNguoiDungTrongHeThong != null && checkNguoiDungTrongHeThong.NguoiDungID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Người dùng đã được thêm";
                        return Result;
                    }

                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("NguoiDungID",SqlDbType.Int),
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                      };
                    parameters[0].Value = NguoiDungNhomNguoiDungModel.NguoiDungID;
                    parameters[1].Value = NguoiDungNhomNguoiDungModel.NhomNguoiDungID;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_Insert_One", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("Người dùng - Nhóm người dùng");
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
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }
        public BaseResultModel NguoiDung_NhomNguoiDung_Delete(int? NguoiDungID, int? NhomNguoiDungID)
        {
            var Result = new BaseResultModel();
            if (NhomNguoiDungID < 1 || NguoiDungID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(NhomNguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else if (!int.TryParse(NguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NguoiDungID '" + NguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID(NguoiDungID.Value, NhomNguoiDungID.Value);
                    if (crObj == null || crObj.NhomNguoiDungID == null || crObj.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Người dùng không tồn tại";
                        return Result;
                    }
                    else
                    {
                        var crNguoiDung = new HeThongNguoiDungDAL().GetByID(NguoiDungID.Value);
                        var listNhomNguoiDungDangThaoTac = NhomNguoiDung_DanhSachNhomTrongNhomTong(NhomNguoiDungID.Value);
                        var nhomNguoiDungDung = 0;
                        nhomNguoiDungDung = listNhomNguoiDungDangThaoTac.FirstOrDefault(x => x.CoQuanID == crNguoiDung.CoQuanID).NhomNguoiDungID;
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("NguoiDungID", SqlDbType.Int),
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                        };
                        parameters[0].Value = NguoiDungID;
                        parameters[1].Value = nhomNguoiDungDung > 0 ? nhomNguoiDungDung : NhomNguoiDungID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_NguoiDung_NhomNguoiDung_Delete", parameters);
                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    return Result;
                                    throw ex;
                                }
                            }
                            //var listNhomTrucThuoc = NhomNguoiDung_GetByNhomTongID(NhomNguoiDungID.Value);
                            //if (listNhomTrucThuoc != null && listNhomTrucThuoc.Count > 0)
                            //{
                            //    for (int i = 0; i < listNhomTrucThuoc.Count; i++)
                            //    {
                            //        var dlNguoiDungNhomNguoiDung = NguoiDung_NhomNguoiDung_DeleteOne(NguoiDungID.Value, listNhomTrucThuoc[i].NhomNguoiDungID);
                            //        if (dlNguoiDungNhomNguoiDung.Status < 1)
                            //        {
                            //            return dlNguoiDungNhomNguoiDung;
                            //        }
                            //    }
                            //}
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Người dùng - Nhóm người dùng");
                return Result;
            }
        }

        public BaseResultModel NguoiDung_NhomNguoiDung_DeleteOne(int? NguoiDungID, int? NhomNguoiDungID)
        {
            var Result = new BaseResultModel();
            if (NhomNguoiDungID < 1 || NguoiDungID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(NhomNguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else if (!int.TryParse(NguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NguoiDungID '" + NguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = NguoiDung_NhomNguoiDung_GetQuyenByNguoiDungID_And_NhomNguoiDungID(NguoiDungID.Value, NhomNguoiDungID.Value);
                    if (crObj == null || crObj.NhomNguoiDungID == null || crObj.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Người dùng không tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("NguoiDungID", SqlDbType.Int),
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                        };
                        parameters[0].Value = NguoiDungID;
                        parameters[1].Value = NhomNguoiDungID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_NguoiDung_NhomNguoiDung_Delete", parameters);
                                    trans.Commit();
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
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Người dùng - Nhóm người dùng");
                return Result;
            }
        }
        public NguoiDungNhomNguoiDungModel NguoiDung_NhomNguoiDung_GetQuyenByNguoiDungID_And_NhomNguoiDungID(int NguoiDungID, int NhomNguoiDungID)
        {
            NguoiDungNhomNguoiDungModel Result = new NguoiDungNhomNguoiDungModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 new SqlParameter(@"NguoiDungID",SqlDbType.Int)
                ,new SqlParameter(@"NhomNguoiDungID",SqlDbType.Int)
              };
            parameters[0].Value = NguoiDungID;
            parameters[1].Value = NhomNguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        Result.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        Result.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
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

        public NguoiDungNhomNguoiDungModel NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID(int NguoiDungID, int NhomNguoiDungID)
        {
            NguoiDungNhomNguoiDungModel Result = new NguoiDungNhomNguoiDungModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 new SqlParameter(@"NguoiDungID",SqlDbType.Int)
                ,new SqlParameter(@"NhomNguoiDungID",SqlDbType.Int)
              };
            parameters[0].Value = NguoiDungID;
            parameters[1].Value = NhomNguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        Result.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        Result.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
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
        public List<NguoiDungNhomNguoiDungModel> NguoiDung_NhomNguoiDung_GetByNguoiDungID(int NguoiDungID)
        {
            List<NguoiDungNhomNguoiDungModel> Result = new List<NguoiDungNhomNguoiDungModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 new SqlParameter(@"NguoiDungID",SqlDbType.Int)

              };
            parameters[0].Value = NguoiDungID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_GetByNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel = new NguoiDungNhomNguoiDungModel();
                        NguoiDungNhomNguoiDungModel.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        NguoiDungNhomNguoiDungModel.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        Result.Add(NguoiDungNhomNguoiDungModel);
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

        public NguoiDungNhomNguoiDungModel NguoiDung_NhomNguoiDung_GetByNguoiDungID_ForCheckRef(int NguoiDungID)
        {
            NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel = new NguoiDungNhomNguoiDungModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 new SqlParameter(@"NguoiDungID",SqlDbType.Int)

              };
            parameters[0].Value = NguoiDungID;

            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_GetByNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        NguoiDungNhomNguoiDungModel.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        NguoiDungNhomNguoiDungModel.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                throw;
            }
            return NguoiDungNhomNguoiDungModel;
        }
        #endregion


        #region ChucNang - PhanQuyen

        /// <summary>
        /// Danh sách Quyền được thao tác trong nhóm người dùng hiện tại
        /// lấy danh sách quyền theo nhóm người dùng tổng - NhomTongID=0
        /// </summary>
        /// <param name="NhomNguoiDungID"></param>
        /// <returns></returns>
        public List<ChucNangModel> ChucNang_GetQuyenDuocThaoTacTrongNhom(int NhomNguoiDungID, int CoQuanID, int NguoiDungID)
        {
            List<ChucNangModel> Result = new List<ChucNangModel>();
            var crNhomNguoiDung = NhomNguoiDung_GetByID(NhomNguoiDungID);
            if (crNhomNguoiDung.CoQuanTao == CoQuanID)
            {
                Result = new ChucNangDAL().GetListChucNangByCoQuanID(CoQuanID, NguoiDungID);
                return Result;
            }
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomNguoiDungID",SqlDbType.NVarChar)
              };
            parameters[0].Value = NhomNguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_ChucNang_GetQuyenDuocThaoTac", parameters))
                {
                    while (dr.Read())
                    {
                        ChucNangModel item = new ChucNangModel(
                           Utils.ConvertToInt32(dr["ChucNangID"], 0)
                          , Utils.ConvertToString(dr["TenChucNang"], String.Empty)
                          , Utils.ConvertToString(dr["MaChucNang"], String.Empty)
                          , Utils.ConvertToInt32(dr["Quyen"], 0))
                          ;
                        item.PhanQuyenID = Utils.ConvertToInt32(dr["PhanQuyenID"], 0);
                        Result.Add(item);
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
        public List<ChucNangModel> ChucNang_GetQuyenByNhomNguoiDungID(int NhomNguoiDungID)
        {
            List<ChucNangModel> Result = new List<ChucNangModel>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomNguoiDungID",SqlDbType.NVarChar)
              };
            parameters[0].Value = NhomNguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_ChucNang_GetQuyenByNhomNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        ChucNangModel item = new ChucNangModel(
                            Utils.ConvertToInt32(dr["ChucNangID"], 0)
                           , Utils.ConvertToString(dr["TenChucNang"], String.Empty)
                           , Utils.ConvertToString(dr["MaChucNang"], String.Empty)
                           , Utils.ConvertToInt32(dr["Quyen"], 0))
                           ;
                        item.PhanQuyenID = Utils.ConvertToInt32(dr["PhanQuyenID"], 0);
                        //ChucNangModel item = new ChucNangModel();
                        //item.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        //item.TenChucNang = Utils.ConvertToString(dr["TenChucNang"], String.Empty);
                        //item.MaChucNang = Utils.ConvertToString(dr["MaChucNang"], String.Empty);
                        //item.Quyen = Utils.ConvertToInt32(dr["Quyen"], 0);
                        //item.PhanQuyenID = Utils.ConvertToInt32(dr["PhanQuyenID"], 0);
                        Result.Add(item);
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

        public BaseResultModel PhanQuyen_DeleteByNhomNguoiDungID(int? NhomNguoiDungID)
        {
            var Result = new BaseResultModel();
            if (NhomNguoiDungID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(NhomNguoiDungID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = NhomNguoiDung_GetByID(NhomNguoiDungID.Value);
                    if (crObj == null || crObj.NhomNguoiDungID == null || crObj.NhomNguoiDungID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "NhomNguoiDungID '" + NhomNguoiDungID.ToString() + "' không tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                        };
                        parameters[0].Value = NhomNguoiDungID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_DeleteByNhomNguoiDungID", parameters);
                                    trans.Commit();
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
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Phân quyền");
                return Result;
            }
        }

        public BaseResultModel PhanQuyen_InsertOne(PhanQuyenModel PhanQuyenModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (PhanQuyenModel == null || PhanQuyenModel.ChucNangID == null || PhanQuyenModel.NhomNguoiDungID == null || PhanQuyenModel.ChucNangID < 1 || PhanQuyenModel.NhomNguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập ChucNangID và NhomNguoiDungID";
                    return Result;
                }
                else if (PhanQuyenModel.Quyen == null || PhanQuyenModel.Quyen < 0)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập Quyền";
                    return Result;
                }

                else
                {
                    var checkPhanQuyen = PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID(PhanQuyenModel.NhomNguoiDungID, PhanQuyenModel.ChucNangID);
                    if (checkPhanQuyen == null || checkPhanQuyen.ChucNangID < 1)
                    {
                        //Result.Status = 0;
                        //Result.Message = "Chức năng đã tồn tại";
                        //return Result;
                        if (PhanQuyenModel.Quyen == null || PhanQuyenModel.Quyen < 1)// map quyền
                        {
                            PhanQuyenModel = new PhanQuyenModel(PhanQuyenModel.NhomNguoiDungID, PhanQuyenModel.ChucNangID, PhanQuyenModel.Xem ?? 0, PhanQuyenModel.Them ?? 0, PhanQuyenModel.Sua ?? 0, PhanQuyenModel.Xoa ?? 0);
                        }
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                            new SqlParameter("ChucNangID", SqlDbType.Int),
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int),
                            new SqlParameter("Quyen", SqlDbType.Int),
                          };
                        parameters[0].Value = PhanQuyenModel.ChucNangID;
                        parameters[1].Value = PhanQuyenModel.NhomNguoiDungID;
                        parameters[2].Value = PhanQuyenModel.Quyen;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_Insert", parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Phân quyền");
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
            Result.Status = 1;
            Result.Message = ConstantLogMessage.Alert_Insert_Success("Phân quyền");
            return Result;
        }

        public BaseResultModel PhanQuyen_Insert(PhanQuyenModel PhanQuyenModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (PhanQuyenModel == null || PhanQuyenModel.ChucNangID == null || PhanQuyenModel.NhomNguoiDungID == null || PhanQuyenModel.ChucNangID < 1 || PhanQuyenModel.NhomNguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập ChucNangID và NhomNguoiDungID";
                    return Result;
                }
                else if (PhanQuyenModel.Quyen == null || PhanQuyenModel.Quyen < 0)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập Quyền";
                    return Result;
                }

                else
                {

                    if (PhanQuyenModel.Quyen == null || PhanQuyenModel.Quyen < 1)// map quyền
                    {
                        PhanQuyenModel = new PhanQuyenModel(PhanQuyenModel.NhomNguoiDungID, PhanQuyenModel.ChucNangID, PhanQuyenModel.Xem ?? 0, PhanQuyenModel.Them ?? 0, PhanQuyenModel.Sua ?? 0, PhanQuyenModel.Xoa ?? 0);
                    }
                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("ChucNangID", SqlDbType.Int),
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int),
                            new SqlParameter("Quyen", SqlDbType.Int),
                      };
                    parameters[0].Value = PhanQuyenModel.ChucNangID;
                    parameters[1].Value = PhanQuyenModel.NhomNguoiDungID;
                    parameters[2].Value = PhanQuyenModel.Quyen;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        var checkPhanQuyen = PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID(PhanQuyenModel.NhomNguoiDungID, PhanQuyenModel.ChucNangID);
                        if (checkPhanQuyen == null || checkPhanQuyen.ChucNangID < 1)
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_Insert", parameters);
                                    trans.Commit();
                                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Quyền");
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

                        var listNhomNguoiDungCon = NhomNguoiDung_GetByNhomTongID(PhanQuyenModel.NhomNguoiDungID).Where(x => x.NhomNguoiDungID != PhanQuyenModel.NhomNguoiDungID).ToList();
                        if (listNhomNguoiDungCon != null && listNhomNguoiDungCon.Count > 0)
                        {
                            for (int i = 0; i < listNhomNguoiDungCon.Count; i++)
                            {
                                var isQuyenChoNhomcon = PhanQuyen_InsertOne(new PhanQuyenModel(listNhomNguoiDungCon[i].NhomNguoiDungID, PhanQuyenModel.ChucNangID, PhanQuyenModel.Xem ?? 0, PhanQuyenModel.Them ?? 0, PhanQuyenModel.Sua ?? 0, PhanQuyenModel.Xoa ?? 0));
                                //var isQuyenChoNhomConObj = PhanQuyenModel;
                                //isQuyenChoNhomConObj.NhomNguoiDungID = listNhomNguoiDungCon[i].NhomNguoiDungID;
                                //var isQuyenChoNhomcon = PhanQuyen_InsertOne(isQuyenChoNhomConObj);
                                if (isQuyenChoNhomcon.Status < 1)
                                {
                                    return isQuyenChoNhomcon;
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
            Result.Status = 1;
            Result.Message = ConstantLogMessage.Alert_Insert_Success("Quyền");
            return Result;
        }

        public BaseResultModel PhanQuyen_InsertMulti(List<PhanQuyenModel> PhanQuyenModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (PhanQuyenModel == null || PhanQuyenModel.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập ChucNangID cập nhật";
                }
                else
                {
                    for (int i = 0; i < PhanQuyenModel.Count; i++)
                    {
                        var isPhanQuyen = PhanQuyen_Insert(PhanQuyenModel[i]);
                        if (isPhanQuyen.Status < 1)
                        {
                            return isPhanQuyen;
                        }
                    }
                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Phân quyền");
                    Result.Status = 1;
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

        public BaseResultModel PhanQuyen_Delete(int? PhanQuyenID)
        {
            var Result = new BaseResultModel();
            if (PhanQuyenID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(PhanQuyenID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "PhanQuyenID '" + PhanQuyenID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = PhanQuyen_GetByID(PhanQuyenID.Value);
                    if (crObj == null || crObj.PhanQuyenID == null || crObj.PhanQuyenID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "PhanQuyenID '" + PhanQuyenID.ToString() + "' không tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("PhanQuyenID", SqlDbType.Int)
                        };
                        parameters[0].Value = PhanQuyenID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_Delete", parameters);
                                    trans.Commit();
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
                            var listNhomNguoiDungCon = NhomNguoiDung_GetByNhomTongID(crObj.NhomNguoiDungID);
                            if (listNhomNguoiDungCon != null && listNhomNguoiDungCon.Count > 0)
                            {
                                for (int i = 0; i < listNhomNguoiDungCon.Count; i++)
                                {
                                    var dlObj = PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID(listNhomNguoiDungCon[i].NhomNguoiDungID, crObj.ChucNangID);
                                    if (dlObj != null && dlObj.ChucNangID > 0)
                                    {
                                        var query = PhanQuyen_DeleteOneByID(dlObj.PhanQuyenID);
                                    }
                                }
                            }
                        }
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Phân quyền");
                return Result;
            }
        }

        public BaseResultModel PhanQuyen_Update(PhanQuyenModel PhanQuyenModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (PhanQuyenModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập ChucNangID và NhomNguoiDungID";
                }
                else
                {
                    var checkPhanQuyen = PhanQuyen_GetByID(PhanQuyenModel.PhanQuyenID);
                    //PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID(PhanQuyenModel.NhomNguoiDungID, PhanQuyenModel.ChucNangID);
                    if (checkPhanQuyen == null || checkPhanQuyen.ChucNangID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Chức năng không tồn tại";
                        return Result;
                    }
                    //if (PhanQuyenModel.Quyen == null || PhanQuyenModel.Quyen < 1)// map quyền
                    //{
                    var crID = PhanQuyenModel.PhanQuyenID;
                    PhanQuyenModel = new PhanQuyenModel(checkPhanQuyen.NhomNguoiDungID, checkPhanQuyen.ChucNangID, PhanQuyenModel.Xem ?? 0, PhanQuyenModel.Them ?? 0, PhanQuyenModel.Sua ?? 0, PhanQuyenModel.Xoa ?? 0);
                    PhanQuyenModel.PhanQuyenID = crID;
                    //}
                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("PhanQuyenID", SqlDbType.Int),
                            //new SqlParameter("ChucNangID", SqlDbType.Int),
                            //new SqlParameter("NhomNguoiDungID", SqlDbType.Int),
                            new SqlParameter("Quyen", SqlDbType.Int),
                      };
                    parameters[0].Value = PhanQuyenModel.PhanQuyenID;
                    //parameters[1].Value = checkPhanQuyen.ChucNangID;
                    //parameters[2].Value = checkPhanQuyen.NhomNguoiDungID;
                    parameters[1].Value = PhanQuyenModel.Quyen;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_Update", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Update_Success("Quyền");
                            }
                            catch (Exception ex)
                            {
                                Result.Status = -1;
                                Result.Message = ConstantLogMessage.API_Error_System;
                                trans.Rollback();
                                throw ex;
                            }
                            var listNhomNguoiDungCon = NhomNguoiDung_GetByNhomTongID(PhanQuyenModel.NhomNguoiDungID);
                            if (listNhomNguoiDungCon != null && listNhomNguoiDungCon.Count > 0)
                            {
                                for (int i = 0; i < listNhomNguoiDungCon.Count; i++)
                                {
                                    var itemPhanQuyen = PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID(listNhomNguoiDungCon[i].NhomNguoiDungID, PhanQuyenModel.ChucNangID);
                                    var crObjUpdate = new PhanQuyenModel(listNhomNguoiDungCon[i].NhomNguoiDungID, PhanQuyenModel.ChucNangID, PhanQuyenModel.Xem ?? 0, PhanQuyenModel.Them ?? 0, PhanQuyenModel.Sua ?? 0, PhanQuyenModel.Xoa ?? 0);
                                    crObjUpdate.PhanQuyenID = itemPhanQuyen.PhanQuyenID;
                                    var isQuyenChoNhomcon = PhanQuyen_UpdateOne(crObjUpdate);
                                    //if (isQuyenChoNhomcon.Status < 1)
                                    //{
                                    //    return isQuyenChoNhomcon;
                                    //}
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

        public BaseResultModel PhanQuyen_UpdateOne(PhanQuyenModel PhanQuyenModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (PhanQuyenModel == null)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập ChucNangID và NhomNguoiDungID";
                }
                else
                {
                    var checkPhanQuyen = PhanQuyen_GetByID(PhanQuyenModel.PhanQuyenID);
                    //PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID(PhanQuyenModel.NhomNguoiDungID, PhanQuyenModel.ChucNangID);
                    if (checkPhanQuyen == null || checkPhanQuyen.ChucNangID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "Chức năng không tồn tại";
                        return Result;
                    }
                    //if (PhanQuyenModel.Quyen == null || PhanQuyenModel.Quyen < 1)// map quyền
                    //{
                    var crID = PhanQuyenModel.PhanQuyenID;
                    PhanQuyenModel = new PhanQuyenModel(checkPhanQuyen.NhomNguoiDungID, checkPhanQuyen.ChucNangID, PhanQuyenModel.Xem ?? 0, PhanQuyenModel.Them ?? 0, PhanQuyenModel.Sua ?? 0, PhanQuyenModel.Xoa ?? 0);
                    PhanQuyenModel.PhanQuyenID = crID;
                    //}
                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("PhanQuyenID", SqlDbType.Int),
                            //new SqlParameter("ChucNangID", SqlDbType.Int),
                            //new SqlParameter("NhomNguoiDungID", SqlDbType.Int),
                            new SqlParameter("Quyen", SqlDbType.Int),
                      };
                    parameters[0].Value = PhanQuyenModel.PhanQuyenID;
                    //parameters[1].Value = checkPhanQuyen.ChucNangID;
                    //parameters[2].Value = checkPhanQuyen.NhomNguoiDungID;
                    parameters[1].Value = PhanQuyenModel.Quyen;
                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_Update", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Update_Success("Quyền");
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
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        public BaseResultModel PhanQuyen_UpdateMulti(List<PhanQuyenModel> PhanQuyenModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (PhanQuyenModel == null || PhanQuyenModel.Count < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Vui lòng nhập ChucNangID cập nhật";
                }
                else
                {
                    for (int i = 0; i < PhanQuyenModel.Count; i++)
                    {
                        var isPhanQuyen = PhanQuyen_Update(PhanQuyenModel[i]);
                        if (isPhanQuyen.Status < 1)
                        {
                            return isPhanQuyen;
                        }
                    }
                    Result.Message = ConstantLogMessage.Alert_Update_Success("Phân quyền");
                    Result.Status = 1;
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

        public BaseResultModel PhanQuyen_DeleteOneByID(int? PhanQuyenID)
        {
            var Result = new BaseResultModel();
            if (PhanQuyenID < 1)
            {
                Result.Status = 0;
                Result.Message = "Vui lòng chọn dữ liệu trước khi xóa";
                return Result;
            }
            else
            {
                int crID;
                if (!int.TryParse(PhanQuyenID.ToString(), out crID))
                {
                    Result.Status = 0;
                    Result.Message = "PhanQuyenID '" + PhanQuyenID.ToString() + "' không đúng định dạng";
                    return Result;
                }
                else
                {
                    var crObj = PhanQuyen_GetByID(PhanQuyenID.Value);
                    if (crObj == null || crObj.PhanQuyenID == null || crObj.PhanQuyenID < 1)
                    {
                        Result.Status = 0;
                        Result.Message = "PhanQuyenID '" + PhanQuyenID.ToString() + "' không tồn tại";
                        return Result;
                    }
                    else
                    {
                        SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("PhanQuyenID", SqlDbType.Int)
                        };
                        parameters[0].Value = PhanQuyenID;
                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    var val = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_Delete", parameters);
                                    trans.Commit();
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
                    }
                }
                Result.Status = 1;
                Result.Message = ConstantLogMessage.Alert_Delete_Success("Phân quyền");
                return Result;
            }
        }

        public ChucNangModel PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID(int NhomNguoiDungID, int ChucNangID)
        {
            ChucNangModel Result = new ChucNangModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomNguoiDungID",SqlDbType.Int),
                new SqlParameter(@"ChucNangID",SqlDbType.Int)
              };
            parameters[0].Value = NhomNguoiDungID;
            parameters[1].Value = ChucNangID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_GetByNhomNguoiDungID_And_ChucNangID", parameters))
                {
                    while (dr.Read())
                    {
                        Result = new ChucNangModel();
                        Result.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        Result.TenChucNang = Utils.ConvertToString(dr["TenChucNang"], String.Empty);
                        Result.MaChucNang = Utils.ConvertToString(dr["MaChucNang"], String.Empty);
                        Result.Quyen = Utils.ConvertToInt32(dr["Quyen"], 0);
                        Result.PhanQuyenID = Utils.ConvertToInt32(dr["PhanQuyenID"], 0);
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

        public PhanQuyenModel PhanQuyen_GetByID(int PhanQuyenID)
        {
            PhanQuyenModel Result = new PhanQuyenModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"PhanQuyenID",SqlDbType.Int),
              };
            parameters[0].Value = PhanQuyenID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_GetByID", parameters))
                {
                    while (dr.Read())
                    {
                        Result = new PhanQuyenModel();
                        Result.PhanQuyenID = Utils.ConvertToInt32(dr["PhanQuyenID"], 0);
                        Result.ChucNangID = Utils.ConvertToInt32(dr["ChucNangID"], 0);
                        Result.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        Result.Quyen = Utils.ConvertToInt32(dr["Quyen"], 0);
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

        #endregion


        #region other
        public List<DanhMucCoQuanDonViModel> DanhMucCoQuan_GetAllFoPhanQuyen(int CoQuanID, int NguoiDungID)
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
        public List<HeThongCanBoModel> HeThongCanBo_GetAllByListCoQuanID(int NhomNguoiDungID, int CoQuanID, int NguoiDungID)
        {
            List<HeThongCanBoModel> Result = new List<HeThongCanBoModel>();

            try
            {
                var ListCoQuanID = new List<int>();
                var ListNhomNguoiDung = NhomNguoiDung_GetByNhomTongID(NhomNguoiDungID);
                var NhomHienTai = NhomNguoiDung_GetByID(NhomNguoiDungID);
                if (NhomHienTai != null && NhomHienTai.CoQuanID > 0) ListCoQuanID.Add(NhomHienTai.CoQuanID);
                if (ListNhomNguoiDung.Count > 0)
                {
                    ListNhomNguoiDung.ForEach(x => ListCoQuanID.Add(x.CoQuanID));
                }
                //if (UserRole.CheckAdmin(NguoiDungID))
                //{
                //    ListCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(0).Where(x => x.CoQuanID != CoQuanID).ToList();

                //}
                //else ListCoQuan = new DanhMucCoQuanDonViDAL().GetAllCapCon(CoQuanID);
                return new HeThongCanBoDAL().GetAllByListCoQuanID(ListCoQuanID);
            }
            catch (Exception ex)
            {
                return Result;
                throw ex;
            }
        }

        public List<HeThongNguoiDungModel> HeThongNguoiDung_GetAllByListCoQuanID(int NhomNguoiDungID)
        {
            List<HeThongNguoiDungModel> Result = new List<HeThongNguoiDungModel>();

            try
            {
                var ListCoQuanID = new List<int>();
                var ListNhomNguoiDung = NhomNguoiDung_GetByNhomTongID(NhomNguoiDungID);
                var NhomHienTai = NhomNguoiDung_GetByID(NhomNguoiDungID);
                if (NhomHienTai != null && NhomHienTai.CoQuanID > 0) ListCoQuanID.Add(NhomHienTai.CoQuanID);
                if (ListNhomNguoiDung.Count > 0)
                {
                    ListNhomNguoiDung.ForEach(x => ListCoQuanID.Add(x.CoQuanID));
                }
                return new HeThongNguoiDungDAL().GetAllByListCoQuanID(ListCoQuanID);
            }
            catch (Exception ex)
            {
                return Result;
                throw ex;
            }
        }
        #endregion


        #region create by AnhVH 27/12/2019
        ///// <summary>
        ///// thêm mới nhóm người dùng 
        ///// chưa dùng
        ///// </summary>
        ///// <param name="NhomNguoiDungModel"></param>
        ///// <param name="NguoiDungID"></param>
        ///// <param name="CoQuanID"></param>
        ///// <returns></returns>
        //public BaseResultModel NhomNguoiDung_Insert_New(NhomNguoiDungModel NhomNguoiDungModel, int NguoiDungID, int CoQuanID)
        //{
        //    var Result = new BaseResultModel();
        //    try
        //    {
        //        if (NhomNguoiDungModel == null || NhomNguoiDungModel.TenNhom == null || NhomNguoiDungModel.TenNhom.Trim().Length < 1)
        //        {
        //            Result.Status = 0;
        //            Result.Message = "Tên nhóm người dùng không được trống";
        //        }
        //        else if (NhomNguoiDungModel.TenNhom.Trim().Length > 200)
        //        {
        //            Result.Status = 0;
        //            Result.Message = "Tên nhóm người dùng không được quá 200 ký tự";
        //        }
        //        else if (NhomNguoiDungModel.GhiChu != null && NhomNguoiDungModel.GhiChu.Trim().Length > 200)
        //        {
        //            Result.Status = 0;
        //            Result.Message = "Ghi chú không được quá 200 ký tự";
        //        }
        //        else
        //        {
        //            var crObj = NhomNguoiDung_GetByName(NhomNguoiDungModel.TenNhom);
        //            if (crObj != null && crObj.NhomNguoiDungID > 0)
        //            {
        //                Result.Status = 0;
        //                Result.Message = "Tên nhóm người dùng đã tồn tại";
        //            }
        //            else
        //            {
        //                var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
        //                pList.TypeName = "dbo.list_ID";
        //                // var TrangThai = 400;
        //                var tbCoQuanID = new DataTable();
        //                tbCoQuanID.Columns.Add("ID", typeof(string));
        //                if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
        //                {
        //                    NhomNguoiDungModel.DanhSachCoQuanID = new List<int>();
        //                    var listCoQuan = new List<DanhMucCoQuanDonViModel>();
        //                    // nếu là Admin hệ thống thì tạo cho toàn bộ hệ thống
        //                    if (UserRole.CheckAdmin(NguoiDungID))
        //                    {
        //                        listCoQuan = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(0).Where(x => x.CoQuanID != CoQuanID).ToList();
        //                    }
        //                    // nếu ko thì tạo cho toàn bộ đơn vị trực thuộc đơn vị
        //                    else
        //                    {
        //                        listCoQuan = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID);
        //                    }
        //                    if (listCoQuan.Count > 0)
        //                    {
        //                        listCoQuan.ForEach(x => NhomNguoiDungModel.DanhSachCoQuanID.Add(x.CoQuanID));
        //                    }
        //                    if (!UserRole.CheckAdmin(NguoiDungID))
        //                    {
        //                        NhomNguoiDungModel.DanhSachCoQuanID.Add(CoQuanID);
        //                    }
        //                }
        //                SqlParameter[] parameters = new SqlParameter[]
        //                  {
        //                    new SqlParameter("TenNhom", SqlDbType.NVarChar),
        //                    new SqlParameter("GhiChu", SqlDbType.NVarChar),
        //                    new SqlParameter("CoQuanID", SqlDbType.NVarChar),
        //                    new SqlParameter("NhomTongID", SqlDbType.NVarChar),
        //                    new SqlParameter("NhomTongOut",SqlDbType.Int),
        //                    new SqlParameter("ApDungCho", SqlDbType.TinyInt),
        //                     pList
        //                   ,
        //                  };
        //                parameters[0].Value = NhomNguoiDungModel.TenNhom.Trim();
        //                parameters[1].Value = NhomNguoiDungModel.GhiChu.Trim();
        //                parameters[2].Value = NhomNguoiDungModel.CoQuanID;
        //                parameters[3].Value = 0;
        //                parameters[4].Direction = ParameterDirection.Output;
        //                parameters[4].Size = 8;
        //                if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
        //                    parameters[5].Value = 1;
        //                else parameters[5].Value = Convert.DBNull;
        //                parameters[6].Value = tbCoQuanID;

        //                using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
        //                {
        //                    conn.Open();
        //                    using (SqlTransaction trans = conn.BeginTransaction())
        //                    {
        //                        try
        //                        {
        //                            Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_Insert", parameters);
        //                            trans.Commit();
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Result.Status = -1;
        //                            Result.Message = ConstantLogMessage.API_Error_System;
        //                            trans.Rollback();
        //                            throw ex;
        //                        }
        //                    }

        //                    for (int i = 0; i < NhomNguoiDungModel.DanhSachCoQuanID.Count; i++)
        //                    {
        //                        NhomNguoiDungModel.CoQuanID = NhomNguoiDungModel.DanhSachCoQuanID[i];
        //                        NhomNguoiDungModel.NhomTongID = Utils.ConvertToInt32(parameters[4].Value, 0); ;
        //                        var query = NhomNguoiDung_InsertOne(NhomNguoiDungModel);
        //                        if (query.Status < 1)
        //                        {
        //                            return query;
        //                        }
        //                    }
        //                    Result.Message = ConstantLogMessage.Alert_Insert_Success("Nhóm người dùng");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Status = -1;
        //        Result.Message = ConstantLogMessage.API_Error_System;
        //        throw ex;
        //    }
        //    if (Result.Status > 0)
        //    {
        //        Result.Status = 1;
        //    }
        //    return Result;
        //}


        /// <summary>
        /// lấy danh sách người dùng được phép thêm vào trong nhóm người dùng
        /// đã loại bỏ những người dùng đã được thêm trước đó
        /// </summary>
        /// <param name="NhomNguoiDungID"></param>
        /// <param name="NguoiDungID"></param>
        /// <returns></returns>
        public List<HeThongNguoiDungModel> HeThong_NguoiDung_GetListBy_NhomNguoiDungID(int NhomNguoiDungID, int NguoiDungID)
        {
            List<HeThongNguoiDungModel> Result = new List<HeThongNguoiDungModel>();

            try
            {
                var crNguoiDung = new HeThongNguoiDungDAL().GetByID(NguoiDungID);
                var crNhomNguoiDung = NhomNguoiDung_GetByID(NhomNguoiDungID);
                var listDonViCoTrongNhomNguoiDungAll = NhomNguoiDung_GetByNhomTongID(crNhomNguoiDung.NhomTongID == 0 ? crNhomNguoiDung.NhomNguoiDungID : crNhomNguoiDung.NhomTongID.Value);
                var listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(/*crNhomNguoiDung.CoQuanID*/ crNguoiDung.CoQuanID);
                var DanhSachCoQuanID = new List<int>();
                if (UserRole.CheckAdmin(NguoiDungID))
                {
                    listDonViTrucThuoc = new List<DanhMucCoQuanDonViModel>();
                    listDonViTrucThuoc = new DanhMucCoQuanDonViDAL().GetAllCapCon(0);
                    //.Where(x => x.CoQuanID != crNhomNguoiDung.CoQuanID).ToList();
                }

                var listDonViCoTrongNhomNguoiDung = new List<NhomNguoiDungModel>();
                listDonViCoTrongNhomNguoiDung = listDonViCoTrongNhomNguoiDungAll.Where(x => listDonViTrucThuoc.Select(y => y.CoQuanID).ToList().Contains(x.CoQuanID)).ToList();
                if (listDonViCoTrongNhomNguoiDung.Count != listDonViTrucThuoc.Count)
                {
                    if (listDonViCoTrongNhomNguoiDung.Count > 0)
                    {
                        listDonViCoTrongNhomNguoiDung.ForEach(x => DanhSachCoQuanID.Add(x.CoQuanID));
                    }
                }
                else
                {
                    if (listDonViTrucThuoc != null && listDonViTrucThuoc.Count > 0)
                    {
                        listDonViTrucThuoc.ForEach(x => DanhSachCoQuanID.Add(x.CoQuanID));
                    }
                }
                var DanhSachNguoiDungDaCoTrongNhom = NguoiDung_NhomNguoiDung_GetByListNhomNguoiDungID(listDonViCoTrongNhomNguoiDung.Select(x => x.NhomNguoiDungID).ToList());
                Result = new HeThongNguoiDungDAL().GetAllByListCoQuanID(DanhSachCoQuanID).Where(x => !DanhSachNguoiDungDaCoTrongNhom.Select(y => y.NguoiDungID).ToList().Contains(x.NguoiDungID)).ToList();
                //var list = new HeThongNguoiDungDAL().GetAllByListCoQuanID(DanhSachCoQuanID);

            }
            catch (Exception ex)
            {
                return new List<HeThongNguoiDungModel>();
                throw ex;
            }


            //SqlParameter[] parameters = new SqlParameter[]
            //  {
            //        new SqlParameter("@NhomNguoiDungID",SqlDbType.Int)
            //  };
            //parameters[0].Value = NhomNguoiDungID;
            //try
            //{
            //    using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, @"v1_HeThong_NguoiDung_GetListBy_NhomNguoiDungID", parameters))
            //    {
            //        while (dr.Read())
            //        {
            //            HeThongNguoiDungModel NguoiDungModel = new HeThongNguoiDungModel();
            //            NguoiDungModel.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
            //            NguoiDungModel.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
            //            NguoiDungModel.TenCanBo = Utils.ConvertToString(dr["TenCanBo"], string.Empty);
            //            NguoiDungModel.TenCoQuan = Utils.ConvertToString(dr["TenCoQuan"], string.Empty);
            //            Result.Add(NguoiDungModel);
            //        }
            //        dr.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return Result;
        }


        /// <summary>
        /// lấy danh sách người dùng đã có trong danh sách nhóm người dùng
        /// </summary>
        /// <param name="DanhSachNhomNguoiDungID"></param>
        /// <returns></returns>
        public List<NguoiDungNhomNguoiDungModel> NguoiDung_NhomNguoiDung_GetByListNhomNguoiDungID(List<int> DanhSachNhomNguoiDungID)
        {
            List<NguoiDungNhomNguoiDungModel> Result = new List<NguoiDungNhomNguoiDungModel>();
            var table = new DataTable();
            table.Columns.Add("NhomNguoiDungID", typeof(string));
            DanhSachNhomNguoiDungID.ForEach(x => table.Rows.Add(x));

            var pList = new SqlParameter("@DanhSachNhomNguoiDungID", SqlDbType.Structured);
            pList.TypeName = "dbo.list_ID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                pList
            };
            parameters[0].Value = table;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NguoiDung_NhomNguoiDung_GetBy_ListNhomNguoiDungID", parameters))
                {
                    while (dr.Read())
                    {
                        var crObj = new NguoiDungNhomNguoiDungModel();
                        crObj.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        crObj.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        crObj.TenNguoiDung = Utils.ConvertToString(dr["TenNguoiDung"], string.Empty);
                        Result.Add(crObj);
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

        #endregion

        public bool CheckQuyenDuyetCongKhaiBanKeKhai(int CanBoID)
        {
            try
            {
                var crNguoiDung = new HeThongNguoiDungDAL().GetByCanBoID(CanBoID);
                var ListQuyen = new ChucNangDAL().GetListChucNangByNguoiDungID(crNguoiDung.NguoiDungID);
                if (/*ListQuyen.Any(x => x.ChucNangID == ChucNangEnum.KeKhai_DuyetKeKhaiCongKhai.GetHashCode() && x.Quyen == 15) &&*/ crNguoiDung.VaiTro < EnumVaiTroCanBo.ChuyenVien.GetHashCode())
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public bool CheckQuyenDuyetBanKeKhai(int CanBoID)
        {

            try
            {
                var crNguoiDung = new HeThongNguoiDungDAL().GetByCanBoID(CanBoID);
                var ListQuyen = new ChucNangDAL().GetListChucNangByNguoiDungID(crNguoiDung.NguoiDungID);
                if (ListQuyen.Any(x => /*x.ChucNangID == ChucNangEnum.KeKhai_QuanLyBanKeKhai.GetHashCode() &&*/ x.Quyen == 15) && crNguoiDung.VaiTro < EnumVaiTroCanBo.ChuyenVien.GetHashCode())
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {

                throw;
            }

        }
        #region new code 15.01.2020 - AnhVH - lưu nhóm người dùng theo cách khác
        /// <summary>
        /// Thêm nhóm người dùng cho cơ quan hiện tại và tất cả cơ quan trực thuộc
        /// Tất cả các cấp
        /// </summary>
        /// <param name="NhomNguoiDungModel"></param>
        /// <param name="NguoiDungID"></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public BaseResultModel NhomNguoiDung_InsertNew(NhomNguoiDungModel NhomNguoiDungModel, int NguoiDungID, int CoQuanID)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NhomNguoiDungModel == null || NhomNguoiDungModel.TenNhom == null || NhomNguoiDungModel.TenNhom.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được trống";
                }
                else if (NhomNguoiDungModel.TenNhom.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được quá 200 ký tự";
                }
                else if (NhomNguoiDungModel.GhiChu != null && NhomNguoiDungModel.GhiChu.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Ghi chú không được quá 200 ký tự";
                }
                else
                {
                    var crObj = NhomNguoiDung_GetByName(NhomNguoiDungModel.TenNhom);
                    if (crObj != null && crObj.NhomNguoiDungID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Tên nhóm người dùng đã tồn tại";
                    }
                    else
                    {
                        var pList = new SqlParameter("@DanhSachCanBoID", SqlDbType.Structured);
                        pList.TypeName = "dbo.list_ID";
                        // var TrangThai = 400;
                        var tbCoQuanID = new DataTable();
                        tbCoQuanID.Columns.Add("ID", typeof(string));
                        if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
                        {
                            NhomNguoiDungModel.DanhSachCoQuanID = new List<int>();
                            var listCoQuan = new List<DanhMucCoQuanDonViModel>();
                            // nếu là Admin hệ thống thì tạo cho toàn bộ hệ thống
                            if (UserRole.CheckAdmin(NguoiDungID))
                            {
                                listCoQuan = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(0).Where(x => x.CoQuanChaID == 0 || x.CoQuanChaID == null).ToList();
                            }
                            // nếu ko thì tạo cho toàn bộ đơn vị trực thuộc đơn vị
                            else
                            {
                                listCoQuan = new DanhMucCoQuanDonViDAL().GetAllIDByCoQuanChaID(CoQuanID);
                            }
                            if (listCoQuan.Count > 0)
                            {
                                listCoQuan.ForEach(x => NhomNguoiDungModel.DanhSachCoQuanID.Add(x.CoQuanID));
                            }
                            if (!UserRole.CheckAdmin(NguoiDungID))
                            {
                                NhomNguoiDungModel.DanhSachCoQuanID.Add(CoQuanID);
                            }
                        }
                        SqlParameter[] parameters = new SqlParameter[]
                          {
                            new SqlParameter("TenNhom", SqlDbType.NVarChar),
                            new SqlParameter("GhiChu", SqlDbType.NVarChar),
                            new SqlParameter("CoQuanID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongOut",SqlDbType.Int),
                            new SqlParameter("ApDungCho", SqlDbType.TinyInt),
                             pList
                           ,
                          };
                        parameters[0].Value = NhomNguoiDungModel.TenNhom.Trim();
                        parameters[1].Value = NhomNguoiDungModel.GhiChu.Trim();
                        parameters[2].Value = NhomNguoiDungModel.CoQuanID;
                        parameters[3].Value = 0;
                        parameters[4].Direction = ParameterDirection.Output;
                        parameters[4].Size = 8;
                        if (NhomNguoiDungModel.DanhSachCoQuanID == null || NhomNguoiDungModel.DanhSachCoQuanID.Count < 1)
                            parameters[5].Value = 1;
                        else parameters[5].Value = Convert.DBNull;
                        parameters[6].Value = tbCoQuanID;

                        using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                        {
                            conn.Open();
                            using (SqlTransaction trans = conn.BeginTransaction())
                            {
                                try
                                {
                                    Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_Insert", parameters);
                                    trans.Commit();
                                }
                                catch (Exception ex)
                                {
                                    Result.Status = -1;
                                    Result.Message = ConstantLogMessage.API_Error_System;
                                    trans.Rollback();
                                    throw ex;
                                }
                            }

                            for (int i = 0; i < NhomNguoiDungModel.DanhSachCoQuanID.Count; i++)
                            {
                                NhomNguoiDungModel.CoQuanID = NhomNguoiDungModel.DanhSachCoQuanID[i];
                                NhomNguoiDungModel.NhomTongID = Utils.ConvertToInt32(parameters[4].Value, 0); ;
                                var query = NhomNguoiDung_InsertOne(NhomNguoiDungModel);
                                if (query.Status < 1)
                                {
                                    return query;
                                }
                            }
                            Result.Message = ConstantLogMessage.Alert_Insert_Success("Nhóm người dùng");
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
            if (Result.Status > 0)
            {
                Result.Status = 1;
            }
            return Result;
        }


        /// <summary>
        /// Thêm nhóm người cho các cơ quan trực thuộc. 1 cấp
        /// </summary>
        /// <param name="NhomNguoiDungModel"></param>
        /// <returns></returns>
        public BaseResultModel NhomNguoiDung_InsertFoCapCon(NhomNguoiDungModel NhomNguoiDungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NhomNguoiDungModel == null || NhomNguoiDungModel.TenNhom == null || NhomNguoiDungModel.TenNhom.Trim().Length < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được trống";
                }
                else if (NhomNguoiDungModel.TenNhom.Trim().Length > 200)
                {
                    Result.Status = 0;
                    Result.Message = "Tên nhóm người dùng không được quá 200 ký tự";
                }
                else if (NhomNguoiDungModel.CoQuanID == null || NhomNguoiDungModel.CoQuanID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "CoQuanID không được trống";
                }
                else if (!CheckTonTaiCoQuan(NhomNguoiDungModel.CoQuanID))
                {
                    Result.Status = 0;
                    Result.Message = "CoQuanID " + NhomNguoiDungModel.CoQuanID + " không tồn tại";
                }
                else
                {
                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("TenNhom", SqlDbType.NVarChar),
                            new SqlParameter("GhiChu", SqlDbType.NVarChar),
                            new SqlParameter("CoQuanID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongID", SqlDbType.NVarChar),
                            new SqlParameter("NhomTongOut",SqlDbType.Int),
                            new SqlParameter("ApDungCho", SqlDbType.TinyInt)
                      };
                    parameters[0].Value = NhomNguoiDungModel.TenNhom.Trim();
                    parameters[1].Value = NhomNguoiDungModel.GhiChu ?? Convert.DBNull;
                    parameters[2].Value = NhomNguoiDungModel.CoQuanID;
                    parameters[3].Value = NhomNguoiDungModel.NhomTongID ?? 0;
                    parameters[4].Direction = ParameterDirection.Output;
                    parameters[4].Size = 8;
                    parameters[5].Value = Convert.DBNull;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v1_HeThong_PhanQuyen_NhomNguoiDung_Insert", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("Nhóm người dùng");
                                Result.Data = Utils.ConvertToInt32(parameters[4].Value, 0);
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
                //}
            }
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }

        /// <summary>
        /// danh sách nhóm người dùng mà cơ quan hiện tại được phân
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public List<NhomNguoiDungModel> GetListNhomNguoiDung_ByCoQuanID(int? CoQuanID)
        {
            List<NhomNguoiDungModel> list = new List<NhomNguoiDungModel>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("CoQuanID",SqlDbType.Int)
            };
            parameters[0].Value = CoQuanID ?? Convert.DBNull;
            try
            {

                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, CommandType.StoredProcedure, "v1_HT_GetNhomNguoiDungByCoQuanID", parameters))
                {
                    while (dr.Read())
                    {
                        NhomNguoiDungModel NhomNguoiDungModel = new NhomNguoiDungModel();
                        NhomNguoiDungModel.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        NhomNguoiDungModel.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        NhomNguoiDungModel.GhiChu = Utils.ConvertToString(dr["GhiChu"], string.Empty);
                        NhomNguoiDungModel.ApDungCho = Utils.ConvertToInt32(dr["ApDungCho"], 0);
                        NhomNguoiDungModel.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                        NhomNguoiDungModel.NhomTongID = Utils.ConvertToInt32(dr["NhomTongID"], 0);
                        list.Add(NhomNguoiDungModel);
                    }
                    dr.Close();
                }
                //TotalRow = Utils.ConvertToInt32(parameters[6].Value, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        #endregion


        #region v2 create anhvh - 06/08/2021

        /// <summary>
        /// Thêm mới các nhóm người dùng, chức năng cho từng nhóm mặc định khi tạo cơ quan sử dụng phần mềm mới
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public BaseResultModel NhomNguoiDung_IsertNhomChoCoQuanSuDungPhanMem(int CoQuanID, int NguoiDungID)
        {
            var Result = new BaseResultModel();
            try
            {
                var dsNhomMacDinhThemChoCoQuan = new List<int>();
                try
                {
                    dsNhomMacDinhThemChoCoQuan = new SystemConfigDAL().GetByKey("NhomNguoiDungMacDinhThemMoiChoCoQuan").ConfigValue.ToString().Split(",").ToList()
                        .Select(x => Utils.ConvertToInt32(x, 0)).ToList();
                }
                catch (Exception)
                {
                    dsNhomMacDinhThemChoCoQuan = new List<int>();
                }
                if (dsNhomMacDinhThemChoCoQuan != null && dsNhomMacDinhThemChoCoQuan.Count > 0)
                {
                    foreach (var item in dsNhomMacDinhThemChoCoQuan)
                    {
                        var nhomTong = new PhanQuyenDAL().NhomNguoiDung_GetByID(item);
                        if (nhomTong != null && nhomTong.NhomNguoiDungID > 0)
                        {
                            var itemInSert = new NhomNguoiDungModel();
                            itemInSert.TenNhom = nhomTong.TenNhom;
                            itemInSert.CoQuanID = CoQuanID;
                            itemInSert.NhomTongID = nhomTong.NhomNguoiDungID;
                            itemInSert.GhiChu = nhomTong.GhiChu;
                            //thêm nhóm người dùng cho cơ quan
                            var qeury = NhomNguoiDung_InsertOne(itemInSert);
                            if (qeury.Status < 1)
                            {
                                return Result;
                            }
                            // lấy danh sách chức năng của nhóm tổng
                            var listChucNang = ChucNang_GetQuyenByNhomNguoiDungID(nhomTong.NhomNguoiDungID);
                            // insert chức năng cho nhóm mới được tạo
                            if (listChucNang != null && listChucNang.Count > 0)
                            {
                                for (int j = 0; j < listChucNang.Count; j++)
                                {
                                    var itemPhanQuyen = new PhanQuyenModel();
                                    itemPhanQuyen.ChucNangID = listChucNang[j].ChucNangID;
                                    itemPhanQuyen.Quyen = listChucNang[j].Quyen;
                                    itemPhanQuyen.NhomNguoiDungID = Utils.ConvertToInt32(qeury.Data, 0);
                                    var insertChucNang = PhanQuyen_InsertOne(itemPhanQuyen);
                                }
                            }
                            // insert người dùng cho nhóm người dùng
                            var isNguoiDungVaoNhom = PhanQuyen_ThemNguoiDungVaoNhonm(new NguoiDungNhomNguoiDungModel() { NguoiDungID = NguoiDungID, NhomNguoiDungID = Utils.ConvertToInt32(qeury.Data, 0) });
                            if (isNguoiDungVaoNhom.Status < 1)
                            {
                                return isNguoiDungVaoNhom;
                            }
                        }
                    }
                    Result.Status = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        /// <summary>
        /// Thêm người dùng vào nhóm
        /// </summary>
        /// <param name="NguoiDungNhomNguoiDungModel"></param>
        /// <returns></returns>
        public BaseResultModel PhanQuyen_ThemNguoiDungVaoNhonm(NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel)
        {
            var Result = new BaseResultModel();
            try
            {
                if (NguoiDungNhomNguoiDungModel.NhomNguoiDungID == null || NguoiDungNhomNguoiDungModel.NhomNguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "NhomNguoiDungID không được trống";
                }
                else if (NguoiDungNhomNguoiDungModel.NguoiDungID == null || NguoiDungNhomNguoiDungModel.NguoiDungID < 1)
                {
                    Result.Status = 0;
                    Result.Message = "NguoiDungID không được trống";
                }
                else
                {
                    var checkNguoiDungTrongHeThong = NguoiDung_NhomNguoiDung_GetByNguoiDungID_And_NhomNguoiDungID(NguoiDungNhomNguoiDungModel.NguoiDungID, NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
                    if (checkNguoiDungTrongHeThong != null && checkNguoiDungTrongHeThong.NguoiDungID > 0)
                    {
                        Result.Status = 0;
                        Result.Message = "Người dùng đã được thêm";
                        return Result;
                    }

                    SqlParameter[] parameters = new SqlParameter[]
                      {
                            new SqlParameter("NguoiDungID",SqlDbType.Int),
                            new SqlParameter("NhomNguoiDungID", SqlDbType.Int)
                      };
                    parameters[0].Value = NguoiDungNhomNguoiDungModel.NguoiDungID;
                    parameters[1].Value = NguoiDungNhomNguoiDungModel.NhomNguoiDungID;

                    using (SqlConnection conn = new SqlConnection(SQLHelper.appConnectionStrings))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                Result.Status = SQLHelper.ExecuteNonQuery(trans, System.Data.CommandType.StoredProcedure, "v2_HeThong_PhanQuyen_ThemNguoiDungVaoNhom", parameters);
                                trans.Commit();
                                Result.Message = ConstantLogMessage.Alert_Insert_Success("Người dùng - Nhóm người dùng");
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
            catch (Exception ex)
            {
                Result.Status = -1;
                Result.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
            return Result;
        }


        /// <summary>
        /// danh sách nhóm người dùng của người dùng
        /// </summary>
        /// <param name="NguoiDungID"></param>
        /// <returns></returns>
        public List<NhomNguoiDungCuNguoiDung> DanhSachNhomNguoiDungCuaNguoiDung(int NguoiDungID)
        {
            List<NhomNguoiDungCuNguoiDung> Result = new List<NhomNguoiDungCuNguoiDung>();
            SqlParameter[] parameters = new SqlParameter[]
              {
                 new SqlParameter(@"NguoiDungID",SqlDbType.Int)
              };
            parameters[0].Value = NguoiDungID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v2_DanhSachNhomNguoiDungCuaNguoiDung", parameters))
                {
                    while (dr.Read())
                    {
                        NhomNguoiDungCuNguoiDung data = new NhomNguoiDungCuNguoiDung();
                        data.NguoiDungID = Utils.ConvertToInt32(dr["NguoiDungID"], 0);
                        data.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        data.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        Result.Add(data);
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

        /// <summary>
        /// lấy nhóm người dùng của cơ quan theo nhóm tổng
        /// </summary>
        /// <param name="NhomTongID"></param>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        public NhomNguoiDungModel NhomNguoiDungCuaCoQuan(int NhomTongID, int CoQuanID)
        {
            NhomNguoiDungModel Result = new NhomNguoiDungModel();
            SqlParameter[] parameters = new SqlParameter[]
              {
                new SqlParameter(@"NhomTongID",SqlDbType.Int),
                new SqlParameter(@"CoQuanID",SqlDbType.Int)
              };
            parameters[0].Value = NhomTongID;
            parameters[1].Value = CoQuanID;
            try
            {
                using (SqlDataReader dr = SQLHelper.ExecuteReader(SQLHelper.appConnectionStrings, System.Data.CommandType.StoredProcedure, "v2_NhomNguoiDungCuaCoQuan", parameters))
                {
                    while (dr.Read())
                    {
                        Result.NhomNguoiDungID = Utils.ConvertToInt32(dr["NhomNguoiDungID"], 0);
                        Result.TenNhom = Utils.ConvertToString(dr["TenNhom"], string.Empty);
                        Result.CoQuanID = Utils.ConvertToInt32(dr["CoQuanID"], 0);
                    }
                    dr.Close();
                }
            }
            catch
            {
                new NhomNguoiDungModel();
            }
            return Result;
        }



        #endregion
    }
}
