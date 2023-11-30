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

    public class PriceListVoucherController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly IPriceListVoucher _plVoucer;

        public PriceListVoucherController(IRepository repository, IMapper mapper, IAppsLog appsLog, IPriceListVoucher plVoucer, IHelpers helpers)
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
            try
            {
                var GridLimit = request.GridRequest();
                if (GridLimit != null)
                    Items.PageSize = GridLimit.offset.ToString();

                var Rs = await _plVoucer.List(Items);
                if (!Rs.Status)
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }

                var FieldNya = "supplierID,itemID,beginQty,endQty,price";

                var ItemPage = _mapper.Map<ListPage>(Items);

                var (Generated, Message, recordsTotal, recordsFilteredTotal, dataReturn, colsName) = await _repository.GenerateDataForDatatableExtAsync(Rs.Result.AsQueryable(), FieldNya, ItemPage);

                return Requests.Response(this, new ApiStatus(200), new ApiDatatableResponse(recordsTotal, recordsFilteredTotal, dataReturn, string.Join(", ", colsName)).Result, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("ListPageExt")]
        public async Task<IActionResult> GetListPageExt([FromBody] ListPageDetail Items, string request)
        {
            try
            {
                var GridLimit = request.GridRequest();
                if (GridLimit != null)
                    Items.PageSize = GridLimit.offset.ToString();

                var Rs = await _plVoucer.ListObject(Items);
                if (!Rs.Status)
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }

                return Requests.Response(this, new ApiStatus(200), Rs.Result, Rs.Message);
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetPrice")]
        public async Task<IActionResult> GetPrice(string SupplierID, string ItemID, long Qty)
        {
            try
            {
                var Parm = new MPriceListVoucherGetPrice()
                {
                    supplierID = SupplierID,
                    itemID = ItemID,
                    Qty = Qty,
                };
                var (Status, Rs, Msg) = await _plVoucer.GetPrice(Parm);
                if (Status)
                {
                    return Requests.Response(this, new ApiStatus(200), Rs, Msg);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(400), null, Msg);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetPrice/{SupplierID}/{ItemID}/{Qty}")]
        public async Task<IActionResult> GetPriceRoute(string SupplierID, string ItemID, long Qty)
        {
            try
            {
                var Parm = new MPriceListVoucherGetPrice()
                {
                    supplierID = SupplierID,
                    itemID = ItemID,
                    Qty = Qty,
                };
                var (Status, Rs, Msg) = await _plVoucer.GetPrice(Parm);
                if (Status)
                {
                    return Requests.Response(this, new ApiStatus(200), Rs, Msg);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(400), null, Msg);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(long Id)
        {
            try
            {
                var (Status, Rs, Msg) = await _plVoucer.DetailId(Id);
                if (Status)
                {
                    return Requests.Response(this, new ApiStatus(200), Rs, Msg);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(400), null, Msg);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("GetByIdExt/{id:int}")]
        public async Task<IActionResult> GetByIdExtAsync(long Id)
        {
            try
            {
                var (Status, Rs, Msg) = await _plVoucer.DetailIdExt(Id);
                if (Status)
                {
                    return Requests.Response(this, new ApiStatus(200), Rs, Msg);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(400), null, Msg);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("BulkMany")]
        public async Task<IActionResult> BulkAsync([FromBody] List<MPriceListVoucherDto> Items)
        {
            try
            {
                var Rs = await _plVoucer.BulkMany(GetUserId(), Items);
                if (!Rs.Status)
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync([FromBody] MPriceListVoucherExt Items)
        {
            try
            {
                var Rs = await _plVoucer.Add(GetUserId(), Items);

                if (!Rs.Status)
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("Edit")]
        public async Task<IActionResult> UpdateAsync([FromBody] MPriceListVoucherExt Items)
        {
            try
            {
                var Rs = await _plVoucer.Update(GetUserId(), Items);

                if (!Rs.Status)
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpDelete("Delete/{Id:int}")]
        public async Task<IActionResult> DeleteAsync(long Id)
        {
            try
            {
                var Rs = await _plVoucer.Delete(GetUserId(), Id);

                if (!Rs.Status)
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }
                else
                {
                    return Requests.Response(this, new ApiStatus(200), Rs.Result, null);
                }
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        
    }
}
