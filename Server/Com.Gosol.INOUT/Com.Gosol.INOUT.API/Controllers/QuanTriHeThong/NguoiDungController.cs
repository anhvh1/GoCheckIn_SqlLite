using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.Models.QuanTriHeThong;
using Com.Gosol.INOUT.BUS.QuanTriHeThong;
using Microsoft.Extensions.Options;
using Com.Gosol.INOUT.API.Config;
using Com.Gosol.INOUT.Ultilities;
using System.Security.Claims;
using Com.Gosol.INOUT.API.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Com.Gosol.INOUT.Security;
using Microsoft.Extensions.Logging;
using Com.Gosol.INOUT.Ultilities;
using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.Models.DanhMuc;

namespace Com.Gosol.INOUT.API.Controllers.QuanTriHeThong
{
    [Route("api/v1/Nguoidung")]
    [ApiController]
    public class NguoidungController : ControllerBase
    {
        private IOptions<AppSettings> _AppSettings;
        private INguoiDungBUS _NguoiDungBUS;
        private IPhanQuyenBUS _PhanQuyenBUS;
        private ILogHelper _ILogHelper;
        private ILogger logger;
        public NguoidungController(IOptions<AppSettings> Settings, INguoiDungBUS NguoiDungBUS, IPhanQuyenBUS PhanQuyenBUS, ILogHelper LogHelper, ILogger<NguoidungController> logger)
        {
            _AppSettings = Settings;
            _NguoiDungBUS = NguoiDungBUS;
            _PhanQuyenBUS = PhanQuyenBUS;
            _ILogHelper = LogHelper;
            this.logger = logger;
        }


        /// <summary>
        /// login
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        [Route("DangNhap")]
        [HttpPost]
        public IActionResult Login(LoginModel User)
        {
            try
            {
                string Password = Cryptor.EncryptPasswordUser(User.UserName.Trim().ToLower(), User.Password);
                NguoiDungModel NguoiDung = null;
                if (_NguoiDungBUS.VerifyUser(User.UserName.Trim(), Password, ref NguoiDung))
                {
                    Task.Run(() => _ILogHelper.Log(NguoiDung.CanBoID, "Đăng nhập hệ thống", (int)EnumLogType.DangNhap));
                    var claims = new List<Claim>();
                    var ListChucNang = _PhanQuyenBUS.GetListChucNangByNguoiDungID(NguoiDung.NguoiDungID);
                    //string ClaimFull = "," + string.Join(",", ListChucNang.Where(t => t.Quyen == (int)AccessLevel.FullAccess).Select(t => new { t.ChucNangID }).ToList()) + ",";

                    string ClaimRead = "," + string.Join(",", ListChucNang.Where(t => t.Xem == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimCreate = "," + string.Join(",", ListChucNang.Where(t => t.Them == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimEdit = "," + string.Join(",", ListChucNang.Where(t => t.Sua == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimDelete = "," + string.Join(",", ListChucNang.Where(t => t.Xoa == 1).Select(t => t.ChucNangID).ToArray()) + ",";
                    string ClaimFullAccess = "," + string.Join(",", ListChucNang.Where(t => t.Xem == 1 && t.Them == 1 && t.Sua == 1 && t.Xoa == 1).Select(t => t.ChucNangID).ToArray()) + ",";

                    //claims.Add(new Claim(PermissionLevel.FULLACCESS, ClaimFull));
                    claims.Add(new Claim(PermissionLevel.READ, ClaimRead));
                    claims.Add(new Claim(PermissionLevel.CREATE, ClaimCreate));
                    claims.Add(new Claim(PermissionLevel.EDIT, ClaimEdit));
                    claims.Add(new Claim(PermissionLevel.DELETE, ClaimDelete));
                    claims.Add(new Claim(PermissionLevel.FULLACCESS, ClaimFullAccess));

                    claims.Add(new Claim("CanBoID", NguoiDung?.CanBoID.ToString()));
                    claims.Add(new Claim("NguoiDungID", NguoiDung?.NguoiDungID.ToString()));
                    claims.Add(new Claim("CoQuanID", NguoiDung?.CoQuanID.ToString()));
                    claims.Add(new Claim("CapCoQuan", NguoiDung?.CapCoQuan.ToString()));
                    claims.Add(new Claim("VaiTro", NguoiDung?.VaiTro.ToString()));
                    claims.Add(new Claim("expires_at", Utils.ConvertToDateTime(DateTime.UtcNow.AddDays(_AppSettings.Value.NumberDateExpire).ToString(), DateTime.Now.Date).ToString()));
                    claims.Add(new Claim("TenCanBo", NguoiDung?.TenCanBo.ToString()));
                    claims.Add(new Claim("QuanLyThanNhan", NguoiDung?.QuanLyThanNhan.ToString()));
                    claims.Add(new Claim("CoQuanSuDungPhanMem", NguoiDung?.CoQuanSuDungPhanMem.ToString()));
                    //claims.Add(new Claim("TenCoQuanSuDungPhanMem", NguoiDung?.TenCoQuanSuDungPhanMem.ToString()));
                    claims.Add(new Claim("TinhID", NguoiDung?.TinhID.ToString()));
                    claims.Add(new Claim("RoleID", NguoiDung?.RoleID.ToString()));
                    //claims.Add(new Claim("expires_at", new DateTime(2020,01,07,13,45,00).ToString()));
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_AppSettings.Value.AudienceSecret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddDays(_AppSettings.Value.NumberDateExpire),
                        //new DateTime(2020, 01, 07, 13, 45, 00),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        //,Issuer = _AppSettings.Value.ApiUrl
                        //, Audience = _AppSettings.Value.AudienceSecret

                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    NguoiDung.Token = tokenHandler.WriteToken(token);
                    NguoiDung.expires_at = DateTime.UtcNow.AddDays(_AppSettings.Value.NumberDateExpire);
                    DanhMucCoQuanDonViPartialNew CoQuanInfo = new DanhMucCoQuanDonViDAL().GetByID(NguoiDung.CoQuanID);

                    if ((NguoiDung.CoQuanSuDungPhanMem == null || NguoiDung.CoQuanSuDungPhanMem == 0) && NguoiDung.NguoiDungID != 1)
                    {
                        return Ok(new
                        {
                            Status = -1,
                            Message = Constant.NOT_EXPIRED
                        });
                    }

                    //if (UserRole.CheckAdmin(NguoiDung.NguoiDungID))
                    //{
                    //    ListChucNang = ListChucNang.Where(x => x.MaChucNang != "checkin-out" && x.MaChucNang != "bao-cao").ToList();
                    //}
                    //tokenDescriptor.Expires;
                    return Ok(new
                    {
                        Status = 1,
                        User = NguoiDung,
                        ListRole = ListChucNang
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Status = -1,
                        Message = Constant.NOT_ACCOUNT
                    });
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message, "Đăng nhập hệ thống");
                throw;
            }


        }
    }
}