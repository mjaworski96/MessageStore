using Common.Dto;
using Common.Exceptions;
using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace API.Security
{
    public class NoInternalAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpMetadataService = (IHttpMetadataService)context.HttpContext.RequestServices.GetService(typeof(IHttpMetadataService));
            if (httpMetadataService.InternalToken)
            {
                var exception = new ForbiddenException("You can't acces this resuorce");
                context.Result = new ObjectResult(new ApiError(exception))
                {
                    StatusCode = exception.StatusCode,
                };
            }
            else
            {
                base.OnActionExecuting(context);
            }
            
        }
    }
}
