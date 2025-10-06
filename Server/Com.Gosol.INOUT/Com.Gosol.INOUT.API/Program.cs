using Com.Gosol.INOUT.API;
using Com.Gosol.INOUT.API.Config;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.BUS.Dashboard;
using Com.Gosol.INOUT.BUS.QuanTriHeThong;
using Com.Gosol.INOUT.BUS.VaoRa;
using Com.Gosol.INOUT.BUS.VaoRaV2;
using Com.Gosol.INOUT.DAL;
using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.DAL.Dashboard;
using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.DAL.FileDinhKem;
using Com.Gosol.INOUT.DAL.InOut;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.DAL.VaoRaV2;
using Com.Gosol.INOUT.Ultilities;
using Com.Gosol.KKTS.BUS.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NLog.Web;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using websocket;

//var builder = WebApplication.CreateBuilder(args);
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Look for static files in "wwwroot-custom"
    WebRootPath = "Client"
});

// 🔹 NLog config
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

var basePath = AppContext.BaseDirectory;
var path = Path.Combine(AppContext.BaseDirectory, "App_Data", "queries.json");
Console.WriteLine($"[DEBUG] Full Query Path: {path}");
Console.WriteLine($"File exists? {File.Exists(path)}");

builder.Configuration
    .SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;
var connStr = configuration.GetValue<string>("ConnectionStringDB:DefaultConnection")
                           .Replace("{BasePath}", basePath);
SQLHelper.connectionString = connStr;
var queryFilePath = configuration.GetValue<string>("QueryFile:Path")
                                 .Replace("{BasePath}", basePath);

// Nếu bạn có service đọc file query, truyền path vào đó
builder.Services.AddSingleton(new SQLiteHelper(queryFilePath));
// 🔹 Cấu hình appsettings
SQLHelper.appConnectionStrings = configuration.GetConnectionString("DefaultConnection");
SQLHelper.backupPath = configuration.GetConnectionString("BackupPath");
SQLHelper.dbName = configuration.GetConnectionString("DBName");

// 🔹 Add Services
var services = builder.Services;
QueryManager.Load(configuration);

// Controllers + JSON
services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    })
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

// Session
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
});

// CORS
services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

services.AddCors(options => options.AddPolicy("myDomain", builder =>
{
    builder.WithOrigins("http://gocheckin.gosol.com.vn")
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// JWT
var appSettingsSection = configuration.GetSection("AppSettings");
services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.AudienceSecret);

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// HttpClient ignore SSL
services.AddHttpClient("HttpClientWithSSLUntrusted")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ClientCertificateOptions = ClientCertificateOption.Manual,
        ServerCertificateCustomValidationCallback = (req, cert, chain, errors) => true
    });

// SignalR
services.AddSignalR(conf =>
{
    conf.MaximumReceiveMessageSize = int.MaxValue;
});

// Cache & Session
services.AddDistributedMemoryCache();
services.AddSession();

services.AddScoped<INhacViecBUS, NhacViecBUS>();
services.AddScoped<INhacViecDAL, NhacViecDAL>();
services.AddScoped<IDashboardDAL, DashboardDAL>();
services.AddScoped<IDashboardBUS, DashboardBUS>();

// He thong
services.AddScoped<ILogHelper, LogHelper>();
services.AddScoped<ISystemLogBUS, SystemLogBUS>();
services.AddScoped<ISystemLogDAL, SystemLogDAL>();
services.AddScoped<INguoiDungBUS, NguoiDungBUS>();
services.AddScoped<INguoiDungDAL, NguoiDungDAL>();
services.AddScoped<IPhanQuyenBUS, PhanQuyenBUS>();
services.AddScoped<IPhanQuyenDAL, PhanQuyenDAL>();
services.AddScoped<IChucNangDAL, ChucNangDAL>();
services.AddScoped<ISystemConfigBUS, SystemConfigBUS>();
services.AddScoped<ISystemConfigDAL, SystemConfigDAL>();
services.AddScoped<IHeThongNguoidungBUS, HeThongNguoidungBUS>();
services.AddScoped<IHeThongNguoiDungDAL, HeThongNguoiDungDAL>();
services.AddScoped<IQuanTriDuLieuBUS, QuanTriDuLieuBUS>();
services.AddScoped<IQuanTriDuLieuDAL, QuanTriDuLieuDAL>();
services.AddScoped<IQuanTriDuLieuBUS, QuanTriDuLieuBUS>();
services.AddScoped<IQuanTriDuLieuDAL, QuanTriDuLieuDAL>();
services.AddScoped<IChucNangBUS, ChucNangBUS>();
services.AddScoped<IChucNangDAL, ChucNangDAL>();
services.AddScoped<IHuongDanSuDungBUS, HuongDanSuDungBUS>();
services.AddScoped<IHuongDanSuDungDAL, HuongDanSuDungDAL>();



//Danh muc
services.AddScoped<IDanhMucChucVuBUS, DanhMucChucVuBUS>();
services.AddScoped<IDanhMucChucVuDAL, DanhMucChucVuDAL>();
services.AddScoped<IHeThongCanBoBUS, HeThongCanBoBUS>();
services.AddScoped<IHeThongCanBoDAL, HeThongCanBoDAL>();
services.AddScoped<IDanhMucDiaGioiHanhChinhDAL, DanhMucDiaGioiHanhChinhDAL>();
services.AddScoped<IDanhMucDiaGioiHanhChinhBUS, DanhMucDiaGioiHanhChinhBUS>();
services.AddScoped<IDanhMucCoQuanDonViBUS, DanhMucCoQuanDonViBUS>();
services.AddScoped<IDanhMucCoQuanDonViBUS, DanhMucCoQuanDonViBUS>();
services.AddScoped<IDanhMucLoaiTaiSanBUS, DanhMucLoaiTaiSanBUS>();
services.AddScoped<IDanhMucLoaiTaiSanDAL, DanhMucLoaiTaiSanDAL>();
services.AddScoped<IDanhMucNhomTaiSanBUS, DanhMucNhomTaiSanBUS>();
services.AddScoped<IDanhMucNhomTaiSanDAL, DanhMucNhomTaiSanDAL>();
services.AddScoped<IDanhMucTrangThaiDAL, DanhMucTrangThaiDAL>();
services.AddScoped<IDanhMucTrangThaiBUS, DanhMucTrangThaiBUS>();

services.AddScoped<IDanhMucCoQuanDonViDAL, DanhMucCoQuanDonViDAL>();
services.AddScoped<IDanhMucCoQuanDonViBUS, DanhMucCoQuanDonViBUS>();


//////////////////////////////
///

services.AddScoped<IVaoRaBUS, VaoRaBUS>();
services.AddScoped<IVaoRaDAL, VaoRaDAL>();
services.AddScoped<IFileDinhKemBUS, FileDinhKemBUS>();
services.AddScoped<IFileDinhKemDAL, FileDinhKemDAL>();

///////////////////////////////////////////

//V2
services.AddScoped<IVaoRaV2BUS, VaoRaV2BUS>();
services.AddScoped<IVaoRaV2DAL, VaoRaV2DAL>();

services.AddScoped<IDanhMucCoQuanDonViV2DAL, DanhMucCoQuanDonViV2DAL>();
services.AddScoped<IDanhMucCoQuanDonViV2BUS, DanhMucCoQuanDonViV2BUS>();

services.AddScoped<IHeThongCanBoV2DAL, HeThongCanBoV2DAL>();
services.AddScoped<IHeThongCanBoV2BUS, HeThongCanBoV2BUS>();
///
//V4
services.AddScoped<IVaoRaV4BUS, VaoRaV4BUS>();
services.AddScoped<IVaoRaV4DAL, VaoRaV4DAL>();

services.AddScoped<IDanhMucCoQuanDonViV4DAL, DanhMucCoQuanDonViV4DAL>();
services.AddScoped<IDanhMucCoQuanDonViV4BUS, DanhMucCoQuanDonViV4BUS>();

var app = builder.Build();

// 🔹 Middleware Pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

app.UseDefaultFiles();  // ← tự chọn index.html nếu có
app.UseStaticFiles();

var imagePath = configuration["StaticFiles:ImagePath"];
if (!string.IsNullOrEmpty(imagePath) && Directory.Exists(imagePath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(imagePath),
        RequestPath = "/ImageGoCheckIn"
    });
}


app.UseCors("AllowOrigin");
app.UseSession();
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SocketHub>("/SocketHub", options =>
{
    options.Transports = HttpTransportType.WebSockets;
});
try
{
    var logFile = Path.Combine(AppContext.BaseDirectory, "service_log.txt");
    File.AppendAllText(logFile, $"[{DateTime.Now}] Service starting...\n");

    QueryManager.Load(configuration);
    File.AppendAllText(logFile, $"[{DateTime.Now}] QueryManager loaded successfully.\n");

    app.Run();
}
catch (Exception ex)
{
    File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "service_log.txt"),
        $"[{DateTime.Now}] ERROR: {ex}\n");
    throw;
}

app.Run();
