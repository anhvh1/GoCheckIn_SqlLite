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

namespace Com.Gosol.INOUT.API.Controllers.QuanTriHeThong
{
    [Route("api/v1/PhanQuyen")]
    [ApiController]
    public class PhanQuyenController : BaseApiController
    {
        private IPhanQuyenBUS _PhanQuyenBUS;

        public PhanQuyenController(IPhanQuyenBUS PhanQuyenBUS, ILogHelper _LogHelper, ILogger<PhanQuyenController> logger) : base(_LogHelper, logger)
        {
            this._PhanQuyenBUS = PhanQuyenBUS;
        }

        [HttpGet]
        [Route("GetListPaging")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult GetListPaging([FromQuery] BasePagingParamsForFilter p)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_GetListPaging, EnumLogType.GetList, () =>
                 {
                     int TotalRow = 0;
                     IList<NhomNguoiDungModel> Data;
                     var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                     var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                     //Data = _PhanQuyenBUS.NhomNguoiDung_GetPagingBySearch(p, CoQuanID, NguoiDungID, ref TotalRow);
                     Data = _PhanQuyenBUS.NhomNguoiDung_GetAll(p, CoQuanID, NguoiDungID, ref TotalRow);
                     base.Status = 1;
                     base.TotalRow = TotalRow;
                     base.Data = Data;
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
        [Route("NhomNguoiDung_Insert")]
        // [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Create)]
        public IActionResult Insert(NhomNguoiDungModel NhomNguoiDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_Them, EnumLogType.Insert, () =>
                {
                    NhomNguoiDungModel.CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    NhomNguoiDungModel.NhomTongID = 0;
                    var Result = _PhanQuyenBUS.NhomNguoiDung_Insert(NhomNguoiDungModel, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));
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
        [Route("NhomNguoiDung_GetFoUpdate")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult NhomNguoiDung_GetFoUpdate(int NhomNguoiDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_GetByID, EnumLogType.GetList, () =>
                {
                    var result = _PhanQuyenBUS.NhomNguoiDung_GetForUpdate(NhomNguoiDungID,
                            Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0),
                            Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));
                    base.Status = result.Status;
                    base.Data = result.Data;
                    base.Message = result.Message;
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
        [Route("NhomNguoiDung_GetChiTietByNhomNguoiDungID")]
        // [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult NhomNguoiDung_GetChiTietByNhomNguoiDungID(int NhomNguoiDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_GetByID, EnumLogType.GetList, () =>
                {
                    var result = _PhanQuyenBUS.NhomNguoiDung_GetChiTietByID(NhomNguoiDungID,
                            Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0),
                            Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0)
                            );
                    base.Status = result.Status;
                    base.Data = result.Data;
                    base.Message = result.Message;
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
        [Route("NhomNguioDung_Delete")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Delete)]
        public IActionResult NhomNguioDung_Delete(DeleteModel NhomNguoiDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _PhanQuyenBUS.NhomNguoiDung_Delete(NhomNguoiDungID.NhomNguoiDungID.Value,
                        Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0),
                         Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0)
                        );
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
        [Route("NhomNguoiDung_Update")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Edit)]
        public IActionResult NhomNguoiDung_Update(NhomNguoiDungModel NhomNguoiDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_Sua, EnumLogType.Insert, () =>
                {
                    var Result = _PhanQuyenBUS.NhomNguoiDung_Update(NhomNguoiDungModel, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0));
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
        [Route("NguoiDung_NhomNguoiDung_Insert")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Create)]
        public IActionResult NguoiDung_NhomNguoiDung_Insert(NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NguoiDung_NhomNguoiDung_Them, EnumLogType.Insert, () =>
                {
                    var Result = _PhanQuyenBUS.NguoiDung_NhomNguoiDung_Insert(NguoiDungNhomNguoiDungModel);
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
        [Route("NguoiDung_NhomNguoiDung_Delete")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Delete)]
        public IActionResult NguoiDung_NhomNguoiDung_Delete(NguoiDungNhomNguoiDungModel NguoiDungNhomNguoiDungModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NguoiDung_NhomNguoiDung_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _PhanQuyenBUS.NguoiDung_NhomNguoiDung_Delete(NguoiDungNhomNguoiDungModel.NguoiDungID, NguoiDungNhomNguoiDungModel.NhomNguoiDungID);
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
        [Route("PhanQuyen_Insert")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Create)]
        public IActionResult PhanQuyen_Insert(PhanQuyenModel PhanQuyenModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_PhanQuyen_Them, EnumLogType.Insert, () =>
                {
                    var Result = _PhanQuyenBUS.PhanQuyen_Insert(PhanQuyenModel);
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
        [Route("PhanQuyen_InsertMult")]
        // [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Create)]
        public IActionResult PhanQuyen_InsertMult([FromBody] List<PhanQuyenModel> PhanQuyenModels)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_PhanQuyen_Them, EnumLogType.Insert, () =>
                {
                    var Result = _PhanQuyenBUS.PhanQuyen_InsertMulti(PhanQuyenModels);
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
        [Route("PhanQuyen_Delete")]
        // [CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Delete)]
        public IActionResult PhanQuyen_Delete(DeleteModel PhanQuyenID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_PhanQuyen_Xoa, EnumLogType.Delete, () =>
                {
                    var Result = _PhanQuyenBUS.PhanQuyen_Delete(PhanQuyenID.PhanQuyenID.Value);
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
        [Route("PhanQuyen_Update")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Create)]
        public IActionResult PhanQuyen_Update(List<PhanQuyenModel> PhanQuyenModel)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_PhanQuyen_Sua, EnumLogType.Insert, () =>
                {
                    var Result = _PhanQuyenBUS.PhanQuyen_UpdateMulti(PhanQuyenModel);
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
        [Route("PhanQuyen_GetDanhSachCoQuan")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult PhanQuyen_GetDanhMucCoQuan()
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    var result = _PhanQuyenBUS.DanhMucCoQuan_GetAllFoPhanQuyen(Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0));
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu Cơ quan" : "";
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
        [Route("PhanQuyen_GetDanhSachCanBo")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult PhanQuyen_GetAllCanBoByListCoQuanID(int NhomNguoiDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var result = _PhanQuyenBUS.HeThongCanBo_GetAllByListCoQuanID(NhomNguoiDungID, Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0), Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0));
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu cán bộ" : "";
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
        [Route("PhanQuyen_GetDanhSachNguoiDung")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult PhanQuyen_GetAllNguoiDungByListCoQuanID(int NhomNguoiDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.DanhMuc_CoQuanDonVi_GetForPhanQuyen, EnumLogType.GetList, () =>
                {
                    var result = _PhanQuyenBUS.HeThongNguoiDung_GetAllByListCoQuanID(NhomNguoiDungID);
                    base.Status = 1;
                    base.Data = result;
                    base.Message = result.Count < 1 ? "Không có dữ liệu người dùng" : "";
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



        /// <summary>
        /// Danh sách quyền được thao tác trong nhóm
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PhanQuyen_GetQuyenDuocThaoTacTrongNhom")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult PhanQuyen_GetQuyenDuocThaoTacTrongNhom(int NhomNguoiDungID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_GetByID, EnumLogType.GetList, () =>
                {
                    var NguoiDungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "NguoiDungID").Value, 0);
                    var CoQuanID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(c => c.Type == "CoQuanID").Value, 0);
                    if (UserRole.CheckAdmin(NguoiDungID))
                        Data = _PhanQuyenBUS.GetListChucNangByNguoiDungID(NguoiDungID);
                    else
                        Data = _PhanQuyenBUS.ChucNang_GetQuyenDuocThaoTacTrongNhom(NhomNguoiDungID, CoQuanID, NguoiDungID);
                    base.Status = 1;
                    //base.TotalRow = TotalRow;
                    base.Data = Data;
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


        /// <summary>
        ///  danh sách nhóm người dùng cơ quan hiện tại được phân
        /// </summary>
        /// <param name="CoQuanID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetListNNDByCoQuanID")]
        //[CustomAuthAttribute(ChucNangEnum.HeThong_QuanLy_PhanQuyen, AccessLevel.Read)]
        public IActionResult GetListNhomNguoiDung_ByCoQuanID(int? CoQuanID)
        {
            try
            {
                return CreateActionResult(ConstantLogMessage.HT_NhomNguoiDung_GetListPaging, EnumLogType.GetList, () =>
                {
                    int TotalRow = 0;
                    IList<NhomNguoiDungModel> Data;
                    Data = _PhanQuyenBUS.GetListNhomNguoiDung_ByCoQuanID(CoQuanID);
                    base.Status = 1;
                    base.TotalRow = TotalRow;
                    base.Data = Data;
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
    }
    public class DeleteModel
    {
        public int? NhomNguoiDungID { get; set; }
        public int? PhanQuyenID { get; set; }
    }

}