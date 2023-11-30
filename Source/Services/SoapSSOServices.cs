using SoapSSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.Entities;
using VMS.Interface;

namespace VMS.Services
{
    public class SoapSSOServices : ISoapSSO
    {
        private SSOWSSoapClient Ws = new SSOWSSoapClient(SSOWSSoapClient.EndpointConfiguration.SSOWSSoap);

        public async Task<(bool Status, string Msg, ValidateUserResponse Data)> GetValidasiUser(dataAuthExt Items)
        {
            try
            {
                ValidateUserRequestBody DtBody = new ValidateUserRequestBody()
                {
                    username = Items.UserId,
                    password = Items.Password,
                };

                ValidateUserRequest DtRequest = new ValidateUserRequest()
                {
                    Body = DtBody,
                };

                var Rs = await Ws.ValidateUserAsync(DtRequest);
                return (true, null, Rs);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
