//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models
{
    public class BasePagingParams
    {
        public string Keyword { get; set; } = "";
        public string OrderByOption { get; set; } = "";
        public string OrderByName { get; set; } = "";
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public int Offset { get { return (PageSize == 0 ? 10 : PageSize) * ((PageNumber == 0 ? 1 : PageNumber) - 1); } }
        public int Limit { get { return (PageSize == 0 ? 10 : PageSize); } }
        /// <summary>
        /// format dd/MM/yyyy
        /// </summary> 
        public string TuNgay { get; set; }
        /// <summary>
        /// format dd/MM/yyyy
        /// </summary>
        public string DenNgay { get; set; }
        public int? NgayCheckIn  { get; set; }
        public DateTime StartDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(TuNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;

                }
            }
        }
        public DateTime EndDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(DenNgay, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;

                }
            }
        }
    }

    public class BasePagingParamsFilter : BasePagingParams
    {
        public int? CanBoGapID { get; set; }
        public int? LeTanID { get; set; }
        public int? DonViCaNhan { get; set; }
        public int? DonViSuDungID { get; set; }
        public int? TinhID { get; set; }

    }



    public class BasePagingParamsOffset
    {
        public string Keyword { get; set; }
        public string OrderByOption { get; set; }
        public string OrderByName { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Offset { get { return (PageSize == 0 ? 10 : PageSize) * ((PageNumber == 0 ? 1 : PageNumber) - 1); } }
        public int Limit { get { return (PageSize == 0 ? 10 : PageSize); } }

        /// <summary>
        /// format dd/MM/yyyy
        /// </summary> 
        public string TuNgayStr { private get; set; }
        /// <summary>
        /// format dd/MM/yyyy
        /// </summary>
        public string DenNgayStr { private get; set; }
        public DateTime StartDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(TuNgayStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
        public DateTime EndDate
        {
            get
            {
                try
                {
                    return DateTime.ParseExact(DenNgayStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }
    }
    public class BaseDeleteParams
    {
        public List<int> ListID { get; set; }
    }

    public class BasePagingParamsForFilter : BasePagingParams
    {
        public int? CoQuanID { get; set; }
        public int? CoQuanChaID { get; set; }
        public int? CanBoID { get; set; }
        public int? Nam { get; set; }
        public int? TrangThai { get; set; }
        public int? Loai { get; set; }
        public int? ApDungCho { get; set; }
        public int? GiaTri { get; set; }
        public int? NamKeKhai { get; set; }
        public int? ChucNangID { get; set; }
        public string NhomChucNang { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? NhiemVuID {get; set;}
        public int? DateFilterType { get; set;}

    }
    public class NewParams
    {
        public int CanBoID { get; set; }
        public List<int?> DanhSachNhomTaiSanID { get; set; }
    }

    public class ThongKeParams
    {
        public int? NamKeKhai { get; set; }
        public int? CoQuanID { get; set; }
        public int? DotKekhaiID { get; set; }
        public int? TrangThai { get; set; }
        public int? CapQuanLy { get; set; }
        public int? LoaiKeKhai { get; set; }

    }

}
