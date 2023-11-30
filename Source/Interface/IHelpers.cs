using Microsoft.AspNetCore.Authorization.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface IHelpers
    {
        Task<(bool Status, string Msg, T Data)> PostDataAPIUrl<T>(string Url, string Api, string Content);
        Task<(bool Status, string Msg, T Data)> PostDataAPIUrlWithAuth<T>(string Url, string Api, string authorizevalue, string Content);
        Task<T> GetDataAPIUrlWithAuth<T>(string api, string authorizevalue, string defValue = null);
        Task<T> GetDataAPIUrl<T>(string api, string defValue = null);
        Task<(bool RsStatus, string RsMsg, SystemConfig RsData)> GetByIdSysConfig(SystemConfigParm Items);
        Task<(bool RsStatus, string RsMsg, string RsData)> BulkSysConfig(List<SystemConfigParm> Items, string CurrentUser);
        Task<(bool RsStatus, string RsMsg, List<SystemConfig> RsData)> ListSysConfig(SystemConfigParm Items);
        Task<(bool Status, string Msg, TokenExt Data)> GetPolicy(string token);
    }
}
