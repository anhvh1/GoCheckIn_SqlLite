using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.KKTS.BUS.Dashboard;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.Dashboard;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace Com.Gosol.INOUT.API.Controllers.Dashboard
{
    [Route("api/v1/Dashboard")]
    [ApiController]
    public class DashboardController : BaseApiController
    {
        private IDashboardBUS IDashboardBUS;

        public DashboardController(IDashboardBUS DashboardBUS, ILogHelper _LogHelper, ILogger<DashboardController> logger) : base(_LogHelper, logger)
        {
            this.IDashboardBUS = DashboardBUS;
        }


        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_NhomTaiSan, AccessLevel.Read)]
        [Route("GetListPaging")] // danh sách theo loại ở màn hình chính
        public IActionResult GetListPaging([FromQuery]BasePagingParamsForFilter p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_TrangThai_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    int CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    int CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CanBoID").Value, 0);
                    int VaiTro = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "VaiTro").Value, 0);
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

    }
}
