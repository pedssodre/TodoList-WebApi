using TodoList.Domain.DTOs.Common;

namespace TodoList.Domain.DTOs.TodoItemDTO
{
    public class GetAllTodoItemsResponse : ApiResponse
    {
        public GetAllTodoItemsResponse() { }
        public GetAllTodoItemsResponse(PaginatedList<TodoItemBase> paginatedItemBaseList)
        {
            StatusCode = System.Net.HttpStatusCode.OK;
            Data = paginatedItemBaseList;
        }
    }
}
