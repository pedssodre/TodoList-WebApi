using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Infra.Context;
using static TodoList.Domain.Utils.UtilsService;

namespace TodoList_WebApi.Configuration
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using(var context = new TodoContext(serviceProvider.GetRequiredService<DbContextOptions<TodoContext>>()))
            {
                if (context.TodoItems.Any())
                    return;

                var todoItems = new TodoItem[]
                {
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 1",
                        Description = "This is sample task 1 description.",
                        DueDate = DateTime.Now.AddDays(-4),
                        TodoItemStatus = TodoItemStatus.Overdue,
                        CreatedAt = DateTime.Now.AddDays(-14),
                        UpdatedAt = DateTime.Now.AddDays(-14)
                    },
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 2",
                        Description = "This is sample task 2 description.",
                        DueDate = DateTime.Now.AddDays(-2),
                        TodoItemStatus = TodoItemStatus.Overdue,
                        CreatedAt = DateTime.Now.AddDays(-12),
                        UpdatedAt = DateTime.Now.AddDays(-12)
                    },
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 3",
                        Description = "This is sample task 3 description.",
                        DueDate = DateTime.Now.AddDays(-2),
                        TodoItemStatus = TodoItemStatus.Pending,
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UpdatedAt = DateTime.Now.AddDays(-10)
                    },
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 4",
                        Description = "This is sample task 4 description.",
                        DueDate = DateTime.Now.AddDays(4),
                        TodoItemStatus = TodoItemStatus.Pending,
                        CreatedAt = DateTime.Now.AddDays(-8),
                        UpdatedAt = DateTime.Now.AddDays(-8)
                    },
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 5",
                        Description = "This is sample task 5 description.",
                        DueDate = DateTime.Now.AddDays(30),
                        TodoItemStatus = TodoItemStatus.Pending,
                        CreatedAt = DateTime.Now.AddDays(-6),
                        UpdatedAt = DateTime.Now.AddDays(-6)
                    },
                    new() {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 6",
                        Description = "This is sample task 6 description.",
                        DueDate = DateTime.Now.AddDays(-2),
                        TodoItemStatus = TodoItemStatus.Completed,
                        CreatedAt = DateTime.Now.AddDays(-4),
                        UpdatedAt = DateTime.Now.AddDays(-4)
                    },
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 7",
                        Description = "This is sample task 7 description.",
                        DueDate = DateTime.Now.AddDays(-1),
                        TodoItemStatus = TodoItemStatus.Completed,
                        CreatedAt = DateTime.Now.AddDays(-2),
                        UpdatedAt = DateTime.Now.AddDays(-2)
                    },
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 8",
                        Description = "This is sample task 8 description.",
                        DueDate = DateTime.Now,
                        TodoItemStatus = TodoItemStatus.Completed,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new()
                    {
                        Id = CombGenerator.GenerateComb(),
                        Title = "Sample Task 9",
                        Description = "This is sample task 9 description.",
                        DueDate = DateTime.Now.AddDays(2),
                        TodoItemStatus = TodoItemStatus.Pending,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                context.TodoItems.AddRange(todoItems);
                context.SaveChanges();
            }
        }
    }
}
