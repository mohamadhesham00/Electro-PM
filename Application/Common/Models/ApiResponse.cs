using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        // Success Factory Methods
        public static ApiResponse<T> Success(T data, string? message = null) =>
            new() { Succeeded = true, Data = data, Message = message };

        // Failure Factory Methods
        public static ApiResponse<T> Failure(List<string> errors, string? message = null) =>
            new() { Succeeded = false, Errors = errors, Message = message };

        public static ApiResponse<T> Failure(string error, string? message = null) =>
            new() { Succeeded = false, Errors = new List<string> { error }, Message = message };
    }
}
