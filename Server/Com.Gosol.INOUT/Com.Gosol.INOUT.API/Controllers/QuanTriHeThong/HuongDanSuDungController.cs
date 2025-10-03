using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Authorization;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.QuanTriHeThong;
//
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Microsoft.AspNetCore.Hosting;

namespace Com.Gosol.INOUT.API.Controllers.KeKhai
{
    [Route("api/v1/HuongDanSuDung")]
    [ApiController]
    public class HuongDanSuDungController : BaseApiController
    {
        private IHuongDanSuDungBUS _HuongDanSuDungBUS;
        private ISystemConfigBUS _SystemConfigBUS;
        private readonly IHostingEnvironment _host;
        public HuongDanSuDungController(IHuongDanSuDungBUS HuongDanSuDungBUS, ISystemConfigBUS SystemConfigBUS, ILogHelper logHelper, ILogger<HuongDanSuDungController> logger, IHostingEnvironment host) : base(logHelper, logger)
        {
            this._HuongDanSuDungBUS = HuongDanSuDungBUS;
            this._SystemConfigBUS = SystemConfigBUS;
            _host = host;
        }



        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_HuongDanSuDung, AccessLevel.Read)]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery]BasePagingParamsForFilter p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<HuongDanSuDungModel> Data;
                    Data = _HuongDanSuDungBUS.GetPagingBySearch(p, ref TotalRow);
                    //var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    //var folderName = Path.Combine(pathSaveFile);
                    //for (int i = 0; i < Data.Count; i++)
                    //{
                    //    Data[i].UrlFile = Path.Combine(_host.WebRootPath + folderName, Data[i].TenFileHeThong);

                    //}
                    var cmClass = new Commons();
                    var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    var folderName = Path.Combine(pathSaveFile);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    for (int i = 0; i < Data.Count; i++)
                    {
                        //var base64 = string.Empty;
                        //var fullPath = Path.Combine(pathToSave, Data[i].TenFileHeThong);
                        //base64 = "data:application/pdf;base64" + "," + cmClass.ConvertFileToBase64(fullPath);
                        //if (base64 != string.Empty && base64.Length > 0)
                        //{
                        //    Data[i].Base64String = base64;
                        //    base.Status = 1;
                        //    base.Message = string.Empty;
                        //}
                        //Data[i].UrlFile = Path.Combine(_host.WebRootPath + folderName, Data[i].TenFileHeThong);
                        //Data[i].UrlFile = "Upload/FileDinhKemDuyetKeKhai/" + Data[i].TenFileHeThong;
                        Data[i].UrlFile = "HuongDanSuDung/taifile?HuongDanSuDungID=" + Data[i].HuongDanSuDungID;
                    }


                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw;
            }

        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_HuongDanSuDung, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParamsForFilter p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<HuongDanSuDungModel> Data;
                    Data = _HuongDanSuDungBUS.GetPagingBySearch(p, ref TotalRow);
                    for (int i = 0; i < Data.Count; i++)
                    {
                        Data[i].UrlFile = "Upload/FileDinhKemDuyetKeKhai/" + Data[i].TenFileHeThong;
                    }
                    //var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    //var folderName = Path.Combine(pathSaveFile);
                    //for (int i = 0; i < Data.Count; i++)
                    //{
                    //    Data[i].UrlFile = Path.Combine(_host.WebRootPath + folderName, Data[i].TenFileHeThong);

                    //}
                    //var cmClass = new Commons();
                    //var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    //var folderName = Path.Combine(pathSaveFile);
                    //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    //for (int i = 0; i < Data.Count; i++)
                    //{
                    //    var base64 = string.Empty;
                    //    var fullPath = Path.Combine(pathToSave, Data[i].TenFileHeThong);
                    //    base64 = "data:application/pdf;base64" + "," + cmClass.ConvertFileToBase64(fullPath);
                    //    if (base64 != string.Empty && base64.Length > 0)
                    //    {
                    //        Data[i].Base64String = base64;
                    //        base.Status = 1;
                    //        base.Message = string.Empty;
                    //    }
                    //    Data[i].UrlFile = Path.Combine(_host.WebRootPath + folderName, Data[i].TenFileHeThong);
                    //}


                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw;
            }

        }


        [HttpPost]
        // [CustomAuthAttribute(ChucNangEnum.HeThong_HuongDanSuDung, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(HuongDanSuDungModel HuongDanSuDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_Them, EnumLogType.Insert, () =>
                {
                    var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    HuongDanSuDungModel.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + HuongDanSuDungModel.TenFileGoc;
                    var Result = _HuongDanSuDungBUS.Insert(HuongDanSuDungModel, crCanBoID);
                    var crFile = new FileModel();
                    crFile.TenFile = HuongDanSuDungModel.TenFileGoc;
                    crFile.Base64 = HuongDanSuDungModel.Base64String;
                    var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    var cmClass = new Commons();
                    cmClass.SaveBase64ToFile(crFile, HuongDanSuDungModel.TenFileHeThong, pathSaveFile);
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


        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HT_HuongDanSuDung, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(HuongDanSuDungModel HuongDanSuDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_Sua, EnumLogType.Update, () =>
                {
                    var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    HuongDanSuDungModel.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + HuongDanSuDungModel.TenFileGoc;
                    var Result = _HuongDanSuDungBUS.Update(HuongDanSuDungModel, CanBoID);
                    if (HuongDanSuDungModel.Base64String != null && HuongDanSuDungModel.Base64String.Length > 0)
                    {
                        var crFile = new FileModel();
                        crFile.TenFile = HuongDanSuDungModel.TenFileGoc;
                        crFile.Base64 = HuongDanSuDungModel.Base64String;
                        var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                        var cmClass = new Commons();
                        cmClass.SaveBase64ToFile(crFile, HuongDanSuDungModel.TenFileHeThong, pathSaveFile);
                    }


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


        [HttpPost]
        [CustomAuthAttribute(ChucNangEnum.HT_HuongDanSuDung, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_Xoa, EnumLogType.Delete, () =>
                {
                    var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    var Result = _HuongDanSuDungBUS.Delete(p.ListID, CanBoID);
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
        //[CustomAuthAttribute(ChucNangEnum.HeThong_HuongDanSuDung, AccessLevel.Read)]
        [Route("GetByID")]
        public IActionResult GetByID(int HuongDanSuDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_GetByID, EnumLogType.GetByID, () =>
                {
                    var Data = _HuongDanSuDungBUS.GetByID(HuongDanSuDungID);
                    if (Data == null || Data.HuongDanSuDungID < 1)
                    { base.Message = ConstantLogMessage.API_NoData; base.Status = 0; }
                    else
                    {
                        var cmClass = new Commons();
                        base.Message = " "; base.Status = 1;
                        var base64 = string.Empty;
                        var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                        var folderName = Path.Combine(pathSaveFile);
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var fullPath = Path.Combine(pathToSave, Data.TenFileHeThong);
                        base64 = "data:application/pdf;base64" + "," + cmClass.ConvertFileToBase64(fullPath);
                        if (base64 != string.Empty && base64.Length > 0)
                        {
                            Data.Base64String = base64;
                            base.Status = 1;
                            base.Message = string.Empty;
                            Data.UrlFile = "HuongDanSuDung/taifile?HuongDanSuDungID=" + Data.HuongDanSuDungID;
                        }
                        else
                        {
                            base.Status = 0;
                            base.Message = "Bản cứng không tồn tại";
                        }
                    }
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw;
            }
        }

        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_HuongDanSuDung, AccessLevel.Read)]
        [Route("GetByMaChucNang")]
        public IActionResult GetByMaChucNang(string MaChucNang)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_GetByMaChucNang, EnumLogType.GetByID, () =>
                {
                    var Data = _HuongDanSuDungBUS.GetByMaChucNang(MaChucNang);
                    if (Data == null || Data.HuongDanSuDungID < 1)
                    { base.Message = ConstantLogMessage.API_NoData; base.Status = 0; }
                    else
                    {
                        var cmClass = new Commons();
                        base.Message = " "; base.Status = 1;
                        var stringBase64 = string.Empty;
                        var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                        var folderName = Path.Combine(pathSaveFile);
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                        var fullPath = Path.Combine(pathToSave, Data.TenFileHeThong);
                        stringBase64 = cmClass.ConvertFileToBase64(fullPath);

                        if (stringBase64 != string.Empty && stringBase64.Length > 0)
                        {
                            Data.Base64String = "data:application/pdf;base64" + "," + stringBase64;
                            base.Status = 1;
                            base.Message = string.Empty;
                        }
                        else
                        {
                            base.Status = 0;
                            base.Message = "Bản cứng không tồn tại";
                        }
                    }
                    base.Data = Data;
                    return base.GetActionResult();
                });
            }
            catch (Exception)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw;
            }
        }


        //[HttpGet]
        ////[CustomAuthAttribute(ChucNangEnum.HeThong_HuongDanSuDung, AccessLevel.Read)]
        //[Route("TaiFile")]
        //public async Task<IActionResult> TaiFile(int HuongDanSuDungID)
        //{
        //    try
        //    {
        //        //return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_GetByID, EnumLogType.GetByID, () =>
        //        //{
        //        var Data = _HuongDanSuDungBUS.GetByID(HuongDanSuDungID);
        //        if (Data == null || Data.HuongDanSuDungID < 1)
        //        { base.Message = ConstantLogMessage.API_NoData; base.Status = 0; return null; }
        //        else
        //        {
        //            var cmClass = new Commons();
        //            base.Message = " "; base.Status = 1;
        //            var base64 = string.Empty;
        //            var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
        //            var folderName = Path.Combine(pathSaveFile);
        //            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //            var fullPath = Path.Combine(pathToSave, Data.TenFileHeThong);
        //            var memory = new MemoryStream();
        //            using (var stream = new FileStream(fullPath, FileMode.Open))
        //            {
        //                await stream.CopyToAsync(memory);
        //            }
        //            memory.Position = 0;
        //            return File(memory, GetContentType(fullPath));
        //        }
        //        //base.Data = Data;
        //        //return base.GetActionResult();
        //        //});
        //    }
        //    catch (Exception)
        //    {
        //        base.Status = -1;
        //        return base.GetActionResult();
        //        throw;
        //    }
        //}


        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_HuongDanSuDung, AccessLevel.Read)]
        [Route("TaiFile")]
        public IActionResult TaiFile(int HuongDanSuDungID)
        {
            try
            {
                //return CreateActionResult(ConstantLogMessage.HT_HuongDanSuDung_GetByID, EnumLogType.GetByID, () =>
                //{
                var Data = _HuongDanSuDungBUS.GetByID(HuongDanSuDungID);
                if (Data == null || Data.HuongDanSuDungID < 1)
                { base.Message = ConstantLogMessage.API_NoData; base.Status = 0; return null; }
                else
                {
                    var cmClass = new Commons();
                    base.Message = " "; base.Status = 1;
                    var base64 = string.Empty;
                    var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
                    var folderName = Path.Combine(pathSaveFile);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    var fullPath = Path.Combine(pathToSave, Data.TenFileHeThong);
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(fullPath, FileMode.Open))
                    {
                        stream.CopyTo(memory);
                    }
                    memory.Position = 0;
                    return File(memory, GetContentType(fullPath));
                }
                //base.Data = Data;
                //return base.GetActionResult();
                //});
            }
            catch (Exception ex)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw ex;
            }
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                //{".xlsx", "application/vnd.openxmlformats
                //           officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }













        //[HttpPost]
        ////[CustomAuthAttribute(ChucNangEnum.KeKhai_TaiSan, AccessLevel.Create)]
        //[Route("UploadBanCung")]
        //public IActionResult UploadBanCung([FromBody] PostFile HuongDanSuDung)
        //{
        //    try
        //    {
        //        var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
        //        return CreateActionResult(ConstantLogMessage.KK_HuongDanSuDung_Them, EnumLogType.Insert, () =>
        //         {
        //             if (HuongDanSuDung.KeKhaiID != null && HuongDanSuDung.ListBase64.Count > 0)
        //             {
        //                 var DanhSachFileDaCo = _HuongDanSuDungBUS.GetListHuongDanSuDungByKeKhaiID(HuongDanSuDung.KeKhaiID).Where(x => x.TrangThai == 1).ToList();
        //                 for (int i = 0; i < DanhSachFileDaCo.Count; i++)
        //                 {
        //                     if (HuongDanSuDung.ListBase64.Any(x => x.TenFile == DanhSachFileDaCo[i].TenFileGoc))
        //                     {
        //                         base.Status = 0;
        //                         base.Message = "Tên file đã tồn tại";
        //                         return base.GetActionResult();
        //                     }

        //                 }
        //                 for (int i = 0; i < HuongDanSuDung.ListBase64.Count; i++)
        //                 {
        //                     var TenFileGoc = HuongDanSuDung.ListBase64[i].TenFile;
        //                     var FileType = HuongDanSuDung.ListBase64[i].Base64.Split(',')[0];
        //                     var crObj = new HuongDanSuDungModel();
        //                     crObj.NgayCapNhat = DateTime.Now;
        //                     crObj.NguoiCapNhat = crCanBoID;
        //                     crObj.KeKhaiID = HuongDanSuDung.KeKhaiID;
        //                     crObj.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + TenFileGoc;
        //                     crObj.TenFileGoc = TenFileGoc;
        //                     crObj.FileType = FileType;
        //                     crObj.Base64String = HuongDanSuDung.ListBase64[i].Base64;
        //                     var Result = _HuongDanSuDungBUS.Insert(crObj);
        //                     if (Result.Status < 1)
        //                     {
        //                         base.Status = 0;
        //                         base.Message = "Không thể đính kèm file";
        //                     }
        //                     var crFile = new FileModel();
        //                     crFile.TenFile = crObj.TenFileGoc;
        //                     crFile.Base64 = HuongDanSuDung.ListBase64[i].Base64;
        //                     SaveBase64ToFile(crFile, crObj.TenFileHeThong);
        //                 }
        //                 base.Status = 1;
        //                 base.Message = "Thêm mới file đính kèm thành công";
        //             }
        //             else
        //             {
        //                 base.Status = 0;
        //                 base.Message = "Vui lòng chọn file đính kèm";
        //             }
        //             return base.GetActionResult();
        //         });
        //    }
        //    catch (Exception ex)
        //    {
        //        base.Status = -1;
        //        base.Message = ConstantLogMessage.API_Error_System;
        //        base.GetActionResult();
        //        throw ex;
        //    }
        //}

        //[HttpGet]
        ////[CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Read)]
        //[Route("GetByID")]
        //public IActionResult GetByID(int HuongDanSuDungID)
        //{
        //    try
        //    {
        //        return CreateActionResult(ConstantLogMessage.KK_HuongDanSuDung_GetByID, EnumLogType.GetList, () =>
        //        {
        //            HuongDanSuDungModel Data = new HuongDanSuDungModel();
        //            Data = _HuongDanSuDungBUS.GetByID(HuongDanSuDungID);
        //            if (Data == null || Data.KeKhaiID == null || Data.KeKhaiID < 1)
        //            {
        //                base.Status = 0;
        //                base.Message = "Bản cứng không tồn tại";
        //            }
        //            //if (Data != null && Data.FileID > 0 && Data.TenFileHeThong != null && Data.TenFileHeThong.Length > 0)
        //            //{
        //            //    var folderName = Path.Combine("Upload", "HuongDanSuDungDuyetKeKhai");
        //            //    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //            //    var pathFile = Path.Combine(pathToSave, Data.TenFileHeThong);
        //            //    if (!System.IO.File.Exists(pathFile))
        //            //    {
        //            //        base.Status = 0;
        //            //        base.Message = "File không tồn tại";
        //            //    }
        //            //    var Base64String = ConvertFileToBase64(pathFile);
        //            //    if (Base64String != string.Empty && Base64String.Length > 0)
        //            //    {
        //            //        Data.Base64String = string.Concat(Data.FileType + "," + Base64String);
        //            //    }
        //            //}
        //            else
        //            {
        //                var base64 = string.Empty;
        //                var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
        //                var folderName = Path.Combine(pathSaveFile);
        //                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //                var fullPath = Path.Combine(pathToSave, Data.TenFileHeThong);
        //                base64 = Data.FileType + "," + ConvertFileToBase64(fullPath);
        //                if (base64 != string.Empty && base64.Length > 0)
        //                {
        //                    Data.Base64String = base64;
        //                    base.Status = 1;
        //                    base.Message = string.Empty;
        //                }
        //                else
        //                {
        //                    base.Status = 0;
        //                    base.Message = "Bản cứng không tồn tại";
        //                }

        //            }
        //            base.Data = Data;
        //            return base.GetActionResult();
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        base.Status = -1;
        //        return base.GetActionResult();
        //        throw;
        //    }
        //}



        //[HttpPost]
        ////[CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Delete)]
        //[Route("Delete")]
        //public IActionResult Delete([FromBody] BaseDeleteParams p)
        //{
        //    try
        //    {
        //        return CreateActionResult(ConstantLogMessage.KK_HuongDanSuDung_Xoa, EnumLogType.Delete, () =>
        //        {
        //            var Result = _HuongDanSuDungBUS.Delete(p.ListID);
        //            base.Status = Result.Status;
        //            base.Message = Result.Message;
        //            return base.GetActionResult();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        base.Status = -1;
        //        base.Message = ConstantLogMessage.API_Error_System;
        //        return base.GetActionResult();
        //        throw ex;
        //    }
        //}


        //[HttpGet]
        ////[CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Read)]
        //[Route("GetAllField_HuongDanSuDung_ByKeKhaiID")]
        //public IActionResult GetAllField_HuongDanSuDung_ByKeKhaiID(int KeKhaiID)
        //{
        //    try
        //    {
        //        return CreateActionResult(ConstantLogMessage.KK_HuongDanSuDung_GetListPaging, EnumLogType.GetList, () =>
        //        {
        //            List<HuongDanSuDungModel> Data = new List<HuongDanSuDungModel>();
        //            Data = _HuongDanSuDungBUS.GetAllField_HuongDanSuDung_ByKeKhaiID(KeKhaiID);
        //            base.Status = 1;
        //            base.Message = string.Empty;
        //            base.Data = Data;
        //            return base.GetActionResult();
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        base.Status = -1;
        //        return base.GetActionResult();
        //        throw;
        //    }
        //}





        /// <summary>
        /// chưa chùng
        /// </summary>
        /// <param name="HuongDanSuDungID"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("DownloadFile")]
        //public async Task<IActionResult> DownloadFile([FromQuery] int HuongDanSuDungID)
        //{
        //    try
        //    {
        //        HuongDanSuDungModel Data = new HuongDanSuDungModel();
        //        Data = _HuongDanSuDungBUS.GetByID(HuongDanSuDungID);
        //        if (Data != null && Data.FileID > 0 && Data.TenFileHeThong != null && Data.TenFileHeThong.Length > 0)
        //        {
        //            var folderName = Path.Combine("Upload", "HuongDanSuDungDuyetKeKhai");
        //            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //            var pathFile = Path.Combine(pathToSave, Data.TenFileHeThong);
        //            var memory = new MemoryStream();
        //            using (var stream = new FileStream(pathFile, FileMode.Open))
        //            {
        //                await stream.CopyToAsync(memory);
        //            }
        //            memory.Position = 0;
        //            return File(memory, GetContentType(pathFile), Data.TenFileHeThong);
        //        }
        //        else return null;
        //    }
        //    catch (Exception)
        //    {
        //        base.Status = -1;
        //        return base.GetActionResult();
        //        throw;
        //    }
        //}



        //[HttpPost]
        ////[CustomAuthAttribute(ChucNangEnum.KeKhai_TaiSan, AccessLevel.Create)]
        //[Route("UploadBanCung_UploadFile")]
        //public IActionResult UploadBanCung_UploadFile([FromBody] PostFile HuongDanSuDung)
        //{
        //    try
        //    {
        //        //var b64 = HuongDanSuDung.ListBase64[0];
        //        //b64 = b64.Split(',')[1];
        //        //byte[] bytes = Convert.FromBase64String(b64);
        //        //var folderName = Path.Combine("Upload", "HuongDanSuDungDuyetKeKhai");
        //        //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //        //var fullPath = Path.Combine(pathToSave, "xxx.png");
        //        //System.IO.File.WriteAllBytes(fullPath, bytes);
        //        //  int KeKhaiID = 0;
        //        var crCanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
        //        return CreateActionResult(ConstantLogMessage.KK_HuongDanSuDung_Them, EnumLogType.Insert, () =>
        //        {
        //            // gọi hàm upload file
        //            if (HuongDanSuDung.KeKhaiID != null && HuongDanSuDung.ListBase64.Count > 0)
        //            {
        //                for (int i = 0; i < HuongDanSuDung.ListBase64.Count; i++)
        //                {
        //                    var TenFileGoc = HuongDanSuDung.ListBase64[i].TenFile;
        //                    var FileType = HuongDanSuDung.ListBase64[i].Base64.Split(',')[0];
        //                    var crObj = new HuongDanSuDungModel();
        //                    crObj.NgayCapNhat = DateTime.Now;
        //                    crObj.NguoiCapNhat = crCanBoID;
        //                    crObj.KeKhaiID = HuongDanSuDung.KeKhaiID;
        //                    crObj.TenFileHeThong = crCanBoID.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + "_" + TenFileGoc;
        //                    crObj.TenFileGoc = TenFileGoc;
        //                    crObj.FileType = FileType;
        //                    var isHuongDanSuDung = _HuongDanSuDungBUS.Insert(crObj);
        //                    if (isHuongDanSuDung.Status > 0)
        //                    {
        //                        var upload = UploadFileFromBase64(HuongDanSuDung.ListBase64[i], crObj.TenFileHeThong);
        //                        if (!upload)
        //                        {
        //                            base.Status = 0;
        //                            base.Message = "Không thể upload file";
        //                            break;
        //                        }
        //                    }
        //                }
        //                base.Status = 1;
        //                base.Message = "Thêm mới file đính kèm thành công";
        //            }
        //            else
        //            {
        //                base.Status = 0;
        //                base.Message = "Vui lòng chọn file đính kèm";
        //            }
        //            return base.GetActionResult();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        base.Status = -1;
        //        base.GetActionResult();
        //        throw ex;
        //    }
        //}


        /// <summary>
        /// save base64 to file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="TenFileHeThong"></param>
        /// <returns></returns>
        //public bool SaveBase64ToFile(FileModel file, string TenFileHeThong)
        //{
        //    try
        //    {
        //        if (file.Base64.Length > 0)
        //        {
        //            var pathSaveFile = _SystemConfigBUS.GetByKey("UploadFile_Path").ConfigValue;
        //            var b64 = file.Base64;
        //            b64 = b64.Split(',')[1];
        //            byte[] bytes = Convert.FromBase64String(b64);
        //            //var folderName = Path.Combine("Upload", "HuongDanSuDungDuyetKeKhai");
        //            var folderName = Path.Combine(pathSaveFile);
        //            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        //            var fullPath = Path.Combine(pathToSave, TenFileHeThong);
        //            System.IO.File.WriteAllBytes(fullPath, bytes);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //        throw ex;
        //    }
        //}

        //public string ConvertFileToBase64(string pathFile)
        //{
        //    try
        //    {
        //        var at = System.IO.File.GetAttributes(pathFile);

        //        byte[] fileBit = System.IO.File.ReadAllBytes(pathFile);
        //        var file = System.IO.Path.Combine(pathFile);

        //        string AsBase64String = Convert.ToBase64String(fileBit);
        //        return AsBase64String;
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //        throw ex;
        //    }
        //}










    }
}
