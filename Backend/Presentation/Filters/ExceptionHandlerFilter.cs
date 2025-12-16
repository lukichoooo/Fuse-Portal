using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Filters
{
    public class ExceptionHandlerFilter(ILogger<ExceptionHandlerFilter> logger) : IExceptionFilter
    {
        private readonly ILogger<ExceptionHandlerFilter> _logger = logger;

        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            _logger.LogError(ex, "An unhandled exception occurred");

            if (ex is ICustomException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Error = ex.Message
                });
            }
            else
            {
                context.Result = new ObjectResult(new())
                {
                    Value = ex.Message,
                    StatusCode = 500
                };
            }
            context.ExceptionHandled = true;
        }
    }
}
