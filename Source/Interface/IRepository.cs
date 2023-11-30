using VMS.Entities;
using VMS.SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VMS.Interface
{
    public partial interface IRepository
    {
        void BeginTransaction();
        void BeginTransactionAsync();
        Task<(bool Commited, string Message)> RollbackAsync();

        Task<(bool Commited, string Message)> CommitSync();
        (bool Commited, string Message) Commit();

        Task<(bool Commited, string Message)> CommitManySync();
        void AddBulkAsync<T>(IList<T> entities) where T : BaseEntity;
        Task AddBulkTaskAsync<T>(IList<T> entities) where T : BaseEntity;
        Task<(bool Added, string Message)> BulkInsertAsync<T>(IList<T> entity) where T : BaseEntity;
        Task<(bool Added, string Message)> BulkInsertManyAsync<T>(IList<T> entity) where T : BaseEntity;

        List<T> ExecSPToList<T>(string SPName, Dictionary<string, object> parameter);
        DataTable ExecSPToDataTable(string SPName, Dictionary<string, object> parameter);
        Task<IEnumerable<TRes>> executeProcedure<TRes>(string spName, object param, CommandType? commandType = null);
        IEnumerable<TRes> executeProcedureNonAsync<TRes>(string spName, object param, CommandType? commandType = null);
        Task<(bool Generated, string Message, int recordsTotal, int recordsFilteredTotal, List<T> data, List<string> colsName)>
            GenerateDataForDatatableExtAsync<T>(IQueryable<T> basisData, string FieldNya, ListPage request, params Expression<Func<T, object>>[] includes) where T : BaseEntity;
        
    }
}
