﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;

namespace VMS.Interface
{
    public interface IGrDetail
    {
        Task<(bool Status, object Result, string Message)> ListObject(ListPageExt Items);
    }
}
