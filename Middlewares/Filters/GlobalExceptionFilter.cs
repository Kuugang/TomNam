using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TomNam.Exceptions;
using TomNam.Models.DTO;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ApplicationExceptionBase ex)
        {
            context.Result = new ObjectResult(new ErrorResponseDTO
            {
                Message = ex.Message,
                Error = ex.Error
            })
            {
                StatusCode = ex.StatusCode
            };
        }
        else
        {
            context.Result = new ObjectResult(new ErrorResponseDTO
            {
                Message = context.Exception.Message,
                Error = context.Exception.StackTrace
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        context.ExceptionHandled = true;
    }
}
