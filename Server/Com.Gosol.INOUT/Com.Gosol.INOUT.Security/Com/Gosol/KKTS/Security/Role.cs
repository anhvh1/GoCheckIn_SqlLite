using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Security
{
    //public enum Role
    //{
    //    Admin = 1,      // quản trị hệ thống - toàn quyền trong hệ thống
    //    LanhDao = 2,    // Lãnh đạo - có quyền xem, thêm, sửa, xóa các dữ liệu liên quan đến hệ thống, và nghiệp vụ của đơn vị mình và đơn vị cấp dưới
    //    NhanVien = 3,   // Chỉ được truy cập và thao tác trên những dữ liệu thuộc tài khoản đang được phân quyền
    //}
    public static class UserRole
    {
        public static bool CheckAdmin(int NguoiDungID)
        {
            if (NguoiDungID == 1)
            {
                return true;
            }
            else
                return false;
        }
    }
}
