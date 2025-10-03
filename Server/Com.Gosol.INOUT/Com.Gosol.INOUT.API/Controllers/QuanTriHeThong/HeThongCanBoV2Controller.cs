using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.QuanTriHeThong;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Diagnostics;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.API.Authorization;
using Microsoft.Extensions.Logging;
using Com.Gosol.INOUT.Models.InOut;
using Com.Gosol.INOUT.BUS.VaoRa;
using Microsoft.Extensions.Options;
using Com.Gosol.INOUT.API.Config;

namespace Com.Gosol.INOUT.API.Controllers.QuanTriHeThong
{
    [Route("api/v2/HeThongCanBo")]
    [ApiController]
    public class HeThongCanBoV2Controller : BaseApiController
    {
        private IHeThongCanBoV2BUS _HeThongCanBoBUS;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private IHostingEnvironment _host;
        private IOptions<AppSettings> _AppSettings;

        public HeThongCanBoV2Controller(IHeThongCanBoV2BUS HeThongCanBoBUS, IFileDinhKemBUS FileDinhKemBUS, IHostingEnvironment HostingEnvironment, IOptions<AppSettings> Settings, ILogHelper _LogHelper, ILogger<HeThongCanBoController> logger) : base(_LogHelper, logger)
        {
            this._HeThongCanBoBUS = HeThongCanBoBUS;
            this._FileDinhKemBUS = FileDinhKemBUS;
            this._host = HostingEnvironment;
            this._AppSettings = Settings;
        }

        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.HT_NhatKyHeThong, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(HeThongCanBoModel HeThongCanBoModel)
        {
            try
            {

                return CreateActionResult(ConstantLogMessage.HT_CanBo_ThemCanBo, EnumLogType.Insert, () =>
                {
                    string Message = null;
                    int val = 0;
                    int CanBoID = 0;
                    val = _HeThongCanBoBUS.Insert(HeThongCanBoModel, ref CanBoID, ref Message);
                    if (val > 0)
                    {
                        base.Status = 1;
                    }
                    base.Message = Message;
                    base.Data = val;
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
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(HeThongCanBoModel HeThongCanBoModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_SuaCanBo, EnumLogType.Update, () =>
                {
                    string Message = null;
                    int val = 0;
                    val = _HeThongCanBoBUS.Update(HeThongCanBoModel, ref Message);
                    base.Message = Message;
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
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_XoaCanBo, EnumLogType.Delete, () =>
                 {

                     var Result = _HeThongCanBoBUS.Delete(p.ListID);
                     if (Result.Count > 0)
                     {
                         base.Message = "Lỗi!";
                         base.Data = Result;
                         base.Status = 0;
                         return base.GetActionResult();
                     }
                     else
                     {
                         base.Message = "Xóa thành công!";
                         base.Data = Result;
                         base.Status = 1;
                         return base.GetActionResult();
                     }
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
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("GetByID")]
        public IActionResult GetCanBoByID([FromQuery] int CanBoID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetByID, EnumLogType.GetByID, () =>
                 {
                     HeThongCanBoModel Data;
                     Data = _HeThongCanBoBUS.GetCanBoByID(CanBoID);
                     base.Status = Data.CanBoID > 0 ? 1 : 0;
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
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetPagingBySearch([FromQuery] BasePagingParams p, int? CoQuanID, int? TrangThaiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetListPaging, EnumLogType.Insert, () =>
                 {
                     IList<HeThongCanBoModel> Data;
                     int TotalRow = 0;
                     Data = _HeThongCanBoBUS.GetPagingBySearch(p, ref TotalRow, CoQuanID, TrangThaiID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CoQuanID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0));
                     if (Data.Count == 0)
                     {
                         base.Message = ConstantLogMessage.API_NoData;
                         base.Status = 1;
                         base.TotalRow = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     if (Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "NguoiDungID").Value, 0) == 1)
                     {
                         TotalRow = TotalRow - 1;
                     }
                     base.Status = TotalRow > 0 ? 1 : 0;
                     base.Data = Data;
                     base.TotalRow = TotalRow;
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
        //[HttpGet]
        //[Route("FilterByName")]
        //public IActionResult FilterByName(string TenCanBo, int IsStatus, int CoQuanID)
        //{
        //    return CreateActionResult(ConstantLogMessage.HT_CanBo_FilterByName, () =>
        //    {
        //        IList<HeThongCanBoModel> Data;
        //        Data = _HeThongCanBoBUS.FilterByName(TenCanBo, IsStatus, CoQuanID);
        //        int totalRow = Data.Count();
        //        base.Status = totalRow > 0 ? 1 : 0;
        //        base.Data = Data;
        //        base.TotalRow = totalRow;
        //        return base.GetActionResult();
        //    });
        //}
        [HttpPost]
        // [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("ReadExcelFile")]
        public async Task<IActionResult> ReadExcelFile([FromBody] Files file)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_ImportFile, EnumLogType.Other, () =>
                {
                    string SavePath = _host.ContentRootPath + "\\Upload\\" + "CanBo.xlsx";
                    if (System.IO.File.Exists(SavePath))
                    {
                        System.IO.File.Delete(SavePath);
                    }
                    using (FileStream stream = System.IO.File.Create(SavePath))
                    {
                        byte[] byteArray = Convert.FromBase64String(file.files);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }

                    var Result = _HeThongCanBoBUS.ReadExcelFile(SavePath, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));
                    //var ResultNew = Result.Where(x => x.NguyenNhan.Where(y=>y.Contains("Files không có dữ liệu!")).Select(x=>x)).ToList();
                    //if (ResultNew.Count > 0)
                    //{
                    //    base.Message = "Files không có dữ liệu!";
                    //    base.Status = 0;
                    //    return base.GetActionResult();
                    //}
                    if (Result.Where(x => x.NguyenNhan.Count > 0).ToList().Count <= 0)
                    {
                        base.Message = "Import thành công!";
                        base.Status = 1;
                        return base.GetActionResult();
                    }

                    else
                    {
                        foreach (var item in Result)
                        {
                            var FileNo = item.NguyenNhan.Where(x => x.ToString().Trim() == "Files không có dữ liệu!").ToList();
                            if (FileNo.Count > 0)
                            {
                                base.Message = "Files không có dữ liệu!";
                                base.Status = 0;
                                return base.GetActionResult();
                            }
                        }
                        base.Message = "Import không thành công!" + "<br>" + " Danh sách lỗi :" + "<br>";
                        base.Status = 0;
                        base.Data = Result;
                        return base.GetActionResult();
                    }
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("DowloadExel")]
        public async Task<IActionResult> DowloadExel()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_ExportFile, EnumLogType.Other, () =>
                 {

                     var host = _host.ContentRootPath;
                     var expath = host + "\\Upload\\CanBo_Template.xlsm";
                     _HeThongCanBoBUS.ImportToExel(expath, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));
                     var memory = new MemoryStream();
                     Byte[] bytes = System.IO.File.ReadAllBytes(expath);
                     String file = Convert.ToBase64String(bytes);
                     file = string.Concat("data:application/vnd.ms-excel;base64,", file);
                     //httpResponseMessage.StatusCode = HttpStatusCode.OK;
                     base.Data = file;
                     base.Status = 1;
                     return base.GetActionResult();

                 });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("GetThanNhanByCanBoID")]
        public IActionResult GetThanNhanByCanBoID() // lấy thân nhân của cán bộ đang đăng nhập
        {
            try
            {
                return CreateActionResult("Lấy thân nhân theo cán bộ", EnumLogType.Insert, () =>
                {
                    List<HeThongCanBoShortModel> list = new List<HeThongCanBoShortModel>();
                    list = _HeThongCanBoBUS.GetThanNhanByCanBoID(Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0));
                    base.Status = 1;
                    Data = list;
                    base.Message = "Danh sách thân nhân";
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
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("GetAllCanBoByCoQuanID")]
        public IActionResult GetAllCanBoByCoQuanID(int CoQuanID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_GetListPaging, EnumLogType.Other, () =>
                {
                    var Result = _HeThongCanBoBUS.GetAllCanBoByCoQuanID(CoQuanID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0));
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("GetAllCanBoTrongCoQuan")]
        public IActionResult GetAllCanBoTrongCoQuan()
        {
            try
            {
                return CreateActionResult("Lấy tất cả cán bộ trong cơ quan", EnumLogType.Other, () =>
                {
                    var Result = _HeThongCanBoBUS.GetAllByCoQuanID(Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0));
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Read)]
        [Route("GenerationMaCanBo")]
        public IActionResult GenerationMaCanBo([FromQuery] int CoQuanID)
        {
            try
            {
                return CreateActionResult("Tạo mã bởi cơ quan", EnumLogType.GetByID, () =>
                {
                    var Data = _HeThongCanBoBUS.GenerationMaCanBo(CoQuanID);
                    if (string.IsNullOrEmpty(Data))
                    {
                        base.Status = 0;
                        base.Data = Data;
                        return base.GetActionResult();

                    }
                    base.Status = 1;
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
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("GetAllInCoQuanCha")]
        public IActionResult GetAllInCoQuanCha([FromQuery] int? CoQuanID)
        {
            try
            {
                return CreateActionResult("Lấy tất cả cán bộ trong cơ quan", EnumLogType.Other, () =>
                {
                    var Result = _HeThongCanBoBUS.GetAllInCoQuanCha(CoQuanID.Value);
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("HeThongCanBo_GetThongTinCoQuan")]
        public IActionResult HeThongCanBo_GetThongTinCoQuan()
        {
            try
            {
                return CreateActionResult("Lấy thông tin cơ quan của cán bộ", EnumLogType.Other, () =>
                {
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0);
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("NguoiDungID")).Value, 0);
                    var Result = _HeThongCanBoBUS.HeThongCanBo_GetThongTinCoQuan(CanBoID, NguoiDungID);
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("HeThongCanBo_GetAllCanBo")]
        public IActionResult HeThongCanBo_GetAllCanBo()
        {
            try
            {
                return CreateActionResult("Lấy danh sách cán bộ", EnumLogType.Other, () =>
                {
                    var Result = _HeThongCanBoBUS.HeThongCanBo_GetAllCanBo();
                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("GetDanhSachLeTan")]
        public IActionResult GetDanhSachLeTan()
        {
            try
            {
                return CreateActionResult("Lấy danh sách lễ tân", EnumLogType.GetList, () =>
                {
                    var Result = new List<HeThongCanBoModel>();
                    if (Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0) == 1)
                    {
                        Result = _HeThongCanBoBUS.GetDanhSachLeTan();
                    }
                    else
                    {
                        var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);
                        Result = _HeThongCanBoBUS.GetDanhSachLeTan_TrongCoQuanSuDungPhanMem(DonViSuDungID);
                    }

                    base.Data = Result;
                    base.Status = 1;
                    return base.GetActionResult();

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// lấy toàn bộ danh sách cán bộ thuộc hệ thống của cơ quan sử dụng phần mềm theo cơ quan đăng nhập
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.FullAccess)]
        [Route("DanhSachCanBo_TrongCoQuanSuDungPhanMem")]
        public IActionResult DanhSachCanBo_TrongCoQuanSuDungPhanMem()
        {
            try
            {
                return CreateActionResult("Lấy tất cả cán bộ trong cơ quan", EnumLogType.Other, () =>
                {
                    var Result = new List<HeThongCanBoModel>();
                    if (Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CanBoID")).Value, 0) == 1)
                    {
                        Result = _HeThongCanBoBUS.HeThongCanBo_GetAllCanBo();
                    }
                    else
                        Result = _HeThongCanBoBUS.DanhSachCanBo_TrongCoQuanSuDungPhanMem(Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0));
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


        #region v2
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Edit)]
        [Route("CapNhatThongTinCanBo")]
        public IActionResult CapNhatThongTinCanBo(HeThongCanBoV2Model rq)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_CanBo_SuaCanBo, EnumLogType.Update, () =>
                {
                    var val = _HeThongCanBoBUS.CapNhatThongTinCanBo(rq);
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

        #endregion

        #region v4
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_CanBo, AccessLevel.Edit)]
        [Route("CapNhatThongTinCanBo_v4")]
        public async Task<IActionResult> CapNhatThongTinCanBoAsync(IList<IFormFile> files, [FromForm] string base64, [FromForm] string HeThongCanBoV2ModelStr)
        {
            try
            {
                //return CreateActionResult(ConstantLogMessage.HT_CanBo_SuaCanBo, EnumLogType.Update, () =>
                //{
                HeThongCanBoV2Model rq = JsonConvert.DeserializeObject<HeThongCanBoV2Model>(HeThongCanBoV2ModelStr);
                var val = _HeThongCanBoBUS.CapNhatThongTinCanBo(rq);
                Message = val.Message;
                base.Status = val.Status;
                //base.Data = Data;
                if(rq.CanBoID > 0 && ((files != null && files.Count > 0) || (base64 != null && base64.Length > 0)))
                {
                    FileDinhKemModel FileDinhKemModel = new FileDinhKemModel();
                    FileDinhKemModel.LoaiFile = EnumLoaiFile.AnhChanDung.GetHashCode();
                    FileDinhKemModel.Base64File = base64;
                    FileDinhKemModel.ThongTinVaoRaID = rq.CanBoID;
                    FileDinhKemModel.LaCanBo = true;
                    FileDinhKemModel.TenFileGoc = rq.CanBoID.ToString();
                    Commons cm = new Commons();
                    var file = await cm.InsertFileAnhAsync(files[0], FileDinhKemModel, _host, _FileDinhKemBUS);
                    if(file.FileDinhKemID > 0)
                    {
                        var host = _host.ContentRootPath;
                        var AIFaceAPI = new AIFaceAPI(_AppSettings);

                        if (_AppSettings.Value.Version == "1")
                        {
                            var user = AIFaceAPI.Login();
                            //ktra can bo 
                            HeThongCanBoV2Model cb = _HeThongCanBoBUS.GetCanBoByID_v4(rq.CanBoID);
                            if (cb.FaceID == 0)
                            {
                                faceinfo info = new faceinfo();
                                info.full_name = "NV." + rq.CanBoID.ToString();//do nhận diện trả về full_name
                                info.status = "Active";
                                info.organization_id = Utils.ConvertToInt32(_AppSettings.Value.OfficeID, 0);
                                info.office_id = Utils.ConvertToInt32(_AppSettings.Value.GroupID, 0);
                                var face = AIFaceAPI.Add_FaceInfo(info);
                                if (face.status != "200") Message += "Add_FaceInfo - " + face.message + " ";
                                if (face.user != null && face.user.id > 0)
                                {
                                    //update FaceID
                                    cb.FaceID = face.user.id ?? 0;
                                    _HeThongCanBoBUS.CapNhatFaceID(cb);

                                    byte[] imageArray = System.IO.File.ReadAllBytes(host + "\\" + file.FileUrl);
                                    string base64image = Convert.ToBase64String(imageArray);
                                    var feature = AIFaceAPI.Add_Feature(cb.FaceID, base64image, user.session_key);
                                    if (feature.status == "200")
                                    {
                                        //update feature
                                        FileDinhKemModel.FileDinhKemID = file.FileDinhKemID;
                                        FileDinhKemModel.FeatureID = feature.id;
                                        _FileDinhKemBUS.UpdateFeatureID(FileDinhKemModel);
                                    }
                                    else
                                    {
                                        Message += "Add_Feature - " + feature.message + " ";
                                    }
                                }
                            }
                            else
                            {
                                byte[] imageArray = System.IO.File.ReadAllBytes(host + "\\" + file.FileUrl);
                                string base64image = Convert.ToBase64String(imageArray);
                                var feature = AIFaceAPI.Add_Feature(cb.FaceID, base64image, user.session_key);
                                if (feature.status == "200")
                                {
                                    //update feature
                                    FileDinhKemModel.FileDinhKemID = file.FileDinhKemID;
                                    FileDinhKemModel.FeatureID = feature.id;
                                    _FileDinhKemBUS.UpdateFeatureID(FileDinhKemModel);
                                }
                                else
                                {
                                    Message += "Add_Feature - " + feature.message + " ";
                                }
                            }
                            //api update index org and token
                            AIFaceAPI.Update_Index(0, user.session_key);
                        }
                        else
                        {
                            var user = AIFaceAPI.GetToken();
                            //ktra can bo 
                            HeThongCanBoV2Model cb = _HeThongCanBoBUS.GetCanBoByID_v4(rq.CanBoID);
                            if (cb.FaceID == 0)
                            {
                                faceinfo info = new faceinfo();
                                info.full_name = "NV." + rq.CanBoID.ToString();//do nhận diện trả về full_name

                                byte[] imageArray = System.IO.File.ReadAllBytes(host + "\\" + file.FileUrl);
                                string base64image = Convert.ToBase64String(imageArray);
                                info.avatar = base64image;
                                var face = AIFaceAPI.AddFace_v2(info, user.token);
                                if (face.status != "200") Message += "Add_FaceInfo - " + face.message + " ";
                                if (face.user != null && face.user.id > 0)
                                {
                                    //update FaceID
                                    cb.FaceID = face.user.id ?? 0;
                                    _HeThongCanBoBUS.CapNhatFaceID(cb);
                                    var feature = AIFaceAPI.AddFeature_v2(cb.FaceID, base64image, user.token);
                                    if (feature.status == "200")
                                    {
                                        //update feature
                                        FileDinhKemModel.FileDinhKemID = file.FileDinhKemID;
                                        FileDinhKemModel.FeatureID = feature.id;
                                        _FileDinhKemBUS.UpdateFeatureID(FileDinhKemModel);
                                    }
                                    else
                                    {
                                        Message += "Add_Feature - " + feature.message + " ";
                                    }
                                }
                            }
                            else
                            {
                                byte[] imageArray = System.IO.File.ReadAllBytes(host + "\\" + file.FileUrl);
                                string base64image = Convert.ToBase64String(imageArray);
                                var feature = AIFaceAPI.AddFeature_v2(cb.FaceID, base64image, user.token);
                                if (feature.status == "200")
                                {
                                    //update feature
                                    FileDinhKemModel.FileDinhKemID = file.FileDinhKemID;
                                    FileDinhKemModel.FeatureID = feature.id;
                                    _FileDinhKemBUS.UpdateFeatureID(FileDinhKemModel);
                                }
                                else
                                {
                                    Message += "Add_Feature - " + feature.message + " ";
                                }
                            }
                            //api update index org and token
                            //AIFaceAPI.Update_Index(0, user.session_key);
                        }
                        
                    }
                }
                base.Message = Message;
                return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.GetActionResult();
                throw;
            }

        }

        #endregion
    }
}
