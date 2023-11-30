using VMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Interface
{
    interface IProfileManager
    {
        string GetUserName();
        Task<HelpersModel.SimpleViewModel> GetCurrentUserId();
    }
}
