using System;
using System.Web;
namespace Com.Gosol.INOUT.Security
{
   
    public enum AccessLevel
    {
        Create = 2,
        Delete = 8,
        Edit = 4,
        FullAccess = 0x1f,
        NoAccess = 0,
        Publish = 0x10,
        Read = 1
    }
   
}
