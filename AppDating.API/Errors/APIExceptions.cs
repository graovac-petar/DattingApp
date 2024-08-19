namespace AppDating.API.Errors
{
    public class APIExceptions
    {
        public int StatusCode { get; }
        public string Message { get; }
        public string? Details { get; }

        public APIExceptions(int statusCode, string message, string? details)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
