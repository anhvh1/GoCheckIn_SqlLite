using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models
{
    public class NhacViecModel
    {
        public string Name { get; set; }

        public string Key { get; set; }
        public NhacViecModel()
        {

        }
        public NhacViecModel(string Name, string Key)
        {
            this.Name = Name;
            this.Key = Key;
        }
        public class NotifyModel
        {
            public int ThongBaoID { get; set; }
            public string ThongBao { get; set; }
            public int? CanBoNhanThongBao { get; set; }
            public bool? TrangThaiXem { get; set; }
            public int? CanBoTaoThongBao { get; set; }
            public string MaChucNang { get; set; }
        }
    }
}
