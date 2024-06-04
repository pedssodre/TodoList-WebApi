using Microsoft.EntityFrameworkCore;
using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infra.Context;

namespace TodoList.Infra.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoContext _context;

        public TodoRepository(TodoContext context)
        {
            _context = context;
        }

        public IQueryable<TodoItem> GetAll()
        {
            return _context.TodoItems.AsQueryable();
        }

        public IQueryable<TodoItem> Filter(TodoItemFilter todoItemFilter)
        {
            var query = _context.TodoItems.AsQueryable();

            if (todoItemFilter.TodoItemStatus.HasValue)
            {
                query = query.Where(t => t.TodoItemStatus == todoItemFilter.TodoItemStatus.Value);
            }
            if (todoItemFilter.CreatedAt.HasValue)
            {
                query = query.Where(t => t.CreatedAt.Date <= todoItemFilter.CreatedAt.Value.Date);
            }
            if (todoItemFilter.DueDate.HasValue)
            {
                query = query.Where(t => t.DueDate.Date <= todoItemFilter.DueDate.Value.Date).OrderByDescending(t => t.DueDate);
            }

            if (!string.IsNullOrEmpty(todoItemFilter.Title))
            {
                query = query.Where(t => t.Title.Contains(todoItemFilter.Title));
            }

            return query;
        }

        public async Task<PaginatedList<TodoItem>> GetPaginatedItemsWithFilter(IQueryable<TodoItem> itemsFiltered, int pageIndex, int qt)
        {
            var count = await itemsFiltered.CountAsync();
            var totalPages = (count/qt) + 1;

            var todoItems = await itemsFiltered
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageIndex- 1) * qt)
                .Take(qt)
                .ToListAsync();

            return new PaginatedList<TodoItem>(todoItems, pageIndex, totalPages);
        }

        public async Task<TodoItem> GetByIdAsync(Guid id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task AddAsync(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoItem todoItem)
        {
            _context.TodoItems.Update(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodoItem todoItem)
        {
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ItemExistsAsync(string title)
        {
            return await _context.TodoItems.AnyAsync(x => x.Title == title);
        }

        public async Task<bool> ItemExistsAsync(string title, Guid excludeId)
        {
            return await _context.TodoItems.AnyAsync(x => x.Title == title && x.Id != excludeId);
        }
    }
}
