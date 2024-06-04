using FluentValidation;
using TodoList.Domain.DTOs.TodoItemDTO;

namespace TodoList.Domain.Validations.TodoItem
{
    public class TodoItemFilterValidator : AbstractValidator<TodoItemFilter>
    {
        public TodoItemFilterValidator()
        {
            RuleFor(filter => filter.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageIndex must be greater than or equal to 1");

            RuleFor(filter => filter.PageSize)
                .GreaterThanOrEqualTo(10).WithMessage("PageSize must be at least 10.")
                .Must(pageSize => pageSize >= 10).WithMessage("PageSize must be at least 10.");

            RuleFor(filter => filter.TodoItemStatus)
                .IsInEnum().When(filter => filter.TodoItemStatus.HasValue)
                .WithMessage("Invalid status value.");
        }
    }
}
