using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;
using TodoList_WebApi.Configuration;

namespace TodoList.Tests.Api
{
    public class AutoMapperConfigTests
    {
        private IMapper _mapper;
        private MapperConfiguration _mapperConfiguration;

        [SetUp]
        public void Setup()
        {
            _mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperConfig>());
            _mapper = _mapperConfiguration.CreateMapper();
        }

        [Test]
        public void AutoMapperConfig_IsValid()
        {
            _mapperConfiguration.AssertConfigurationIsValid();
        }

        [Test]
        public void Should_Map_CreateTodoItemRequest_To_TodoItem()
        {
            var request = new CreateTodoItemRequest
            {
                Title = "Test Title",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(1)
            };

            var todoItem = _mapper.Map<TodoItem>(request);

            Assert.AreEqual(request.Title, todoItem.Title);
            Assert.AreEqual(request.Description, todoItem.Description);
            Assert.AreEqual(request.DueDate, todoItem.DueDate);
        }

        [Test]
        public void Should_Map_TodoItem_To_TodoItemBase()
        {
            var todoItem = new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(1),
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now
            };

            var todoItemBase = _mapper.Map<TodoItemBase>(todoItem);

            Assert.AreEqual(todoItem.Id, todoItemBase.Id);
            Assert.AreEqual(todoItem.Title, todoItemBase.Title);
            Assert.AreEqual(todoItem.Description, todoItemBase.Description);
            Assert.AreEqual(todoItem.DueDate, todoItemBase.DueDate);
        }

        [Test]
        public void Should_Map_PaginatedList_TodoItem_To_PaginatedList_TodoItemBase()
        {
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Task 1" },
                new TodoItem { Id = Guid.NewGuid(), Title = "Task 2" }
            };

            var paginatedTodoItems = new PaginatedList<TodoItem>(todoItems, 1, 1);

            var paginatedTodoItemBases = _mapper.Map<PaginatedList<TodoItemBase>>(paginatedTodoItems);

            Assert.AreEqual(paginatedTodoItems.PageIndex, paginatedTodoItemBases.PageIndex);
            Assert.AreEqual(paginatedTodoItems.TotalPages, paginatedTodoItemBases.TotalPages);
            Assert.AreEqual(paginatedTodoItems.Items.Count, paginatedTodoItemBases.Items.Count);
            Assert.AreEqual(paginatedTodoItems.Items[0].Title, paginatedTodoItemBases.Items[0].Title);
            Assert.AreEqual(paginatedTodoItems.Items[1].Title, paginatedTodoItemBases.Items[1].Title);
        }
    }
}
