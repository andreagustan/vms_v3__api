using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;
using VMS.Error;
using VMS.Interface;
using Microsoft.AspNetCore.Authorization;

namespace VMS.Controllers
{
    public partial class UsersController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ISoapSSO _soapSSOws;
        private readonly IUserService _user;

        public UsersController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers, ISoapSSO soapSSOws, IUserService user)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _soapSSOws = soapSSOws;
            _soapSSOws = soapSSOws;
            _user = user;
        }
                
        [HttpPost("authenticate")]
        [Produces("application/json")]
        public async Task<IActionResult> Authenticate([FromBody] dataAuthExt Items) 
        {
            bool Authenticated = false;
            string Message = "";
            Object Result = "";
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description =_soapSSOws.GetType().Name;
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _soapSSOws.GetValidasiUser(Items);
                if (Rs.Status && Rs.Data.Body.ValidateUserResult)
                {
                    (Authenticated, Result, Message) = await _user.AuthenticateAsync(Items);
                    if (Authenticated)
                    {
                        DtLog.Response = StringHelpers.PrepareJsonstring(Result);
                        _appsLog.WriteAppsLog(DtLog);

                        return Requests.Response(this, new ApiStatus(200), Result, Message);
                    }
                    else
                    {
                        DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Message });
                        DtLog.StatusLog = ConstValue.LogError;
                        _appsLog.WriteAppsLog(DtLog);

                        return Requests.Response(this, new ApiStatus(400), Result, Message);
                    }
                }
                else 
                {
                    return Requests.Response(this, new ApiStatus(400), Rs.Data.Body.ValidateUserResult, "Invalid login please try again or contact Administrator.");
                }
                
            }
            catch (Exception ex)
            {
                DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = ex.Message });
                DtLog.StatusLog = ConstValue.LogError;
                DtLog.FlagData = ConstValue.LogEdit;
                _appsLog.WriteAppsLog(DtLog);

                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }
    }
}
