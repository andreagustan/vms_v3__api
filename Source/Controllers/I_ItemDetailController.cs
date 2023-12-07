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
    public partial class I_ItemDetailController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ISoapSSO _soapSSOws;
        private readonly II_ItemDetail _I_ItemDetail;

        public I_ItemDetailController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers, ISoapSSO soapSSOws
		, II_ItemDetail I_ItemDetail)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _soapSSOws = soapSSOws;
            _soapSSOws = soapSSOws;
            _I_ItemDetail = I_ItemDetail;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetListPage([FromQuery] ListPageExt Items)
        {
            try
            {
                var Rs = await _I_ItemDetail.ListObject(Items);
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
