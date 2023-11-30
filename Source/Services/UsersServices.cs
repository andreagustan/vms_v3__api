using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VMS.Data;
using VMS.Entities;
using VMS.Interface;

namespace VMS.Services
{
    public class UsersServices : IUserService
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public UsersServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
        {
            this.repository = _repository;
            this.logger = _logger;
            this.helpers = _helpers;
            this.appsLog = _appsLog;
            this.mapper = _mapper;
        }

        public object GenerateToken(string UserId)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(Settings.AppSettingValue("AppSettings", "Secret", null));
            int TokenExpired = Settings.AppSettingValue("AppSettings", "TokenExpired", 10);
            string Issuer = Settings.AppSettingValue("AppSettings", "Issuer", "www.pertaminaretail.com");
            string Audience = Settings.AppSettingValue("AppSettings", "Audience", "PTPR-API-VMS");
                
            string avatar = "";

            DateTime dtNow = Others.DateTimeConvertToZone(DateTime.Now);
            TokenExpired = TokenExpired <= 0 ? TokenExpired = 60 : TokenExpired;
            var tokenExp = dtNow.AddMinutes(TokenExpired);

            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("userId", UserId),
                    new Claim("avatar", avatar),
                    new Claim("lastSync",dtNow.ToFormatIDStringDate())
                };

            var token = new JwtSecurityToken(
                           claims: claims,
                           issuer:Issuer,
                           audience: Audience,
                           expires: tokenExp,
                           signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256)
                        );

            var r = new dataAuth();
            r.Token = tokenHandler.WriteToken(token);
            r.UserId = UserId;

            return r;
        }

        public async Task<(bool Authenticated, Object Result, string Message)> AuthenticateAsync(dataAuthExt Items)
        {
            try
            {
                var PasswordHash = ExtensionClass.ToEncryptString(Items.Password);
                
                var result = GenerateToken(Items.UserId);

                return (true, result, null);


            }
            catch (Exception ex)
            {
                logger.LogError("Data failed to auth with error : " + ex.Message, ex.InnerException);
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        
    }
}
