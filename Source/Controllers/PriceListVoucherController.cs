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
    //public class PriceListVoucherController : BaseApiController
    //{
    //}

    public class T_PriceListVoucherController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly IPriceListVoucher _plVoucer;

        public T_PriceListVoucherController(IRepository repository, IMapper mapper, IAppsLog appsLog, IPriceListVoucher plVoucer, IHelpers helpers)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
            _plVoucer = plVoucer;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("ListPage")]
        public async Task<IActionResult> GetListPage([FromBody] ListPageDetail Items, string request)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                var GridLimit = request.QueryBuilder();
                //if (GridLimit != null)
                //    Items.PageSize = GridLimit.Offset.ToString();

                if (GridLimit != null)
                {
                    Items.PageSize = GridLimit.Offset == "0" ? GridLimit.Limit : GridLimit.Offset;
                }
                else
                {
                    Items.PageSize = "100";
                }
                Items.PageNumber = "0";

                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _plVoucer.List(Items);
                if (!Rs.Status)
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }

                var FieldNya = "supplierID,itemID,beginQty,endQty,price";

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
        //public async Task<IActionResult> GetListPageExt([FromBody] ListPageDetail Items, string request)
        public async Task<IActionResult> GetListPageExt([FromQuery] ListPageExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                var GridLimit = Items.request.QueryBuilder();
                //if (GridLimit != null)
                //    Items.PageSize = GridLimit.Offset.ToString();

                if (GridLimit != null)
                {
                    //Items.PageSize = GridLimit.Offset == "0" ? GridLimit.Limit : GridLimit.Offset;
                    Items.Size = GridLimit.Offset == "0" ? GridLimit.Limit.ToInt() : GridLimit.Offset.ToInt();
                }
                else
                {
                    //Items.PageSize = "100";
                    Items.Size = "100".ToInt();
                }
                //Items.PageNumber = "0";
                Items.Page ??= "0".ToInt();

                Items.OrderBy ??= "";Items.OrderBy = Items.OrderBy.Trim();
                Items.Search ??= "";Items.Search = Items.Search.Trim();

                //DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Description = "pM_PriceListVoucher_View";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _plVoucer.ListObject(Items);
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
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetList([FromQuery] ListPageExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                var GridLimit = Items.request.QueryBuilder();

                if (GridLimit != null)
                {
                    Items.Size = GridLimit.Offset == "0" ? GridLimit.Limit.ToInt() : GridLimit.Offset.ToInt();
                }
                else
                {
                    Items.Size = "100".ToInt();
                }
                Items.Page ??= "0".ToInt();

                Items.OrderBy ??= ""; Items.OrderBy = Items.OrderBy.Trim();
                Items.Search ??= ""; Items.Search = Items.Search.Trim();

                DtLog.Description = "pM_PriceListVoucher_View";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _plVoucer.ListObject(Items);
                if (!Rs.Status)
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(Rs.Result);
                    DtLog.StatusLog = ConstValue.LogError;
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(500), null, Rs.Message);
                }

                DtLog.Response = StringHelpers.PrepareJsonstring(Rs.Result);
                _appsLog.WriteAppsLog(DtLog);

                return Ok(Rs.Result);

                //return Requests.Response(this, new ApiStatus(200), Rs.Result, Rs.Message);
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
        [HttpGet("GetPrice")]
        public async Task<IActionResult> GetPrice(string SupplierID, string ItemID, long Qty)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                var Parm = new MPriceListVoucherGetPrice()
                {
                    supplierID = SupplierID,
                    itemID = ItemID,
                    Qty = Qty,
                };

                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(Parm);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _plVoucer.GetPrice(Parm);
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
        [HttpGet("GetPrice/{SupplierID}/{ItemID}/{Qty}")]
        public async Task<IActionResult> GetPriceRoute(string SupplierID, string ItemID, long Qty)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {                
                var Parm = new MPriceListVoucherGetPrice()
                {
                    supplierID = SupplierID,
                    itemID = ItemID,
                    Qty = Qty,
                };

                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(Parm);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _plVoucer.GetPrice(Parm);
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
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(long Id)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _plVoucer.DetailId(Id);
                if (Status)
                {
                    DtLog.Response = StringHelpers.PrepareJsonstring(Rs);
                    _appsLog.WriteAppsLog(DtLog);

                    return Requests.Response(this, new ApiStatus(200), Rs, Msg);
                }
                else
                {
                    DtLog.ErrorLog = StringHelpers.PrepareJsonstring(new { Detail =Msg});
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
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var (Status, Rs, Msg) = await _plVoucer.DetailIdExt(Id);
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
        public async Task<IActionResult> BulkAsync([FromBody] List<MPriceListVoucherDto> Items)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _plVoucer.BulkMany(GetUserId(), Items);
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
        public async Task<IActionResult> AddAsync([FromBody] MPriceListVoucherExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _plVoucer.Add(GetUserId(), Items);

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
        public async Task<IActionResult> UpdateAsync([FromBody] MPriceListVoucherExt Items)
        {
            var DtLog = new LogsDto();
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(Items);
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _plVoucer.Update(GetUserId(), Items);

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
            DtLog.UserId = GetUserId();
            DtLog.Action = Request.Method;
            DtLog.Module = Request.Path;
            DtLog.StatusLog = ConstValue.LogInformation;
            try
            {
                DtLog.Description = "ProcCRUDPriceListVoucher";
                DtLog.Request = StringHelpers.PrepareJsonstring(new { ID=Id});
                DtLog.FlagData = ConstValue.LogAdd;
                var RsLog = await _appsLog.WriteAppsLogAsync(DtLog);
                DtLog.Id = !RsLog.Status ? "0" : RsLog.Id;
                DtLog.FlagData = ConstValue.LogEdit;

                var Rs = await _plVoucer.Delete(GetUserId(), Id);

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
