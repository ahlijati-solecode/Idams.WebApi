using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace Idams.WebApi.Utils
{
    public class ApiController : Controller
    {
        public virtual string? Message { get; set; } = null;

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.ErrorCount > 0)
            {
                object message = null;
                var errors = context.ModelState.Values.SelectMany(n => n.Errors.Select(e => e.ErrorMessage));
                if (context.ModelState.ErrorCount == 1)
                    message = errors?.FirstOrDefault();
                else
                    message = string.Join("|", errors);
                context.Result = BadRequest(message);
            }
            var task = base.OnActionExecutionAsync(context, next);

            return task;
        }

        public override OkObjectResult Ok([ActionResultObjectValue] object? value)
        {
            if (value is bool)
                return base.Ok(new
                {
                    code = 200,
                    status = (bool)value ? "Success" : "Failed",
                    message = Message,
                });
            return base.Ok(new
            {
                code = 200,
                status = "Success",
                message = Message,
                data = value
            });
        }

        public override NotFoundObjectResult NotFound([ActionResultObjectValue] object? value)
        {
            if (value is string)
            {
                return base.NotFound(new
                {
                    code = 404,
                    status = "NotFound",
                    message = value
                });
            }
            return base.NotFound(new
            {
                code = 404,
                status = "NotFound",
                data = value
            });
        }

        public override BadRequestObjectResult BadRequest([ActionResultObjectValue] object? error)
        {
            return base.BadRequest(new
            {
                code = 400,
                status = "BadRequest",
                data = error
            });
        }
    }
}