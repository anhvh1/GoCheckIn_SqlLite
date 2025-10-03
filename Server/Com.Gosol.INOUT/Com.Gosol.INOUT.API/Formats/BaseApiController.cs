using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.INOUT.API.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using Com.Gosol.KKTS.BUS.Dashboard;

namespace Com.Gosol.INOUT.API.Formats
{
    public class BaseApiController : ControllerBase
    {
        private string _Message;
        protected string Message { get { return string.IsNullOrEmpty(_Message) ? Constant.GetUserMessage(Status) : _Message; } set { _Message = value; } }
        protected object Data { get; set; }
        protected int Status { get; set; }
        protected int TotalRow { get; set; }
        protected int CanBoID { get; }
        protected int NguoidungID { get; }
        protected int CoQuanID { get; }

        private ILogHelper _LogHelper;
        private readonly ILogger _BugLogger;
        private INhacViecBUS _NhacViecBUS;
        public BaseApiController(ILogHelper logHelper)
        {
            _LogHelper = logHelper;
            //_BugLogger = this._BugLogger;
            ClaimsPrincipal User = _LogHelper.getCurrentUser();
            if (User.Claims.Any(t => t.Type == "CanBoID")) CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "CanBoID").Value, 0);
            else CanBoID = 0; // trường hợp không đăng nhập - test
            if (User.Claims.Any(t => t.Type == "NguoidungID")) NguoidungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "NguoidungID").Value, 0);
            var _BugLogger = this._BugLogger;
        }

        public BaseApiController(ILogHelper logHelper, ILogger bugLogger)
        {
            _LogHelper = logHelper;
            //_BugLogger = this._BugLogger;
            ClaimsPrincipal User = _LogHelper.getCurrentUser();
            if (User.Claims.Any(t => t.Type == "CanBoID")) CanBoID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "CanBoID").Value, 0);
            else CanBoID = 0; // trường hợp không đăng nhập - test
            if (User.Claims.Any(t => t.Type == "NguoidungID")) NguoidungID = Utils.ConvertToInt32(User.Claims.FirstOrDefault(t => t.Type == "NguoidungID").Value, 0);
            _BugLogger = bugLogger;
        }
        public BaseApiController(INhacViecBUS nhacViecBUS)
        {
            _NhacViecBUS = nhacViecBUS;
        }
        protected IActionResult GetActionResult()
        {
            return Ok(new
            {
                Status = Status,
                Message = Message,
                Data = Data,
                TotalRow = TotalRow,
            });
        }
        protected IActionResult GetActionResult(int status, string message, object data = null, int totalRow = 0)
        {
            if (data == null)
                return Ok(new
                {
                    Status = status,
                    Message = message,
                });
            else
                return Ok(new
                {
                    Status = status,
                    Message = message,
                    Data = data,
                    TotalRow = totalRow,
                });
        }
        protected IActionResult CreateActionResult(string LogString, EnumLogType Action, Func<IActionResult> funct)
        {
            try
            {
                WriteLog(LogString, (int)Action);
                return funct.Invoke();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, (int)EnumLogType.Error);
                if (_BugLogger != null)
                    _BugLogger.LogInformation(ex.Message, LogString);
                Status = -1;
                return Ok(new
                {
                    Status = -1,
                    Message = ex.Message,
                });
            }
        }
      

        /// <summary>
        /// dùng để gọi và lưu log của người sử dụng trên các nhiệm vụ, công việc
        /// </summary>
        /// <param name="LogString"></param>
        /// <param name="Action"></param>
        /// <param name="NhiemVuID"></param>
        /// <param name="funct"></param>
        /// <returns></returns>
        protected IActionResult CreateActionResult_Action(string LogString, EnumLogType Action, EnumSystemLogType SystemLogType, int? NhiemVuID, Func<IActionResult> funct)
        {
            try
            {
                WriteLog_ThaoTac(LogString, (int)Action, (int)SystemLogType, NhiemVuID);
                return funct.Invoke();
            }
            catch (Exception ex)
            {
                WriteLog_ThaoTac(ex.Message, (int)Action, (int)EnumSystemLogType.Error, NhiemVuID);
                if (_BugLogger != null)
                    _BugLogger.LogInformation(ex.Message, LogString);
                Status = -1;
                return Ok(new
                {
                    Status = -1,
                    Message = ex.Message,
                });
            }
        }
        /// <summary>
        /// dùng để lấy ra 
        /// </summary>
        /// <param name="LogString"></param>
        /// <param name="Action"></param>
        /// <param name="SystemLogType"></param>
        /// <param name="NhiemVuID"></param>
        /// <param name="LogID"></param>
        /// <param name="funct"></param>
        /// <returns></returns>
        protected IActionResult CreateActionResult_Action_Notify(string LogString, EnumLogType Action, EnumSystemLogType SystemLogType, int? NhiemVuID, ref int LogID,Func<IActionResult> funct)
        {
            try
            {
                WriteLog_ThaoTac_ThongBao(LogString, (int)Action, (int)SystemLogType, NhiemVuID,ref LogID);
                return funct.Invoke();
            }
            catch (Exception ex)
            {
                WriteLog_ThaoTac_ThongBao(ex.Message, (int)Action, (int)EnumSystemLogType.Error, NhiemVuID, ref LogID);
                if (_BugLogger != null)
                    _BugLogger.LogInformation(ex.Message, LogString);
                Status = -1;
                return Ok(new
                {
                    Status = -1,
                    Message = ex.Message,
                });
            }
        }

        protected void LogError(string LogString)
        {
            _BugLogger.LogInformation(LogString);
        }

        protected IActionResult CreateActionResultNew(int Status, string LogString, EnumLogType Action, Func<IActionResult> funct)
        {
            try
            {
                if (Status == 1)
                {
                    WriteLog(LogString, (int)Action);
                }
                return funct.Invoke();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, (int)EnumLogType.Error);
                Status = -1;
                return Ok(new
                {
                    Status = -1,
                    Message = ex.Message,
                });
            }
        }
        //protected IActionResult ExportFileResult(string LogString, Func<IActionResult> funct)
        //{

        //    try
        //    {
        //        WriteLog(LogString, (int)LogType.Action);
        //        return funct.Invoke();
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex.Message, (int)LogType.Error);
        //        return Ok(new
        //        {
        //            Status = -1,
        //            Message = ex.Message,
        //        });
        //    }
        //}

        protected async Task WriteLog(string message, int logType)
        {
            _LogHelper.Log(CanBoID, message, logType);
        }

        /// <summary>
        ///  /// dùng để gọi và lưu log của người sử dụng trên các nhiệm vụ, công việc
        /// </summary>
        /// <param name="message"></param>
        /// <param name="SystemLogType"></param>
        /// <param name="NhiemVuID"></param>
        /// <returns></returns>
        protected async Task WriteLog_ThaoTac(string message, int Action, int SystemLogType, int? NhiemVuID)
        {
            _LogHelper.Log_ThaoTao(CanBoID, message, Action, SystemLogType, NhiemVuID);
        }
        protected IActionResult Update_Log(int LogID)
        {
            Update_Log_No(LogID);
            return Ok(new
            {
                Status = 1
                //Message = ex.Message,
            });
        }
        protected  void WriteLog_ThaoTac_ThongBao(string message, int Action, int SystemLogType, int? NhiemVuID,ref int LogID)
        {
            _LogHelper.Log_ThaoTao_ThongBao(CanBoID, message, Action, SystemLogType, NhiemVuID,ref LogID);
        }
        //protected IActionResult Update_Log()
        //{
        //    //InsertNotify();
        //    return Ok(new
        //    {
        //        Status = 1
        //        //Message = ex.Message,
        //    });
        //}
        protected void Update_Log_No(int? LogID)
        {
            try
            {
                _LogHelper.UpdateLog(LogID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //_NhacViecBUS.UpdateLog(LogID);
        }

    }
}
