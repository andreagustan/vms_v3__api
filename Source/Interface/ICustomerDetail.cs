using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface ICustomerDetail
    {
        Task<(bool Status, List<MCustomerDetail> Result, string Message)> List(ListPage Items);
        Task<(bool Status, object Result, string Message)> ListObject(ListPage Items);
        Task<(bool Status, RsList Result, string Message)> ListObjectExt(ListPageExt Items);
        Task<(bool Status, MCustomerDetail Result, string Message)> GetById(long Id);
        Task<(bool Status, object Result, string Message)> GetByIdExt(long Id);
        Task<(bool Status, string Result, string Message)> Add(string CurrentUserId, MCustomerDetailExt Items);
        Task<(bool Status, string Result, string Message)> Update(string CurrentUserId, MCustomerDetailExt Items);
        Task<(bool Status, string Result, string Message)> Delete(string CurrentUserId, long Id);
        Task<(bool Status, string Result, string Message)> BulkMany(string CurrentUserId, List<MCustomerDetailDto> Items);
    }
}
