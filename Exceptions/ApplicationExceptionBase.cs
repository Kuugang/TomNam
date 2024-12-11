namespace TomNam.Exceptions
{

    public class ApplicationExceptionBase : Exception
    {
        public int StatusCode { get; private set; }

        public ApplicationExceptionBase() : base()
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }

        public ApplicationExceptionBase(string message, int statusCode = StatusCodes.Status500InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public ApplicationExceptionBase(string message, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

}