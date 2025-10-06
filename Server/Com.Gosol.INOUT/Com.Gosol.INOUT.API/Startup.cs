using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Gosol.INOUT.API.Config;
using Com.Gosol.INOUT.BUS.DanhMuc;
using Com.Gosol.INOUT.BUS.QuanTriHeThong;
using Com.Gosol.INOUT.DAL.DanhMuc;
using Com.Gosol.INOUT.DAL.EFCore;
using Com.Gosol.INOUT.DAL.QuanTriHeThong;
using Com.Gosol.INOUT.Ultilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Com.Gosol.INOUT.API.Formats;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Com.Gosol.INOUT.BUS;
using Com.Gosol.INOUT.DAL;
using Com.Gosol.KKTS.BUS.Dashboard;
using Com.Gosol.INOUT.DAL.Dashboard;
using Com.Gosol.INOUT.BUS.Dashboard;
using Com.Gosol.INOUT.BUS.VaoRa;
using Com.Gosol.INOUT.DAL.InOut;
using System.Net.Http;
using Com.Gosol.INOUT.DAL.FileDinhKem;
using Com.Gosol.INOUT.BUS.VaoRaV2;
using Com.Gosol.INOUT.DAL.VaoRaV2;
using websocket;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.FileProviders;

namespace Com.Gosol.INOUT.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Com.Gosol.INOUT.DAL.Connection.appConnectionStrings = configuration.GetValue<String>("AppSetting:appConnectionStrings");
            // Com.Gosol.INOUT.Ultilities.SQLHelper.appConnectionStrings= configuration.GetValue<String>("AppSetting:appConnectionStrings");
            Com.Gosol.INOUT.Ultilities.SQLHelper.appConnectionStrings = configuration.GetConnectionString("DefaultConnection");
            Com.Gosol.INOUT.Ultilities.SQLHelper.backupPath = configuration.GetConnectionString("BackupPath");
            Com.Gosol.INOUT.Ultilities.SQLHelper.dbName = configuration.GetConnectionString("DBName");
            Com.Gosol.INOUT.Ultilities.SQLHelper.connectionString = configuration.GetValue<String>("ConnectionStringDB:DefaultConnection");

        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connString = Configuration["ConnectionStringDB:DefaultConnection"];

            // Nếu bạn có SQLiteHelper
            services.AddSingleton(new SQLiteHelper(connString));
            QueryManager.Load(Configuration);
            services.AddControllers()
            .AddNewtonsoftJson();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });
            //    services.AddSignalR()
            //.AddNewtonsoftJsonProtocol();
            services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
            services.AddMvc()
            .AddNewtonsoftJson(
            options => options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddMvc()
             .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
                                       = new DefaultContractResolver());
            //inject option appsettings in appsettings.json
            services.AddOptions();
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            //inject dbcontext
            //services.AddDbContext<DbINOUTContext>(
            //options => options.UseMySql(SQLHelper.appConnectionStrings,
            //        mysqlOptions =>
            //        {
            //            mysqlOptions
            //                .CharSetBehavior(CharSetBehavior.AppendToAllColumns)
            //                .AnsiCharSet(CharSet.Latin1)
            //                .UnicodeCharSet(CharSet.Utf8mb4);
            //        }
            //));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Dashboard
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

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddCors(options => options.AddPolicy("myDomain", builder =>
            {
                builder.WithOrigins("http://gocheckin.gosol.com.vn")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
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

            services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver() { NamingStrategy = new Newtonsoft.Json.Serialization.DefaultNamingStrategy() });
            services.AddMvc().AddNewtonsoftJson();
            services.AddHttpContextAccessor();
            services.AddScoped(sp => sp.GetRequiredService<HttpContext>().Request);


            services.AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
            });


            //var context = new CustomAssemblyLoadContext();
            //context.LoadUnmanagedLibrary(Path.GetFullPath(@"C:\Users\Royal\.nuget\packages\dinktopdf\1.0.8\lib\netstandard1.6\libwkhtmltox.dll"));


            services.AddMvc();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            //config.EnableSystemDiagnosticsTracing();
            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            //config.Formatters.Remove(config.Formatters.XmlFormatter);


            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            //    options.HttpsPort = 7005;
            //});
            // service for socket
            services.AddSignalR(conf =>
            {
                conf.MaximumReceiveMessageSize = Int32.MaxValue;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Cho phép lấy file trong wwwroot
            app.UseStaticFiles();

            var imagePath = config["StaticFiles:ImagePath"];
            if (!string.IsNullOrEmpty(imagePath) && Directory.Exists(imagePath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(imagePath),
                    RequestPath = "/ImageGoCheckIn"
                });
            }

            // Cho phép phục vụ file tĩnh (FE build)
            app.UseDefaultFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory(), "Client")),
            //    RequestPath = ""
            //});

            app.UseCors("AllowOrigin");
            app.UseSession();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SocketHub>("/SocketHub", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });

                // ✅ Đưa vào đây
                endpoints.MapFallbackToFile("index.html");
            });
        }

    }
}
