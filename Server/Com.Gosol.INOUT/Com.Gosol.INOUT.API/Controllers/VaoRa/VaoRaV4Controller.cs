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
using Com.Gosol.INOUT.BUS.VaoRa;

namespace Com.Gosol.INOUT.API.Controllers.VaoRa
{
    [Route("api/v4/VaoRa")]
    [ApiController]
    public class VaoRaV4Controller : BaseApiController
    {
        private IVaoRaV4BUS _VaoRaBUS;
        private ISystemConfigBUS _SystemConfigBUS;
        private IDanhMucCoQuanDonViBUS _DanhMucCoQuanDonViBUS;
        private IFileDinhKemBUS _FileDinhKemBUS;
        private IHeThongCanBoV2BUS _HeThongCanBoBUS;
        public IOptions<AppSettings> settings;
        private IHostingEnvironment _host;
        public VaoRaV4Controller(IOptions<AppSettings> Settings, IHeThongCanBoV2BUS HeThongCanBoBUS, IFileDinhKemBUS FileDinhKemBUS, IHostingEnvironment HostingEnvironment, IDanhMucCoQuanDonViBUS danhMucCoQuanDonViBUS, IVaoRaV4BUS VaoRaBUS, ISystemConfigBUS SystemConfigBUS, ILogHelper _LogHelper, ILogger<VaoRaV4Controller> logger) : base(_LogHelper, logger)
        {
            this._VaoRaBUS = VaoRaBUS;
            this._SystemConfigBUS = SystemConfigBUS;
            this._DanhMucCoQuanDonViBUS = danhMucCoQuanDonViBUS;
            this._HeThongCanBoBUS = HeThongCanBoBUS;
            this._FileDinhKemBUS = FileDinhKemBUS;
            this.settings = Settings;
            this._host = HostingEnvironment;
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
        [Route("NhanDienKhuonMat")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Create)]
        public IActionResult NhanDienKhuonMat(ThongTinVaoRaModel VaoRaModel)
        {
            try
            {
                return CreateActionResult("CheckIn_V4", EnumLogType.Insert, () =>
                {
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                    var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);

                    var GioCheckInGanNhat = DateTime.MinValue;
                    var KhoangThoiGian = Utils.ConvertToInt32(_SystemConfigBUS.GetByKey("KhoangThoiGianChekIn").ConfigValue, 0);

                    //nhận diện khuôn mặt v2
                    string base64image = "";
                    if (VaoRaModel.AnhChanDungBase64 != null && VaoRaModel.AnhChanDungBase64.Length > 0)
                    {
                        var arr = VaoRaModel.AnhChanDungBase64.Split(',');
                        if (arr != null && arr.Length > 1)
                        {
                            base64image = arr[1];
                        }
                        else base64image = VaoRaModel.AnhChanDungBase64;
                    } 
                    var AIFaceAPI = new AIFaceAPI(settings);

                    if (settings.Value.Version == "1")
                    {
                        var PredictFace = AIFaceAPI.Annotation_Embd(0, base64image);
                        Message = PredictFace.message;
                        if (PredictFace.result != null && PredictFace.result.Count > 0)
                        {
                            foreach (var item in PredictFace.result)
                            {
                                var listArr = item.annotation.Split('.');
                                if (listArr.Length > 1)
                                {
                                    var CanBoID = Utils.ConvertToInt32(listArr[1], 0);
                                    if (CanBoID > 0)
                                    {
                                        if (listArr[0] == "NV")
                                        {
                                            var cb = _HeThongCanBoBUS.GetCanBoByID(CanBoID);
                                            VaoRaModel.LaCanBo = true;
                                            VaoRaModel.HoVaTen = cb.TenCanBo;
                                            VaoRaModel.NgaySinh = cb.NgaySinh;
                                            VaoRaModel.CanBoID = cb.CanBoID;
                                            if (cb.GioiTinh == 0)
                                            {
                                                VaoRaModel.GioiTinh = "Nữ";
                                            }
                                            else VaoRaModel.GioiTinh = "Nam";
                                            VaoRaModel.HoKhau = cb.DiaChi;
                                            VaoRaModel.DienThoai = cb.DienThoai;
                                            VaoRaModel.TenCoQuan = cb.TenCoQuan;
                                            VaoRaModel.SoCMND = cb.CMND;
                                            VaoRaModel.NgayCapCMND = cb.NgayCap;
                                            VaoRaModel.NoiCapCMND = cb.NoiCap;

                                            var cr = _VaoRaBUS.Get_By_ThongTinKhachID(VaoRaModel.CanBoID, VaoRaModel.LaCanBo, EnumLoaiCheckOut.CMND.GetHashCode(), DonViSuDungID, serverPath);
                                            if (cr != null && cr.Count > 0) GioCheckInGanNhat = cr[0].GioVao;
                                            if (cr != null && cr.Count > 0 && (cr[0].GioRa == null || cr[0].GioRa == DateTime.MinValue))
                                            {
                                                VaoRaModel = cr[0];
                                                VaoRaModel.LaCanBo = true;
                                                VaoRaModel.CanBoID = cb.CanBoID;

                                            }

                                            //base.Data = VaoRaModel;
                                        }
                                        else if (listArr[0] == "KH")
                                        {
                                            //Get thong tin khach
                                            VaoRaModel.LaCanBo = false;
                                            VaoRaModel.CanBoID = CanBoID;
                                            var khach = _VaoRaBUS.GetByID(CanBoID);
                                            VaoRaModel.HoVaTen = khach.HoVaTen;
                                            VaoRaModel.SoCMND = khach.SoCMND;
                                            VaoRaModel.GioiTinh = khach.GioiTinh;
                                            VaoRaModel.NgaySinh = khach.NgaySinh;
                                            VaoRaModel.NoiCapCMND = khach.NoiCapCMND;
                                            VaoRaModel.NgayCapCMND = khach.NgayCapCMND;
                                            VaoRaModel.DienThoai = khach.DienThoai;
                                            VaoRaModel.HoKhau = khach.HoKhau;

                                            var cr = _VaoRaBUS.Get_By_ThongTinKhachID(VaoRaModel.CanBoID, VaoRaModel.LaCanBo, EnumLoaiCheckOut.CMND.GetHashCode(), DonViSuDungID, serverPath);
                                            if (cr != null && cr.Count > 0) GioCheckInGanNhat = cr[0].GioVao;
                                            if (cr != null && cr.Count > 0 && (cr[0].GioRa == null || cr[0].GioRa == DateTime.MinValue))
                                            {
                                                VaoRaModel = cr[0];
                                                VaoRaModel.LaCanBo = false;
                                                VaoRaModel.CanBoID = CanBoID;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        var user = AIFaceAPI.GetToken();
                        var PredictFace = AIFaceAPI.CheckFaceImage(user.token, base64image);
                        Message = PredictFace.message;
                        if(PredictFace.full_name != null && PredictFace.full_name.Length > 0)
                        {
                            var listArr = PredictFace.full_name.Split('.');
                            if (listArr.Length > 1)
                            {
                                var CanBoID = Utils.ConvertToInt32(listArr[1], 0);
                                if (CanBoID > 0)
                                {
                                    if (listArr[0] == "NV")
                                    {
                                        var cb = _HeThongCanBoBUS.GetCanBoByID(CanBoID);
                                        VaoRaModel.LaCanBo = true;
                                        VaoRaModel.HoVaTen = cb.TenCanBo;
                                        VaoRaModel.NgaySinh = cb.NgaySinh;
                                        VaoRaModel.CanBoID = cb.CanBoID;
                                        if (cb.GioiTinh == 0)
                                        {
                                            VaoRaModel.GioiTinh = "Nữ";
                                        }
                                        else VaoRaModel.GioiTinh = "Nam";
                                        VaoRaModel.HoKhau = cb.DiaChi;
                                        VaoRaModel.DienThoai = cb.DienThoai;
                                        VaoRaModel.TenCoQuan = cb.TenCoQuan;
                                        VaoRaModel.SoCMND = cb.CMND;
                                        VaoRaModel.NgayCapCMND = cb.NgayCap;
                                        VaoRaModel.NoiCapCMND = cb.NoiCap;

                                        var cr = _VaoRaBUS.Get_By_ThongTinKhachID(VaoRaModel.CanBoID, VaoRaModel.LaCanBo, EnumLoaiCheckOut.CMND.GetHashCode(), DonViSuDungID, serverPath);
                                        if (cr != null && cr.Count > 0) GioCheckInGanNhat = cr[0].GioVao;
                                        if (cr != null && cr.Count > 0 && (cr[0].GioRa == null || cr[0].GioRa == DateTime.MinValue))
                                        {
                                            VaoRaModel = cr[0];
                                            VaoRaModel.LaCanBo = true;
                                            VaoRaModel.CanBoID = cb.CanBoID;

                                        }

                                        //base.Data = VaoRaModel;
                                    }
                                    else if (listArr[0] == "KH")
                                    {
                                        //Get thong tin khach
                                        VaoRaModel.LaCanBo = false;
                                        VaoRaModel.CanBoID = CanBoID;
                                        var khach = _VaoRaBUS.GetByID(CanBoID);
                                        VaoRaModel.HoVaTen = khach.HoVaTen;
                                        VaoRaModel.SoCMND = khach.SoCMND;
                                        VaoRaModel.GioiTinh = khach.GioiTinh;
                                        VaoRaModel.NgaySinh = khach.NgaySinh;
                                        VaoRaModel.NoiCapCMND = khach.NoiCapCMND;
                                        VaoRaModel.NgayCapCMND = khach.NgayCapCMND;
                                        VaoRaModel.DienThoai = khach.DienThoai;
                                        VaoRaModel.HoKhau = khach.HoKhau;

                                        var cr = _VaoRaBUS.Get_By_ThongTinKhachID(VaoRaModel.CanBoID, VaoRaModel.LaCanBo, EnumLoaiCheckOut.CMND.GetHashCode(), DonViSuDungID, serverPath);
                                        if (cr != null && cr.Count > 0) GioCheckInGanNhat = cr[0].GioVao;
                                        if (cr != null && cr.Count > 0 && (cr[0].GioRa == null || cr[0].GioRa == DateTime.MinValue))
                                        {
                                            VaoRaModel = cr[0];
                                            VaoRaModel.LaCanBo = false;
                                            VaoRaModel.CanBoID = CanBoID;
                                        }
                                    }
                                }
                            }

                        }
                    }
                        
                    TimeSpan Time = DateTime.Now - GioCheckInGanNhat;
                    if (Time.TotalSeconds < KhoangThoiGian && KhoangThoiGian > 0)
                    {
                        base.Data = null;
                    }
                    else base.Data = VaoRaModel;
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
        [Route("Vao")]
        //[CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Create)]
        public IActionResult Vao(ThongTinVaoRaModel VaoRaModel)
        {
            try
            {
                var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanID")).Value, 0);
                var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);

                return CreateActionResult("CheckIn", EnumLogType.Insert, () =>
                {
                    var clsCommon = new Commons();
                    string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                    //if (VaoRaModel.CanBoID > 0)
                    //{
                    //    var cr = _VaoRaBUS.Get_By_ThongTinKhachID(VaoRaModel.CanBoID, VaoRaModel.LaCanBo, EnumLoaiCheckOut.CMND.GetHashCode(), DonViSuDungID, serverPath);
                    //    if(cr.Count > 0)
                    //    {
                    //        var GioCheckInGanNhat = cr[0].GioVao;
                    //        TimeSpan Time = DateTime.Now - GioCheckInGanNhat;
                    //        var KhoangThoiGian = Utils.ConvertToInt32(_SystemConfigBUS.GetByKey("KhoangThoiGianChekIn").ConfigValue, 0);
                    //        if (Time.TotalSeconds < KhoangThoiGian && KhoangThoiGian > 0)
                    //        {
                    //            base.Status = 3;
                    //            if (VaoRaModel.LaCanBo)
                    //            {
                    //                base.Message = "Cán bộ đã checkin. Vui lòng kiểm tra lại.";
                    //            }
                    //            base.Message = "Khách đã checkin. Vui lòng kiểm tra lại.";
                    //            return base.GetActionResult();
                    //        }
                    //    }
                    //}
     
                    if (VaoRaModel.SoCMND != null && VaoRaModel.SoCMND.Length > 0)
                    {
                        var cr = _VaoRaBUS.Get_By_MaThe(VaoRaModel.SoCMND, EnumLoaiCheckOut.CMND.GetHashCode(), DonViSuDungID, serverPath);
                        if (cr != null && cr.Count > 0)
                        {
                            base.Status = 0;
                            base.Message = "Khách chưa checkout. Vui lòng kiểm tra lại.";
                            return base.GetActionResult();
                        }

                    }
                    if (VaoRaModel.LaCanBo == false && VaoRaModel.CanBoID == 0)
                    {
                        ThongTinKhachModel ThongTinKhachModel = new ThongTinKhachModel();
                        ThongTinKhachModel.HoVaTen = VaoRaModel.HoVaTen;
                        ThongTinKhachModel.NgaySinh = VaoRaModel.NgaySinh;
                        ThongTinKhachModel.GioiTinh = VaoRaModel.GioiTinh;
                        ThongTinKhachModel.HoKhau = VaoRaModel.HoKhau;
                        ThongTinKhachModel.DienThoai = VaoRaModel.DienThoai;
                        ThongTinKhachModel.SoCMND = VaoRaModel.SoCMND;
                        ThongTinKhachModel.NoiCapCMND = VaoRaModel.NoiCapCMND;
                        ThongTinKhachModel.NgayCapCMND = VaoRaModel.NgayCapCMND;

                        string ms = "";
                        var ThongTinKhachID = _VaoRaBUS.InsertThongTinKhach(ThongTinKhachModel, ref ms);
                        Message = ms;

                        if (ThongTinKhachID > 0 && VaoRaModel.AnhChanDungBase64 != null && VaoRaModel.AnhChanDungBase64.Length > 0)
                        {
                            VaoRaModel.ThongTinKhachID = ThongTinKhachID;

                            var host = _host.ContentRootPath;
                            // bỏ qua AI Face
                            var AIFaceAPI = new AIFaceAPI(settings);

                            if (settings.Value.Version == "1")
                            {
                                var user = AIFaceAPI.Login();

                                faceinfo info = new faceinfo();
                                info.full_name = "KH." + ThongTinKhachID;//do nhận diện trả về full_name
                                info.status = "Active";
                                info.organization_id = Utils.ConvertToInt32(settings.Value.OfficeID, 0);
                                info.office_id = Utils.ConvertToInt32(settings.Value.GroupID, 0);
                                var face = AIFaceAPI.Add_FaceInfo(info);
                                if (face.status != "200") Message += "Add_FaceInfo - " + face.message + " ";
                                if (face.user != null && face.user.id > 0)
                                {
                                    //update FaceID
                                    ThongTinKhachModel.ThongTinKhachID = ThongTinKhachID;
                                    ThongTinKhachModel.FaceID = face.user.id ?? 0;
                                    _VaoRaBUS.UpdateFaceID(ThongTinKhachModel);

                                    //byte[] imageArray = System.IO.File.ReadAllBytes(host + "\\" + file.FileUrl);
                                    //string base64image = Convert.ToBase64String(imageArray);
                                    var base64image = VaoRaModel.AnhChanDungBase64;
                                    //base64image = base64image.Split(',')[1];
                                    if (base64image.Contains(","))
                                    {
                                        base64image = base64image.Split(',')[1];
                                    }
                                    var feature = AIFaceAPI.Add_Feature(ThongTinKhachModel.FaceID, base64image, user.session_key);
                                    if (feature.status == "200")
                                    {
                                        try
                                        {
                                            FileDinhKemModel FileDinhKemModel = new FileDinhKemModel();
                                            FileDinhKemModel.LoaiFile = EnumLoaiFile.AnhChanDung.GetHashCode();
                                            FileDinhKemModel.Base64File = base64image;
                                            FileDinhKemModel.ThongTinVaoRaID = ThongTinKhachID;
                                            FileDinhKemModel.LaCanBo = false;
                                            FileDinhKemModel.TenFileGoc = ThongTinKhachID.ToString();
                                            Commons cm = new Commons();
                                            var file = cm.InsertFileAnhAsync(null, FileDinhKemModel, _host, _FileDinhKemBUS);
                                            //update feature
                                            FileDinhKemModel.FileDinhKemID = file.Result.FileDinhKemID;
                                            FileDinhKemModel.FeatureID = feature.id;
                                            _FileDinhKemBUS.UpdateFeatureID(FileDinhKemModel);
                                        }
                                        catch (Exception ex)
                                        {
                                            Message += ex.Message;
                                        }
                                    }
                                    else
                                    {
                                        Message += "Add_Feature - " + feature.message + " ";
                                    }
                                    //api update index org and token
                                    AIFaceAPI.Update_Index(0, user.session_key);
                                }
                            }
                            else
                            {
                                var user = AIFaceAPI.GetToken();

                                faceinfo info = new faceinfo();
                                info.full_name = "KH." + ThongTinKhachID;//do nhận diện trả về full_name
                                info.avatar = VaoRaModel.AnhChanDungBase64;
                                var face = AIFaceAPI.AddFace_v2(info, user.token);
                                if (face.status != "200") Message += "Add_FaceInfo - " + face.message + " ";
                                if (face.user != null && face.user.id > 0)
                                {
                                    //update FaceID
                                    ThongTinKhachModel.ThongTinKhachID = ThongTinKhachID;
                                    ThongTinKhachModel.FaceID = face.user.id ?? 0;
                                    _VaoRaBUS.UpdateFaceID(ThongTinKhachModel);

                                    //byte[] imageArray = System.IO.File.ReadAllBytes(host + "\\" + file.FileUrl);
                                    //string base64image = Convert.ToBase64String(imageArray);
                                    var base64image = VaoRaModel.AnhChanDungBase64;
                                    //base64image = base64image.Split(',')[1];
                                    if (base64image.Contains(","))
                                    {
                                        base64image = base64image.Split(',')[1];
                                    }
                                    var feature = AIFaceAPI.AddFeature_v2(ThongTinKhachModel.FaceID, base64image, user.token);
                                    if (feature.status == "200")
                                    {
                                        try
                                        {
                                            FileDinhKemModel FileDinhKemModel = new FileDinhKemModel();
                                            FileDinhKemModel.LoaiFile = EnumLoaiFile.AnhChanDung.GetHashCode();
                                            FileDinhKemModel.Base64File = base64image;
                                            FileDinhKemModel.ThongTinVaoRaID = ThongTinKhachID;
                                            FileDinhKemModel.LaCanBo = false;
                                            FileDinhKemModel.TenFileGoc = ThongTinKhachID.ToString();
                                            Commons cm = new Commons();
                                            var file = cm.InsertFileAnhAsync(null, FileDinhKemModel, _host, _FileDinhKemBUS);
                                            //update feature
                                            FileDinhKemModel.FileDinhKemID = file.Result.FileDinhKemID;
                                            FileDinhKemModel.FeatureID = feature.id;
                                            _FileDinhKemBUS.UpdateFeatureID(FileDinhKemModel);
                                        }
                                        catch (Exception ex)
                                        {
                                            Message += ex.Message;
                                        }
                                    }
                                    else
                                    {
                                        Message += "Add_Feature - " + feature.message + " ";
                                    }
                                    //api update index org and token
                                    //AIFaceAPI.Update_Index(0, user.session_key);
                                }
                            }
                        }
                    }
                    else
                    {
                        if(VaoRaModel.LaCanBo == false && VaoRaModel.CanBoID > 0)
                        {
                            //update thông tin khách
                            ThongTinKhachModel ThongTinKhachModel = new ThongTinKhachModel();
                            ThongTinKhachModel.ThongTinKhachID = VaoRaModel.CanBoID;
                            ThongTinKhachModel.HoVaTen = VaoRaModel.HoVaTen;
                            ThongTinKhachModel.NgaySinh = VaoRaModel.NgaySinh;
                            ThongTinKhachModel.GioiTinh = VaoRaModel.GioiTinh;
                            ThongTinKhachModel.HoKhau = VaoRaModel.HoKhau;
                            ThongTinKhachModel.DienThoai = VaoRaModel.DienThoai;
                            ThongTinKhachModel.SoCMND = VaoRaModel.SoCMND;
                            ThongTinKhachModel.NoiCapCMND = VaoRaModel.NoiCapCMND;
                            ThongTinKhachModel.NgayCapCMND = VaoRaModel.NgayCapCMND;
                            
                            _VaoRaBUS.UpdateThongTinKhach(ThongTinKhachModel);
                        }
                    }
                    
                    // thêm thông tin vào ra
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0);
                    VaoRaModel.LeTan_Vao = CanBoID;
                    VaoRaModel.DonViSuDungID = DonViSuDungID;
                    var MaThe = ""; var SoCMND = "";
                    var gioVao = DateTime.Now;
                    VaoRaModel.GioVao = gioVao;
                    if(VaoRaModel.CanBoID > 0) VaoRaModel.ThongTinKhachID = VaoRaModel.CanBoID;//nguoi check in la can bo hoặc khách đã có trong hệ thống
                    var Result = _VaoRaBUS.Vao(VaoRaModel, ref MaThe, ref SoCMND, DonViSuDungID);
                    base.Status = Result.Status;
                    Message = Result.Message;
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
                            if (InsertAnh_CheckIn(TenFile, Result.Status, EnumLoaiFile.MatTruocCMND.GetHashCode(), VaoRaModel.AnhCMND_MTBase64, DonViSuDungID).Status < 1)
                            {
                                base.Status = -1;
                                Message += ConstantLogMessage.API_Error_System;
                            }
                        }

                        if (VaoRaModel.AnhCMND_MSBase64 != null && VaoRaModel.AnhCMND_MSBase64.Length > 0)
                        {
                            if (InsertAnh_CheckIn(TenFile, Result.Status, EnumLoaiFile.MatSauCMND.GetHashCode(), VaoRaModel.AnhCMND_MSBase64, DonViSuDungID).Status < 1)
                            {
                                base.Status = -1;
                                Message += ConstantLogMessage.API_Error_System;
                            }
                        }

                        if (VaoRaModel.AnhChanDungBase64 != null && VaoRaModel.AnhChanDungBase64.Length > 0)
                        {
                            if (InsertAnh_CheckIn(TenFile, Result.Status, EnumLoaiFile.AnhChanDung.GetHashCode(), VaoRaModel.AnhChanDungBase64, DonViSuDungID).Status < 1)
                            {
                                base.Status = -1;
                                Message += ConstantLogMessage.API_Error_System;
                            }
                        }
                    }
                    base.Message = Message;
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
                    var CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0);
                    VaoRaModel.LeTan_Ra = CanBoID;

                    //var clsCommon = new Commons();
                    //string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                    //var cr = _VaoRaBUS.Get_By_ThongTinVaoRaID(VaoRaModel.ThongTinVaoRaID, serverPath);
                    //var GioCheckInGanNhat = cr.GioVao;
                    //TimeSpan Time = DateTime.Now - GioCheckInGanNhat;
                    //var KhoangThoiGian = Utils.ConvertToInt32(_SystemConfigBUS.GetByKey("KhoangThoiGianChekIn").ConfigValue, 0);
                    //if (Time.TotalSeconds < KhoangThoiGian && KhoangThoiGian > 0)
                    //{
                    //    base.Status = 3;
                    //    if (VaoRaModel.LaCanBo)
                    //    {
                    //        base.Message = "Cán bộ đã checkin. Vui lòng kiểm tra lại.";
                    //    }
                    //    base.Message = "Khách đã checkin. Vui lòng kiểm tra lại.";
                    //    return base.GetActionResult();
                    //}

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
        [CustomAuthAttribute(ChucNangEnum.CheckIn_CheckOut, AccessLevel.Read)]
        public IActionResult GetByMaThe(string MaThe, int LoaiCheckOut)
        {
            try
            {
                var DonViSuDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == ("CoQuanSuDungPhanMem")).Value, 0);
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
                    ////var folderName = Path.Combine(_SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue);
                    //string serverPath = clsCommon.GetServerPath(HttpContext) + "/ImageGoCheckIn/";
                    var folderName = Path.Combine(_SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue);
                    string serverPath = clsCommon.GetServerPath(HttpContext) + "/" + folderName + "/";
                    //string serverPath = folderName + "/UploadFile/";
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
                    else
                    {
                        var clsCommon = new Commons();
                        string serverPath = clsCommon.GetServerPath(HttpContext) + "/UploadFile/";
                        foreach (var item in Data)
                        {
                            if(item.AnhChanDungBase64 != null && item.AnhChanDungBase64.Length > 0)
                            {
                                item.AnhChanDungBase64 = serverPath + item.AnhChanDungBase64;
                            }
                            if (item.AnhCMND_MTBase64 != null && item.AnhCMND_MTBase64.Length > 0)
                            {
                                item.AnhCMND_MTBase64 = serverPath + item.AnhCMND_MTBase64;
                            }
                        }
                        
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
                    else
                    {
                        var clsCommon = new Commons();
                        string serverPath = clsCommon.GetServerPath(HttpContext) + "/ImageGoCheckIn/";
                        foreach (var item in Data)
                        {
                            if (item.AnhChanDungBase64 != null && item.AnhChanDungBase64.Length > 0)
                            {
                                item.AnhChanDungBase64 = serverPath + item.AnhChanDungBase64;
                            }
                            if (item.AnhCMND_MTBase64 != null && item.AnhCMND_MTBase64.Length > 0)
                            {
                                item.AnhCMND_MTBase64 = serverPath + item.AnhCMND_MTBase64;
                            }
                        }
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
                    //var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    var relativePath = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    var pathSaveFile = Path.Combine(_host.ContentRootPath, relativePath);
                    var b64 = file.Base64;
                    //b64 = b64.Split(',')[1];
                    if (b64.Contains(","))
                    {
                        b64 = b64.Split(',')[1];
                    }
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
        public class AttachmentType
        {
            public string image_cmt { get; set; } // ảnh mặt trước CMND
            public string image_cmt2 { get; set; } // ảnh mặt sau CMND
            public string image_live { get; set; } // ảnh chân dung
        }
    }
}