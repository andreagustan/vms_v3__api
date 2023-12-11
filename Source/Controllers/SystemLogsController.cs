using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VMS.Data;
using VMS.Entities;
using VMS.Error;
using VMS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Controllers
{
    public partial class SystemLogsController : BaseApiController
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHelpers _helper;
        private readonly IAppsLog _appsLog;

        public SystemLogsController(IRepository repository, IMapper mapper, IAppsLog appsLog, IHelpers helpers)
        {
            _repository = repository;
            _mapper = mapper;
            _appsLog = appsLog;
            _helper = helpers;
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpPost("list")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var param = new Dictionary<string, object>()
                {
                    { "FlagData", "List" }
                };

                var Rs = _repository.ExecSPToList<SystemLogsList>("ProcCRUDSystemLogs", param);

                return Requests.Response(this, new ApiStatus(200), Rs, "");
            }
            catch (Exception ex)
            {
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

        [TypeFilter(typeof(CustomAuthorizationFilter))]
        [Produces("application/json")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var param = new Dictionary<string, object>()
                {
                    { "FlagData", "Detail" },
                    { "Id", id },
                };
                                
                var Rs = _repository.ExecSPToList<SystemLogsList>("ProcCRUDSystemLogs", param);

                return Requests.Response(this, new ApiStatus(200), Rs, "");
            }
            catch (Exception ex)
            {                
                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
            }
        }

    }   
}
