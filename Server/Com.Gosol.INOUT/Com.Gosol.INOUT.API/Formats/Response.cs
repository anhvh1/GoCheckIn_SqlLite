using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Gosol.INOUT.API.Formats
{
   
    // RULES
    // If success, status code = 1, message is empty, data is not null
    // If error, status code = 0, message is error message, data is null

    public class Response<T> : Response
    {
        [JsonProperty("Data")]
        public T Data { get; private set; }

        public Response(int status, string message = null, T data = default(T))
            : base(status, message)
        {
            Data = data;
        }
    }

    public class Response
    {
        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        public Response() { }

        public Response(int status, string message = null)
        {
            Status = status;
            Message = message;
        }
    }
    

}
