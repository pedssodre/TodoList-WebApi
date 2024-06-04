using TodoList.Domain.Enums;
using static TodoList.Domain.Utils.UtilsService;

namespace TodoList.Domain.Entities
{
    public class TodoItem
    {
        public Guid Id { get; set; } = CombGenerator.GenerateComb();
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TodoItemStatus TodoItemStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
