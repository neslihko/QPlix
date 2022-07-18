namespace QPlixInvestment
{
    public class Result<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        private Result() { }

        public static Result<T> OK(T data) => new Result<T>() { Success = true, Data = data };

        public static Result<T> Fail(string reason) => new Result<T>() { Success = false, Message = reason };
    }
}
