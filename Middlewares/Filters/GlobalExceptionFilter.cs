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
                Message = "Reservation creation failed",
                Error = ex.Message
            })
            {
                StatusCode = ex.StatusCode
            };
        }
        else
        {
            context.Result = new ObjectResult(new ErrorResponseDTO
            {
                Message = "An unexpected error occurred.",
                Error = context.Exception.Message
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        context.ExceptionHandled = true;
    }
}
