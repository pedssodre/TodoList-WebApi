namespace TodoList.Domain.DTOs.TodoItemDTO
{
    public class CreateTodoItemRequest
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
