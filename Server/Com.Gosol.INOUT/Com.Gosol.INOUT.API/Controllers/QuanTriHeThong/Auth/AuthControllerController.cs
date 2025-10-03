using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Microsoft.Extensions.Configuration;


namespace Com.Gosol.INOUT.API.Controllers.QuanTriHeThong
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthControllerController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

    }
}