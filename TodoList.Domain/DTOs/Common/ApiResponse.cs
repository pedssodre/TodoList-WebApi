using System.Net;

namespace TodoList.Domain.DTOs.Common
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Data { get; set; }
    }
}
