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
using Microsoft.Extensions.Logging;
using Com.Gosol.INOUT.BUS.VaoRaV2;
using Com.Gosol.INOUT.Models.InOut;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using static System.Net.WebRequestMethods;
using System.Diagnostics;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Microsoft.Extensions.Options;
using Com.Gosol.INOUT.API.Config;

namespace Com.Gosol.INOUT.API.Controllers.VaoRaV2
{

    [Route("api/v2/VaoRa")]
    [ApiController]
    public class VaoRaV2Controller : BaseApiController
    {
        private IVaoRaV2BUS _VaoRaBUS;
        private ISystemConfigBUS _SystemConfigBUS;
        private IDanhMucCoQuanDonViBUS _DanhMucCoQuanDonViBUS;
        public IOptions<AppSettings> settings;
        public VaoRaV2Controller(IOptions<AppSettings> Settings, IDanhMucCoQuanDonViBUS danhMucCoQuanDonViBUS, IVaoRaV2BUS VaoRaBUS, ISystemConfigBUS SystemConfigBUS, ILogHelper _LogHelper, ILogger<VaoRaV2Controller> logger) : base(_LogHelper, logger)
        {
            this._VaoRaBUS = VaoRaBUS;
            this._SystemConfigBUS = SystemConfigBUS;
            this._DanhMucCoQuanDonViBUS = danhMucCoQuanDonViBUS;
            this.settings = Settings;
        }
        [HttpGet]
        [Route("test")]
        public string test()
        {
            var result = "";
            result = ConvertImage_FromUrl_ToBase64(_SystemConfigBUS.GetByKey("Camera_Path").ConfigValue);
            return result;
        }


        /// <summary>
        /// trả về base64 từ camera trong cấu hình hệ thống cho client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get_AnhChanDung_FromCamera")]
        public IActionResult Get_AnhChanDung_FromCamera()
        {
            var result = "";
            try
            {
                var cameraIp = _SystemConfigBUS.GetByKey("Camera_Path").ConfigValue;
                //var t = cameraIp.Split('/')[2];
                var tt = "http://" + cameraIp.Split('/')[2] + "/jpg/1/image.jpg";
                result = ConvertImage_FromUrl_ToBase64(tt);
                if (result == null || result == string.Empty || result == "" || result.Length == 0)
                {
                    base.Status = 1;
                    base.Message = ConstantLogMessage.API_NoData;
                    return base.GetActionResult();
                }
                base.Status = 1;
                base.Data = result;
                return base.GetActionResult();
            }
            catch (Exception)
            {
                base.Status = -2;
                base.GetActionResult();
                throw;
            }
        }

        [HttpPost]
        [Route("Vao")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Create)]
        public IActionResult Vao(ThongTinVaoRaModel VaoRaModel)
        {
            try
            {
                //var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0);
                //var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);

                return CreateActionResult("CheckIn_V2", EnumLogType.Insert, () =>
                {
                    // check xem cán bộ đã checkin chưa bằng ảnh chân dung
                    //if (VaoRaModel.AnhChanDungBase64 != null && VaoRaModel.AnhChanDungBase64.Length > 0)
                    //{
                    //    if (KiemTraKhachDaCheckInChua_By_AnhChanDung(VaoRaModel.AnhChanDungBase64))
                    //    {
                    //        base.Status = 0;
                    //        base.Message = "Khách chưa checkout. Vui lòng kiểm tra lại.";
                    //        return base.GetActionResult();
                    //    }

                    //}

                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                    if (VaoRaModel.SoCMND != null && VaoRaModel.SoCMND.Length > 0)
                    {
                        var cr = _VaoRaBUS.Get_By_MaThe(VaoRaModel.SoCMND, EnumLoaiCheckOut.CMND.GetHashCode(), VaoRaModel.DonViSuDungID.Value, serverPath);
                        if (cr != null && cr.Count > 0)
                        {
                            base.Status = 0;
                            base.Message = "Khách chưa checkout. Vui lòng kiểm tra lại.";
                            return base.GetActionResult();
                        }

                    }
                    var crDonViSuDung = _DanhMucCoQuanDonViBUS.GetByID(VaoRaModel.DonViSuDungID.Value);
                    if (crDonViSuDung == null || crDonViSuDung.CoQuanID < 1)
                    {
                        base.Status = 0;
                        base.Message = "Cơ quan không tồn tại";
                        return base.GetActionResult();
                    }
                    VaoRaModel.TenDonViSuDung = crDonViSuDung.TenCoQuan;

                    // thêm thông tin vào ra
                    //var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0);
                    //VaoRaModel.LeTan_Vao = CanBoID;
                    //VaoRaModel.DonViSuDungID = DonViSuDungID;
                    var MaThe = ""; var SoCMND = "";
                    var gioVao = DateTime.Now;
                    VaoRaModel.GioVao = gioVao;
                    var Result = _VaoRaBUS.Vao(VaoRaModel, ref MaThe, ref SoCMND, VaoRaModel.DonViSuDungID.Value);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
                    VaoRaModel.ThongTinVaoRaID = Result.Status;
                    VaoRaModel.QRCode = new Commons().GenQRCode(settings.Value.Domain + "/guest?c=" + Utils.ConvertToNullableInt32(Result.Status, null) + "&q=" + VaoRaModel.DonViSuDungID.Value);
                    base.Data = VaoRaModel;

                    // thêm ảnh
                    if (Result.Status > 0)
                    {
                        var TenFile = SoCMND + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                        //thêm ảnh cmnd mặt trước
                        //var query = InsertFileDinhKem(SoCMND, Result.Status, VaoRaModel.AnhChanDungBase64);
                        if (VaoRaModel.AnhCMND_MTBase64 != null && VaoRaModel.AnhCMND_MTBase64.Length > 0)
                        {
                            if (InsertAnh_CheckIn(TenFile, Result.Status, EnumLoaiFile.MatTruocCMND.GetHashCode(), VaoRaModel.AnhCMND_MTBase64, VaoRaModel.DonViSuDungID.Value).Status < 1)
                            {
                                //base.Status = -1;
                                //base.Message = ConstantLogMessage.API_Error_System;
                            }
                        }

                        if (VaoRaModel.AnhCMND_MSBase64 != null && VaoRaModel.AnhCMND_MSBase64.Length > 0)
                        {
                            if (InsertAnh_CheckIn(TenFile, Result.Status, EnumLoaiFile.MatSauCMND.GetHashCode(), VaoRaModel.AnhCMND_MSBase64, VaoRaModel.DonViSuDungID).Status < 1)
                            {
                                //base.Status = -1;
                                //base.Message = ConstantLogMessage.API_Error_System;
                            }
                        }

                        if (VaoRaModel.AnhChanDungBase64 != null && VaoRaModel.AnhChanDungBase64.Length > 0)
                        {
                            if (InsertAnh_CheckIn(TenFile, Result.Status, EnumLoaiFile.AnhChanDung.GetHashCode(), VaoRaModel.AnhChanDungBase64, VaoRaModel.DonViSuDungID).Status < 1)
                            {
                                //base.Status = -1;
                                //base.Message = ConstantLogMessage.API_Error_System;
                            }
                        }
                    }

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
        [Route("Ra")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Create)]
        public IActionResult Ra(ThongTinVaoRaModel VaoRaModel)
        {
            try
            {
                return CreateActionResult("CheckOut", EnumLogType.Update, () =>
                 {
                     //var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0);
                     //VaoRaModel.LeTan_Ra = CanBoID;
                     var Result = _VaoRaBUS.Ra(VaoRaModel);
                     base.Status = Result.Status;
                     base.Message = Result.Message;
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
        [Route("GetByMaThe_old")]
        [CustomAuthAttribute()]
        public IActionResult GetByMaThe_old(string MaThe, int LoaiCheckOut)
        {
            try
            {
                var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0);
                return CreateActionResult("Lấy thông tin Vao Ra by Mã thẻ", EnumLogType.GetByName, () =>
                 {
                     var clsCommon = new Commons();
                     string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                     var ttVao = _VaoRaBUS.Get_By_MaThe(MaThe, LoaiCheckOut, CoQuanID, serverPath);
                     if (ttVao.Count < 1) { base.Message = "Không có Dữ liệu"; base.Status = 0; }
                     //else
                     //{
                     //    var clsCommon = new Commons();
                     //    string serverPath = clsCommon.GetServerPath(HttpContext);
                     //    base.Message = " "; base.Status = 1;
                     //    foreach (var item in ttVao)
                     //    {
                     //        var DSFileAnh = _VaoRaBUS.FileDinhKem_GetBy_ThongTinRaVaoID(item.ThongTinVaoRaID);
                     //        if (DSFileAnh.Count > 0)
                     //        {
                     //            //var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                     //            //var folderName = Path.Combine(pathSaveFile);
                     //            //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                     //            //var cmClass = new Commons();
                     //            var cmndMT = DSFileAnh.FirstOrDefault(x => x.LoaiFile == EnumLoaiFile.MatTruocCMND.GetHashCode());
                     //            var cmndMS = DSFileAnh.FirstOrDefault(x => x.LoaiFile == EnumLoaiFile.MatSauCMND.GetHashCode());
                     //            var anhCD = DSFileAnh.FirstOrDefault(x => x.LoaiFile == EnumLoaiFile.AnhChanDung.GetHashCode());
                     //            item.AnhChanDungBase64 = serverPath + (cmndMT == null ? "" : anhCD.TenFileHeThong);
                     //            item.AnhCMND_MTBase64 = serverPath + (cmndMT == null ? "" : cmndMT.TenFileHeThong);
                     //            item.AnhCMND_MSBase64 = serverPath + (cmndMT == null ? "" : cmndMS.TenFileHeThong);
                     //            //var mt = cmClass.ConvertFileToBase64(Path.Combine(pathToSave, cmndMT == null ? "" : cmndMT.TenFileHeThong));
                     //            //var ms = cmClass.ConvertFileToBase64(Path.Combine(pathToSave, cmndMS == null ? "" : cmndMS.TenFileHeThong).ToString());
                     //            //var cd = cmClass.ConvertFileToBase64(Path.Combine(pathToSave, anhCD == null ? "" : anhCD.TenFileHeThong).ToString());
                     //            //item.AnhCMND_MTBase64 = mt.Length > 0 ? ("data:image/jpeg;base64," + mt) : "";
                     //            //item.AnhCMND_MSBase64 = ms.Length > 0 ? ("data:image/jpeg;base64," + ms) : "";
                     //            //item.AnhChanDungBase64 = cd.Length > 0 ? ("data:image/jpeg;base64," + cd) : "";
                     //        }
                     //    }
                     //}
                     base.Data = ttVao;
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
        [Route("GetByMaThe")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Read)]
        public IActionResult GetByMaThe(string MaThe, int LoaiCheckOut, int DonViSuDungID)
        {
            try
            {
                //var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);
                return CreateActionResult("Lấy thông tin Vao Ra by Mã thẻ", EnumLogType.GetByName, () =>
                {
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                    var ttVao = _VaoRaBUS.Get_By_MaThe(MaThe, LoaiCheckOut, DonViSuDungID, serverPath);
                    if (ttVao.Count < 1) { base.Message = "Không có Dữ liệu"; base.Status = 0; }
                    base.Data = ttVao;
                    base.Status = 1;
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
        [Route("Get_By_AnhChanDung")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Read)]
        public IActionResult Get_By_AnhChanDung(FileDinhKemModel fileDinhKem, int DonViSuDungID)
        {
            try
            {
                //var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);

                return CreateActionResult("Lấy thông tin vào by ảnh chân dung", EnumLogType.GetByName, () =>
                {
                    var DanhSachDaCheckIn = _VaoRaBUS.Get_For_CheckOut_By_AnhChanDung(DonViSuDungID);
                    var checkResult = false;
                    if (DanhSachDaCheckIn.Count > 0)
                    {
                        var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                        var folderName = Path.Combine(pathSaveFile);
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var cmClass = new Commons();
                        //if (fileDinhKem.LoaiUpload==2)//chụp ảnh
                        //{
                        //    //fileDinhKem.Base64File = ConvertImage_FromUrl_ToBase64("http://192.168.100.158/jpg/1/image.jpg");
                        //    fileDinhKem.Base64File = ConvertImage_FromUrl_ToBase64(_SystemConfigBUS.GetByKey("Camera_Path").ConfigValue);
                        //}
                        foreach (var item in DanhSachDaCheckIn)
                        {
                            var fullPath = Path.Combine(pathToSave, item.TenFileHeThong);
                            var anhChanDung = "data:image/jpeg;base64," + cmClass.ConvertFileToBase64(fullPath.ToString());
                            //gọi api đối chiếu 2 ảnh AnhChanDungBase64 và crBase64

                            if (checkAnhDaiDienAsync(fileDinhKem.Base64File, anhChanDung).Result)
                            {
                                checkResult = true;
                                var DSFileAnh = _VaoRaBUS.FileDinhKem_GetBy_ThongTinRaVaoID(item.ThongTinVaoRaID);
                                if (DSFileAnh.Count > 0)
                                {
                                    var cmndMT = DSFileAnh.FirstOrDefault(x => x.LoaiFile == EnumLoaiFile.MatTruocCMND.GetHashCode());
                                    var cmndMS = DSFileAnh.FirstOrDefault(x => x.LoaiFile == EnumLoaiFile.MatSauCMND.GetHashCode());
                                    var anhCD = DSFileAnh.FirstOrDefault(x => x.LoaiFile == EnumLoaiFile.AnhChanDung.GetHashCode());
                                    item.AnhCMND_MTBase64 = "data:image/jpeg;base64," + cmClass.ConvertFileToBase64(Path.Combine(pathToSave, cmndMT == null ? "" : cmndMT.TenFileHeThong));
                                    item.AnhCMND_MSBase64 = "data:image/jpeg;base64," + cmClass.ConvertFileToBase64(Path.Combine(pathToSave, cmndMS == null ? "" : cmndMS.TenFileHeThong).ToString());
                                    item.AnhChanDungBase64 = "data:image/jpeg;base64," + cmClass.ConvertFileToBase64(Path.Combine(pathToSave, anhCD == null ? "" : anhCD.TenFileHeThong).ToString());
                                }
                                base.Message = ""; base.Status = 1;
                                base.Data = item;
                                break;
                            }
                        }
                    }

                    if (!checkResult)
                    {
                        base.Message = "Không có Dữ liệu trùng khớp"; base.Status = 0;
                    }
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
        //[CustomAuthAttribute(ChucNangEnum.BaoCao, AccessLevel.Read)]
        [Route("ThongKe_KhachVaoRa")]
        public IActionResult ThongKe_KhachVaoRa([FromQuery] BasePagingParamsFilter p)
        {
            try
            {
                return CreateActionResult("Thống kê khách vào ra", EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinVaoRaModel> Data;
                    var DonViSuDungID = Utils.ConvertToNullableInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, null);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    if (UserRole.CheckAdmin(NguoiDungID))
                        Data = _VaoRaBUS.ThongKe_KhachVaoRa(p, ref TotalRow, null);
                    else
                        Data = _VaoRaBUS.ThongKe_KhachVaoRa(p, ref TotalRow, DonViSuDungID ?? null);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count >= 0 ? 1 : 0;
                    base.TotalRow = TotalRow;
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
        [Route("GetByID")]
        public IActionResult GetByID(int ThongTinVaoRaID)
        {
            try
            {
                return CreateActionResult("Lấy chi tiết thông tin vào ra", EnumLogType.GetByName, () =>
                {
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                    var ttVao = _VaoRaBUS.Get_By_ThongTinVaoRaID(ThongTinVaoRaID, serverPath);
                    base.Data = ttVao;
                    base.TotalRow = TotalRow;
                    base.Status = 1;
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
        [Route("GetListPageBySearch")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Read)]
        public IActionResult GetListPageBySearch([FromQuery] BasePagingParams p, int? Type)
        {
            try
            {
                int TotalRow = 0;
                var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("NguoiDungID")).Value, 0);
                return CreateActionResult("Lấy danh sách phân trang", EnumLogType.GetByName, () =>
                {
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                    var ttVao = _VaoRaBUS.GetListPageBySearch(p, Type, ref TotalRow, DonViSuDungID, NguoiDungID, serverPath);
                    if (ttVao.Count < 1) { base.Message = "Không có Dữ liệu"; }
                    base.Status = 1;
                    base.Data = ttVao;
                    base.TotalRow = TotalRow;
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
        //[Route("TongHopTheoNgay")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Read)]
        public IActionResult TongHopTheoNgay()
        {
            try
            {
                int TotalRow = 0;
                var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);
                var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("NguoiDungID")).Value, 0);
                return CreateActionResult("Tổng hợp khách theo ngày của cơ quan " + CoQuanID, EnumLogType.GetByName, () =>
                {

                    var ttVao = _VaoRaBUS.ThongTinVaoRa_TongHopTheoNgay(DonViSuDungID, NguoiDungID);
                    if (ttVao.Tong < 1) { base.Message = "Không có Dữ liệu"; }
                    base.Data = ttVao;
                    base.TotalRow = TotalRow;
                    base.Status = 1;
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
        [Route("DanhSachDoiTuongGap")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Read)]
        public IActionResult DanhSachDoiTuongGap(int? DonViSuDungID)
        {
            try
            {

                return CreateActionResult("Danh sách đối tượng gặp của cơ quan " + DonViSuDungID, EnumLogType.GetByName, () =>
                {
                    var clsCommon = new Commons();
                    var Result = _VaoRaBUS.DanhSachDoiTuongGap(DonViSuDungID ?? 0);
                    if (Result == null || Result.ID == null || Result.ID < 1) { base.Message = "Không có Dữ liệu"; }
                    base.Data = Result;
                    base.Status = 1;
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

        #region truy vết y tế
        [HttpGet]
        [CustomAuthAttribute(ChucNangEnum.TruyVetYTe_ToanTinh, AccessLevel.Read)]
        [Route("TruyVetYTe_ToanTinh")]
        public IActionResult TruyVetYTe_ToanTinh([FromQuery] BasePagingParamsFilter p)
        {
            try
            {
                return CreateActionResult("Truy vết y tế trên toàn tỉnh", EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinVaoRaModel> Data;
                    var TinhID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("TinhID")).Value, 0);
                    p.TinhID = TinhID;
                    Data = _VaoRaBUS.TruyVetYTe_ToanTinh(p, ref TotalRow);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count >= 0 ? 1 : 0;
                    base.TotalRow = TotalRow;
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
        [CustomAuthAttribute(ChucNangEnum.TruyVetYTe_TrongDonVi, AccessLevel.Read)]
        [Route("TruyVetYTe_TrongDonVi")]
        public IActionResult TruyVetYTe_TrongDonVi([FromQuery] BasePagingParamsFilter p)
        {
            try
            {
                return CreateActionResult("Truy vết y tế trong đơn vị", EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<ThongTinVaoRaModel> Data;
                    var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);
                    p.DonViSuDungID = DonViSuDungID;
                    Data = _VaoRaBUS.TruyVetYTe_TrongDonVi(p, ref TotalRow);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = Data.Count >= 0 ? 1 : 0;
                    base.TotalRow = TotalRow;
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
        #endregion


        #region function
        /// <summary>
        /// dùng khi lấy ảnh từ đường dẫn ảnh của IP Camera
        /// trả về Image
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        //public System.Drawing.Image DownloadImageFromUrl(string imageUrl)
        //{
        //    System.Drawing.Image image = null;

        //    try
        //    {
        //        System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
        //        webRequest.AllowWriteStreamBuffering = true;
        //        webRequest.Timeout = 30000;

        //        System.Net.WebResponse webResponse = webRequest.GetResponse();

        //        System.IO.Stream stream = webResponse.GetResponseStream();

        //        //byte[] bytes;
        //        //using (var memoryStream = new MemoryStream())
        //        //{
        //        //    stream.CopyTo(memoryStream);
        //        //    bytes = memoryStream.ToArray();
        //        //}

        //        //string base64 = Convert.ToBase64String(bytes);

        //        image = System.Drawing.Image.FromStream(stream);

        //        webResponse.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //    return image;
        //}

        /// <summary>
        /// dùng khi lấy ảnh từ đường dẫn ảnh của IP Camera
        /// trả về Image
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public String ConvertImage_FromUrl_ToBase64(string imageUrl)
        {
            var Result = "";
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;
                webRequest.Credentials = new NetworkCredential("root", "root");

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                Result = Convert.ToBase64String(bytes);
                webResponse.Close();
            }
            catch (Exception ex)
            {
                return "";
                throw ex;
            }

            return Result;
        }



        public bool SaveBase64ToFile(FileModel file, string TenFileHeThong, int? DonViSuDungPhanMemID)
        {
            try
            {
                //var tenFile = "df.jpg";
                //tenFile = TenFileHeThong.Split('/').LastOrDefault() != null ? TenFileHeThong.Split('/').LastOrDefault() : "df.jpg";
                if (file.Base64.Length > 0)
                {
                    var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    var b64 = file.Base64;
                    b64 = b64.Split(',')[1];
                    byte[] bytes = Convert.FromBase64String(b64);
                    //var folderName = Path.Combine("Upload", "FileDinhKemDuyetKeKhai");
                    var folderName = Path.Combine(pathSaveFile);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    CheckAndCreateFolder(Path.Combine(pathToSave, DonViSuDungPhanMemID.Value.ToString()));
                    var fullPath = Path.Combine(pathToSave, TenFileHeThong);
                    System.IO.File.WriteAllBytes(fullPath, bytes);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        //public BaseResultModel InsertFileDinhKem(string SoCMND, int ThongTinVaoRaID, string Base64)
        //{
        //    try
        //    {
        //        var Result = new BaseResultModel();
        //        var FileType = Base64.Split(',')[0];
        //        var crObj = new FileDinhKemModel();
        //        crObj.TenFileHeThong = "AnhChanDung_" + SoCMND + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".jpg";
        //        crObj.LoaiFile = EnumLoaiFile.AnhChanDung.GetHashCode();
        //        crObj.ThongTinVaoRaID = ThongTinVaoRaID;
        //        crObj.Base64File = Base64;
        //        Result = _VaoRaBUS.InsertFilDinhKem(crObj);
        //        if (Result.Status < 1)
        //        {
        //            Result.Status = 0;
        //            Result.Message = "Không thể đính kèm file";
        //        }
        //        var crFile = new FileModel();
        //        crFile.TenFile = crObj.TenFileGoc;
        //        crFile.Base64 = Base64;
        //        if (SaveBase64ToFile(crFile, crObj.TenFileHeThong))
        //        {
        //            Result.Status = 1;
        //            Result.Message = "Thêm mới thành công";
        //        }
        //        else
        //        {
        //            Result.Status = 0;
        //            Result.Message = ConstantLogMessage.API_Error_System;
        //        }
        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        base.Status = -1;
        //        base.Message = ConstantLogMessage.API_Error_System;
        //        throw ex;
        //    }
        //}

        public BaseResultModel InsertAnh_CheckIn(string TenFile, int ThongTinVaoRaID, int LoaiAnh, string Base64, int? DonViSuDungPhanMemID)
        {
            try
            {
                var Result = new BaseResultModel();
                var FileType = Base64.Split(',')[0];
                var crObj = new FileDinhKemModel();

                crObj.TenFileHeThong = DonViSuDungPhanMemID.ToString() + "/" + (LoaiAnh == 1 ? "AnhCMND_MT_" : LoaiAnh == 2 ? "AnhCMND_MS_" : "AnhChanDung_") + TenFile + ".jpg";
                crObj.LoaiFile = LoaiAnh;
                //EnumLoaiFile.AnhChanDung.GetHashCode();
                crObj.ThongTinVaoRaID = ThongTinVaoRaID;
                crObj.Base64File = Base64;
                Result = _VaoRaBUS.InsertFilDinhKem(crObj);
                if (Result.Status < 1)
                {
                    Result.Status = 0;
                    Result.Message = "Không thể đính kèm file";
                }
                var crFile = new FileModel();
                crFile.TenFile = crObj.TenFileGoc;
                crFile.Base64 = Base64;
                if (SaveBase64ToFile(crFile, crObj.TenFileHeThong, DonViSuDungPhanMemID))
                {
                    Result.Status = 1;
                    Result.Message = "Thêm mới thành công";
                }
                else
                {
                    Result.Status = 0;
                    Result.Message = ConstantLogMessage.API_Error_System;
                }
                return Result;
            }
            catch (Exception ex)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                throw ex;
            }
        }

        public async Task<bool> checkAnhDaiDienAsync(string AnhCheckOut, string AnhCheckIn)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string parameter = "Z9Gsi9B0sOK1QVZLayoAMZ2wJPi89Es9";
                    client.DefaultRequestHeaders.Add("key", parameter);
                    //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    var ct = new AttachmentType();
                    ct.image_cmt = AnhCheckOut;
                    ct.image_live = AnhCheckIn;
                    StringContent content = new StringContent(JsonConvert.SerializeObject(ct), Encoding.UTF8, "application/json");

                    HttpResponseMessage result = await client.PostAsync("http://api.cloudekyc.com/v3.1/face/verification", content);



                    if (!result.IsSuccessStatusCode)
                    {
                        throw new Exception(result.RequestMessage.Content.ToString());
                    }
                    Task<string> task = result.Content.ReadAsStringAsync();

                    JObject obj2 = JObject.Parse(JsonConvert.DeserializeObject(task.Result.ToString()).ToString());

                    if (obj2.Count > 0)
                    {
                        var Result = Utils.ConvertToInt32(obj2["verify_result"].ToString(), 0);
                        if (Result == 2)
                            return true;
                        else return false;
                    }
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// kiểm tra folder có tồn tại ko, nếu ko thì tạo
        /// </summary>
        /// <param name="_host"></param>
        /// <param name="folderPath"></param>
        public void CheckAndCreateFolder(string folderPath)
        {
            bool isFolder = Directory.Exists(folderPath);
            if (!isFolder)
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        #endregion
    }


    public class AttachmentType
    {
        public string image_cmt { get; set; } // ảnh mặt trước CMND
        public string image_cmt2 { get; set; } // ảnh mặt sau CMND
        public string image_live { get; set; } // ảnh chân dung
    }

}