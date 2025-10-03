using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Authorization;
using Com.Gosol.INOUT.API.Config;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Com.Gosol.INOUT.API.Controllers.DanhMuc
{
    [Route("api/v1/DanhMucChucVu")]
    [ApiController]

    public class DanhMucChucVuController : BaseApiController
    {
        private IDanhMucChucVuBUS _DanhMucChucVuBUS;
        private IHostingEnvironment _host;
        public DanhMucChucVuController(IDanhMucChucVuBUS DanhMucChucVuBUS, IHostingEnvironment hostingEnvironment, ILogHelper _LogHelper, ILogger<DanhMucChucVuController> logger) : base(_LogHelper, logger)
        {
            this._DanhMucChucVuBUS = DanhMucChucVuBUS;
            this._host = hostingEnvironment;
        }

        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.HT_DanhMucChucVu, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucChucVuModel DanhMucChucVuModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_ThemChucVu, EnumLogType.Insert, () =>
                {
                    var Result = _DanhMucChucVuBUS.Insert(DanhMucChucVuModel);
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
        //  [CustomAuthAttribute(ChucNangEnum.DanhMuc_ChucVu, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucChucVuModel DanhMucChucVuModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_SuaChucVu, EnumLogType.Update, () =>
                {
                    var Result = _DanhMucChucVuBUS.Update(DanhMucChucVuModel);
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_ChucVu, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_XoaChucVu, EnumLogType.Delete, () =>
                 {

                     var Result = _DanhMucChucVuBUS.Delete(p.ListID);
                     if (Result.Count > 0)
                     {
                         string Mes = "";                        
                         foreach(var item in Result)
                         {
                            Mes= string.Concat(Mes, item,",");
                         };
                         Mes = string.Concat("Chức vụ ", Mes, " đang được sử dụng. Không thể xóa!");
                         Mes = Mes.Remove(Mes.LastIndexOf(","),1);
                         base.Message = Mes;
                         //base.Data = Result;
                         base.Status = 0;
                         return base.GetActionResult();
                     }
                     else
                     {
                         base.Message = "Xóa thành công!";
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
        [Route("GetByID")]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_ChucVu, AccessLevel.Read)]
        public IActionResult GetByID(int? ChucVuID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_GetByID, EnumLogType.GetByID, () =>
                 {
                     var Data = _DanhMucChucVuBUS.GetByID(ChucVuID);
                     if (Data != null && Data.ChucVuID > 0)
                     {
                         base.Message = ConstantLogMessage.API_Success;
                         base.Data = Data;
                     }
                     else
                     {
                         base.Message = ConstantLogMessage.API_NoData;
                     }
                     base.Status = 1;

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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_ChucVu, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_GetListPaging, EnumLogType.GetList, () =>
                 {
                     int TotalRow = 0;
                     IList<DanhMucChucVuModel> Data;
                     Data = _DanhMucChucVuBUS.GetPagingBySearch(p, ref TotalRow);
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
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_ChucVu, AccessLevel.Read)]
        [Route("ImportChucVu")]
        public async Task<IActionResult> ImportChucVu([FromBody]Files file)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DM_ChucVu_ImportFile, EnumLogType.Other, () =>
                {
                    string SavePath = _host.ContentRootPath + "\\Upload\\" + "Import_ChucVu.xlsx";
                    using (FileStream stream = System.IO.File.Create(SavePath))
                    {
                        byte[] byteArray = Convert.FromBase64String(file.files);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }

                    var Result = _DanhMucChucVuBUS.ImportChucVu(SavePath);
                    base.Status = Result.Status;
                    base.Message = Result.Message;
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
    }
}
