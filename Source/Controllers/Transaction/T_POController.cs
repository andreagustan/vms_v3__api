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
using Dapper;

namespace VMS.Controllers
{
    public partial class T_POController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ISoapSSO _soapSSOws;
        private readonly IT_PO _T_PO;

        public T_POController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers, ISoapSSO soapSSOws, IT_PO T_PO)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _soapSSOws = soapSSOws;
            _soapSSOws = soapSSOws;
            _T_PO = T_PO;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetListPage([FromQuery] ListPageExt Items)
        {
            try
            {
                var Rs = await _T_PO.ListObject(Items);
                if (Rs.Status)
                {
                    return Ok(Rs.Result);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }

            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpGet("GetPODetail")]
        public async Task<IActionResult> ListDetailObject([FromQuery] ListPageExt Items)
        {
            try
            {
                var Rs = await _T_PO.ListDetailObject(Items);
                if (Rs.Status)
                {
                    return Ok(Rs.Result);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }

            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [Produces("application/json")]
        [HttpPost("BulkUpdate")]
        public async Task<IActionResult> BulkUpdate([FromBody] T_PO Items)
        {
            try
            {
                var p = new DynamicParameters();
                var Rs = await _T_PO.BulkUpdate(Items);
                if (Rs.Status)
                {
                    return Ok(Rs.Result);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }

            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }
    }
}
