using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public sealed class ApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; } = default!;
        public T? Data { get; set; }
    }
}
