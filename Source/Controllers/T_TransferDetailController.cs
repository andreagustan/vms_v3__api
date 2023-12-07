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
    public partial class T_TransferDetailController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ISoapSSO _soapSSOws;
        private readonly IT_TransferDetail _T_TransferDetail;

        public T_TransferDetailController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers, ISoapSSO soapSSOws, IT_TransferDetail T_TransferDetail)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _soapSSOws = soapSSOws;
            _soapSSOws = soapSSOws;
            _T_TransferDetail = T_TransferDetail;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetListPage([FromQuery] ListPageExt Items)
        {
            try
            {
                var Rs = await _T_TransferDetail.ListObject(Items);
                if (Rs.Status)
                {
                    return Ok(Rs.Result);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(500), Rs.Result, Rs.Message);
                }

            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }
    }
}