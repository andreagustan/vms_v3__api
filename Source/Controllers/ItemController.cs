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
using static VMS.Entities.Commons;

namespace VMS.Controllers
{
    public partial class I_ItemController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ISoapSSO _soapSSOws;
        private readonly I_iTem _Item;

        public I_ItemController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers, ISoapSSO soapSSOws, I_iTem Item)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _soapSSOws = soapSSOws;
            _soapSSOws = soapSSOws;
            _Item = Item;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetListPage([FromQuery] ListPageExt Items)
        {
            try
            {
                var Rs = await _Item.ListObject(Items);
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

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] CommonDelete Items)
        {
            try
            {
                Items.UserId = GetUserId();
                var Rs = await _Item.DeleteById(Items);
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

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("BulkUpdate")]
        public async Task<IActionResult> BulkUpdate([FromBody] I_ItemBulk_Request Items)
        {
            try
            {
                Items.EntryUser = GetUserId();
                var Rs = await _Item.BulkUpdate(Items);
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
