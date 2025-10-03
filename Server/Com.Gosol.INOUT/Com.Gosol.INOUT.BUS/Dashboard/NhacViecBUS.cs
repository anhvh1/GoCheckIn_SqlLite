using Com.Gosol.INOUT.DAL.Dashboard;
using Com.Gosol.INOUT.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.KKTS.BUS.Dashboard
{
    public interface INhacViecBUS
    {
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID, INOUT.Models.BasePagingParams p, ref int TotalRow);
        public int InsertNotify(int? CanBoDangNhap, int? LoaiThongBao, List<int> ListCanBoNhanThongBao, string ThongBaoChoChinhBan, string ThongBaoChoNguoiKhac,int? ChinhBanHayNguoiKhac);
        public int UpdateNotify(INOUT.Models.Dashboard.NhacViecModel.NhacViecPar nhacViecPar);
        void UpdateLog(int? LogID);
    }
    public class NhacViecBUS : INhacViecBUS
    {
        private INhacViecDAL _INhacViecDAL;

        public NhacViecBUS(INhacViecDAL nhacViecDAL)
        {
            this._INhacViecDAL = nhacViecDAL;
        }
        public List<NhacViecModel> GetViecLam(int? NguoiDungID, int? CanBoID, int CoQuanID, INOUT.Models.BasePagingParams p, ref int TotalRow)
        {
            return _INhacViecDAL.GetViecLam(NguoiDungID,CanBoID, CoQuanID, p, ref  TotalRow);
        }
        public int InsertNotify(int? CanBoDangNhap, int? LoaiThongBao, List<int> ListCanBoNhanThongBao, string ThongBaoChoChinhBan, string ThongBaoChoNguoiKhac, int? ChinhBanHayNguoiKhac)
        {
            return 0;
            //return _INhacViecDAL.InsertNotify(CanBoDangNhap,  LoaiThongBao, ListCanBoNhanThongBao,  ThongBaoChoChinhBan,  ThongBaoChoNguoiKhac, ChinhBanHayNguoiKhac);
        }
        public int UpdateNotify(INOUT.Models.Dashboard.NhacViecModel.NhacViecPar nhacViecPar)
        {
            return _INhacViecDAL.UpdateNotify(nhacViecPar.ThongBaoID, nhacViecPar.LoaiUpdate, nhacViecPar.CanBoNhanThongBao);
        }
        public void UpdateLog(int? LogID)
        {
             _INhacViecDAL.UpdateLog(LogID);
        }
    }
}
