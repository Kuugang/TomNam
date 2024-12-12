namespace TomNam.Exceptions
{

    public class ApplicationExceptionBase : Exception
    {
        public string Error { get; private set; }
        public int StatusCode { get; private set; }

        public ApplicationExceptionBase() : base()
        {
            StatusCode = StatusCodes.Status500InternalServerError;
            Error = "An unexpected error occurred.";
        }

        public ApplicationExceptionBase(string message, string error, int statusCode = StatusCodes.Status500InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public ApplicationExceptionBase(string message, string error, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Error = error;
        }
    }
}