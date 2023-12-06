using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Data;

namespace VMS.Entities
{
    public class Commons
    {
        public class ResponseString
        {
            public string Status { get; set; }
        }

        public class CommonsResponse
        {
            public bool Status { get; set; } = false;
            public string Message { get; set; } = ConstValue.NotFound;
            public int CurrPage { get; set; }
            public long TotalPage { get; set; }
            public long TotalRecords { get; set; }
            public object Data { get; set; }
        }

        public class CommonResponse
        {
            public bool Status { get; set; } = false;
            public string Message { get; set; } = ConstValue.NotFound;
            public object Data { get; set; }
        }

        public class CommonDataResponse
        {
            public string Data { get; set; }
        }

        public class CommonDelete
        {
            public string Id { get; set; }
            public string UserId { get; set; }
        }
    }
}
