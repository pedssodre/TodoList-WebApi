using FluentValidation;
using TodoList.Domain.DTOs.TodoItemDTO;

namespace TodoList.Domain.Validations.TodoItem
{
    public class UpdateTodoItemRequestValidator : AbstractValidator<UpdateTodoItemRequest>
    {
        public UpdateTodoItemRequestValidator()
        {
            RuleFor(src => src.Id).NotEmpty().NotNull();
            RuleFor(src => src.Title).NotEmpty().NotNull();
            RuleFor(src => src.Description).NotEmpty().NotNull().MaximumLength(80).WithMessage("Description maximum length must be 80 characters");
            RuleFor(src => src.DueDate.Date).NotEmpty().NotNull().GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Can't update task to a past date");
            RuleFor(src => src.TodoItemStatus).IsInEnum();
        }
    }
}
