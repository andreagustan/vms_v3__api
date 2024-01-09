using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Class
{
    public class ApiResponse
    {
        private readonly int StatusCode;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private readonly string Message;
        public ApiResponse(int statusCode, string message = null)
        {
            this.StatusCode = statusCode;
            this.Message = message; //?? GetDefaultMessageForStatusCode(statusCode);
        }

        public class ApiDatatableResponse : ApiResponse
        {
            public readonly object Result;
            public ApiDatatableResponse(int _recordsTotal, int _recordsFiltered, object dataValue, string colNname = "") : base(200)
            {
                var ResultDataValue = new
                {
                    draw = 0,
                    colsName = colNname,
                    recordsFiltered = _recordsFiltered,
                    recordsTotal = _recordsTotal,
                    data = dataValue
                };
                Result = ResultDataValue;
            }

        }
    }
}
