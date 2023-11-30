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

    public class CustomerDetailController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;
        private readonly ICustomerDetail _customerDetail;

        public CustomerDetailController(IRepository repository, IMapper mapper, IAppsLog appsLog, ICustomerDetail customerDetail, IHelpers helpers)
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
            try
            {
                var GridLimit = request.GridRequest();
                if (GridLimit != null)
                    Items.PageSize = GridLimit.offset.ToString();

                var Rs = await _customerDetail.List(Items);
                if (!Rs.Status) 
                {
                    return Requests.Response(this, new ApiStatus(400), null, Rs.Message);
                }

                var FieldNya = "customerID,customerName,subCustomerID,subCustomerName";
                                
                var (Generated, Message, recordsTotal, recordsFilteredTotal, dataReturn, colsName) = await _repository.GenerateDataForDatatableExtAsync(Rs.Result.AsQueryable(), FieldNya, Items);
                
                return Requests.Response(this, new ApiStatus(200), new ApiDatatableResponse(recordsTotal, recordsFilteredTotal, dataReturn, string.Join(", ", colsName)).Result, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        //[Authorize]
        [Produces("application/json")]
        [HttpPost("ListPageExt")]
        public async Task<IActionResult> GetListPageExt([FromBody] ListPage Items, string request)
        {
            try
            {

                var GridLimit = request.GridRequest();
                if (GridLimit != null)
                    Items.PageSize = GridLimit.offset.ToString();
                var Rs = await _customerDetail.ListObject(Items);
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
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(long Id)
        {
            try
            {
                var (Status, Rs, Msg) = await _customerDetail.GetById(Id);
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
                var (Status, Rs, Msg) = await _customerDetail.GetByIdExt(Id);
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
        public async Task<IActionResult> BulkAsync([FromBody] List<MCustomerDetailDto> Items)
        {
            try
            {
                var Rs = await _customerDetail.BulkMany(GetUserId(), Items);
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
        public async Task<IActionResult> AddAsync([FromBody] MCustomerDetailExt Items)
        {
            try
            {
                var Rs = await _customerDetail.Add(GetUserId(), Items);

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
        public async Task<IActionResult> UpdateAsync([FromBody] MCustomerDetailExt Items)
        {
            try
            {
                var Rs = await _customerDetail.Update(GetUserId(), Items);

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
                var Rs = await _customerDetail.Delete(GetUserId(), Id);

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
