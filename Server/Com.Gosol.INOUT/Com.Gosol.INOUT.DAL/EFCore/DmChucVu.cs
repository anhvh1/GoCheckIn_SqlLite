using System;
using System.Collections.Generic;

namespace Com.Gosol.INOUT.DAL.EFCore
{
    public partial class DanhMucChucVu
    {
        public DanhMucChucVu()
        {
            DanhMucCanBo = new HashSet<DanhMucCanBo>();
        }

        public long ChucVuId { get; set; }
        public string TenChucVu { get; set; }
        public string GhiChu { get; set; }

        public virtual ICollection<DanhMucCanBo> DanhMucCanBo { get; set; }
    }
}
