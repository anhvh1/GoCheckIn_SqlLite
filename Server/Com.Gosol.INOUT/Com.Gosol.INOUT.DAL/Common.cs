using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.DAL
{
    public  static class CommonDAL
    {
        public  static string getTenLoaiDotKeKhai(int LoaiDotKeKhaiID)
        {
            var Result = "";
            if (LoaiDotKeKhaiID == 1) Result = "Kê khai hàng năm";
            else if (LoaiDotKeKhaiID == 2) Result = "Kê khai bổ sung";
            else if (LoaiDotKeKhaiID == 3) Result = "Kê khai phục vụ công tác cán bộ";
            else Result = "Kê khai lần đầu";
            return Result;

        }
    }
}
