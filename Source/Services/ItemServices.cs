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

                var Rs = await repository.executeProcedure<object>("pI_Item_View", p);

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
                        Data = Rs.FirstOrDefault().ToString().Split("=")[1].Replace("'", "").Replace("}", "").Trim(),
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
                DataSet ds = new DataSet();
                if (Items.dataUOM == null || Items.dataUOM.Count == 0) {
                    ds = GetItemUnit();
                }
                else {
                    ds.Tables.Add(JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(Items.dataUOM)));
                }

                var p = new DynamicParameters();
                p.Add("@ItemId", Items.dataItem.ItemId);
                p.Add("@ItemName", Items.dataItem.ItemName);
                p.Add("@ItemStructName", Items.dataItem.ItemStructName);
                p.Add("@CategoryId", Items.dataItem.CategoryId);
                p.Add("@ItemType", Items.dataItem.ItemType);
                p.Add("@Barcode1", Items.dataItem.Barcode1);
                p.Add("@Barcode2", Items.dataItem.Barcode2);
                p.Add("@Barcode3", Items.dataItem.Barcode3);
                p.Add("@Active", Items.dataItem.Active);
                p.Add("@BOM", Items.dataItem.BOM);
                p.Add("@BaseUOM", Items.dataItem.BaseUOM);
                p.Add("@QtyMultiplyOrder", Items.dataItem.QtyMultiplyOrder);
                p.Add("@OpenPrice", Items.dataItem.OpenPrice);
                p.Add("@Konversi", Items.dataItem.Konversi);
                p.Add("@PPN", Items.dataItem.PPN);
                p.Add("@StatusItem", Items.dataItem.StatusItem);
                p.Add("@Point", Items.dataItem.Point);
                p.Add("@UserId", Items.EntryUser);
                p.Add("@OldItemId", Items.dataItem.OldItemId);
                p.Add("@UOMOrder", Items.dataItem.UOMOrder);
                p.Add("@MainSupplierId", Items.dataItem.MainSupplierId);
                p.Add("@StsSO", Items.dataItem.StsSO);
                p.Add("@StsPO", Items.dataItem.StsPO);
                p.Add("@FlagRetur", Items.dataItem.FlagRetur);
                p.Add("@FlagTukarGuling", Items.dataItem.FlagTukarGuling);
                p.Add("@FlagBKL", Items.dataItem.FlagBKL);
                p.Add("@KodeTag", Items.dataItem.KodeTag);
                p.Add("@idDivisi", Items.dataItem.idDivisi);
                p.Add("@idDepartement", Items.dataItem.idDepartement);
                p.Add("@idSubDepartement", Items.dataItem.idSubDepartement);
                p.Add("@idKategori", Items.dataItem.idKategori);
                p.Add("@ClassId", Items.dataItem.ClassId);
                p.Add("@idPajak", Items.dataItem.idPajak);
                p.Add("@idJenisProduk", Items.dataItem.idJenisProduk);
                p.Add("@idKemasan", Items.dataItem.idKemasan);
                p.Add("@RetDay", Items.dataItem.RetDay);
                p.Add("@itemUnit", ds.Tables[0].AsTableValuedParameter("tI_ItemUnit"));
                //p.Add("@itemUnit", ds.Tables[0]);

                var Rs = await repository.executeProcedure<object>("pI_Item_Bulk", p);

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
