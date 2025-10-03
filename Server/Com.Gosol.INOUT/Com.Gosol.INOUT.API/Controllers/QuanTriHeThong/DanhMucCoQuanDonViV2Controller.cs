using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.INOUT.API.Authorization;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.Models.DanhMuc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using QRCoder;
using Microsoft.Extensions.Options;
using Com.Gosol.INOUT.API.Config;
using System.Drawing;

namespace Com.Gosol.INOUT.API.Controllers.DanhMuc
{
    [Route("api/v2/DanhMucCoQuanDonVi")]
    [ApiController]
    public class DanhMucCoQuanDonViV2Controller : BaseApiController
    {
        private IDanhMucCoQuanDonViV2BUS _DanhMucCoQuanDonViBUS;
        private IHostingEnvironment _host;
        private ISystemConfigBUS _systemConfigBUS;
        public IOptions<AppSettings> settings;
        public DanhMucCoQuanDonViV2Controller(IOptions<AppSettings> Settings, ISystemConfigBUS systemConfigBUS, IDanhMucCoQuanDonViV2BUS DanhMucCoQuanDonViBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DanhMucCoQuanDonViV2Controller> logger) : base(_LogHelper, logger)
        {
            this._DanhMucCoQuanDonViBUS = DanhMucCoQuanDonViBUS;
            this._host = hostingEnvironment;
            this._systemConfigBUS = systemConfigBUS;
            this.settings = Settings;
        }
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_ThemCoQuanDonVi, EnumLogType.Insert, () =>
                 {
                     string Message = null;
                     int val = 0;
                     int CoQuanID = 0;
                     val = _DanhMucCoQuanDonViBUS.Insert(DanhMucCoQuanDonViModel, ref CoQuanID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0), ref Message);
                     base.Message = Message;
                     base.Status = val > 0 ? 1 : 0;
                     base.Data = val;
                     //base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucCoQuanDonViModel DanhMucCoQuanDonViModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_SuaCoQuanDonVi, EnumLogType.Update, () =>
                 {
                     string Message = null;
                     int val = 0;
                     val = _DanhMucCoQuanDonViBUS.Update(DanhMucCoQuanDonViModel, ref Message);
                     base.Message = Message;
                     base.Status = val;
                     //base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        // CHưa Sửa
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
                {
                    Dictionary<int, string> dic = new Dictionary<int, string>();
                    dic = _DanhMucCoQuanDonViBUS.Delete(p.ListID);
                    base.Status = dic.FirstOrDefault().Key;
                    base.Message = dic.FirstOrDefault().Value;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Read)]
        [Route("FilterByName")]
        public IActionResult FilterByName(string TenCoQuan)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_FilterByName, EnumLogType.GetList, () =>
                 {
                     int val = 0;
                     List<DanhMucCoQuanDonViModel> Data;
                     Data = _DanhMucCoQuanDonViBUS.FilterByName(TenCoQuan);
                     base.Status = val > 0 ? 1 : 0;
                     base.Data = Data;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Read)]
        [Route("GetByIDAndCap")]
        public IActionResult GetByIDAndCap(int CoQuanID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetByID, EnumLogType.GetList, () =>
                 {
                     DanhMucCoQuanDonViPartialNew Data;
                     Data = _DanhMucCoQuanDonViBUS.GetByID(CoQuanID);
                     if (Data != null && Data.CoQuanID > 0 && Data.SuDungPM == true)
                         Data.QRCode = new Commons().GenQRCode(settings.Value.Domain + "/guest?q=" + Utils.ConvertToNullableInt32(Data.CoQuanID, null));
                     base.Status = Data.CoQuanID > 0 ? 1 : 0;
                     base.Data = Data;
                     base.Message = Data.CoQuanID > 0 ? ConstantLogMessage.API_Success : ConstantLogMessage.API_Error;
                     return base.GetActionResult();
                 });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Read)]
        [Route("GetListByidAndCap")]
        public IActionResult GetListByidAndCap()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetByID, EnumLogType.GetList, () =>
                {
                    int val = 0;
                    List<DanhMucCoQuanDonViModel> Data;
                    Data = _DanhMucCoQuanDonViBUS.GetListByidAndCap();
                    base.Status = val > 0 ? 1 : 0;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }

            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Read)]
        [Route("GetAllByCap")]
        public IActionResult GetAllByCap([FromQuery] int ID, int Cap, string Keyword)
        {
            try
            {

                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetAllByCap, EnumLogType.GetList, () =>
                {

                    IList<DanhMucCoQuanDonViModelPartial> Data;
                    Data = _DanhMucCoQuanDonViBUS.GetAllByCap(ID, Cap, Keyword);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = Data.Count;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                return base.GetActionResult();
            }
        }

        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Read)]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] int ID, string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetAllByCap, EnumLogType.GetList, () =>
                {
                    IList<DanhMucCoQuanDonViModelPartial> Data;
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    Data = _DanhMucCoQuanDonViBUS.GetALL(ID, CoQuanID, Keyword);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count > 0 ? 1 : 0;
                    base.Data = Data;
                    base.TotalRow = Data.Count;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                return base.GetActionResult();
            }
        }
        [HttpGet]
        [Route("GetListByUser")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult PhanQuyen_GetDanhMucCoQuan()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var result = _DanhMucCoQuanDonViBUS.GetListByUser(CoQuanID, NguoiDungID);
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu Cơ quan" : "";
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetListByUser_FoPhanQuyen")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult GetListByUser_FoPhanQuyen(string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var result = _DanhMucCoQuanDonViBUS.GetByUser_FoPhanQuyen(CoQuanID, NguoiDungID, Keyword);
                    var cm = new Commons();
                    if (result != null && result.Count > 0)
                    {
                        result.ForEach(x => x.QRCode = x.SuDungPM == true ? cm.GenQRCode(settings.Value.Domain + "/guest?q=" + Utils.ConvertToNullableInt32(x.CoQuanID, null)) : null);
                    }
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu Cơ quan" : "";
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw ex;
            }
        }

        [HttpGet]
        [Route("GetByUser_FoPhanQuyen")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult GetByUser_FoPhanQuyen(string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var result = _DanhMucCoQuanDonViBUS.GetByUser_FoPhanQuyen(CoQuanID, NguoiDungID, Keyword);
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu Cơ quan" : "";
                    return base.GetActionResult();
                });
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw ex;
            }
        }

        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_ChucVu, AccessLevel.Read)]
        [Route("ImportCoQuan")]
        public async Task<IActionResult> ImportCoQuan([FromBody] Files file)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_ImportFile, EnumLogType.Other, () =>
                {
                    string Message = "";
                    string SavePath = _host.ContentRootPath + "\\Upload\\" + "Import_ChucVu.xlsx";
                    using (FileStream stream = System.IO.File.Create(SavePath))
                    {
                        byte[] byteArray = Convert.FromBase64String(file.files);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }

                    var Result = _DanhMucCoQuanDonViBUS.ImportFile(SavePath, ref Message, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0));
                    base.Status = Result;
                    base.Message = Message;
                    base.Data = Data;
                    return base.GetActionResult();
                });

            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Read)]
        [Route("CheckMaCQ")]
        public IActionResult CheckMaCQ([FromQuery] int? CoQuanID, string MaCQ)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetByID, EnumLogType.GetList, () =>
                {
                    var Data = _DanhMucCoQuanDonViBUS.CheckMaCQ(CoQuanID, MaCQ);
                    base.Status = Data.Status;
                    base.Message = Data.Message;
                    //base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }


        #region V2
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_ChucVu, AccessLevel.Read)]
        [Route("KhoiTaoDonViSuDungPhanMem")]
        public async Task<IActionResult> KhoiTaoDonViSuDungPhanMem(ThongTinDangKyModel DangKyInfo)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
                {


                    HeThongCanBoModel canBoInfo = new HeThongCanBoModel();
                    var query = _DanhMucCoQuanDonViBUS.KhoiTaoDonViSuDungPhanMem(DangKyInfo);
                    if (query != null && query.Status > 0)
                    {
                        DangKyInfo.QRCode = new Commons().GenQRCode(settings.Value.Domain + "/guest?q=" + Utils.ConvertToNullableInt32(query.Data, null));
                        DangKyInfo.MatKhau = _systemConfigBUS.GetByKey("MatKhau_MacDinh").ConfigValue ?? "123456";
                        DangKyInfo.TenNguoiDung = DangKyInfo.Email;
                        DangKyInfo.CoQuanID = Utils.ConvertToNullableInt32(query.Data, null);
                        DangKyInfo.SuDungPM = true;
                        DangKyInfo.CQCoHieuLuc = true;
                        DangKyInfo.CoQuanSuDungPhanMenID = DangKyInfo.CoQuanID;
                    }
                    base.Status = query.Status;
                    base.Message = query.Message;
                    base.Data = DangKyInfo;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }


        /// <summary>
        /// danh sách đơn vị sử dụng phần mềm cho super admin
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HT_QuanTriHeThong, AccessLevel.FullAccess)]
        [Route("DanhSachDonViSuDungPhanMem")]
        public async Task<IActionResult> DanhSachDonViSuDungPhanMem([FromQuery] BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
                {
                    var crNguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    if (!UserRole.CheckAdmin(crNguoiDungID))
                    {
                        base.Status = -98;
                        base.Message = "Người dùng không có quyền sử dụng chức năng này.";
                        return base.GetActionResult();
                    }
                    int TotalRow = 0;
                    var data = _DanhMucCoQuanDonViBUS.DanhSachDonViSuDungPhanMem(p, ref TotalRow);
                    var cm = new Commons();
                    if (data != null && data.Count > 0 && TotalRow > 0)
                    {
                        data.ForEach(x => x.QRCode = x.SuDungPM == true ? cm.GenQRCode(settings.Value.Domain + "/guest?q=" + Utils.ConvertToNullableInt32(x.CoQuanID, null)) : null);
                        base.Status = 1;
                        base.Message = "OK";
                        base.Data = data;
                        base.TotalRow = TotalRow;
                    }
                    else
                    {
                        base.Status = 1;
                        base.Message = "No Data";
                        base.Data = data;
                        base.TotalRow = TotalRow;
                    }
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error;
                base.GetActionResult();
                throw;
            }
        }


        /// <summary>
        /// cập nhật trạng thái sử dụng phần mềm của đơn vị sử dụng phần mềm, dùng cho superadmin
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HT_DanhMucCoQuan, AccessLevel.FullAccess)]
        [Route("CapNhatTrangThaiSuDungPhanMem")]
        public IActionResult CapNhatTrangThaiSuDungPhanMem(DanhMucCoQuanDonViModel rq)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_SuaCoQuanDonVi, EnumLogType.Update, () =>
                {
                    var crNguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    if (!UserRole.CheckAdmin(crNguoiDungID))
                    {
                        base.Status = -98;
                        base.Message = "Người dùng không có quyền sử dụng chức năng này.";
                        return base.GetActionResult();
                    }
                    var val = _DanhMucCoQuanDonViBUS.CapNhatTrangThaiSuDungPhanMem(rq.CoQuanID, rq.TrangThai);
                    base.Message = val.Message;
                    base.Status = val.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }


        /// <summary>
        /// thông tin chi tiết của đơn vị sử dụng phần mềm
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HT_QuanTriHeThong, AccessLevel.Read)]
        [Route("ChiTietDonViSuDungPhanMem")]
        public async Task<IActionResult> ChiTietDonViSuDungPhanMem(int CoQuanID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
                {
                    var crNguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    if (!UserRole.CheckAdmin(crNguoiDungID))
                    {
                        base.Status = -98;
                        base.Message = "Người dùng không có quyền sử dụng chức năng này.";
                        return base.GetActionResult();
                    }
                    int TotalRow = 0;
                    var data = _DanhMucCoQuanDonViBUS.ChiTietCoQuanSuDungPhanMem(CoQuanID);
                    if (data != null && data.CoQuanID > 0)
                    {
                        var cm = new Commons();
                        data.QRCode = cm.GenQRCode(settings.Value.Domain + "/guest?q=" + Utils.ConvertToNullableInt32(data.CoQuanID, null));
                        base.Status = 1;
                        base.Message = "OK";
                        base.Data = data;
                        base.TotalRow = TotalRow;
                    }
                    else
                    {
                        base.Status = 1;
                        base.Message = "No Data";
                        base.Data = data;
                        base.TotalRow = TotalRow;
                    }
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error;
                base.GetActionResult();
                throw;
            }
        }


        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HT_DanhMucCoQuan, AccessLevel.FullAccess)]
        [Route("CapNhatThongTinDonViSuDungPhanMem")]
        public IActionResult CapNhatThongTinDonViSuDungPhanMem(ThongTinDangKyModel rq)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_SuaCoQuanDonVi, EnumLogType.Update, () =>
                {
                    var crNguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    if (!UserRole.CheckAdmin(crNguoiDungID))
                    {
                        base.Status = -98;
                        base.Message = "Người dùng không có quyền sử dụng chức năng này.";
                        return base.GetActionResult();
                    }
                    var val = _DanhMucCoQuanDonViBUS.CapNhatThongTinDonViSuDungPhanMem(rq);
                    base.Message = val.Message;
                    base.Status = val.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.HT_QuanTriHeThong, AccessLevel.Read)]
        [Route("DanhSachCanBoThuocDonViSuDungPhanMen")]
        public async Task<IActionResult> DanhSachCanBoThuocDonViSuDungPhanMen([FromQuery] BasePagingParamsForFilter p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
                {
                    var crDonViSuDungPhanMen = _DanhMucCoQuanDonViBUS.GetByID(Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanSuDungPhanMem").Value, 0));
                    p.CoQuanChaID = crDonViSuDungPhanMen.CoQuanID;
                    int TotalRow = 0;
                    var data = new ThongTinDangKyModel();
                    data.CoQuanID = crDonViSuDungPhanMen.CoQuanID;
                    data.TenCoQuanSuDungPhanMen = crDonViSuDungPhanMen.TenCoQuan;
                    data.DanhSachCanBo = _DanhMucCoQuanDonViBUS.DanhSachCanBoThuocDonViSuDungPhanMen(p, ref TotalRow);
                    var cm = new Commons();
                    if (data != null)
                    {
                        data.QRCode = cm.GenQRCode(settings.Value.Domain + "/guest?q=" + Utils.ConvertToNullableInt32(crDonViSuDungPhanMen.CoQuanID, null));
                        base.Status = 1;
                        base.Message = "OK";
                        base.Data = data;
                        base.TotalRow = TotalRow;
                    }
                    else
                    {
                        base.Status = 1;
                        base.Message = "No Data";
                        base.Data = data;
                        base.TotalRow = TotalRow;
                    }
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error;
                base.GetActionResult();
                throw;
            }
        }

        /// <summary>
        /// thêm mới cán bộ vào phòng ban,
        /// nếu coquanid =null or =0 thì thêm mới cơ quan rồi thêm cán bộ,
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HT_DanhMucCoQuan, AccessLevel.Create)]
        [Route("ThemCanBoVaoDonViSuDungPhanMen")]
        public IActionResult ThemCanBoVaoDonViSuDungPhanMen(ThongTinDangKyModel rq)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_SuaCoQuanDonVi, EnumLogType.Update, () =>
                {
                    var crDonViSuDungPhanMem = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanSuDungPhanMem").Value, 0);
                    rq.CoQuanSuDungPhanMenID = crDonViSuDungPhanMem;

                    var val = _DanhMucCoQuanDonViBUS.ThemCanBoVaoDonViSuDungPhanMen(rq);
                    base.Message = val.Message;
                    base.Status = val.Status;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        /// <summary>
        /// sửa thông tin phòng ban,
        /// sửa tên
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [Route("SuaPhongBan")]
        [CustomAuthAttribute(ChucNangEnum.HT_DanhMucCoQuan, AccessLevel.Edit)]
        public IActionResult SuaPhongBan(DanhMucCoQuanDonViModel rq)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_SuaCoQuanDonVi, EnumLogType.Update, () =>
                {
                    var val = _DanhMucCoQuanDonViBUS.SuaPhongBan(rq.CoQuanID, rq.TenCoQuan);
                    base.Message = val.Message;
                    base.Status = val.Status;
                    //base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }

        /// <summary>
        /// xoá phòng ban,
        /// xoá phân quyền, người dùng_nhóm người dùng, nhóm người dùng, người dùng, cán bộ, phòng ban
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HT_DanhMucCoQuan, AccessLevel.Delete)]
        [Route("XoaPhongBan")]
        public IActionResult XoaPhongBan(DanhMucCoQuanDonViModel rq)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
                {
                    var val = _DanhMucCoQuanDonViBUS.XoaPhongBan(rq.CoQuanID);
                    base.Status = val.Status;
                    base.Message = val.Message;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }
        }
        //[HttpPost]
        ////[CustomAuthAttribute(ChucNangEnum.DanhMuc_CoQuan, AccessLevel.Delete)]
        //[Route("DeleteCoQuanSuDungPhanMem")]
        //public IActionResult DeleteCoQuanSuDungPhanMem([FromBody] BaseDeleteParams p)
        //{
        //    try
        //    {
        //        return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_XoaCoQuanDonVi, EnumLogType.Delete, () =>
        //        {
        //            Dictionary<int, string> dic = new Dictionary<int, string>();
        //            dic = _DanhMucCoQuanDonViBUS.Delete(p.ListID);
        //            base.Status = dic.FirstOrDefault().Key;
        //            base.Message = dic.FirstOrDefault().Value;
        //            return base.GetActionResult();
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        base.Status = -1;
        //        base.GetActionResult();
        //        throw;
        //    }
        //}
        #endregion
    }
}