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
    //public class VoucherDetailController : ControllerBase
    //{
    //}

    public class T_VoucherDetailController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly IVoucherDetail _voucherDetail;

        public T_VoucherDetailController(IRepository repository, IMapper mapper, IAppsLog appsLog, IVoucherDetail voucherDetail, IHelpers helpers)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _voucherDetail = voucherDetail;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("ListPage")]
        public async Task<IActionResult> GetListPage([FromBody] ListPageDetail Items, string request)
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

                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _voucherDetail.List(Items);
                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail = Rs.Message });
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }

                var FieldNya = "refId,itemID,startNo,endNo,expDateVoucher,qty,sources";

                var ItemPage = _mapper.Map<ListPage>(Items);

                var (Generated, Message, recordsTotal, recordsFilteredTotal, dataReturn, colsName) = await _repository.GenerateDataForDatatableExtAsync(Rs.Result.AsQueryable(), FieldNya, ItemPage);

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
        [Produces("application/json")]
        [HttpPost("ListPageExt")]
        public async Task<IActionResult> GetListPageExt([FromBody] ListPageDetail Items, string request)
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

                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _voucherDetail.ListObject(Items);
                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(Rs.Result);
                    DtLog.StatusLog = ConstValue.LogError;
                    DtLog.FlagData = ConstValue.LogEdit;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), Rs.Result, Rs.Message);
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
        [HttpPost("BulkMany")]
        public async Task<IActionResult> BulkAsync([FromBody] List<VoucherDetailDto> Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _voucherDetail.BulkMany(GetUserId(), Items);
                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(Rs.Result);
                    DtLog.StatusLog = ConstValue.LogError;
                    DtLog.FlagData = ConstValue.LogEdit;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), Rs.Result, Rs.Message);
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
        public async Task<IActionResult> AddAsync([FromBody] VoucherDetailExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _voucherDetail.Add(GetUserId(), Items);

                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(Rs.Result);
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), Rs.Result, Rs.Message);
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
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(long Id)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _voucherDetail.DetailId(Id);
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
                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _voucherDetail.DetailIdExt(Id);
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
        [HttpPost("Edit")]
        public async Task<IActionResult> UpdateAsync([FromBody] VoucherDetailExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _voucherDetail.Update(GetUserId(), Items);

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
                DtLog.Description = "ProcCRUDVoucherDetail";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _voucherDetail.Delete(GetUserId(), Id);

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
