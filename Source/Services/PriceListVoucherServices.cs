using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VMS.Data;
using VMS.Entities;
using VMS.Interface;

namespace VMS.Services
{
    public class PriceListVoucherServices : IPriceListVoucher
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public PriceListVoucherServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
        {
            this.repository = _repository;
            this.logger = _logger;
            this.helpers = _helpers;
            this.appsLog = _appsLog;
            this.mapper = _mapper;
        }

        public async Task<(bool Status, List<MPriceListVoucher> Result, string Message)> List(ListPageDetail Items)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@totalrecords", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@totalrecordFilter", dbType: DbType.Int32, direction: ParameterDirection.Output);
                if (!string.IsNullOrEmpty(Items.KeyHeader)) p.Add("@KeyHeader", Items.KeyHeader);
                if (!string.IsNullOrEmpty(Items.JSONFilter)) p.Add("@JsonFilter", Items.JSONFilter);

                var Rs = await repository.executeProcedure<MPriceListVoucher>("ProcCRUDPriceListVoucher", p);

                return (true, Rs.ToList(), null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        public async Task<(bool Status, RsList Result, string Message)> ListObject(ListPageExt Items)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@TotalPage", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //p.Add("@PageSize", Items.PageSize);
                p.Add("@PageSize", Items.Size);
                //p.Add("@PageNumber", Items.PageNumber ?? "0");
                p.Add("@PageNumber", Items.Page);
                p.Add("@UserId", Items.UserId);
                if (Items.OrderBy!="") p.Add("@OrderBy", Items.OrderBy);
                if (Items.Search!="") p.Add("@KeyWord", Items.Search);
                //if (!string.IsNullOrEmpty(Items.KeyHeader)) p.Add("@KeyHeader", Items.KeyHeader);
                //if (!string.IsNullOrEmpty(Items.JSONFilter)) p.Add("@JsonFilter", Items.JSONFilter);

                //var Rs = await repository.executeProcedure<object>("ProcCRUDPriceListVoucher", p);
                var Rs = await repository.executeProcedure<object>("pM_PriceListVoucher_View", p);

                var DataRs = new RsList();

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    DataRs.TotalRecords = 0;
                    DataRs.TotalPage = 0;
                    DataRs.Data = Rs.FirstOrDefault();
                    //return (false, Rs.FirstOrDefault(), null);
                    return (false, DataRs, null);
                }
                else
                {
                    //var Data = new
                    //{
                    //    TotalRecords = p.Get<int>("TotalRecords"),
                    //    TotalPage = p.Get<int>("TotalPage"),
                    //    Data = Rs.ToList(),

                    //};

                    //return (true, Data, null);
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

        public async Task<(bool Status, MPriceListVoucher Result, string Message)> DetailId(long Id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Field", "GetById");
                p.Add("@KeyWord", Id);
                p.Add("@totalrecords", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@totalrecordFilter", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var Rs = (await repository.executeProcedure<MPriceListVoucher>("ProcCRUDPriceListVoucher", p)).FirstOrDefault();

                return (true, Rs, null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        public async Task<(bool Status, object Result, string Message)> DetailIdExt(long Id)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Field", "GetById");
                p.Add("@KeyWord", Id);
                p.Add("@totalrecords", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@totalrecordFilter", dbType: DbType.Int32, direction: ParameterDirection.Output);

                var Rs = (await repository.executeProcedure<object>("ProcCRUDPriceListVoucher", p)).FirstOrDefault();

                return (true, Rs, null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        public async Task<(bool Status, float? Result, string Message)> GetPrice(MPriceListVoucherGetPrice Items)
        {
            try
            {
                var param = new Dictionary<string, object>()
                {
                    { "itemId", Items.itemID },
                    { "supplierID", Items.supplierID},
                    { "Qty", Items.Qty},
                    { "Field", "GetPrice"},
                    { "totalrecords", 0},
                    { "totalrecordFilter", 0},
                };

                var Rs = await Task.Run(() => repository.ExecSPToDataTable("ProcCRUDPriceListVoucher", param));

                return (true, Rs.Rows[0].ItemArray[0].ToFloat(), null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        public async Task<(bool Status, string Result, string Message)> Add(string CurrentUserId, MPriceListVoucherExt Items)
        {
            try
            {
                var DtBulkAdd = mapper.Map<MPriceListVoucherDto>(Items);
                DtBulkAdd.FlagData = "Add";

                var DtBulk = StringHelpers.PrepareJsonstring(DtBulkAdd);

                var param = new Dictionary<string, object>()
                {
                    { "UserId", CurrentUserId },
                    { "JsonData", DtBulk},
                    { "totalrecords", 0},
                    { "totalrecordFilter", 0},
                };

                var Rs = await Task.Run(() => repository.ExecSPToDataTable("ProcCRUDPriceListVoucher", param));

                return (true, Rs.Rows[0].ItemArray[0].ToString(), null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        public async Task<(bool Status, string Result, string Message)> Update(string CurrentUserId, MPriceListVoucherExt Items)
        {
            try
            {
                var DtBulkAdd = mapper.Map<MPriceListVoucherDto>(Items);
                DtBulkAdd.FlagData = "Edit";

                var DtBulk = StringHelpers.PrepareJsonstring(DtBulkAdd);

                var param = new Dictionary<string, object>()
                {
                    { "UserId", CurrentUserId },
                    { "JsonData", DtBulk},
                    { "totalrecords", 0},
                    { "totalrecordFilter", 0},
                };

                var Rs = await Task.Run(() => repository.ExecSPToDataTable("ProcCRUDPriceListVoucher", param));

                return (true, Rs.Rows[0].ItemArray[0].ToString(), null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        public async Task<(bool Status, string Result, string Message)> Delete(string CurrentUserId, long Id)
        {
            try
            {
                var DtBulkDelete = new MPriceListVoucherDto()
                {
                    FlagData = "Delete",
                    Id = Id,
                };

                var DtBulk = StringHelpers.PrepareJsonstring(DtBulkDelete);

                var param = new Dictionary<string, object>()
                {
                    { "UserId", CurrentUserId },
                    { "JsonData", DtBulk},
                    { "totalrecords", 0},
                    { "totalrecordFilter", 0},
                };

                var Rs = await Task.Run(() => repository.ExecSPToDataTable("ProcCRUDPriceListVoucher", param));

                return (true, Rs.Rows[0].ItemArray[0].ToString(), null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }

        public async Task<(bool Status, string Result, string Message)> BulkMany(string CurrentUserId, List<MPriceListVoucherDto> Items)
        {
            try
            {
                var DtBulk = StringHelpers.PrepareJsonstring(Items);

                var param = new Dictionary<string, object>()
                {
                    { "UserId", CurrentUserId },
                    { "JsonData", DtBulk},
                    { "totalrecords", 0},
                    { "totalrecordFilter", 0},
                };

                var Rs = await Task.Run(() => repository.ExecSPToDataTable("ProcCRUDPriceListVoucher", param));

                return (true, Rs.Rows[0].ItemArray[0].ToString(), null);
            }
            catch (Exception ex)
            {
                return (false, null, "Trouble happened! \n" + ex.Message);
            }
        }
    }
}
