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
    public class T_TransferServices : IT_Transfer
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public T_TransferServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
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
                    p.Add("@PageSize", GridLimit.Offset.ToString());
                    p.Add("@PageNumber", "0");
                }
                else
                {
                    p.Add("@PageSize", Items.Size == null ? "100" : Items.Size.ToString());
                    p.Add("@PageNumber", "0");
                }

                var Rs = await repository.executeProcedure<object>("pT_Transfer_View", p);

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
                var Rs = await repository.executeProcedure<object>("pT_Transfer_View", p);

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
        public async Task<(bool Status, object Result, string Message)> BulkUpdate(T_TransferRequest Items)
        {
            try
            {
                DataSet ds = new DataSet();
                if (Items.DataDetail == null || Items.DataDetail.Count == 0)
                {
                    ds = GetTransferDetailDS();
                }
                else
                {
                    ds.Tables.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Items.DataDetail)));
                }

                var p = new DynamicParameters();
                p.Add("@TransNo", Items.TransNo);
                p.Add("@TransDate", Items.TransDate);
                p.Add("@BranchIDKirim", Items.BranchIDKirim);
                p.Add("@BranchNameKirim", Items.BranchNameKirim);
                p.Add("@DeliveryBy", Items.DeliveryBy);
                p.Add("@VehicleNo", Items.VehicleNo);
                p.Add("@Amount", Items.Amount);
                p.Add("@Remark", Items.Remark);
                p.Add("@TransTerimaDate", Items.TransTerimaDate);
                p.Add("@BranchIDTerima", Items.BranchIDTerima);
                p.Add("@BranchNameTerima", Items.BranchNameTerima);
                p.Add("@ReceiveBy", Items.ReceiveBy);
                p.Add("@UserId", Items.EntryUser);
                p.Add("@StsPosted", Items.StsPosted);
                p.Add("@StsApprove", Items.StsApprove);
                p.Add("@StsCancel", Items.StsCancel);
                p.Add("@PBNo", Items.PBNo);
                p.Add("@SLocID", Items.SLocID);
                p.Add("@JenisKirim", Items.JenisKirim);
                p.Add("@dt", ds.Tables[0].AsTableValuedParameter("tT_TransferDetail"));

                var Rs = await repository.executeProcedure<object>("pT_Transfer_Bulk", p);

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

        public DataSet GetTransferDetailDS()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID", typeof(Int64)));
            dt.Columns.Add(new DataColumn("TransNo", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemID", typeof(string)));
            dt.Columns.Add(new DataColumn("ItemName", typeof(string)));
            dt.Columns.Add(new DataColumn("UOM", typeof(string)));
            dt.Columns.Add(new DataColumn("Qty", typeof(float)));
            dt.Columns.Add(new DataColumn("COGS", typeof(float)));
            dt.Columns.Add(new DataColumn("SubTotal", typeof(float)));
            dt.Columns.Add(new DataColumn("ExportedFileNo", typeof(string)));
            dt.Columns.Add(new DataColumn("UploadDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("ExportedFileNo2", typeof(string)));
            dt.Columns.Add(new DataColumn("SLocIDKirim", typeof(string)));
            dt.Columns.Add(new DataColumn("SLocIDTerima", typeof(string)));
            dt.Columns.Add(new DataColumn("ExpireDate", typeof(string)));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

    }
}
