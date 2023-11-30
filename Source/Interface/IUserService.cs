using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface IUserService
    {
        Task<(bool Authenticated, Object Result, string Message)> AuthenticateAsync(dataAuthExt Items);
    }
}
