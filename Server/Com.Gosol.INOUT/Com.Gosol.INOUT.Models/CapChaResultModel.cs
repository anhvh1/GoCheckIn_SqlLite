using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Gosol.INOUT.Models
{
   public  class CaptChaResult
    {
        public string CaptchaCode { get; set; }
        public byte[] CaptchaByteData { get; set; }
        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
        public DateTime Timestamp { get; set; }    
    }
}
