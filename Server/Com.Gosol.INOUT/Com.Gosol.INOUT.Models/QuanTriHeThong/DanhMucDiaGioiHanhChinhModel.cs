using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models.DanhMuc
{
    public class DanhMucDiaGioiHanhChinhModel
    {
        public int? TinhID { get; set; }
        public int? HuyenID { get; set; }
        public int? XaID { get; set; }
        public string TenTinh { get; set; }
        public string TenHuyen { get; set; }
        public string TenXa { get; set; }
        public string TenDayDu { get; set; }
        public string MappingCode { get; set; }

        public DanhMucDiaGioiHanhChinhModel() { }
        public DanhMucDiaGioiHanhChinhModel(int TinhID, int HuyenID, int XaID, string tenTinh, string tenHuyen, string tenXa, string MappingCode)
        {
            this.TinhID = TinhID;
            this.HuyenID = HuyenID;
            this.XaID = XaID;
            this.TenTinh = tenTinh;
            this.TenHuyen = tenHuyen;
            this.TenXa = tenXa;
            this.MappingCode = MappingCode;


        }
        //public DiaGioiHanhChinh(int TinhID, string tenTinh, string tenDayDu, string MappingCode)
        //{
        //    TinhID = TinhID;
        //    TenTinh = tenTinh;
        //    TenDayDu = tenDayDu;
        //    MappingCode = MappingCode;
        //}
        //public DiaGioiHanhChinh(int HuyenID,int TinhID, string tenHuyen, string tenDayDu, string MappingCode)
        //{
        //    HuyenID = HuyenID;
        //    TinhID = TinhID;
        //    TenHuyen = tenHuyen;
        //    TenDayDu = tenDayDu;
        //    MappingCode = MappingCode;
        //}
        //public DiaGioiHanhChinh(int XaID,int HuyenID, string tenXa, string tenDayDu, string MappingCode)
        //{
        //    XaID = XaID;
        //    HuyenID = HuyenID;
        //    TenXa = tenXa;
        //    TenDayDu = tenDayDu;
        //    MappingCode = MappingCode;
        //}


    }

    public class DanhMucDiaGioiHanhChinhModelPartial
    {
        public int ID { get; set; }
        public string Ten { get; set; }
        public string TenDayDu { get; set; }
        public int? TotalChildren { get; set; }
        public int? ParentID { get; set; }
        public int? Cap { get; set; }
        public int? Highlight { get; set; }

        public DanhMucDiaGioiHanhChinhModelPartial()
        {

        }
        public DanhMucDiaGioiHanhChinhModelPartial(int ID, string Ten, string TenDayDu, int TotalChildren, int Cap, int Highlight)
        {
            this.ID = ID;
            this.Ten = Ten;
            this.TenDayDu = TenDayDu;
            this.TotalChildren = TotalChildren;
            this.Cap = Cap;
            this.Highlight = Highlight;
        }
    }
    public class DanhMucDiaGioiHanhChinhModelUpdatePartial
    {
        public int ID { get; set; }
        public string Ten { get; set; }
        public string TenDayDu { get; set; }
        public int? TotalChildren { get; set; }
        public int? TinhID { get; set; }
        public int? HuyenID { get; set; }
        public int? Cap { get; set; }
        public int? Highlight { get; set; }

        public DanhMucDiaGioiHanhChinhModelUpdatePartial()
        {

        }
        public DanhMucDiaGioiHanhChinhModelUpdatePartial(int ID, string Ten, string TenDayDu,int TinhID, int HuyenID,int Cap,int TotalChildren,int Highlight)
        {
            this.ID = ID;
            this.Ten = Ten;
            this.TenDayDu = TenDayDu;
            this.TotalChildren = TotalChildren;
            this.HuyenID = HuyenID;
            this.TinhID = TinhID;
            this.Cap = Cap;
            this.Highlight = Highlight;
        }
    }
}
