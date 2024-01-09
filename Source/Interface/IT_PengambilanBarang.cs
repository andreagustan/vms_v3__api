using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface IT_PengambilanBarang
    {
        Task<(bool Status, object Result, string Message)> ListObject(ListPageExt Items);
        Task<(bool Status, RsList Result, string Message)> ListObjectExt(ListPageExt Items);
        Task<(bool Status, object Result, string Message)> BulkUpdate(T_PengambilanBarangRequest Items);
    }
}
