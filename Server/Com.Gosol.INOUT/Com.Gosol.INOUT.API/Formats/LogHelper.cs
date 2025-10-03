using Com.Gosol.INOUT.BUS.QuanTriHeThong;
using Com.Gosol.INOUT.DAL.Dashboard;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Com.Gosol.INOUT.API.Formats
{
    public interface ILogHelper
    {
        void Log(int CanBoID, String logIngo, int logType);
        void Log(int CanBoID, String logInfo, int logType, DateTime logTime);
        void Log_ThaoTao(int CanBoID, String logInfo,int? Action, int? SystemLogType, int? NhiemVuID);
        void Log_ThaoTao_ThongBao(int CanBoID, String logInfo, int? Action, int? SystemLogType, int? NhiemVuID,ref int LogID);
        void UpdateLog(int? LogID);
        ClaimsPrincipal getCurrentUser();
    }
    public class LogHelper : ILogHelper
    {
        private ISystemLogBUS _SystemLogBUS;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public LogHelper(ISystemLogBUS SystemLogBUS, IHttpContextAccessor HttpContextAcess)
        {
            _SystemLogBUS = SystemLogBUS;
            _HttpContextAccessor = HttpContextAcess;
        }
        public void Log(int CanBoID, String logInfo, int logType)
        {
            SystemLogModel systemLogInfo = new SystemLogModel();
            systemLogInfo.CanBoID = CanBoID;
            systemLogInfo.LogInfo = logInfo;
            systemLogInfo.LogTime = DateTime.Now;
            systemLogInfo.LogType = logType;

            try
            {
                _SystemLogBUS.Insert(systemLogInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  /// dùng để gọi và lưu log của người sử dụng trên các nhiệm vụ, công việc
        /// </summary>
        /// <param name="CanBoID"></param>
        /// <param name="logInfo"></param>
        /// <param name="SystemLogType"></param>
        /// <param name="NhiemVuID"></param>
        public void Log_ThaoTao(int CanBoID, String logInfo,int? Action, int? SystemLogType, int? NhiemVuID)
        {
            SystemLogModel systemLogInfo = new SystemLogModel();
            systemLogInfo.CanBoID = CanBoID;
            systemLogInfo.LogInfo = logInfo;
            systemLogInfo.LogTime = DateTime.Now;
            systemLogInfo.SystemLogType = SystemLogType;
            systemLogInfo.NhiemVuID = NhiemVuID;
            systemLogInfo.LogType = Action;

            try
            {
                _SystemLogBUS.Insert(systemLogInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Log_ThaoTao_ThongBao(int CanBoID, String logInfo, int? Action, int? SystemLogType, int? NhiemVuID,ref int LogID)
        {
            SystemLogModel systemLogInfo = new SystemLogModel();
            systemLogInfo.CanBoID = CanBoID;
            systemLogInfo.LogInfo = logInfo;
            systemLogInfo.LogTime = DateTime.Now;
            systemLogInfo.SystemLogType = SystemLogType;
            systemLogInfo.NhiemVuID = NhiemVuID;
            systemLogInfo.LogType = Action;

            try
            {
                _SystemLogBUS.Insert_Notify(systemLogInfo,ref LogID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Log(int CanBoID, String logInfo, int logType, DateTime logTime)
        {

            SystemLogModel systemLogInfo = new SystemLogModel();
            systemLogInfo.CanBoID = CanBoID;
            systemLogInfo.LogInfo = logInfo;
            systemLogInfo.LogTime = logTime;
            systemLogInfo.LogType = logType;

            try
            {
                _SystemLogBUS.Insert(systemLogInfo);
            }
            catch
            {
            }
        }
        public void UpdateLog(int? LogID)
        {
            new NhacViecDAL().UpdateLog(LogID);
        }
        public ClaimsPrincipal getCurrentUser()
        {
            return _HttpContextAccessor.HttpContext.User;
        }
    }
}