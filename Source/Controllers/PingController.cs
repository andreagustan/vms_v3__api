using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VMS.Data;
using VMS.Error;
using VMS.Entities;
using VMS.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace VMS.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class PingController : ControllerBase
    //{
    //}

    public partial class PingController: BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;

        public PingController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
        }

        // GET: api/ping
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Ping()
        {
            string isDebug = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();

            string info = "\n";
            string r = "PONG " + isDebug.ToUpper() + " MODE\n";
            try
            {
                var tExp = Settings.AppSettingValue("AppSettings", "TokenExpired");

                DateTime _datetime = DateTime.Now;
                DateTime _datetimeZoned = Others.DateTimeConvertToZone(DateTime.Now);

                info = "Server Time " + _datetime.ToLongDateString() + " " + _datetime.ToLongTimeString() + "\n";
                info += "DateTimeConvertToZone Time " + _datetimeZoned.ToLongDateString() + " " + _datetimeZoned.ToLongTimeString() + "\n";
                info += "Token Expire : " + tExp + " Minutes\n";
                info += "\n";

                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string ProdVersion = fileVersionInfo.ProductVersion;

                info += "API Version " + ProdVersion + "\n";

                return StatusCode(200, r + info);
            }
            catch (Exception ex)
            {
                return StatusCode(500, r + info + "\n" + ex.Message + "\n" + ex.InnerException.Message.ToString());
            }
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<IActionResult> TestingUsersAsync()
        {
            try
            {
                _repository.BeginTransactionAsync();

                return Requests.Response(this, new ApiStatus(200), null, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpGet("error/500")]        
        public IActionResult Error500()
        {
            return Requests.Response(this, new ApiStatus(500), null, "Test Error");
        }

        [AllowAnonymous]
        [HttpGet("success/200")]
        [Produces("application/json")]
        public IActionResult Success200()
        {
            return Requests.Response(this, new ApiStatus(200), null, "Test Success");
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpGet("Encrypt")]
        public IActionResult EncryptString(string Text)
        {
            var HashText = "";

            //HashText = ExtensionClass.ToEncryptString(Text);
            HashText = Text.ToEncryptString();
            
            return Requests.Response(this, new ApiStatus(200), HashText, null);
        }

        [AllowAnonymous]
        [HttpGet("Decript")]
        [Produces("application/json")]
        public IActionResult DecriptString(string Text)
        {
            var HashText = "";
           
            //HashText = ExtensionClass.ToDecryptString(Text);
            HashText = Text.ToDecryptString();

            return Requests.Response(this, new ApiStatus(200), HashText, null);
        }


        [AllowAnonymous]
        [Produces("application/json")]
        [HttpGet("EncryptAES")]
        public IActionResult EncryptAESString(string Text)
        {
            var HashText = "";
            
            HashText = ExtensionClass.EncryptString(Text);
           

            return Requests.Response(this, new ApiStatus(200), HashText, null);
        }

        [AllowAnonymous]
        [HttpGet("DecriptAES")]
        [Produces("application/json")]
        public IActionResult DecriptAESString(string Text)
        {
            var HashText = "";
            
            HashText = ExtensionClass.DecryptString(Text);
            

            return Requests.Response(this, new ApiStatus(200), HashText, null);
        }

    }
}
