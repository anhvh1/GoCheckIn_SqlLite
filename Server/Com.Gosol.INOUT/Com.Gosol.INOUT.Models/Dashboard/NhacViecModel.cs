using Com.Gosol.INOUT.Models.QuanTriHeThong;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models.Dashboard
{
    public class NhacViecModel : HeThongCanBoModel
    {
        public string Name { get; set; }

        public string Key { get; set; }
        public int CanBoNhanThongBao { get; set; }
        public int TrangThaiXem { get; set; }
        public NhacViecModel()
        {

        }
        public NhacViecModel(string Name, string Key)
        {
            this.Name = Name;
            this.Key = Key;
        }
        public class NhacViecPar {
            public int? ThongBaoID { get; set; }
            public int? LoaiUpdate { get; set; }  // 1- Update một  , 2-  Update all
            public int? CanBoNhanThongBao { get; set; }
        }
    }
}
