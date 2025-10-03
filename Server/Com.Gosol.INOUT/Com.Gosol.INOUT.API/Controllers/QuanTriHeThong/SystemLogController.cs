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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Com.Gosol.INOUT.API.Controllers.QuanTriHeThong
{
    [Route("api/v1/SystemLog")]
    [ApiController]
    public class SystemLogController : BaseApiController
    {
        private ISystemConfigBUS _SystemConfigBUS;
        private IHostingEnvironment _host;
        private ISystemLogBUS _SystemLogBUS;
        public SystemLogController(ISystemLogBUS SystemLogBUS, ILogHelper _LogHelper, ILogger<SystemLogController> logger, IHostingEnvironment hostingEnvironment) : base(_LogHelper, logger)
        {
            this._SystemLogBUS = SystemLogBUS;
            this._host = hostingEnvironment;
        }
        [HttpGet]
        [Route("GetListPaging")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NhatKyHeThong, AccessLevel.Read)]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_SystemLog_GetListPaging, EnumLogType.GetList, () =>
                 {
                     int TotalRow = 0;
                     IList<SystemLogPartialModel> Data;
                     Data = _SystemLogBUS.GetPagingBySearch(p, ref TotalRow);
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
        [Route("GetPagingByQuanTriDuLieu")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NhatKyHeThong, AccessLevel.Read)]
        public IActionResult GetPagingByQuanTriDuLieu([FromQuery]BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_SystemLog_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<SystemLogPartialModel> Data;
                    Data = _SystemLogBUS.GetPagingByQuanTriDuLieu(p, ref TotalRow);
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
        [Route("CreateLogFile")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_NhatKyHeThong, AccessLevel.Read)]
        public IActionResult CreateLogFile()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.CreateLogFile, EnumLogType.Other, () =>
                {

                    DirectoryInfo d = new DirectoryInfo(_host.ContentRootPath + "\\LogConfig");
                    FileInfo[] File = d.GetFiles("*.xml");
                    string[] NgayCuoiCung = new string[] { };
                    if (File.Length > 0)
                    {
                        var LastFileName = File.ToList().LastOrDefault().FullName;
                        NgayCuoiCung = LastFileName.Split("_");
                        if ((int.Parse(DateTime.Now.ToString("yyyyMMdd")) - int.Parse(NgayCuoiCung[1].ToString().Substring(0, NgayCuoiCung[1].ToString().LastIndexOf(".")))) < int.Parse(_SystemConfigBUS.GetByKey("Exp_LogFile").ConfigValue))
                        {
                            return base.GetActionResult();
                        }
                    }

                    string SavePath = _host.ContentRootPath + "\\LogConfig\\SystemLogFile_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";
                    using (FileStream stream = System.IO.File.Create(SavePath))
                    {
                        //byte[] byteArray = Convert.FromBase64String(file.files);
                        //stream.Write(byteArray, 0, byteArray.Length);
                    }
                    _SystemLogBUS.CreateLogFile(SavePath, DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                    base.Status = 1;
                    base.TotalRow = TotalRow;
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
    }
}