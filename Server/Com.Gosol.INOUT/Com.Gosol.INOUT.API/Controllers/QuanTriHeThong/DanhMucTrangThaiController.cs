using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.INOUT.API.Authorization;
using Com.Gosol.INOUT.Security;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.INOUT.API.Controllers.DanhMuc
{
    [Route("api/v1/DanhMucTrangThai")]
    [ApiController]
    public class DanhMucTrangThaiController : BaseApiController
    {
        private IDanhMucTrangThaiBUS _DanhMucTrangThaiBUS;
        public DanhMucTrangThaiController(IDanhMucTrangThaiBUS DanhMucTrangThaiBUS, ILogHelper _LogHelper, ILogger<DanhMucTrangThaiController> logger) : base(_LogHelper, logger)
        {
            this._DanhMucTrangThaiBUS = DanhMucTrangThaiBUS;
        }

        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_TrangThai_Them, EnumLogType.Insert, () =>
                 {
                     var Result = _DanhMucTrangThaiBUS.Insert(DanhMucTrangThaiModel);
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
        // [CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucTrangThaiModel DanhMucTrangThaiModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_TrangThai_Sua, EnumLogType.Update, () =>
                {
                    var Result = _DanhMucTrangThaiBUS.Update(DanhMucTrangThaiModel, Utils.ConvertToInt32(User.Claims.FirstOrDefault(x => x.Type == "CoQuanID").Value, 0));
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_TrangThai_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _DanhMucTrangThaiBUS.Delete(p.ListID);
                    base.Message = Result.Message;
                    base.Status = Result.Status;
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Read)]
        [Route("GetByID")]
        public IActionResult GetByID(int TrangThaiID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_TrangThai_GetByID, EnumLogType.GetByID, () =>
                 {
                     var data = new DanhMucTrangThaiModel();
                     var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                     var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                     if (UserRole.CheckAdmin(NguoiDungID))
                     {
                         data = _DanhMucTrangThaiBUS.GetByID(TrangThaiID);
                         data.ListRoleID = new List<int>() { 2, 3, 4 };
                     }
                     else
                     {
                         data = _DanhMucTrangThaiBUS.GetByIDAndCoQuanID(TrangThaiID, CoQuanID);

                     }
                     if (data == null)
                     { base.Message = ConstantLogMessage.API_NoData; base.Status = 0; }
                     else { base.Message = " "; base.Status = 1; }
                     base.Data = data;
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_TrangThai, AccessLevel.Read)]
        [Route("GetListPaging")]
        public IActionResult GetListPaging([FromQuery]BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_TrangThai_GetListPaging, EnumLogType.GetList, () =>
                 {
                     int TotalRow = 0;
                     IList<DanhMucTrangThaiModel> Data;
                     var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                     Data = _DanhMucTrangThaiBUS.GetPagingBySearch(p, ref TotalRow);
                     var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                     if (UserRole.CheckAdmin(NguoiDungID))
                     {
                         foreach (var item in Data)
                         {
                             item.ListRoleID = new List<int>() { 2, 3, 4 };
                         }
                     }
                     else
                     {
                         foreach (var item in Data)
                         {
                             item.ListRoleID = _DanhMucTrangThaiBUS.GetByCoQuanTrangThai(item.TrangThaiID, CoQuanID).Select(x => x.VaiTro).ToList();
                         }
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
    }
}