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
    public class GrServices : IGr
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public GrServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
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

                var Rs = await repository.executeProcedure<object>("pT_GR_View", p);

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
                var Rs = await repository.executeProcedure<object>("pT_GR_View", p);

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
        public async Task<(bool Status, object Result, string Message)> BulkUpdate(T_GRRequest Items) 
        {
            try
            {
                DataSet ds = new DataSet();
                if (Items.DataDetail == null || Items.DataDetail.Count == 0)
                {
                    ds = GetGRDetailDS();
                }
                else
                {
                    ds.Tables.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Items.DataDetail)));
                }

                var p = new DynamicParameters();
                p.Add("@GRNo", Items.GRNo);
                p.Add("@GRDate", Items.GRDate);
                p.Add("@SupplierID", Items.SupplierID);
                p.Add("@SupplierName", Items.SupplierName);
                p.Add("@BranchID", Items.BranchID);
                p.Add("@BranchName", Items.BranchName);
                p.Add("@PONo", Items.PONo);
                p.Add("@PODate", Items.PODate);
                p.Add("@RefNo", Items.RefNo);
                p.Add("@DeliveryBy", Items.DeliveryBy);
                p.Add("@VehicleNo", Items.VehicleNo);
                p.Add("@ReceiveBy", Items.ReceiveBy);
                p.Add("@Remark", Items.Remark);
                p.Add("@TotalTransaction", Items.TotalTransaction);
                p.Add("@DiscRate", Items.DiscRate);
                p.Add("@DiscAmount", Items.DiscAmount);
                p.Add("@DiscAmount1", Items.DiscAmount1);
                p.Add("@TaxRate", Items.TaxRate);
                p.Add("@TaxAmount", Items.TaxAmount);
                p.Add("@FinalTotal", Items.FinalTotal);
                p.Add("@StsActive", Items.StsActive);
                p.Add("@StsPReturn", Items.StsPReturn);
                p.Add("@StsPInv", Items.StsPInv);
                p.Add("@StsPosted", Items.StsPosted);
                p.Add("@UserId", Items.EntryUser);
                p.Add("@SLocID", Items.SLocID);
                p.Add("@ReturnTypeId", Items.ReturnTypeId);
                p.Add("@dt", ds.Tables[0].AsTableValuedParameter("tT_GRDetail"));

                var Rs = await repository.executeProcedure<object>("pT_GR_Bulk", p);

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
                        Data = Rs.ToList().FirstOrDefault(),
                    };

                    return (true, Data, null);
                }
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n " + ex.Message);
            }
        }

        public DataSet GetGRDetailDS()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID", typeof(Int64)));
            dt.Columns.Add(new DataColumn("GRNo", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemID", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemName", typeof(string)));
            dt.Columns.Add(new DataColumn("UOM", typeof(string)));
            dt.Columns.Add(new DataColumn("Qty", typeof(float)));
            dt.Columns.Add(new DataColumn("Price", typeof(float)));
            dt.Columns.Add(new DataColumn("DiscRate1", typeof(float)));
            dt.Columns.Add(new DataColumn("DiscRate2", typeof(float)));
            dt.Columns.Add(new DataColumn("DiscAmount", typeof(float)));
            dt.Columns.Add(new DataColumn("SubTotal", typeof(float)));
            dt.Columns.Add(new DataColumn("SLocID", typeof(string)));
            dt.Columns.Add(new DataColumn("ExpireDate", typeof(string)));
            dt.Columns.Add(new DataColumn("ppn", typeof(int)));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }
    }
}
