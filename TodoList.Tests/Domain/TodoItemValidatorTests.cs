using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Enums;
using TodoList.Domain.Validations.TodoItem;

namespace TodoList.Tests.Domain
{
    public class TodoItemValidatorTests
    {
        private CreateTodoItemRequestValidator _createValidator;
        private UpdateTodoItemRequestValidator _updateValidator;
        private TodoItemFilterValidator _filterValidator;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateTodoItemRequestValidator();
            _updateValidator = new UpdateTodoItemRequestValidator();
            _filterValidator = new TodoItemFilterValidator();
        }

        [Test]
        public void CreateTodoItem_Should_Have_Error_When_Title_Is_Null()
        {
            var model = new CreateTodoItemRequest { Title = null };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title can't be null");
        }

        [Test]
        public void CreateTodoItem_Should_Have_Error_When_Title_Is_Empty()
        {
            var model = new CreateTodoItemRequest { Title = string.Empty };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("'Title' must not be empty.");
        }

        [Test]
        public void CreateTodoItem_Should_Have_Error_When_Description_Exceeds_Max_Length()
        {
            var model = new CreateTodoItemRequest {Title = "Task", Description = new string('a', 81) };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description).WithErrorMessage("Description maximum length must be 80 characters");
        }

        [Test]
        public void CreateTodoItem_Should_Have_Error_When_DueDate_Is_Past()
        {
            var model = new CreateTodoItemRequest {Title = "Task", DueDate = DateTime.Now.AddDays(-1) };
            var result = _createValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DueDate.Date).WithErrorMessage("Can't create overdue task");
        }

        [Test]
        public void CreateTodoItem_Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new CreateTodoItemRequest
            {
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = DateTime.Now.AddDays(1)
            };
            var result = _createValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
        }

        [Test]
        public void UpdateTodoItem_Should_Have_Error_When_Id_Is_Null()
        {
            var model = new UpdateTodoItemRequest { Id = Guid.Empty };
            var result = _updateValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Test]
        public void UpdateTodoItem_Should_Have_Error_When_Title_Is_Null()
        {
            var model = new UpdateTodoItemRequest { Title = null };
            var result = _updateValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Test]
        public void UpdateTodoItem_Should_Have_Error_When_Description_Exceeds_Max_Length()
        {
            var model = new UpdateTodoItemRequest { Description = new string('a', 81) };
            var result = _updateValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Description).WithErrorMessage("Description maximum length must be 80 characters");
        }

        [Test]
        public void UpdateTodoItem_Should_Have_Error_When_DueDate_Is_Past()
        {
            var model = new UpdateTodoItemRequest { DueDate = DateTime.Now.AddDays(-1) };
            var result = _updateValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DueDate.Date).WithErrorMessage("Can't update task to a past date");
        }

        [Test]
        public void UpdateTodoItem_Should_Have_Error_When_TodoItemStatus_Is_Invalid()
        {
            var model = new UpdateTodoItemRequest { TodoItemStatus = (TodoItemStatus)999 };
            var result = _updateValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TodoItemStatus).WithErrorMessage("'Todo Item Status' has a range of values which does not include '999'.");
        }

        [Test]
        public void UpdateTodoItem_Should_Not_Have_Error_When_Model_Is_Valid()
        {
            var model = new UpdateTodoItemRequest
            {
                Id = Guid.NewGuid(),
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = DateTime.Now.AddDays(1),
                TodoItemStatus = TodoItemStatus.Pending
            };
            var result = _updateValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.DueDate);
            result.ShouldNotHaveValidationErrorFor(x => x.TodoItemStatus);
        }

        [Test]
        public void Should_Have_Error_When_PageIndex_Is_Less_Than_1()
        {
            var model = new TodoItemFilter { PageIndex = 0 };
            var result = _filterValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(filter => filter.PageIndex)
                  .WithErrorMessage("PageIndex must be greater than or equal to 1");
        }

        [Test]
        public void Should_Not_Have_Error_When_PageIndex_Is_Greater_Than_Or_Equal_To_1()
        {
            var model = new TodoItemFilter { PageIndex = 1 };
            var result = _filterValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(filter => filter.PageIndex);
        }

        [Test]
        public void Should_Have_Error_When_PageSize_Is_Less_Than_10()
        {
            var model = new TodoItemFilter { PageSize = 9 };
            var result = _filterValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(filter => filter.PageSize)
                  .WithErrorMessage("PageSize must be at least 10.");
        }

        [Test]
        public void Should_Not_Have_Error_When_PageSize_Is_Greater_Than_Or_Equal_To_10()
        {
            var model = new TodoItemFilter { PageSize = 10 };
            var result = _filterValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(filter => filter.PageSize);
        }

        [Test]
        public void Should_Have_Error_When_TodoItemStatus_Is_Invalid()
        {
            var model = new TodoItemFilter { TodoItemStatus = (TodoItemStatus)999 };
            var result = _filterValidator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(filter => filter.TodoItemStatus)
                  .WithErrorMessage("Invalid status value.");
        }

        [Test]
        public void Should_Not_Have_Error_When_TodoItemStatus_Is_Valid()
        {
            var model = new TodoItemFilter { TodoItemStatus = TodoItemStatus.Pending };
            var result = _filterValidator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(filter => filter.TodoItemStatus);
        }
    }
}
