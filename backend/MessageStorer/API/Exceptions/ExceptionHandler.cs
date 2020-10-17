using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Exceptions
{
    public class ExceptionHandler : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpException exception)
            {
                context.Result = new ObjectResult(new ApiError(exception))
                {
                    StatusCode = exception.StatusCode,
                };
            } 
            else if (context.Exception != null)
            {
                context.Result = new ObjectResult(new ApiError())
                {
                    StatusCode = 500,
                };
            }
            if (context.Exception != null)
                context.ExceptionHandled = true;
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
