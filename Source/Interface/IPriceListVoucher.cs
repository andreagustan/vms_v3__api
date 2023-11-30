using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface IPriceListVoucher
    {
        Task<(bool Status, List<MPriceListVoucher> Result, string Message)> List(ListPageDetail Items);
        Task<(bool Status, object Result, string Message)> ListObject(ListPageDetail Items);
        Task<(bool Status, MPriceListVoucher Result, string Message)> DetailId(long Id);
        Task<(bool Status, object Result, string Message)> DetailIdExt(long Id);
        Task<(bool Status, string Result, string Message)> Add(string CurrentUserId, MPriceListVoucherExt Items);
        Task<(bool Status, string Result, string Message)> Update(string CurrentUserId, MPriceListVoucherExt Items);
        Task<(bool Status, string Result, string Message)> Delete(string CurrentUserId, long Id);
        Task<(bool Status, string Result, string Message)> BulkMany(string CurrentUserId, List<MPriceListVoucherDto> Items);
        Task<(bool Status, float? Result, string Message)> GetPrice(MPriceListVoucherGetPrice Items);
    }
}
