using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces.Repositories
{
    public interface ITodoRepository
    {
        IQueryable<TodoItem> GetAll();
        IQueryable<TodoItem> Filter(TodoItemFilter todoItemFilter);
        Task<PaginatedList<TodoItem>> GetPaginatedItemsWithFilter(IQueryable<TodoItem> itemsFiltered, int pageIndex, int qt);
        Task<TodoItem> GetByIdAsync(Guid id);
        Task AddAsync(TodoItem todoItem);
        Task UpdateAsync(TodoItem todoItem);
        Task DeleteAsync(TodoItem todoItem);
        Task<bool> ItemExistsAsync(string title);
        Task<bool> ItemExistsAsync(string title, Guid excludeId);
    }
}
