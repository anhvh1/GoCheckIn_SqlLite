using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Authorization;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.Models.DanhMuc;
using Com.Gosol.INOUT.Security;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Com.Gosol.INOUT.API.Controllers.DanhMuc
{

    [Route("api/v1/DanhMucDiaGioiHanhChinh")]
    [ApiController]
    public class DanhMucDiaGioiHanhChinhController : BaseApiController
    {
        private IDanhMucDiaGioiHanhChinhBUS _DanhMucDiaGioiHanhChinhBUS;

        public DanhMucDiaGioiHanhChinhController(IDanhMucDiaGioiHanhChinhBUS danhMucDiaGioiHanhChinhBUS, ILogHelper _LogHelper, ILogger<DanhMucDiaGioiHanhChinhController> logger) : base(_LogHelper, logger)
        {

            this._DanhMucDiaGioiHanhChinhBUS = danhMucDiaGioiHanhChinhBUS;

        }
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Read)]
        [Route("GetListByidAndCap")]
        public IActionResult GetListByidAndCap()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_GetListByidAndCap, EnumLogType.GetList, () =>
                 {

                     IList<DanhMucDiaGioiHanhChinhModel> Data;
                     Data = _DanhMucDiaGioiHanhChinhBUS.GetListByidAndCap();
                     int totalRow = Data.Count();
                     base.Status = totalRow > 0 ? 1 : 0;
                     base.Data = Data;
                     base.TotalRow = totalRow;
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Read)]
        [Route("FilterByName")]
        public IActionResult FilterByName(string FilterName)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_FilterByName, EnumLogType.GetList, () =>
                 {

                     IList<DanhMucDiaGioiHanhChinhModel> Data;
                     Data = _DanhMucDiaGioiHanhChinhBUS.FilterByName(FilterName);
                     int totalRow = Data.Count();
                     base.Status = totalRow > 0 ? 1 : 0;
                     base.Data = Data;
                     base.TotalRow = totalRow;
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Read)]
        //[Route("GetByID")]
        //public IActionResult GetByID(int id)
        //{
        //    try
        //    {
        //        return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_GetByID, LogType.GetByID, () =>
        //         {

        //             DanhMucDiaGioiHanhChinhModel Data;
        //             Data = _DanhMucDiaGioiHanhChinhBUS.GetByID(id);
        //             base.Status = (Data.TinhID > 0 || Data.HuyenID > 0 || Data.XaID > 0) ? 1 : 0;
        //             base.Data = Data;
        //             return base.GetActionResult();
        //         });
        //    }
        //    catch (Exception)
        //    {
        //        base.Status = -1;
        //        return base.GetActionResult();
        //        throw;
        //    }
        //}
        [HttpGet]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Read)]
        [Route("GetByIDAndCap")]
        public IActionResult GetByIDAnfCap([FromQuery]int id, int Cap, string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_GetByID, EnumLogType.GetList, () =>
                {

                    DanhMucDiaGioiHanhChinhModelUpdatePartial Data;
                    Data = _DanhMucDiaGioiHanhChinhBUS.GetDGHCByIDAndCap(id, Cap, Keyword);
                    if (Data == null)
                    {
                        base.Status = 1;
                        base.Data = Data;
                        return base.GetActionResult();
                    }
                    base.Status = (Data.ID > 0) ? 1 : 0;
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Read)]
        [Route("GetAllByCap")]
        public IActionResult GetAllByCap([FromQuery] int ID, int Cap, string Keyword)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_GetByID, EnumLogType.GetList, () =>
                {

                    List<object> Data;
                    Data = _DanhMucDiaGioiHanhChinhBUS.GetAllByCap(ID, Cap, Keyword);
                    if (Data.Count == 0)
                    {
                        base.Status = 1;
                        base.Message = ConstantLogMessage.API_NoData;
                    }
                    else
                    {
                        base.Status = Data.Count() > 0 ? 1 : 0;
                    }
                    base.Data = Data;
                    base.TotalRow = Data.Count();
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

        #region Gộp chung

        [HttpPost]
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Create)]
        [Route("InsertDiaGioiHanhChinh")]
        public IActionResult InsertDiaGioiHanhChinh(DanhMucDiaGioiHanhChinhModelPartial DanhMucDiaGioiHanhChinhModelPartial)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_ThemDGHC, EnumLogType.Insert, () =>
                 {
                     if (string.IsNullOrEmpty(DanhMucDiaGioiHanhChinhModelPartial.TenDayDu) || DanhMucDiaGioiHanhChinhModelPartial.TenDayDu.Trim().Length <= 0)
                     {
                         base.Message = ConstantLogMessage.API_Error_NotFill;
                         base.Status = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     if (string.IsNullOrEmpty(DanhMucDiaGioiHanhChinhModelPartial.Ten) || DanhMucDiaGioiHanhChinhModelPartial.Ten.Trim().Length <= 0)
                     {
                         base.Message = ConstantLogMessage.API_Error_NotFill;
                         base.Status = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     if (!Utils.CheckSpecialCharacter(DanhMucDiaGioiHanhChinhModelPartial.Ten))
                     {
                         base.Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                         base.Status = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     Dictionary<int, int> dic = new Dictionary<int, int>();
                     int ID = 0;
                     var crInsert = new DanhMucDiaGioiHanhChinhModel();
                     crInsert.TenTinh = DanhMucDiaGioiHanhChinhModelPartial.Ten;
                     crInsert.TenHuyen = DanhMucDiaGioiHanhChinhModelPartial.Ten;
                     crInsert.TenXa = DanhMucDiaGioiHanhChinhModelPartial.Ten;
                     crInsert.TenDayDu = DanhMucDiaGioiHanhChinhModelPartial.TenDayDu;

                     if (DanhMucDiaGioiHanhChinhModelPartial.Cap == 1)
                     {
                         dic = _DanhMucDiaGioiHanhChinhBUS.InsertTinh(crInsert, ref ID);
                         //var id = dic.FirstOrDefault().Value;
                     }
                     else if (DanhMucDiaGioiHanhChinhModelPartial.Cap == 2)
                     {
                         crInsert.TinhID = DanhMucDiaGioiHanhChinhModelPartial.ParentID.Value;
                         dic = _DanhMucDiaGioiHanhChinhBUS.InsertHuyen(crInsert, ref ID);
                     }
                     else
                     {
                         crInsert.HuyenID = DanhMucDiaGioiHanhChinhModelPartial.ParentID.Value;
                         dic = _DanhMucDiaGioiHanhChinhBUS.InsertXa(crInsert, ref ID);
                     }
                     if (dic.FirstOrDefault().Key == -1) { base.Message = ConstantLogMessage.API_Error_System; }
                     else if (dic.FirstOrDefault().Key == 0) { base.Message = ConstantLogMessage.Alert_Error_NotExist("Địa giới hành chính"); }
                     else if (dic.FirstOrDefault().Key == 2) { base.Message = ConstantLogMessage.Alert_Error_NotExist("Parent ID"); }
                     else if (dic.FirstOrDefault().Key == 1) { base.Message = ConstantLogMessage.Alert_Insert_Success("Địa giới hành chính"); }
                     base.Data = dic.FirstOrDefault().Value;
                     base.Status = dic.FirstOrDefault().Key;
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Delete)]
        [Route("DeleteDiaGioiHanhChinh")]
        public IActionResult DeleteDiaGioiHanhChinh(int ID, int Cap)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_XoaDGHC, EnumLogType.Delete, () =>
                 {

                     Dictionary<int, string> Result = new Dictionary<int, string>();
                     var crDelete = new DanhMucDiaGioiHanhChinhModel();
                     crDelete.TinhID = ID;
                     crDelete.HuyenID = ID;
                     crDelete.XaID = ID;

                     if (Cap == 1)
                     {
                         Result = _DanhMucDiaGioiHanhChinhBUS.DeleteTinh(crDelete.TinhID.Value);
                     }
                     else if (Cap == 2)
                     {
                         Result = _DanhMucDiaGioiHanhChinhBUS.DeleteHuyen(crDelete.HuyenID.Value);
                     }
                     else
                     {
                         Result = _DanhMucDiaGioiHanhChinhBUS.DeleteXa(crDelete.XaID.Value);
                     }
                     //if (Result.FirstOrDefault().Key == -1) { base.Message = "Lỗi Hệ Thống !"; }
                     //else if (Result.FirstOrDefault().Key == 0) { base.Message = "Chưa xóa được địa giới !"; }
                     //else if (Result.FirstOrDefault().Key == 1) { base.Message = "Xóa thành công!"; }
                     base.Message = Result.FirstOrDefault().Value.ToString();
                     base.Status = Utils.ConvertToInt32(Result.FirstOrDefault().Key, 0);
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
        //[CustomAuthAttribute(ChucNangEnum.DanhMuc_DiaGioiHanhChinh, AccessLevel.Edit)]
        [Route("UpdateDiaGioiHanhChinh")]
        public IActionResult UpdateDiaGioiHanhChinh(DanhMucDiaGioiHanhChinhModelPartial DanhMucDiaGioiHanhChinhModelPartial)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_DiaGioiHanhChinh_SuaDGHC, EnumLogType.Update, () =>
                 {
                     if (DanhMucDiaGioiHanhChinhModelPartial.ID == 0)
                     {
                         base.Message = "Chưa có đối tượng được chọn!";
                         base.Status = 0;
                         return base.GetActionResult();
                     }
                     if (string.IsNullOrEmpty(DanhMucDiaGioiHanhChinhModelPartial.TenDayDu) || DanhMucDiaGioiHanhChinhModelPartial.TenDayDu.Trim().Length <= 0)
                     {
                         base.Message = ConstantLogMessage.API_Error_NotFill;
                         base.Status = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     if (string.IsNullOrEmpty(DanhMucDiaGioiHanhChinhModelPartial.Ten) || DanhMucDiaGioiHanhChinhModelPartial.Ten.Trim().Length <= 0)
                     {
                         base.Message = ConstantLogMessage.API_Error_NotFill;
                         base.Status = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     if (!Utils.CheckSpecialCharacter(DanhMucDiaGioiHanhChinhModelPartial.Ten))
                     {
                         base.Message = ConstantLogMessage.API_Error_NotSpecialCharacter;
                         base.Status = 0;
                         base.Data = Data;
                         return base.GetActionResult();
                     }
                     int val = 0;
                     var crUpdate = new DanhMucDiaGioiHanhChinhModel();
                     crUpdate.TinhID = DanhMucDiaGioiHanhChinhModelPartial.ID;
                     crUpdate.HuyenID = DanhMucDiaGioiHanhChinhModelPartial.ID;
                     crUpdate.XaID = DanhMucDiaGioiHanhChinhModelPartial.ID;
                     crUpdate.TenTinh = DanhMucDiaGioiHanhChinhModelPartial.Ten;
                     crUpdate.TenHuyen = DanhMucDiaGioiHanhChinhModelPartial.Ten;
                     crUpdate.TenXa = DanhMucDiaGioiHanhChinhModelPartial.Ten;
                     crUpdate.TenDayDu = DanhMucDiaGioiHanhChinhModelPartial.TenDayDu;

                     if (DanhMucDiaGioiHanhChinhModelPartial.Cap == 1)
                     {
                         val = _DanhMucDiaGioiHanhChinhBUS.UpdateTinh(crUpdate);
                     }
                     else if (DanhMucDiaGioiHanhChinhModelPartial.Cap == 2)
                     {
                         crUpdate.TinhID = DanhMucDiaGioiHanhChinhModelPartial.ParentID.Value;
                         val = _DanhMucDiaGioiHanhChinhBUS.UpdateHuyen(crUpdate);
                     }
                     else
                     {
                         crUpdate.HuyenID = DanhMucDiaGioiHanhChinhModelPartial.ParentID.Value;
                         val = _DanhMucDiaGioiHanhChinhBUS.UpdateXa(crUpdate);
                     }
                     if (val == -1) { base.Message = ConstantLogMessage.API_Error_System; }
                     else if (val == 0) { base.Message = ConstantLogMessage.API_Error_Exist; }
                     else if (val == 2) { base.Message = ConstantLogMessage.API_Error_NotExist; }
                     else if (val == 1) { base.Message = ConstantLogMessage.API_Success; }
                     base.Status = val;
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
    }
}

