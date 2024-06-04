using TodoList.Domain.Enums;

namespace TodoList.Domain.DTOs.TodoItemDTO
{
    public class TodoItemFilter
    {
        public TodoItemStatus? TodoItemStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Title { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
