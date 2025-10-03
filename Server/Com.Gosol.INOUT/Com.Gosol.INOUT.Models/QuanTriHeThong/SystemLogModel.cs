using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models.QuanTriHeThong
{
    public class SystemLogModel
    {
        public int SystemLogid { get; set; }
        public int CanBoID { get; set; }
        public string LogInfo { get; set; }
        public DateTime LogTime { get; set; }
        public int? LogType { get; set; }
        public string TenCanBo { get; set; }
        public string NoiDung { get; set; }
        public int ID { get; set; }
        public int? SystemLogType { get; set; }
        public int? NhiemVuID { get; set; }
        public int Loai { get; set; }


    }
    public class SystemLogPartialModel : SystemLogModel
    {
        public string TenCoQuan { get; set; }
    }
}
