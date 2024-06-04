using TodoList.Domain.Enums;

namespace TodoList.Domain.DTOs.Common
{
    public class TodoItemBase
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public TodoItemStatus TodoItemStatus { get; set; }
    }
}
