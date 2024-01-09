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
    public class I_ItemServices : I_iTem
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;
        private readonly IHelpers helpers;
        private readonly IAppsLog appsLog;
        private readonly IMapper mapper;

        public I_ItemServices(IRepository _repository, IMapper _mapper, ILogger<Logs> _logger, IHelpers _helpers, IAppsLog _appsLog)
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
                    p.Add("@PageSize", GridLimit.Offset=="0"? GridLimit.Limit:GridLimit.Offset);
                    //p.Add("@PageSize", GridLimit.Offset.ToString());
                    p.Add("@PageNumber", "0");
                }
                else
                {
                    p.Add("@PageSize",  Items.Size == null ? "100" : Items.Size.ToString());
                    p.Add("@PageNumber", "0");
                }

                //var Rs = await repository.executeProcedure<object>("pI_Item_View", p);
                var Rs = await repository.executeProcedure<object>("pI_Item_View", p);
                Double TotalRows = Convert.ToDouble(Rs.Select(s => (IDictionary<string, object>)s).FirstOrDefault()["totalrows"]);
                Double PageSize = Convert.ToDouble(Items.Size == null ? "100" : Items.Size.ToString());

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
                        //TotalPage = p.Get<int>("TotalPage"),
                        //TotalRecords = p.Get<int>("totalrecords"),
                        TotalPage = Convert.ToInt64(Math.Ceiling(TotalRows / PageSize)),
                        TotalRecords = Convert.ToInt64(TotalRows),
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
                var Rs = await repository.executeProcedure<object>("pI_Item_View", p);

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
        public async Task<(bool Status, object Result, string Message)> DeleteById(CommonDelete Items) 
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@UserId", Items.UserId);
                p.Add("@Id", Items.Id);

                var Rs = await repository.executeProcedure<object>("pI_Item_Del", p);

                if (Rs.ToList().Where(w => w.ToString().Contains("Err")).Count() != 0)
                {
                    //return (false, null, Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim());
                    return (false, Rs.FirstOrDefault(),null);
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

        public async Task<(bool Status, object Result, string Message)> BulkUpdate(I_ItemBulk_Request Items)
        {
            try
            {
                string NameSp = "";
                var p = new DynamicParameters();
                p.Add("@UserId", Items.EntryUser);

                if (Items.JSONProcess) {
                    NameSp = "pI_Item_Bulk_json";
                    p.Add("@formdata", StringHelpers.PrepareJsonstring(Items));
                } 
                else {
                    DataSet ds = new DataSet();
                    if (Items.DataDetail == null || Items.DataDetail.Count == 0)
                    {
                        ds = GetItemUnit();
                    }
                    else
                    {
                        ds.Tables.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Items.DataDetail)));
                    }

                    p.Add("@ItemId", Items.ItemId);
                    p.Add("@ItemName", Items.ItemName);
                    p.Add("@ItemStructName", Items.ItemStructName);
                    p.Add("@CategoryId", Items.CategoryId);
                    p.Add("@ItemType", Items.ItemType);
                    p.Add("@Barcode1", Items.Barcode1);
                    p.Add("@Barcode2", Items.Barcode2);
                    p.Add("@Barcode3", Items.Barcode3);
                    p.Add("@Active", Items.Active);
                    p.Add("@BOM", Items.BOM);
                    p.Add("@BaseUOM", Items.BaseUOM);
                    p.Add("@QtyMultiplyOrder", Items.QtyMultiplyOrder);
                    p.Add("@OpenPrice", Items.OpenPrice);
                    p.Add("@Konversi", Items.Konversi);
                    p.Add("@PPN", Items.PPN);
                    p.Add("@StatusItem", Items.StatusItem);
                    p.Add("@Point", Items.Point);
                    p.Add("@OldItemId", Items.OldItemId);
                    p.Add("@UOMOrder", Items.UOMOrder);
                    p.Add("@MainSupplierId", Items.MainSupplierId);
                    p.Add("@StsSO", Items.StsSO);
                    p.Add("@StsPO", Items.StsPO);
                    p.Add("@FlagRetur", Items.FlagRetur);
                    p.Add("@FlagTukarGuling", Items.FlagTukarGuling);
                    p.Add("@FlagBKL", Items.FlagBKL);
                    p.Add("@KodeTag", Items.KodeTag);
                    p.Add("@idDivisi", Items.idDivisi);
                    p.Add("@idDepartement", Items.idDepartement);
                    p.Add("@idSubDepartement", Items.idSubDepartement);
                    p.Add("@idKategori", Items.idKategori);
                    p.Add("@ClassId", Items.ClassId);
                    p.Add("@idPajak", Items.idPajak);
                    p.Add("@idJenisProduk", Items.idJenisProduk);
                    p.Add("@idKemasan", Items.idKemasan);
                    p.Add("@RetDay", Items.RetDay);
                    p.Add("@itemUnit", ds.Tables[0].AsTableValuedParameter("tI_ItemUnit"));
                    //p.Add("@itemUnit", ds.Tables[0]);
                    NameSp = "pI_Item_Bulk";
                }

                var Rs = await repository.executeProcedure<object>(NameSp, p);

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

        public DataSet GetItemUnit()
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
