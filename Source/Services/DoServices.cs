﻿using AutoMapper;
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
    public class DoServices : IDo
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public DoServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
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

                var GridLimit = Items.request.GridRequest();
                if (GridLimit != null)
                {
                    p.Add("@PageSize", GridLimit.offset.ToString());
                    p.Add("@PageNumber", "0");
                }
                else
                {
                    p.Add("@PageSize", Items.Size == null ? "100" : Items.Size.ToString());
                    p.Add("@PageNumber", "0");
                }

                var Rs = await repository.executeProcedure<object>("pT_DO_View", p);

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

        public async Task<(bool Status, object Result, string Message)> BulkUpdate(T_DORequest Items)
        {
            try
            {
                DataSet ds = new DataSet();
                if (Items.DataDetail == null || Items.DataDetail.Count == 0)
                {
                    ds = GetDODetailDS();
                }
                else
                {
                    ds.Tables.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Items.DataDetail)));
                }

                var p = new DynamicParameters();
                p.Add("@DONo", Items.DONo);
                p.Add("@DODate", Items.DODate);
                p.Add("@CustomerID", Items.CustomerID);
                p.Add("@CustomerName", Items.CustomerName);
                p.Add("@BranchID", Items.BranchID);
                p.Add("@BranchName", Items.BranchName);
                p.Add("@SONo", Items.SONo);
                p.Add("@SODate", Items.SODate);
                p.Add("@RefNo", Items.RefNo);
                p.Add("@DeliveryBy", Items.DeliveryBy);
                p.Add("@VehicleNo", Items.VehicleNo);
                p.Add("@Remark", Items.Remark);
                p.Add("@TotalTransaction", Items.TotalTransaction);
                p.Add("@DiscRate", Items.DiscRate);
                p.Add("@DiscAmount", Items.DiscAmount);
                p.Add("@DiscAmount1", Items.DiscAmount1);
                p.Add("@TaxRate", Items.TaxRate);
                p.Add("@TaxAmount", Items.TaxAmount);
                p.Add("@FinalTotal", Items.FinalTotal);
                p.Add("@StsActive", Items.StsActive);
                p.Add("@StsSReturn", Items.StsSReturn);
                p.Add("@StsSInv", Items.StsSInv);
                p.Add("@StsPosted", Items.StsPosted);
                p.Add("@UserId", Items.EntryUser);
                p.Add("@SLocID", Items.SLocID);
                p.Add("@perioddate", Items.perioddate);
                p.Add("@SetoranTunaiId", Items.SetoranTunaiId);
                p.Add("@Source", Items.Source);
                p.Add("@dt", ds.Tables[0].AsTableValuedParameter("tT_DODetail"));

                var Rs = await repository.executeProcedure<object>("pT_DO_Bulk", p);

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

        public DataSet GetDODetailDS()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID", typeof(Int64)));
            dt.Columns.Add(new DataColumn("DONo", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemID", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemName", typeof(string)));
            dt.Columns.Add(new DataColumn("UOM", typeof(string)));
            dt.Columns.Add(new DataColumn("Qty", typeof(float)));
            dt.Columns.Add(new DataColumn("Price", typeof(float)));
            dt.Columns.Add(new DataColumn("DiscRate1", typeof(float)));
            dt.Columns.Add(new DataColumn("DiscRate2", typeof(float)));
            dt.Columns.Add(new DataColumn("DiscAmount", typeof(float)));
            dt.Columns.Add(new DataColumn("SubTotal", typeof(float)));
            dt.Columns.Add(new DataColumn("COGS", typeof(float)));
            dt.Columns.Add(new DataColumn("SLocID", typeof(string)));
            dt.Columns.Add(new DataColumn("ExpireDate", typeof(string)));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

    }
}
