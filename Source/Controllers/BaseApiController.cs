using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.Data;
using VMS.Entities;
using VMS.Interface;

namespace VMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : Controller
    {
        [ExcludeFromCodeCoverage]
        [HttpGet("GetUserId")]
        public string GetUserId()
        {
            var CurrentUserId = "N/A";
            try
            {
                if (User.Claims.FirstOrDefault() != null)
                {
                    CurrentUserId = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;
                }
                else
                {
                    var Rq = HttpContext.Request.Headers["Authorization"].ToString().Split(" ");
                    var Ihelpers = HttpContext.RequestServices.GetRequiredService<IHelpers>();

                    var Rs = Ihelpers.GetPolicy(Rq[Rq.Count()-1].ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(Rs.Data.CurrentUser))
                    {
                        CurrentUserId = Rs.Data.CurrentUser;
                    }

                    #region Comm
                    //var DtParm = new SystemConfigParm()
                    //{
                    //    Name = "Common",
                    //    SystemCategory = "Secret",
                    //    SystemSubCategory = "Token",
                    //};

                    //var Rs = Ihelpers.GetPolicy(Rq[1].ToString(),DtParm).ConfigureAwait(false).GetAwaiter().GetResult();

                    //if (Rs.Data.Claims.FirstOrDefault().Value != "") 
                    //{
                    //    CurrentUserId = Rs.Data.Claims.FirstOrDefault().Value;
                    //} 
                    //else 
                    //{
                    //    CurrentUserId = Rs.Data.Claims.FirstOrDefault(x => x.Type == "userId").Value;
                    //}

                    //var Rq = HttpContext.Request.Headers["Authorization"].ToString().Split(" ");
                    //var token = Rq[1].ToString();
                    ////var Key = Encoding.ASCII.GetBytes(Settings.AppSettingValue("AppSettings", "Token"));

                    //var DtParm = new SystemConfigParm() { 
                    //    Name= "Common",
                    //    SystemCategory= "Secret",
                    //    SystemSubCategory= "Token",
                    //    SystemCode= "S1",
                    //};

                    //var Ihelpers = HttpContext.RequestServices.GetRequiredService<IHelpers>();
                    //var Rs = Ihelpers.GetByParmSysConfig(DtParm).ConfigureAwait(false).GetAwaiter().GetResult();

                    //if (!Rs.RsStatus) 
                    //{
                    //    return CurrentUserId;
                    //}

                    //var Key = Encoding.ASCII.GetBytes(Rs.RsData.SystemValue);
                    //var handler = new JwtSecurityTokenHandler();
                    //var validator = new TokenValidationParameters { 
                    //    IssuerSigningKey= new SymmetricSecurityKey(Key),
                    //    ValidateIssuerSigningKey = true,
                    //    ValidateIssuer = false,
                    //    ValidateAudience = false,
                    //};

                    //var claimsCustome = handler.ValidateToken(token, validator, out var tokenSecure);
                    //CurrentUserId = claimsCustome.Claims.FirstOrDefault().Value;

                    #endregion
                }
            }
            catch (Exception)
            {

            }
            return CurrentUserId;
        }
                
    }
}
