using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Data;
using VMS.Entities;
using VMS.Error;
using VMS.Interface;
using static VMS.Class.ApiResponse;

namespace VMS.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class CustomerDetailController : ControllerBase
    //{
    //}

    public class M_CustomerDetailController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ICustomerDetail _customerDetail;

        public M_CustomerDetailController(IRepository repository, IMapper mapper, IAppsLog appsLog, ICustomerDetail customerDetail, IHelpers helpers)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _customerDetail = customerDetail;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        //[Authorize]
        [Produces("application/json")]
        [HttpPost("ListPage")]
        public async Task<IActionResult> GetListPage([FromBody] ListPage Items, string request)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                var GridLimit = request.GridRequest();
                if (GridLimit != null)
                    Items.PageSize = GridLimit.offset.ToString();

                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _customerDetail.List(Items);
                if (!Rs.Status) 
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }

                var FieldNya = "customerID,customerName,subCustomerID,subCustomerName";
                                
                var (Generated, Message, recordsTotal, recordsFilteredTotal, dataReturn, colsName) = await _repository.GenerateDataForDatatableExtAsync(Rs.Result.AsQueryable(), FieldNya, Items);

                DtLog.Response = StringHelpers.PrepareJsonstring(new ApiDatatableResponse(recordsTotal, recordsFilteredTotal, dataReturn, string.Join(", ", colsName)).Result);
                _appsLog.WriteAppsLog(DtLog);

                return Requests.Response(this, new ApiStatus(200), new ApiDatatableResponse(recordsTotal, recordsFilteredTotal, dataReturn, string.Join(", ", colsName)).Result, "");
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
        //[Authorize]
        [Produces("application/json")]
        [HttpPost("ListPageExt")]
        public async Task<IActionResult> GetListPageExt([FromBody] ListPage Items, string request)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                var GridLimit = request.GridRequest();
                if (GridLimit != null)
                    Items.PageSize = GridLimit.offset.ToString();

                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _customerDetail.ListObject(Items);
                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Rs.Message });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }

                DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                _appsLog.WriteAppsLog(DtLog);

                return Requests.Response(this, new ApiStatus(200), Rs.Result, Rs.Message);
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
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(long Id)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _customerDetail.GetById(Id);
                if (Status) 
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs);
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(200), Rs, Msg);
                } 
                else 
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Msg });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Msg);
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
        [HttpGet("GetByIdExt/{id:int}")]
        public async Task<IActionResult> GetByIdExtAsync(long Id)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _customerDetail.GetByIdExt(Id);
                if (Status)
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs);
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(200), Rs, Msg);
                }
                else
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Msg });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Msg);
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
        [HttpPost("BulkMany")]
        public async Task<IActionResult> BulkAsync([FromBody] List<MCustomerDetailDto> Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _customerDetail.BulkMany(GetUserId(), Items);
                if (!Rs.Status) 
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Rs.Message });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                } 
                else 
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
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
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync([FromBody] MCustomerDetailExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _customerDetail.Add(GetUserId(), Items);

                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Rs.Message });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }
                else
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
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
        [HttpPost("Edit")]
        public async Task<IActionResult> UpdateAsync([FromBody] MCustomerDetailExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _customerDetail.Update(GetUserId(), Items);

                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Rs.Message });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }
                else
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
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
        [HttpDelete("Delete/{Id:int}")]
        public async Task<IActionResult> DeleteAsync(long Id)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDCustomerDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _customerDetail.Delete(GetUserId(), Id);

                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Rs.Message });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }
                else
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
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
