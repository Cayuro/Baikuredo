namespace Core
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static Response<T> Ok(T data, string message = "Operación exitosa", int statusCode = 200)
        {
            return new Response<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static Response<T> Fail(string message, List<string>? errors = null, int statusCode = 400)
        {
            return new Response<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }

    public class Response : Response<object>
    {
        public static Response Ok(string message = "Operación exitosa", int statusCode = 200)
        {
            return new Response
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static new Response Fail(string message, List<string>? errors = null, int statusCode = 400)
        {
            return new Response
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }
}
