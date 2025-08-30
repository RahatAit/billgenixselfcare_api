namespace billgenixselfcare_api.Domain.Common
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static Result SuccessResult(string message = "Operation successful") => new()
        {
            Success = true,
            Message = message
        };

        public static Result FailureResult(string message, List<string> errors = null) => new()
        {
            Success = false,
            Message = message,
            Errors = errors ?? new()
        };
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }

        public static Result<T> SuccessResult(T data, string message = "Operation successful") => new()
        {
            Success = true,
            Data = data,
            Message = message
        };

        public static new Result<T> FailureResult(string message, List<string> errors = null) => new()
        {
            Success = false,
            Message = message,
            Errors = errors ?? new()
        };
    }
}
