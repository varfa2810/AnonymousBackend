using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AnonymousApplication.Models
{
    public class ApiResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
