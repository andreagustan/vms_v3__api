using Ardalis.ListStartupServices;
using Autofac;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using VMS.Data;
using VMS.Entities;
using VMS.Error;
using VMS.Infrastructure;
using VMS.Interface;
using VMS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using Microsoft.Data.SqlClient;

namespace VMS
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            string AllowedOriginCors = Configuration.GetSection("AllowedOrigin").Value;

            string isDebug = "development";

            services.Configure<CookiePolicyOptions>(opt =>
            {
                opt.CheckConsentNeeded = context => true;
                opt.MinimumSameSitePolicy = SameSiteMode.None;
            });

            try
            {
                isDebug = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();

                //TODO : Remove kalau sudah pake API sharepoint
                services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
            }
            catch (Exception ex)
            {

            }

            if (isDebug.Contains("development"))
            {
                //services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
                //{
                //    builder.WithOrigins("http://*:5100").AllowAnyMethod().AllowAnyHeader();
                //}));
                services.AddCors(options =>
                {
                    options.AddPolicy("ApiCorsPolicy", police =>
                    {
                        police.AllowAnyHeader();
                        police.AllowAnyMethod();
                        police.AllowCredentials();
                        police.SetIsOriginAllowed(origin =>
                        {                            
                            if (string.IsNullOrWhiteSpace(origin))
                            {
                                throw new Exception("Please contact IT Support.");
                            }
                            string phrase = AllowedOriginCors;
                            string[] _cors = phrase.Split('|');

                            if (_cors.Contains(origin))
                            {
                                return true;
                            }
                            else
                            {
                                throw new Exception("Please contact IT Support.");
                            }

                        });
                    });
                });

            }
            else
            {
                services.AddCors();
            }

            services.AddHsts(options =>
            {
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddAntiforgery(options =>
            {
                options.SuppressXFrameOptionsHeader = true;
            });

            services.AddDbContext(connectionString);

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddControllers().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<JWTSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero

                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {

            });

            services.AddScoped<IProfileManager, ProfileManager>();
            services.AddScoped<IAppsLog, AppsLog>();
            services.AddScoped<IHelpers, HelperServices>();
            services.AddScoped<IAsyncAuthorizationFilter, CustomAuthorizationFilter>();
            //services.AddScoped<IDbConnection, SqlConnection>(
            //    options => new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            //Add new service
            #region Add New Services
            services.AddScoped<IUserService, UsersServices>();
            services.AddScoped<ICustomer, CustomerServices>();
            services.AddScoped<ISupplier, SupplierServices>();
            services.AddScoped<ISalesman, SalesmanServices>();
            services.AddScoped<I_iTem, I_ItemServices>();
            services.AddScoped<II_ItemDetail, I_ItemDetailServices>();
            services.AddScoped<ISo, SoServices>();
            services.AddScoped<ISoDetail, SoDetailServices>();
            services.AddScoped<IGr, GrServices>();
            services.AddScoped<IGrDetail, GrDetailServices>();
            services.AddScoped<IDo, DoServices>();
            services.AddScoped<IDoDetail, DoDetailServices>();
            services.AddScoped<IT_Transfer, T_TransferServices>();
            services.AddScoped<IT_TransferDetail, T_TransferDetailServices>();
            services.AddScoped<IUserBranch, UserBranchServices>();
            services.AddScoped<IT_PermintaanBarang, T_PermintaanBarangServices>();
            services.AddScoped<IT_PermintaanBarangDetail, T_PermintaanBarangDetailServices>();
            services.AddScoped<IT_PengambilanBarang, T_PengambilanBarangServices>();
            services.AddScoped<IT_PengambilanBarangDetail, T_PengambilanBarangDetailServices>();
            services.AddScoped<ICustomerDetail, CustomerDetailServices>();
            services.AddScoped<IVoucherDetail, VoucherDetailServices>();
            services.AddScoped<IPriceListVoucher, PriceListVoucherServices>();
            services.AddScoped<IView, ViewServices>();
            services.AddScoped<IT_PO, T_POServices>();
            #endregion


            #region Service WCF
            services.AddScoped<ISoapSSO, SoapSSOServices>();
            #endregion

            services.AddRazorPages();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VMS", Version = "v3" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                c.EnableAnnotations();
            });
                        
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<ServiceConfig>(config =>
            {
                config.Services = new List<ServiceDescriptor>(services);

                // optional - default path to view services is /listallservices - recommended to choose your own path
                config.Path = "/listservices";
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new DefaultInfrastructureModule(_env.EnvironmentName.ToLower().Contains("development")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext db)
        {            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VMS v3"));

            app.UseDeveloperExceptionPage();
            app.UseShowAllServicesMiddleware();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self' https: 'unsafe-inline' 'unsafe-eval'; img-src 'self' data: ;font-src 'self' data:;");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");

                context.Response.Headers.Add("access-control-allow-credentials", "true");
                context.Response.Headers.Add("access-control-allow-headers", "Authorization, Content-Type, Origin, x-requested-with, x-signalr-user-agent");
                context.Response.Headers.Add("access-control-allow-methods", "POST, PUT, GET, PATCH, DELETE");
                context.Response.Headers.Add("access-control-max-age", "3600");

                context.Response.Headers.Add("Feature-Policy", "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'");

                await next();
            });

            db.Database.EnsureCreated();

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseCors("ApiCorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
