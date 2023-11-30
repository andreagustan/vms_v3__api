using AutoMapper;
using Dapper;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NinjaNye.SearchExtensions;
using VMS.Entities;
using VMS.Interface;
using VMS.SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace VMS.Data
{
    public class Repository: IRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<Logs> _logger;
        private readonly IMapper _mapper;
        private IDbContextTransaction _transaction;

        public Repository(AppDbContext dbContext, ILogger<Logs> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public void BeginTransaction()
        {
            this._transaction = _dbContext.Database.BeginTransaction();

        }

        public async void BeginTransactionAsync()
        {
            this._transaction = await _dbContext.Database.BeginTransactionAsync();

        }
        public async Task<(bool Commited, string Message)> RollbackAsync()
        {
            try
            {
                await _transaction.RollbackAsync();
                //var r = await _dbContext.SaveChangesAsync();
                String msgSuccess = null;

                msgSuccess = "Commited";

                _logger.LogInformation(msgSuccess);
                return (true, msgSuccess);

            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }
        public async Task<(bool Commited, string Message)> CommitSync()
        {
            try
            {
                await _transaction.CommitAsync();
                String msgSuccess = null;

                msgSuccess = "Commited";

                _logger.LogInformation(msgSuccess);
                return (true, msgSuccess);

            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }

        public async Task<(bool Commited, string Message)> CommitManySync()
        {
            try
            {

                var r = await _dbContext.SaveChangesAsync();
                String msgSuccess = null;

                msgSuccess = "Commited";

                _logger.LogInformation(msgSuccess);
                return (true, msgSuccess);

            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }

        public (bool Commited, string Message) Commit()
        {
            try
            {
                _transaction.Commit();
                String msgSuccess = null;

                msgSuccess = "Commited";

                _logger.LogInformation(msgSuccess);
                return (true, msgSuccess);

            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }

        public async void AddBulkAsync<T>(IList<T> entities) where T : BaseEntity
        {
            await _dbContext.BulkInsertAsync<T>(entities);
            _dbContext.SaveChanges();


        }

        public async Task AddBulkTaskAsync<T>(IList<T> entities) where T : BaseEntity
        {
            await _dbContext.BulkInsertAsync<T>(entities);
            _dbContext.SaveChanges();


        }

        public async Task<(bool Generated, string Message, int recordsTotal, int recordsFilteredTotal, List<T> data, List<string> colsName)>
            GenerateDataForDatatableExtAsync<T>(IQueryable<T> basisData, string FieldNya, ListPage request, params Expression<Func<T, object>>[] includes) where T : BaseEntity
        {
            try
            {
                if (request == null)
                {
                    throw new Exception();
                }

                var searchValue = request.Keyword;
                searchValue ??= ""; searchValue = searchValue.Trim().ToLower();

                var field = request.fields;
                field ??= ""; field = field.Trim();
                var colsName = (field == "" ? FieldNya : field).Split(",").ToList();

                var SearchColListOri = new List<string>();
                var SearchList = new List<string>();
                int z = 0;
                foreach (var col in colsName)
                {
                    SearchColListOri.Add("columns[" + z + "]");
                    z++;
                }

                foreach (var c in SearchColListOri)
                {
                    if (!(string.IsNullOrWhiteSpace(searchValue) || String.IsNullOrEmpty(searchValue)))
                    {
                        SearchList.Add(searchValue);
                    }
                    else
                    {
                        SearchList.Add(String.Empty);
                    }
                }

                var totalRowsOri = basisData.Count();
                var filterStateDataOri = SearchList;
                var RuleFilter = new Dictionary<int, IQueryable<T>>();

                z = 0;
                foreach (var item in colsName)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        RuleFilter.Add(z, filterStateDataOri[0].ToString() == "" ? basisData : basisData = basisData.Search(o => Convert.ToString(o.GetType().GetProperty(item).GetValue(o)).ToLower()).Containing(filterStateDataOri[z].Split("^")));
                        z++;
                    }
                }

                Expression<Func<T, bool>> where = (w => w.FilterData(colsName, searchValue));

                // Skiping number of Rows count
                var start = request.PageNumber;
                // Paging Length 10,20
                var length = request.PageSize??"";

                searchValue ??= "";
                start ??= "0";
                //length ??= "10";
                length = length == "" ? totalRowsOri.ToString() : length == "All" ? totalRowsOri.ToString() : length!=""? length: "10";

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                int recordsFilteredTotal = 0;

                // Getting all data
                var tmpD = basisData;
                //recordsTotal = tmpD.Count();
                recordsTotal = totalRowsOri;

                var ifsd = 0;
                var filterStates = SearchList;//filterState.Split("|");
                foreach (string filterStateData in filterStates)
                {
                    if (filterStateData != "")
                    {
                        tmpD = RuleFilter[ifsd];
                    }

                    ifsd++;
                }

                //Search
                tmpD = tmpD.EagerLoadWhere(where);

                skip = skip == 0 ? skip : skip - 1;

                //Paging
                var data = new List<T>();
                if (tmpD is IAsyncEnumerable<T>)
                {
                    //total number of rows count
                    recordsFilteredTotal = await tmpD.CountAsync();
                    data = await tmpD.EagerLoadInclude(includes).Skip(skip * pageSize).Take(pageSize).ToListAsync();

                }
                else
                {
                    //total number of rows count
                    recordsFilteredTotal = tmpD.Count();
                    data = tmpD.EagerLoadInclude(includes).Skip(skip * pageSize).Take(pageSize).ToList();
                }

                var Ritems = _mapper.Map<List<T>>(data);

                return (true, "Ok", recordsTotal, recordsFilteredTotal, Ritems, colsName);
            }
            catch (Exception ex)
            {
                return (false, "Trouble happened! \n" + ex.Message, 0, 0, null, null);
            }
        }

        public async Task<(bool Added, string Message)> BulkInsertAsync<T>(IList<T> entity) where T : BaseEntity
        {
            try
            {
                await _dbContext.BulkInsertAsync<T>(entity);
                await _dbContext.SaveChangesAsync();

                var msgSuccess = "Bulk Insert Success";

                _logger.LogInformation(msgSuccess);
                return (true, msgSuccess);
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }
        public async Task<(bool Added, string Message)> BulkInsertManyAsync<T>(IList<T> entity) where T : BaseEntity
        {
            try
            {
                await _dbContext.BulkInsertAsync<T>(entity);

                var msgSuccess = "Bulk Insert Success";

                _logger.LogInformation(msgSuccess);
                return (true, msgSuccess);
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null) { return (false, "Trouble happened! \n" + ex.Message + "\n" + ex.InnerException.Message); }
                else
                {
                    return (false, "Trouble happened! \n" + ex.Message);
                }
            }
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        object value = dr[column.ColumnName];
                        if (value == DBNull.Value) value = null;
                        pro.SetValue(obj, value, null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        public List<T> ExecSPToList<T>(string SPName, Dictionary<string, object> parameter)
        {
            List<T> data = new List<T>();
            var dt = ExecSPToDataTable(SPName, parameter);
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public DataTable ExecSPToDataTable(string SPName, Dictionary<string, object> parameter)
        {
            DataTable dataTable = new DataTable();
            DbConnection connection = _dbContext.Database.GetDbConnection();
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
            using (var cmd = dbFactory.CreateCommand())
            {
                var Key = "";
                if (parameter.Count() > 0)
                {
                    foreach (var LP in parameter)
                    {
                        if (!string.IsNullOrEmpty(Key)) Key += ", ";
                        Key += $"@{LP.Key}='" + LP.Value + "'";
                        //cmd.Parameters.Add(new SqlParameter("@" + LP.Key, LP.Value == null ? DBNull.Value : LP.Value));
                    }
                }

                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $"Exec dbo.{SPName} {Key}";

                using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        public async Task<IEnumerable<TRes>> executeProcedure<TRes>(string spName, object param, CommandType? commandType = null)
        {
            if (_dbContext.Database.GetDbConnection().State != ConnectionState.Closed)
            {
                return await _dbContext.Database.GetDbConnection().QueryAsync<TRes>(spName, param, commandTimeout: 120, commandType:
                commandType ?? CommandType.StoredProcedure);
            }
            else 
            {
                //IDbConnection _Conn = new SqlConnection(_dbContext.Database.GetDbConnection().ConnectionString);
                IDbConnection _Conn = new SqlConnection(Settings.AppSettingValue("ConnectionStrings", "DefaultConnection", ""));

                return await _Conn.QueryAsync<TRes>(spName, param, commandTimeout: 120, commandType:
                commandType ?? CommandType.StoredProcedure);
            }

        }
        public IEnumerable<TRes> executeProcedureNonAsync<TRes>(string spName, object param, CommandType? commandType = null)
        {
            return _dbContext.Database.GetDbConnection().Query<TRes>(spName, param, commandTimeout: 120, commandType:
                commandType ?? CommandType.StoredProcedure);
        }
    }

    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static bool FilterData<T>(this T item, List<string> columnsName, string searchValue)
        {
            var result = false;
            foreach (var column in columnsName)
            {
                if (!string.IsNullOrEmpty(column))
                {
                    if (result == true) { break; }
                    //if (item.GetType().GetProperty(column.FirstCharToUpper()).GetValue(item) != null)
                    if (item.GetType().GetProperty(column).GetValue(item) != null)
                    {
                        //var value = item.GetType().GetProperty(column.FirstCharToUpper()).GetValue(item).ToString().ToLower();
                        var value = item.GetType().GetProperty(column).GetValue(item).ToString().ToLower();
                        result = value.Contains(searchValue.ToLower());
                    }
                }
            }

            return result;
        }
    }

    public static class IQueryableExtensions
    {
        public static IQueryable<T> EagerLoadInclude<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes) where T : class
        {
            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            return query;
        }

        public static IQueryable<T> EagerLoadWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>> wheres) where T : class
        {
            if (wheres != null)
                query = query.Where(wheres);

            return query;
        }
    }
}
