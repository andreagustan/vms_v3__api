﻿using AutoMapper;
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
    public partial class T_SoController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ISoapSSO _soapSSOws;
        private readonly ISo _So;

        public T_SoController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers, ISoapSSO soapSSOws, ISo So)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _soapSSOws = soapSSOws;
            _soapSSOws = soapSSOws;
            _So = So;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetListPage([FromQuery] ListPageExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "pT_SO_View";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _So.ListObject(Items);
                if (Rs.Status)
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                    _appsLog.WriteAppsLog(DtLog);

                    return Ok(Rs.Result);
                }
                else
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(Rs.Result);
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), Rs.Result, Rs.Message);
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

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("BulkSO")]
        public async Task<IActionResult> BulkSO([FromBody] T_SO_Bulk Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                Items.entryUser = GetUserId();

                DtLog.Description = "pT_SO_Bulk";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _So.BulkUpdateSO(Items);
                if (Rs.Status)
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                    _appsLog.WriteAppsLog(DtLog);

                    return Ok(Rs.Result);
                }
                else
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(Rs.Result);
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), Rs.Result, Rs.Message);
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
