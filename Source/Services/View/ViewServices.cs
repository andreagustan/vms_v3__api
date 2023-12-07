using AutoMapper;
using Dapper;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VMS.Data;
using VMS.Entities;
using VMS.Interface;
using static VMS.Entities.Commons;

namespace VMS.Services
{
    public class ViewServices : IView
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public ViewServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
        {
            this.repository = _repository;
            this.logger = _logger;
            this.helpers = _helpers;
            this.appsLog = _appsLog;
            this.mapper = _mapper;
        }

        public async Task<(bool Status, object Result, string Message)> ListObject(ListCombo Items)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Code", Items.Code, DbType.String, size: int.MaxValue);
                p.Add("@Search", Items.Search ?? "", DbType.String, size: int.MaxValue);
                p.Add("@Page", Items.Page ?? 1, DbType.Int64);
                p.Add("@Row", Items.Size ?? 20, DbType.Int64);
                p.Add("@OrderBy", Items.OrderBy ?? "", DbType.String, size: 100);
                p.Add("@UserId", Items.UserId ?? "", DbType.String, size: 100);
                p.Add("@filterdata", Items.FilterData ?? "" , DbType.String, size: int.MaxValue);
                //p.Add("@totalrecords", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //p.Add("@TotalPage", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //p.Add("@UserId", Items.UserId);
                //if (!string.IsNullOrEmpty(Items.Search)) p.Add("@KeyWord", Items.Search);
                //if (!string.IsNullOrEmpty(Items.OrderBy)) p.Add("@OrderBy", Items.OrderBy);

                //var GridLimit = Items.request.GridRequest();
                //if (GridLimit != null)
                //{
                //    p.Add("@PageSize", GridLimit.offset.ToString());
                //    p.Add("@PageNumber", "0");
                //}
                //else
                //{
                //    p.Add("@PageSize", Items.Size == null ? "100" : Items.Size.ToString());
                //    p.Add("@PageNumber", "0");
                //}

                var Rs = await repository.executeProcedure<object>("getComboBox", p);

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    return (false, null, Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim());
                }
                else
                {
                    var Data = new CommonsResponse
                    {
                        Status = true,
                        Message = ConstValue.StatusOK,
                        CurrPage = 1,
                        //TotalPage = p.Get<int>("TotalPage"),
                        //TotalRecords = p.Get<int>("totalrecords"),
                        Data = Rs.ToList(),
                    };

                    return (true, Data, null);
                }


            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n " + ex.Message);
            }
        }

    }
}
