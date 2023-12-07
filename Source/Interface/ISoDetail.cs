﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface ISoDetail
    {
        Task<(bool Status, object Result, string Message)> ListObject(ListPageExt Items);
    }
}
