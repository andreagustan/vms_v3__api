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
    public class T_PermintaanBarangServices : IT_PermintaanBarang
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public T_PermintaanBarangServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
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

                var Rs = await repository.executeProcedure<object>("pT_PermintaanBarang_View", p);

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    //return (false, null, Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim());
                    return (false, Rs.ToList().FirstOrDefault(), null);
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

        public async Task<(bool Status, object Result, string Message)> BulkUpdate(T_PermintaanBarangRequest Items)
        {
            try
            {
                DataSet ds = new DataSet();
                if (Items.DataDetail == null || Items.DataDetail.Count == 0)
                {
                    ds = GetPermintaanBarangDetailDS();
                }
                else
                {
                    ds.Tables.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Items.DataDetail)));
                }

                var p = new DynamicParameters();
                p.Add("@PBNo", Items.PBNo);
                p.Add("@PBDate", Items.PBDate);
                p.Add("@FromBranchID", Items.FromBranchID);
                p.Add("@FromBranchName", Items.FromBranchName);
                p.Add("@ToBranchID", Items.ToBranchID);
                p.Add("@ToBranchName", Items.ToBranchName);
                p.Add("@Total", Items.Total);
                p.Add("@Remark", Items.Remark);
                p.Add("@TransNo", Items.TransNo);
                p.Add("@UserId", Items.EntryUser);
                p.Add("@StsPosted", Items.StsPosted);
                p.Add("@ExpireDate", Items.ExpireDate);
                p.Add("@StsTransfer", Items.StsTransfer);
                //p.Add("@Source", Items.Source);
                p.Add("@dt", ds.Tables[0].AsTableValuedParameter("tT_PermintaanBarangDetail"));

                var Rs = await repository.executeProcedure<object>("pT_PermintaanBarang_Bulk", p);

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    //return (false, null, Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim());
                    return (false, Rs.ToList().FirstOrDefault(), null);
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

        public DataSet GetPermintaanBarangDetailDS()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID", typeof(Int64)));
            dt.Columns.Add(new DataColumn("PBNo", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemID", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemName", typeof(string)));
            dt.Columns.Add(new DataColumn("Qty", typeof(float)));
            dt.Columns.Add(new DataColumn("SOH", typeof(float)));
            dt.Columns.Add(new DataColumn("AvgDay", typeof(float)));
            dt.Columns.Add(new DataColumn("LeadTime", typeof(float)));
            dt.Columns.Add(new DataColumn("SCD", typeof(float)));
            dt.Columns.Add(new DataColumn("MaxStore", typeof(int)));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

    }
}
