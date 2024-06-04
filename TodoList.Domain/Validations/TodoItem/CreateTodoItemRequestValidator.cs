using FluentValidation;
using TodoList.Domain.DTOs.TodoItemDTO;

namespace TodoList.Domain.Validations.TodoItem
{
    public class CreateTodoItemRequestValidator : AbstractValidator<CreateTodoItemRequest>
    {
        public CreateTodoItemRequestValidator() 
        {
            RuleFor(src => src.Title).NotEmpty().NotNull().WithMessage("Title can't be null");
            RuleFor(src => src.Description).MaximumLength(80).WithMessage("Description maximum length must be 80 characters");
            RuleFor(src => src.DueDate.Date).NotEmpty().NotNull().GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Can't create overdue task");
        }
    }
}
