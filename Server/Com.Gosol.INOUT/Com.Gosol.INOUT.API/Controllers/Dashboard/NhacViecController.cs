using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.KKTS.BUS.Dashboard;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.INOUT.API.Controllers.Dashboard
{
    [Route("api/v1/NhacViec")]
    [ApiController]
    public class NhacViecController : BaseApiController
    {
        private INhacViecBUS _INhacViecBUS;

        public NhacViecController(ILogHelper logHelper, INhacViecBUS _CongKhaiBanKeKhaiTaiSanBUS, ILogger<NhacViecController> logger) : base(logHelper, logger)
        {
            this._INhacViecBUS = _CongKhaiBanKeKhaiTaiSanBUS;
        }
        [HttpGet]
        [Route("GetViecLam")]
        public IActionResult GetViecLam([FromQuery]Models.BasePagingParams p)
        {
            try
            {
                return CreateActionResult("Lấy danh sách nhắc việc", EnumLogType.Other, () =>
                {
                    int TotalRow = 0;
                    var Data = _INhacViecBUS.GetViecLam(Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0),
                        Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0),p, ref  TotalRow);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Data = Data;
                        base.TotalRow = TotalRow;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    base.Status = 1;
                    base.Data = Data;
                    base.TotalRow = TotalRow;
                    return base.GetActionResult();
                });
            }
            catch
            {
                base.Status = -1;
                base.Message = ConstantLogMessage.API_Error_System;
                return base.GetActionResult();
            }
        }
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_LoaiTaiSan, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(Models.Dashboard.NhacViecModel.NhacViecPar p)
        {
            try
            {
                return CreateActionResult_Action("Update Notify", EnumLogType.Update, EnumSystemLogType.NghiepVu,null, () =>
                {
                    int val = 0;
                    p.CanBoNhanThongBao = Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CanBoID").Value, 0);
                    val = _INhacViecBUS.UpdateNotify(p);
                    base.Status = val;
                    return base.GetActionResult();
                });
            }
            catch (Exception e)
            {
                base.Status = -1;
                return base.GetActionResult();
                throw e;
            }

        }
    }
}