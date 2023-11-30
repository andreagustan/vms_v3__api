using Microsoft.AspNetCore.Http;
using VMS.Entities;
using VMS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VMS.Services
{
    public class ProfileManager: IProfileManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            var r = "";
            if (_httpContextAccessor.HttpContext.User != null)
            {
                r = _httpContextAccessor.HttpContext.User.Identity.Name;
            }

            return r;
        }

        public string GetUserId()
        {
            var r = "";
            if (_httpContextAccessor.HttpContext.User != null)
            {
                r = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            return r;
        }

        public async Task<HelpersModel.SimpleViewModel> GetCurrentUserId()
        {
            var userId = "";

            var rUser = new HelpersModel.SimpleViewModel();
            try
            {
                if (_httpContextAccessor.HttpContext.User != null)
                {
                    userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValue);

                var user = await GetUserIdAsync(userId, headerValue);

                rUser.UserId = user.UserId;
                rUser.Token = headerValue;
            }
            catch (Exception)
            {

            }

            return rUser;

        }

        public static async Task<HelpersModel.UserData> GetUserIdAsync(string userId, string authorizevalue)
        {
            try
            {

                var approver = new HelpersModel.UserData
                {
                    UserId = userId,
                    Token = authorizevalue
                };

                return await Task.Run(() => approver);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message.ToString());
            }
        }
    }
}
