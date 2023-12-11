using VMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Interface
{
    public interface IAppsLog
    {
        void WriteAppsLog(LogsDto Items);
        Task<(bool Status, string Id, string Message)> WriteAppsLogAsync(LogsDto Items);
    }
}
