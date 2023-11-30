using SoapSSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface ISoapSSO
    {
        Task<(bool Status, string Msg, ValidateUserResponse Data)> GetValidasiUser(dataAuthExt Items);
    }
}
