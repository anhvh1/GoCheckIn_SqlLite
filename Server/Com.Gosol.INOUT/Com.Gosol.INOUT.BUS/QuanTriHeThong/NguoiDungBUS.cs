using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.BUS.QuanTriHeThong
{
    public interface INguoiDungBUS
    {
        //NguoiDungModel GetInfoByLogin(string UserName,string Password);
        bool VerifyUser(string UserName, string Password, ref NguoiDungModel NguoiDung);
    }
    public class NguoiDungBUS : INguoiDungBUS
    {
        private INguoiDungDAL _NguoiDungDAL;
        public NguoiDungBUS(INguoiDungDAL NguoiDungDAL)
        {
            _NguoiDungDAL = NguoiDungDAL;
        }

        private NguoiDungModel GetInfoByLogin(string UserName, string Password)
        {
            return _NguoiDungDAL.GetInfoByLogin(UserName, Password);
        }

        public bool VerifyUser(string UserName, string Password, ref NguoiDungModel NguoiDung)
        {
            NguoiDung = GetInfoByLogin(UserName, Password);
            if (NguoiDung != null && NguoiDung.TrangThai == 1)
            {
                return true;
            }
            return false;
        }
    }
}
