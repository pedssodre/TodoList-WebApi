using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;

namespace TodoList.Domain.Interfaces.Services
{
    public interface ITodoService
    {
        Task<CreateTodoItemResponse> Create(CreateTodoItemRequest request);
        Task<GetAllTodoItemsResponse> GetFilteredTodoItems(TodoItemFilter todoItemFilter);
        Task<UpdateTodoItemResponse> Update(UpdateTodoItemRequest request);
        Task<ApiResponse> Delete(Guid id);
    }
}
