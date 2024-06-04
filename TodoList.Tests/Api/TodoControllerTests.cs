using AutoMapper;
using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Domain.Interfaces.Services;
using TodoList.Domain.Services;
using TodoList_WebApi.Controllers;

namespace TodoList.Tests.Api
{
    public class TodoControllerTests
    {
        private Mock<ITodoService> _mockTodoService;
        private TodoController _controller;

        [SetUp]
        public void Setup()
        {
            _mockTodoService = new Mock<ITodoService>();
            _controller = new TodoController(_mockTodoService.Object);
        }

        [Test]
        public async Task Create_ShouldReturnCorrectResponse()
        {
            var request = new CreateTodoItemRequest { Title = "Test Title" };
            var response = new CreateTodoItemResponse
            {
                StatusCode = HttpStatusCode.Created,
                Data = new TodoItem { Id = Guid.NewGuid(), Title = "Test Title" }
            };

            _mockTodoService.Setup(s => s.Create(It.IsAny<CreateTodoItemRequest>())).ReturnsAsync(response);

            var result = await _controller.Create(request) as ObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)response.StatusCode);
            result.Value.Should().Be(response.Data);
        }

        [Test]
        public async Task Update_ShouldReturnCorrectResponse()
        {
            var request = new UpdateTodoItemRequest { Id = Guid.NewGuid(), Title = "Updated Title" };
            var response = new UpdateTodoItemResponse
            {
                StatusCode = HttpStatusCode.OK,
                Data = new TodoItem { Id = request.Id, Title = "Updated Title" }
            };

            _mockTodoService.Setup(s => s.Update(It.IsAny<UpdateTodoItemRequest>())).ReturnsAsync(response);

            var result = await _controller.Update(request) as ObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)response.StatusCode);
            result.Value.Should().Be(response.Data);
        }

        [Test]
        public async Task Delete_ShouldReturnCorrectResponse()
        {
            var id = Guid.NewGuid();
            var response = new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                Data = "Todo item deleted successfully"
            };

            _mockTodoService.Setup(s => s.Delete(It.IsAny<Guid>())).ReturnsAsync(response);

            var result = await _controller.Delete(id) as ObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)response.StatusCode);
            result.Value.Should().Be(response.Data);
        }

        [Test]
        public async Task GetAllTodoItemsWithFilter_ShouldReturnCorrectResponse()
        {
            var filter = new TodoItemFilter { PageIndex = 1, PageSize = 10 };
            var response = new GetAllTodoItemsResponse
            {
                StatusCode = HttpStatusCode.OK,
                Data = new PaginatedList<TodoItemBase>(new List<TodoItemBase>
                {
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Task 1" },
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Task 2" }
                }, 1, 1)
            };

            _mockTodoService.Setup(s => s.GetFilteredTodoItems(It.IsAny<TodoItemFilter>())).ReturnsAsync(response);

            var result = await _controller.GetAllTodoItemsWithFilter(filter);
            var content = result.Result as ObjectResult;

            content.Should().NotBeNull();
            content.StatusCode.Should().Be((int)response.StatusCode);
            content.Value.Should().Be(response.Data);
        }
    }
}
