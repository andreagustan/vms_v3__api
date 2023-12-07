using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface IT_PO
    {
        Task<(bool Status, object Result, string Message)> ListObject(ListPageExt Items);
        Task<(bool Status, object Result, string Message)> ListDetailObject(ListPageExt Items);
        Task<(bool Status, object Result, string Message)> BulkUpdate(T_PO Items);

    }
}
