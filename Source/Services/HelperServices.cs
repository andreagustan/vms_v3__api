using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VMS.Entities;
using VMS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using VMS.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization.Policy;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace VMS.Services
{
    public class HelperServices: IHelpers
    {
        private readonly ILogger<Logs> _logger;
        private readonly IRepository _repository;
        private readonly IConfiguration _configs;
        private readonly IMapper _mapper;

        public HelperServices(ILogger<Logs> logger, IRepository repository, IConfiguration configs, IMapper mapper)
        {
            this._logger = logger;
            this._repository = repository;
            this._configs = configs;
            this._mapper = mapper;
        }

        public async Task<T> GetDataAPIUrl<T>(string api, string defValue=null)
        {
            try
            {
                var socketsHandler = new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(60),
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(60),
                    MaxConnectionsPerServer = 500
                };

                using (var client = new HttpClient(socketsHandler) { BaseAddress = new Uri(api) })
                {
                    HttpResponseMessage message = await client.GetAsync(api);
                    var strResponse = await message.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<T>(strResponse);
                    return responseResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Data failed with error : " + ex.Message, ex.InnerException);
                return JsonConvert.DeserializeObject<T>(null);
            }
        }

        public async Task<T> GetDataAPIUrlWithAuth<T>(string api, string authorizevalue, string defValue = null)
        {
            try
            {
                var socketsHandler = new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(60),
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(60),
                    MaxConnectionsPerServer = 500
                };

                using (var client = new HttpClient(socketsHandler) { BaseAddress = new Uri(api) })
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorizevalue);
                    HttpResponseMessage message = await client.GetAsync(api);
                    var strResponse = await message.Content.ReadAsStringAsync();
                    var responseResult = JsonConvert.DeserializeObject<T>(strResponse);
                    return responseResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Data failed with error : " + ex.Message, ex.InnerException);
                return JsonConvert.DeserializeObject<T>(null);
            }
        }

        public async Task<(bool Status, string Msg, T Data)> PostDataAPIUrl<T>(string Url, string Api, string Content)
        {
            try
            {
                var socketsHandler = new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(60),
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(60),
                    MaxConnectionsPerServer = 500
                };

                using (var client = new HttpClient(socketsHandler) { BaseAddress = new Uri(Url) })
                {
                    var JsonContent = JsonConvert.DeserializeObject<T>(Content);

                    string serailizeddto = JsonConvert.SerializeObject(JsonContent, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                    var inputMessage = new HttpRequestMessage
                    {
                        Content = new StringContent(serailizeddto.Replace("\\\\", ""), Encoding.UTF8, "application/json")
                    };

                    inputMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage message = await client.PostAsync(Api, inputMessage.Content);

                    var strResponse = await message.Content.ReadAsStringAsync();
                    var objResponse = JsonConvert.DeserializeObject<T>(strResponse);

                    return (true,"Succes",objResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Data failed with error : " + ex.Message, ex.InnerException);
                return (false,ex.Message, JsonConvert.DeserializeObject<T>(null));
            }
        }

        public async Task<(bool Status, string Msg, T Data)> PostDataAPIUrlWithAuth<T>(string Url, string Api, string authorizevalue, string Content)
        {
            try
            {
                var socketsHandler = new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(60),
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(60),
                    MaxConnectionsPerServer = 500
                };

                using (var client = new HttpClient(socketsHandler) { BaseAddress = new Uri(Url) })
                {
                    var JsonContent = JsonConvert.DeserializeObject<T>(Content);

                    string serailizeddto = JsonConvert.SerializeObject(JsonContent, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                    var inputMessage = new HttpRequestMessage
                    {
                        Content = new StringContent(serailizeddto.Replace("\\\\", ""), Encoding.UTF8, "application/json")
                    };

                    client.DefaultRequestHeaders.Add("Authorization", authorizevalue);
                    inputMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage message = await client.PostAsync(Api, inputMessage.Content);

                    var strResponse = await message.Content.ReadAsStringAsync();
                    var objResponse = JsonConvert.DeserializeObject<T>(strResponse);

                    return (true, "Succes", objResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Data failed with error : " + ex.Message, ex.InnerException);
                return (false, ex.Message, JsonConvert.DeserializeObject<T>(null));
            }
        }

        public async Task<(bool RsStatus, string RsMsg, SystemConfig RsData)> GetByIdSysConfig(SystemConfigParm Items)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@FlagData", "GetById");
                p.Add("@Id", Items.Id);

                var Rs = (await _repository.executeProcedure<SystemConfig>("ProcCRUDSystemConfig", p)).FirstOrDefault();

                return (true, null, Rs);                
                
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        
        public async Task<(bool RsStatus, string RsMsg, string RsData)> BulkSysConfig(List<SystemConfigParm> Items, string CurrentUser)
        {
            try
            {
                var p = new Dictionary<string, object>()
                {
                    { "FlagData", StringHelpers.PrepareJsonstring(Items) },
                    { "UserId", CurrentUser},

                };
                
                var Rs = await Task.Run(() => _repository.ExecSPToDataTable("ProcCRUDSystemConfig", p));

                return (true, null, Rs.Rows[0].ItemArray[0].ToString());

            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool RsStatus, string RsMsg, List<SystemConfig> RsData)> ListSysConfig(SystemConfigParm Items)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Name", Items.Name);
                p.Add("@SysCategory", Items.SystemCategory);
                p.Add("@SysSubCategory", Items.SystemSubCategory);
                p.Add("@SysCode", Items.SystemCode);

                var Rs = await _repository.executeProcedure<SystemConfig>("ProcCRUDSystemConfig", p);
                var DtList = _mapper.Map<List<SystemConfig>>(Rs);

                return (true, null, Rs.ToList());

            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool Status, string Msg, TokenExt Data)> GetPolicy(string Token)
        {
            try
            {
                if(string.IsNullOrEmpty(Token) || Token=="null") return (false, "Token is empty.", new TokenExt());
                var handler = new JwtSecurityTokenHandler();
                //var data = handler.ReadJwtToken(Token).Claims;
                var DataToken = handler.ReadJwtToken(Token) as JwtSecurityToken;


                var Rs = "N/A";
                var TickTime = long.Parse(DataToken.Claims.Where(w => w.Type == "exp").FirstOrDefault().Value);
                var ExpDate = DateTimeOffset.FromUnixTimeSeconds(TickTime).LocalDateTime;


                if (DataToken.Claims.Where(w => w.Type == "userId").FirstOrDefault() != null)
                {
                    Rs = DataToken.Claims.Where(w => w.Type == "userId").FirstOrDefault().Value;
                }
                else if (DataToken.Claims.Where(w => w.Type == "vendor_id").FirstOrDefault() != null)
                {
                    Rs = DataToken.Claims.Where(w => w.Type == "vendor_id").FirstOrDefault().Value;
                }
                else if (DataToken.Claims.FirstOrDefault().Value != "")
                {
                    Rs = DataToken.Claims.FirstOrDefault().Value;
                }

                var DataRs = new TokenExt() 
                {
                    CurrentUser=Rs,
                    Token=Token,
                    //ValidTo=DataToken.ValidTo.ToFormatIDStringDate(),
                    ValidTo= ExpDate.ToFormatIDStringDate(),
                };

                if (ExpDate >= DateTime.Now) 
                {
                    return (true, null, DataRs);
                } 
                else 
                {
                    return (false, "Token is expired.", DataRs);
                }
                
                //var GetLsSecretKey = await ListSysConfig(Items);

                //if (!GetLsSecretKey.RsStatus) {
                //    return (false, GetLsSecretKey.RsMsg, null);
                //}

                //var ClaimExt = new ClaimsPrincipal();

                //foreach (SystemConfig Data in GetLsSecretKey.RsData)
                //{ 
                //    var Key = Encoding.ASCII.GetBytes(Data.SystemValue);

                //    var handler = new JwtSecurityTokenHandler();
                //    var validator = new TokenValidationParameters
                //    {
                //        IssuerSigningKey = new SymmetricSecurityKey(Key),
                //        ValidateIssuerSigningKey = true,
                //        ValidateIssuer = false,
                //        ValidateAudience = false,
                //    };

                //    var claimsCustome = handler.ValidateToken(token, validator, out var tokenSecure);

                //    if (claimsCustome.Claims.FirstOrDefault() != null)
                //    {
                //        ClaimExt = claimsCustome;
                //        break;
                //    }
                //}

                //if (ClaimExt.Claims.FirstOrDefault()==null) 
                //{
                //    return (false, null, null);
                //} 
                //else 
                //{
                //    return (true, null, ClaimExt);
                //}

            }
            catch (Exception ex)
            {
                return (false,ex.Message,null);
            }
        }
           
    }
}
