using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Common.Exceptions
{
    public class ExceptionHandler : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var logger = (ILogger<ExceptionHandler>)context.HttpContext.RequestServices.GetService(typeof(ILogger<ExceptionHandler>));
                var message = $"Exception: {context.Exception.GetType().Name}: {context.Exception.Message}\n" +
                    $"{context.Exception.StackTrace}";
                if (context.Exception is HttpException exception)
                {
                    context.Result = new ObjectResult(new ApiError(exception))
                    {
                        StatusCode = exception.StatusCode,
                    };
                    logger.LogWarning(message);
                }
                else
                {
                    context.Result = new ObjectResult(new ApiError())
                    {
                        StatusCode = 500,
                    };
                    logger.LogError(message);
                }
                context.ExceptionHandled = true;
            }               
        }
    }
    public class ApiError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public ApiError()
        {
            Code = 500;
            Message = "Unknown error";
        }
        public ApiError(HttpException httpException)
        {
            Code = httpException.StatusCode;
            Message = httpException.Message;
        }
    }
}
