using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Com.Gosol.INOUT.Models;
using Com.Gosol.INOUT.Models.InOut;
using System;

namespace websocket
{
    public class SocketHub : Hub
    {
        public async Task Scan(ThongTinVaoRaModel ThongTinKhach)
        {
            await Clients.Others.SendAsync("scan", ThongTinKhach);
        }

        public async Task CheckinReturn(ThongTinVaoRaModel ThongTinCheckin)
        {
            await Clients.Others.SendAsync("checkin", ThongTinCheckin);
        }
    }
}