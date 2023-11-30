using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using VMS.Data;
using VMS.Entities;
using VMS.Interface;

namespace VMS.Error
{
    public class CustomAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IHelpers _helpers;

        public AuthorizationPolicy Policy {get;}

        public CustomAuthorizationFilter(IHelpers helpers)
        {
            Policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            _helpers = helpers;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null) 
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Allow Anonymous skips all authorization
            if (context.Filters.Any(i => i is IAllowAnonymousFilter))
            {
                return;
            }

            var PolicyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateRs = await PolicyEvaluator.AuthenticateAsync(Policy, context.HttpContext);
            var authorizeRs = await PolicyEvaluator.AuthorizeAsync(Policy, authenticateRs, context.HttpContext, context);

            if (authorizeRs.Challenged)
            {
                //Return customer 401 result

                var Rq = context.HttpContext.Request.Headers["Authorization"].ToString().Split(" ");
                var Rs = _helpers.GetPolicy(Rq[1].ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
                if (Rs.Status)
                { return; }
                else
                {
                    Rs.Data.Message = Rs.Msg;
                    context.Result = CreateUnauthorized(Rs.Data);
                    return;
                }
            }

            context.HttpContext.User = authenticateRs.Principal;

            static IActionResult CreateUnauthorized(TokenExt Detail) => new UnauthorizedObjectResult(new ErrorMessage("Unauthorized", 401, Detail));
        }
    }
}
