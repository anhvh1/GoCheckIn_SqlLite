using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.Models.QuanTriHeThong
{
    public class QuanTriDuLieuModel
    {
        public string TenFile { get; set; }
        public QuanTriDuLieuModel(string tenFile) { this.TenFile = tenFile; }
    }

}
