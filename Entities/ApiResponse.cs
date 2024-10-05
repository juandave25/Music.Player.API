using System.Collections.Generic;
namespace Entities
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponse()
        {
            Errors = [];
        }

        public ApiResponse(T data, string message)
        {
            Success = true;
            Message = message;
            Data = data;
            Errors = [];
        }

        public ApiResponse(string message, List<string> errors)
        {
            Success = false;
            Message = message;
            Errors = errors ?? [];
        }
    }
}
