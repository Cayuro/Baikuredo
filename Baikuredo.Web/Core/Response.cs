namespace Baikuredo.Web.Core
{
    public class Response<TResult>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public TResult? Result { get; set; }

        public static Response<TResult> Success(TResult result, string message = "Operación exitosa.")
        {
            return new Response<TResult>
            {
                IsSuccess = true,
                Result = result,
                Message = message
            };
        }

        public static Response<TResult> Success(string message = "Operación exitosa.")
        {
            return new Response<TResult>
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static Response<TResult> Failure(string message, List<string>? errors = null)
        {
            return new Response<TResult>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors
            };
        }

        public static Response<TResult> Failure(Exception ex, string message = "Ocurrió un error inesperado.")
        {
            return new Response<TResult>
            {
                IsSuccess = false,
                Message = $"{message}: {ex.Message}"
            };
        }
    }
}
