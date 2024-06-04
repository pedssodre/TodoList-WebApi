using System.Net;
using TodoList.Domain.DTOs.Common;

namespace TodoList.Domain.DTOs.TodoItemDTO
{
    public class CreateTodoItemResponse : ApiResponse
    {
        public CreateTodoItemResponse() { }
        public CreateTodoItemResponse(TodoItemBase todoItem)
        {
            StatusCode = HttpStatusCode.OK;
            Data = todoItem;
        }
    }
}
