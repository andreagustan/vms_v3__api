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
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "pI_Item_View";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _Item.ListObject(Items);
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
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromBody] CommonDelete Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                Items.UserId = GetUserId();

                DtLog.Description = "pI_Item_Del";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _Item.DeleteById(Items);
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
        [HttpPost("BulkUpdate")]
        public async Task<IActionResult> BulkUpdate([FromBody] I_ItemBulk_Request Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                Items.EntryUser = GetUserId();

                DtLog.Description = "pI_Item_Bulk";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _Item.BulkUpdate(Items);
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
