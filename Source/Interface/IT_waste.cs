using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;
using static VMS.Entities.Commons;

namespace VMS.Interface
{
    public interface IT_waste
    {
        Task<(bool Status, object Result, string Message)> ListObject(ListPageExt Items);
        Task<(bool Status, RsList Result, string Message)> ListObjectExt(ListPageExt Items);
        Task<(bool Status, object Result, string Message)> DeleteById(CommonDelete Items);
        Task<(bool Status, object Result, string Message)> BulkUpdate(T_Waste_BulkUpdate Items);
    }
}
