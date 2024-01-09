using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
    public class SoServices : ISo
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public SoServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
        {
            this.repository = _repository;
            this.logger = _logger;
            this.helpers = _helpers;
            this.appsLog = _appsLog;
            this.mapper = _mapper;
        }

        public async Task<(bool Status, object Result, string Message)> ListObject(ListPageExt Items)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@totalrecords", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@TotalPage", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@UserId", Items.UserId);
                if (!string.IsNullOrEmpty(Items.Search)) p.Add("@KeyWord", Items.Search);
                if (!string.IsNullOrEmpty(Items.OrderBy)) p.Add("@OrderBy", Items.OrderBy);

                var GridLimit = Items.request.QueryBuilder();
                if (GridLimit != null)
                {
                    //p.Add("@PageSize", GridLimit.Offset.ToString());
                    p.Add("@PageSize", GridLimit.Offset == "0" ? GridLimit.Limit : GridLimit.Offset);
                    p.Add("@PageNumber", "0");
                }
                else
                {
                    p.Add("@PageSize", Items.Size == null ? "100" : Items.Size.ToString());
                    p.Add("@PageNumber", "0");
                }

                var Rs = await repository.executeProcedure<object>("pT_SO_View", p);

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    //return (false, null, Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim());
                    return (false, Rs.FirstOrDefault() ,null);
                }
                else
                {
                    var Data = new CommonsResponse
                    {
                        Status = true,
                        Message = ConstValue.StatusOK,
                        CurrPage = 1,
                        TotalPage = p.Get<int>("TotalPage"),
                        TotalRecords = p.Get<int>("totalrecords"),
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
        public async Task<(bool Status, RsList Result, string Message)> ListObjectExt(ListPageExt Items)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@TotalPage", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@PageSize", Items.Size);
                p.Add("@PageNumber", Items.Page);
                p.Add("@UserId", Items.UserId);
                if (Items.OrderBy != "") p.Add("@OrderBy", Items.OrderBy);
                if (Items.Search != "") p.Add("@KeyWord", Items.Search);
                var Rs = await repository.executeProcedure<object>("pT_SO_View", p);

                var DataRs = new RsList();

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    DataRs.TotalRecords = 0;
                    DataRs.TotalPage = 0;
                    DataRs.Data = Rs.FirstOrDefault();
                    return (false, DataRs, null);
                }
                else
                {
                    DataRs.TotalRecords = p.Get<int>("TotalRecords");
                    DataRs.TotalPage = p.Get<int>("TotalPage");
                    DataRs.PageSize = p.Get<int>("PageSize");
                    DataRs.Data = Rs.ToList();
                    return (true, DataRs, null);
                }

            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }
        public async Task<(bool Status, object Result, string Message)> BulkUpdateSO(T_SO_Bulk Items)
        {
            try
            {
                DataSet ds = new DataSet();
                if (Items.DataDetail == null || Items.DataDetail.Count == 0)
                {
                    ds = GetSODetailDS();
                }
                else
                {
                    ds.Tables.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Items.DataDetail)));
                }

                var p = new DynamicParameters();
                p.Add("@SONo", Items.soNo);
                p.Add("@SODate", Items.soDate);
                p.Add("@ExpDate", Items.expDate);
                p.Add("@DeliveryDate", Items.deliveryDate);
                p.Add("@CustomerID", Items.customerID);
                p.Add("@CustomerName", Items.customerName);
                p.Add("@BranchID", Items.branchID);
                p.Add("@BranchName", Items.branchName);
                p.Add("@DelAddress", Items.delAddress);
                p.Add("@SalesmanID", Items.salesmanID);
                p.Add("@SalesmanName", Items.salesmanName);
                p.Add("@StsFreeGoods", Items.stsFreeGoods);
                p.Add("@StsPartial", Items.stsPartial);
                p.Add("@Remark", Items.remark);
                p.Add("@TotalTransaction", Items.totalTransaction);
                p.Add("@DiscAmount", Items.discAmount);
                p.Add("@TaxAmount", Items.taxAmount);
                p.Add("@FinalTotal", Items.finalTotal);
                p.Add("@StsActive", Items.stsActive);
                p.Add("@StsPosted", Items.stsPosted);
                p.Add("@UserId", Items.entryUser);
                p.Add("@dt", ds.Tables[0].AsTableValuedParameter("tT_SODetail"));

                var Rs = await repository.executeProcedure<object>("pT_SO_Bulk", p);

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    //return (false, null, Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim());
                    return (false, Rs.FirstOrDefault(), null);
                }
                else
                {
                    var Data = new CommonsResponse
                    {
                        Status = true,
                        Message = ConstValue.StatusOK,
                        CurrPage = 1,
                        TotalPage = 0,
                        TotalRecords = 0,
                        //Data = Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim(),
                        Data = Rs.FirstOrDefault(),
                    };

                    return (true, Data, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n " + ex.Message);
            }
        }

        public DataSet GetSODetailDS()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ItemId", typeof(string)));
            dt.Columns.Add(new DataColumn("UOM", typeof(string)));
            dt.Columns.Add(new DataColumn("Factor", typeof(float)));
            dt.Columns.Add(new DataColumn("Length", typeof(float)));
            dt.Columns.Add(new DataColumn("Width", typeof(float)));
            dt.Columns.Add(new DataColumn("Height", typeof(float)));
            dt.Columns.Add(new DataColumn("SizeUnit", typeof(string)));
            dt.Columns.Add(new DataColumn("Netto", typeof(float)));
            dt.Columns.Add(new DataColumn("Bruto", typeof(float)));
            dt.Columns.Add(new DataColumn("WeightUnit", typeof(string)));
            dt.Columns.Add(new DataColumn("UpdateUser", typeof(string)));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

    }
}
