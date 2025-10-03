using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.INOUT.API.Formats;
using static Com.Gosol.INOUT.BUS.DanhMuc.DanhMucNhomTaiSanBUS;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.API.Authorization;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.INOUT.API.Controllers.DanhMuc
{
    [Route("api/v1/DanhMucNhomTaiSan")]
    [ApiController]
    public class DanhMucNhomTaiSanController : BaseApiController
    {
        private IDanhMucNhomTaiSanBUS _DanhMucNhomTaiSanBUS;

        public DanhMucNhomTaiSanController(IDanhMucNhomTaiSanBUS DanhMucNhomTaiSanBUS, ILogHelper _LogHelper, ILogger<DanhMucNhomTaiSanController> logger) : base(_LogHelper, logger)
        {
            this._DanhMucNhomTaiSanBUS = DanhMucNhomTaiSanBUS;
        }
        //Get: api//NhomTaiSans
        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_NhomTaiSan, AccessLevel.Create)]
        [Route("Insert")]
        public IActionResult Insert(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_ThemNhomTaiSan, EnumLogType.Insert, () =>
                 {
                     string Message = null;
                     int val = 0;
                     val = _DanhMucNhomTaiSanBUS.Insert(DanhMucNhomTaiSanModel, ref Message);

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
        // [CustomAuthAttribute(ChucNangEnum.DanhMuc_NhomTaiSan, AccessLevel.Edit)]
        [Route("Update")]
        public IActionResult Update(DanhMucNhomTaiSanModel DanhMucNhomTaiSanModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_SuaNhomTaiSan, EnumLogType.Update, () =>
                 {
                     string Message = null;
                     int val = 0;
                     val = _DanhMucNhomTaiSanBUS.Update(DanhMucNhomTaiSanModel, ref Message);
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
        // [CustomAuthAttribute(ChucNangEnum.DanhMuc_NhomTaiSan, AccessLevel.Delete)]
        [Route("Delete")]
        public IActionResult Delete([FromBody] BaseDeleteParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_XoaNhomTaiSan, EnumLogType.Delete, () =>
                 {

                     var Result = _DanhMucNhomTaiSanBUS.Delete(p.ListID);
                     if (Result.Count > 0)
                     {
                         string Mes = "";
                         foreach (var item in Result)
                         {
                             Mes = string.Concat(Mes, item, ",");
                         };
                         Mes = string.Concat("Nhóm tài sản ", Mes, " đang được sử dụng. Không thể xóa!");
                         Mes = Mes.Remove(Mes.LastIndexOf(","), 1);
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
        //[HttpGet]
        //[Route("GetAllNTS")]
        //public IActionResult GetAllNTS()
        //{
        //    return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_XoaNhomTaiSan, () =>
        //    {
        //        IList<DanhMucNhomTaiSanModel> Data;
        //        Data = _DanhMucNhomTaiSanBUS.GetAllNTS();
        //        int totalRow = Data.Count();
        //        base.Status = totalRow > 0 ? 1 : 0;
        //        base.Data = Data;
        //        base.TotalRow = totalRow;
        //        return base.GetActionResult();
        //    });
        //}
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_NhomTaiSan, AccessLevel.Read)]
        [Route("GetNTSByID")]
        public IActionResult GetNTSByID(int NhomTaiSanID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_GetByID, EnumLogType.GetByID, () =>
                 {
                     DanhMucNhomTaiSanModel Data;
                     Data = _DanhMucNhomTaiSanBUS.GetNTSByID(NhomTaiSanID);
                     if (Data.NhomTaiSanID <= 0)
                     {
                         base.Message = ConstantLogMessage.API_Error_NotExist;
                     }
                     else
                     {
                         base.Message = "Tìm thấy nhóm tài sản có ID là " + Data.NhomTaiSanID;
                     }
                     base.Status = Data.NhomTaiSanID > 0 ? 1 : 0;
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_NhomTaiSan, AccessLevel.Read)]
        [Route("GetAllNhomTaiSanCha")]
        public IActionResult GetAllNhomTaiSanCha()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_GetByID, EnumLogType.GetByID, () =>
                {
                    IList<DanhMucNhomTaiSanModel> Data;
                    Data = _DanhMucNhomTaiSanBUS.GetAllNhomTaiSanCha();
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
        //[HttpGet]
        //[Route("FilterByName")]
        //public IActionResult FilterByName(string TenNhomTaiSan)
        //{
        //    return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_FilterByName, () =>
        //    {
        //        IList<DanhMucNhomTaiSanModel> Data;
        //        Data = _DanhMucNhomTaiSanBUS.FilterByName(TenNhomTaiSan);
        //        int totalRow = Data.Count();
        //        base.Status = totalRow > 0 ? 1 : 0;
        //        base.Data = Data;
        //        base.TotalRow = totalRow;
        //        return base.GetActionResult();
        //    });
        //}
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_NhomTaiSan, AccessLevel.Read)]
        [Route("GetPagingBySearch")]
        public IActionResult GetPagingBySearch([FromQuery]BasePagingParams p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_NhomTaiSan_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<DanhMucNhomTaiSanModel> Data = new List<DanhMucNhomTaiSanModel>();
                    Data = _DanhMucNhomTaiSanBUS.GetPagingBySearch(p, ref TotalRow);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                        return base.GetActionResult();
                    }
                    int totalRow = Data.Count();
                    base.Status = totalRow > 0 ? 1 : 0;
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

    }
}