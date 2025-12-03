using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters
{
    public class ExceptionHandlerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;

            if (ex is ICustomException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Error = ex.Message
                });
                context.ExceptionHandled = true;
            }
            else
            {
                context.Result = new ObjectResult(new())
                {
                    Value = ex.Message,
                    StatusCode = 500
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
