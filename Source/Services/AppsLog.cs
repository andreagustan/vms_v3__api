using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VMS.Entities;
using VMS.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Services
{
    public class AppsLog : IAppsLog
    {
        private readonly IRepository repository;
        private readonly ILogger<Logs> logger;

        public AppsLog(IRepository _repository, ILogger<Logs> _logger)
        {
            this.repository = _repository;
            this.logger = _logger;

        }

        public JObject PrepareData(object item)
        {
            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(item));
        }

        public void WriteAppsLog(LogsDto Items) 
        {
            try
            {
                var param = new Dictionary<string, object>() 
                { 
                    { "FlagData", Items.FlagData }, 
                    { "Id", Items.Id }, 
                    { "Description", Items.Description }, 
                    { "Module", Items.Module }, 
                    { "Action", Items.Action }, 
                    { "Request", Items.Request }, 
                    { "Response", Items.Response }, 
                    { "Note", Items.Note },                 
                    { "ErrorLog", Items.ErrorLog },                 
                    { "StatusLog", Items.StatusLog },                 
                    { "UserId", Items.UserId }             
                };

                var Rs = repository.ExecSPToList<LogsExt>("ProcCRUDSystemLogs", param);
            }
            catch (Exception ex)
            {
                logger.LogError("Error : " + ex.Message, ex.InnerException);
            }
        }

        public async Task<(bool Status, string Id, string Message)> WriteAppsLogAsync(LogsDto Items)
        {
            string RsMsg = "";
            try
            {
                var param = new Dictionary<string, object>()
                {
                    { "FlagData", Items.FlagData },
                    { "Id", Items.Id },
                    { "Description", Items.Description },
                    { "Module", Items.Module },
                    { "Action", Items.Action },
                    { "Request", Items.Request },
                    { "Response", Items.Response },
                    { "Note", Items.Note },
                    { "ErrorLog", Items.ErrorLog },
                    { "StatusLog", Items.StatusLog },
                    { "UserId", Items.UserId }
                };

                var Rs = (await Task.Run(() => repository.ExecSPToList<LogsExt>("ProcCRUDSystemLogs", param))).FirstOrDefault();
                RsMsg = Rs.Result;
                return (true, Rs.Id.ToString(), Rs.Result);
            }
            catch (Exception ex)
            {
                logger.LogError("Error : " + ex.Message, ex.InnerException);
                return (false, null, RsMsg);
            }
        }
    }
}
