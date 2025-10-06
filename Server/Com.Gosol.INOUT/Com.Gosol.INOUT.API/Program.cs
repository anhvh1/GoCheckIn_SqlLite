using Com.Gosol.INOUT.API;
using Com.Gosol.INOUT.API.Config;
using Com.Gosol.INOUT.API.Formats;
using Com.Gosol.INOUT.BUS;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.BUS.QuanTriHeThong;
using Com.Gosol.INOUT.DAL;
using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
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

// 🔹 Cấu hình appsettings
var configuration = builder.Configuration;
SQLHelper.appConnectionStrings = configuration.GetConnectionString("DefaultConnection");
SQLHelper.backupPath = configuration.GetConnectionString("BackupPath");
SQLHelper.dbName = configuration.GetConnectionString("DBName");
SQLHelper.connectionString = configuration.GetValue<string>("ConnectionStringDB:DefaultConnection");

// 🔹 Add Services
var services = builder.Services;

var connString = configuration["ConnectionStringDB:DefaultConnection"];
services.AddSingleton(new SQLiteHelper(connString));
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

// 🔹 Đăng ký Dependency Injection (ví dụ, copy từ Startup cũ)
services.AddScoped<ILogHelper, LogHelper>();
services.AddScoped<ISystemLogBUS, SystemLogBUS>();
services.AddScoped<ISystemLogDAL, SystemLogDAL>();
services.AddScoped<INguoiDungBUS, NguoiDungBUS>();
services.AddScoped<INguoiDungDAL, NguoiDungDAL>();
// ... (các service khác tương tự, copy từ Startup cũ)

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

app.Run();
