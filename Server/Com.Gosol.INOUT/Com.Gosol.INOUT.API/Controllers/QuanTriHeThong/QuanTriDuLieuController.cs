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
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.API.Authorization;
using Com.Gosol.INOUT.Security;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.INOUT.API.Controllers.QuanTriHeThong
{
    [Route("api/v1/QuanTriDuLieu")]
    [ApiController]
    public class QuanTriDuLieuController : BaseApiController
    {
        private IQuanTriDuLieuBUS _QuanTriDuLieuBUS;
        private ILogHelper LogHelper;
        private readonly ILogger<QuanTriDuLieuController> _logger;
        public QuanTriDuLieuController(IQuanTriDuLieuBUS QuanTriDuLieuBUS, ILogHelper _LogHelper, ILogger<QuanTriDuLieuController> logger) : base(_LogHelper)
        {
            this._QuanTriDuLieuBUS = QuanTriDuLieuBUS;
            this.LogHelper = _LogHelper;
            _logger = logger;
        }

        [HttpGet]
        [Route("BackupDatabase")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanTri_DuLieu, AccessLevel.FullAccess)]
        public IActionResult BackupDatabase(string fileName)
        {
            try
            {
                int val = 0;
                val = _QuanTriDuLieuBUS.BackupData(fileName);
                if (val == -1)
                {
                    base.Message = ConstantLogMessage.API_Error_System;
                    return base.GetActionResult();
                }
                if (val == 0)
                {
                    base.Message = ConstantLogMessage.Alert_Error_Exist("Tên file sao lưu - " +  "\""+ fileName +"\"");
                    return base.GetActionResult();
                }
                else
                {
                    base.Message = "Sao lưu dữ liệu thành công";
                    return CreateActionResult("Sao lưu dữ liệu - INOUT_" + fileName, EnumLogType.BackupDatabase, () =>
                    {
                      // _logger.LogInformation(User.Claims.FirstOrDefault(c => c.Type == "TenCanBo").Value.ToString() + " - Sao lưu dữ liệu ", fileName);
                        base.Status = val;
                        base.Data = Data;
                        return base.GetActionResult();
                    });
                }

                //return CreateActionResult(ConstantLogMessage.HT_QuanTriDuLieu_BackupDatabase, LogType.BackupDatabase, () =>
                //{
                //    int val = 0;
                //    val = _QuanTriDuLieuBUS.BackupData(fileName);
                //    if (val == -1)
                //    {
                //        base.Message = ConstantLogMessage.API_Error_System;
                //    }
                //    if (val == 0)
                //    {
                //        base.Message = ConstantLogMessage.Alert_Error_Exist(fileName);
                //    }
                //    else
                //    {
                //        base.Message = "Sao lưu dữ liệu thành công";
                //    }
                //    base.Status = val;
                //    base.Data = Data;
                //    return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw;
            }

        }
        [HttpGet]
        [Route("RestoreDatabase")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanTri_DuLieu, AccessLevel.FullAccess)]
        public IActionResult RestoreDatabase(string fileName)
        {
            try
            {
                int val = 0;
                val = _QuanTriDuLieuBUS.RestoreDatabase(fileName);
                if (val == -1)
                {
                    base.Message = ConstantLogMessage.API_Error_System;
                    return base.GetActionResult();
                }
                if (val == 0)
                {
                    base.Message = ConstantLogMessage.Alert_Error_NotExist("File sao lưu - " +  "\"" + fileName + "\"");
                    return base.GetActionResult();
                }
                else
                {
                    base.Message = "Phục hồi dữ liệu thành công";
                    base.Status = val;
                    base.Data = Data;
                    return CreateActionResult("Phục hồi dữ liệu - " + fileName, EnumLogType.RestoreDatabase, () =>
                    {
                       // _logger.LogInformation(User.Claims.FirstOrDefault(c => c.Type == "TenCanBo").Value.ToString() + " - Phục hồi dữ liệu ", fileName);
                        return base.GetActionResult();
                    });

                }

                //return CreateActionResult(ConstantLogMessage.HT_QuanTriDuLieu_RestoreDatabase, LogType.RestoreDatabase, () =>
                //{
                //    if (val == -1)
                //    {
                //        base.Message = ConstantLogMessage.API_Error_System;
                //    }
                //    if (val == 0)
                //    {
                //        base.Message = ConstantLogMessage.Alert_Error_NotExist(fileName);
                //    }
                //    else
                //    {
                //        base.Message = "Phục hồi dữ liệu thành công";
                //    }
                //    base.Status = val;
                //    base.Data = Data;
                //    return base.GetActionResult();
                //});
            }
            catch (Exception)
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
                throw;
            }

        }


        [HttpGet]
        [Route("GetFileInDerectory")]
        public IActionResult GetFileInDerectory()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_QuanTriDuLieu_GetListFileBackup, EnumLogType.GetList, () =>
                {
                    int val = 0;
                    var Data = _QuanTriDuLieuBUS.GetFileInDerectory();
                    if (Data.Count < 1)
                    {
                        base.Message = ConstantLogMessage.API_NoData;
                        val = 1;
                    }
                    else if (Data == null)
                    {
                        base.Message = ConstantLogMessage.API_Error_System;
                        val = 0;
                    }
                    else
                    {
                        base.Message = " ";
                        val = 1;
                    }
                    base.Status = val;
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

    }
}