using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models.DanhMuc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.BUS.DanhMuc
{
    public interface IDanhMucDiaGioiHanhChinhBUS
    {
        public Dictionary<int, int> InsertTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID);
        public int UpdateTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel);
        public Dictionary<int, string> DeleteTinh(int TinhID);
        public Dictionary<int, int> InsertHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID);
        public int UpdateHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel);
        public Dictionary<int, string> DeleteHuyen(int HuyenID);
        public Dictionary<int, int> InsertXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID);
        public int UpdateXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel);
        public Dictionary<int, string> DeleteXa(int XaID);
        public List<DanhMucDiaGioiHanhChinhModel> GetListByidAndCap();
        public List<DanhMucDiaGioiHanhChinhModel> FilterByName(string FilterName);
        //public DanhMucDiaGioiHanhChinhModel GetByID(int id);
        public List<object> GetAllByCap(int ID, int Cap, string Keyword);
        public DanhMucDiaGioiHanhChinhModelUpdatePartial GetDGHCByIDAndCap(int id, int Cap, string Keyword);

    }
    public class DanhMucDiaGioiHanhChinhBUS : IDanhMucDiaGioiHanhChinhBUS
    {

        public Dictionary<int, int> InsertTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel, ref int ID)
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            try
            {
                dic = new DanhMucDiaGioiHanhChinhDAL().InsertTinh(DanhMucDiaGioiHanhChinhModel, ref ID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        public int UpdateTinh(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel)
        {
            int val = 0;
            try
            {
                val = new DanhMucDiaGioiHanhChinhDAL().UpdateTinh(DanhMucDiaGioiHanhChinhModel);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        public Dictionary<int, string> DeleteTinh(int TinhID)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            try
            {
                dic = new DanhMucDiaGioiHanhChinhDAL().DeleteTinh(TinhID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        public Dictionary<int, int> InsertHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel,ref int ID)
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            try
            {
                dic = new DanhMucDiaGioiHanhChinhDAL().InsertHuyen(DanhMucDiaGioiHanhChinhModel,ref ID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        public int UpdateHuyen(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel)
        {
            int val = 0;
            try
            {
                val = new DanhMucDiaGioiHanhChinhDAL().UpdateHuyen(DanhMucDiaGioiHanhChinhModel);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        public Dictionary<int, string> DeleteHuyen(int HuyenID)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            try
            {
                dic = new DanhMucDiaGioiHanhChinhDAL().DeleteHuyen(HuyenID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }

        //CRUD Xa
        public Dictionary<int,int> InsertXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel,ref int ID)
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            try
            {
                dic = new DanhMucDiaGioiHanhChinhDAL().InsertXa(DanhMucDiaGioiHanhChinhModel,ref ID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        public int UpdateXa(DanhMucDiaGioiHanhChinhModel DanhMucDiaGioiHanhChinhModel)
        {
            int val = 0;
            try
            {
                val = new DanhMucDiaGioiHanhChinhDAL().UpdateXa(DanhMucDiaGioiHanhChinhModel);
                return val;
            }
            catch (Exception ex)
            {
                return val;
                throw ex;
            }
        }
        public Dictionary<int, string> DeleteXa(int XaID)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            try
            {
                dic = new DanhMucDiaGioiHanhChinhDAL().DeleteXa(XaID);
                return dic;
            }
            catch (Exception ex)
            {
                return dic;
                throw ex;
            }
        }
        public List<DanhMucDiaGioiHanhChinhModel> GetListByidAndCap()
        {
            List<DanhMucDiaGioiHanhChinhModel> list = new List<DanhMucDiaGioiHanhChinhModel>();
            try
            {
                return new DanhMucDiaGioiHanhChinhDAL().GetListByidAndCap();
            }
            catch (Exception ex)
            {
                return new List<DanhMucDiaGioiHanhChinhModel>();
                throw ex;
            }
        }
        //public List<DanhMucDiaGioiHanhChinhModel> GetByTinhID(int TinhID)
        //{
        //    var list = new List<DanhMucDiaGioiHanhChinhModel>();
        //    try
        //    {
        //        list = new DanhMucDiaGioiHanhChinhDAL().GetByTinhID(TinhID);
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<DanhMucDiaGioiHanhChinhModel>();
        //        throw ex;
        //    }
        //}
        //public List<DanhMucDiaGioiHanhChinhModel> GetByHuyenID(int HuyenID)
        //{
        //    var list = new List<DanhMucDiaGioiHanhChinhModel>();
        //    try
        //    {
        //        list = new DanhMucDiaGioiHanhChinhDAL().GetByHuyenID(HuyenID);
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<DanhMucDiaGioiHanhChinhModel>();
        //        throw ex;
        //    }
        //}
        //public List<DanhMucDiaGioiHanhChinhModel> GetByXaID(int XaID)
        //{
        //    var list = new List<DanhMucDiaGioiHanhChinhModel>();
        //    try
        //    {
        //        list = new DanhMucDiaGioiHanhChinhDAL().GetByXaID(XaID);
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<DanhMucDiaGioiHanhChinhModel>();
        //        throw ex;
        //    }
        //}
        public List<DanhMucDiaGioiHanhChinhModel> FilterByName(string FilterName)
        {
            var list = new List<DanhMucDiaGioiHanhChinhModel>();
            try
            {
                return new DanhMucDiaGioiHanhChinhDAL().FilterByName(FilterName);
            }
            catch (Exception ex)
            {
                return new List<DanhMucDiaGioiHanhChinhModel>();
                throw ex;
            }
        }
        //public DanhMucDiaGioiHanhChinhModel GetByID(int id)
        //{
        //    try
        //    {
        //        //return new DanhMucDiaGioiHanhChinhDAL().GetDGHCByID(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<object> GetAllByCap(int ID, int Cap, string Keyword)
        {
            return new DanhMucDiaGioiHanhChinhDAL().GetAllByCap(ID, Cap, Keyword);
        }
        public DanhMucDiaGioiHanhChinhModelUpdatePartial GetDGHCByIDAndCap(int id, int Cap, string Keyword)
        {
            return new DanhMucDiaGioiHanhChinhDAL().GetDGHCByIDAndCap(id, Cap, Keyword);
        }
    }
}
