using AutoMapper;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Domain.Services;
using TodoList.Infra.Context;
using TodoList_WebApi.Configuration;
using static TodoList.Domain.Utils.UtilsService;

namespace TodoList.Tests.Domain
{

    [TestFixture]
    public class TodoServiceTests
    {
        private ServiceProvider _serviceProvider;
        private DbContextOptions<TodoContext> _dbContextOptions;
        private Mock<ITodoRepository> _mockTodoRepository;
        private Mock<IMapper> _mockMapper;
        private TodoService _todoService;
        private TodoContext _todoContext;


        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoListTest")
                .Options;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEntityFrameworkInMemoryDatabase();
            serviceCollection.AddSingleton(_dbContextOptions);
            serviceCollection.AddScoped<TodoContext>();

            _serviceProvider = serviceCollection.BuildServiceProvider();


            _todoContext = new TodoContext(_dbContextOptions);
            _todoContext.Database.EnsureCreated();
            SeedDatabase(_todoContext); // Seed the database

            _mockTodoRepository = new Mock<ITodoRepository>();
            _mockMapper = new Mock<IMapper>();
            _todoService = new TodoService(_mockTodoRepository.Object, _mockMapper.Object);
        }

        private void SeedDatabase(TodoContext context)
        {

            var todoItems = new List<TodoItem>
            {
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 1",
                    Description = "This is sample task 1 description.",
                    DueDate = DateTime.Now.AddDays(-4),
                    TodoItemStatus = TodoItemStatus.Overdue,
                    CreatedAt = DateTime.Now.AddDays(-14),
                    UpdatedAt = DateTime.Now.AddDays(-14)
                },
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 2",
                    Description = "This is sample task 2 description.",
                    DueDate = DateTime.Now.AddDays(-2),
                    TodoItemStatus = TodoItemStatus.Overdue,
                    CreatedAt = DateTime.Now.AddDays(-12),
                    UpdatedAt = DateTime.Now.AddDays(-12)
                },
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 3",
                    Description = "This is sample task 3 description.",
                    DueDate = DateTime.Now.AddDays(-2),
                    TodoItemStatus = TodoItemStatus.Pending,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    UpdatedAt = DateTime.Now.AddDays(-10)
                },
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 4",
                    Description = "This is sample task 4 description.",
                    DueDate = DateTime.Now.AddDays(4),
                    TodoItemStatus = TodoItemStatus.Pending,
                    CreatedAt = DateTime.Now.AddDays(-8),
                    UpdatedAt = DateTime.Now.AddDays(-8)
                },
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 5",
                    Description = "This is sample task 5 description.",
                    DueDate = DateTime.Now.AddDays(30),
                    TodoItemStatus = TodoItemStatus.Pending,
                    CreatedAt = DateTime.Now.AddDays(-6),
                    UpdatedAt = DateTime.Now.AddDays(-6)
                },
                new() {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 6",
                    Description = "This is sample task 6 description.",
                    DueDate = DateTime.Now.AddDays(-2),
                    TodoItemStatus = TodoItemStatus.Completed,
                    CreatedAt = DateTime.Now.AddDays(-4),
                    UpdatedAt = DateTime.Now.AddDays(-4)
                },
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 7",
                    Description = "This is sample task 7 description.",
                    DueDate = DateTime.Now.AddDays(-1),
                    TodoItemStatus = TodoItemStatus.Completed,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    UpdatedAt = DateTime.Now.AddDays(-2)
                },
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 8",
                    Description = "This is sample task 8 description.",
                    DueDate = DateTime.Now,
                    TodoItemStatus = TodoItemStatus.Completed,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new()
                {
                    Id = CombGenerator.GenerateComb(),
                    Title = "Sample Task 9",
                    Description = "This is sample task 9 description.",
                    DueDate = DateTime.Now.AddDays(2),
                    TodoItemStatus = TodoItemStatus.Pending,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            context.TodoItems.AddRange(todoItems);
            context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _todoContext.Database.EnsureDeleted();
            _todoContext.Dispose();
            _serviceProvider.Dispose();
        }

        private TodoItem CreateTodoItem(string title, DateTime? dueDate = null)
        {
            return new TodoItem()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = "Description",
                DueDate = dueDate ?? DateTime.Now.AddDays(1),
                CreatedAt = DateTime.Now,
                TodoItemStatus = TodoItemStatus.Pending,
                UpdatedAt = DateTime.Now
            };
        }

        [Test]
        public async Task Create_ShouldReturnResponse_WhenNewItemIsAdded()
        {
            
            var request = new CreateTodoItemRequest { Title = "New Item", Description = "Task description", DueDate = DateTime.Now.AddDays(2) };
            var todoItem = new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                TodoItemStatus = TodoItemStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var todoItemBase = new TodoItemBase
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                DueDate = todoItem.DueDate,
                TodoItemStatus = todoItem.TodoItemStatus
            };

            _mockTodoRepository.Setup(repo => repo.ItemExistsAsync(request.Title)).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<TodoItem>(request)).Returns(todoItem);
            _mockMapper.Setup(m => m.Map<TodoItemBase>(todoItem)).Returns(todoItemBase);
            _mockTodoRepository.Setup(repo => repo.AddAsync(todoItem)).Returns(Task.CompletedTask);

            
            var response = await _todoService.Create(request);

            
            var responseData = response.Data as TodoItemBase;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(responseData);
            Assert.AreEqual(todoItemBase.Title, responseData.Title);
        }

        [Test]
        public async Task Create_ShouldntCreateItem_WithExistingTitle()
        {
            
            var request = new CreateTodoItemRequest { Title = "Existing Item" };
            _mockTodoRepository.Setup(repo => repo.ItemExistsAsync(request.Title)).ReturnsAsync(true);

            
            var response = await _todoService.Create(request);

            
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Todo item with specified title already exist.", response.Data);
        }

        [Test]
        public async Task Create_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            
            var request = new CreateTodoItemRequest { Title = "Error Prone Item" };
            _mockTodoRepository.Setup(repo => repo.ItemExistsAsync(request.Title)).ThrowsAsync(new Exception("Database failure"));

            
            var response = await _todoService.Create(request);

            
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual("Database failure", response.Data);
        }

        [Test]
        public async Task Delete_CallsDeleteAsyncOnrepository_WithExistingId()
        {
            
            var existingTodoItem = CreateTodoItem("Todo item to be deleted.");


            _mockTodoRepository.Setup(repo => repo.GetByIdAsync(existingTodoItem.Id)).ReturnsAsync(existingTodoItem);
            _mockTodoRepository.Setup(repo => repo.DeleteAsync(existingTodoItem)).Returns(Task.CompletedTask);

            
            var response = await _todoService.Delete(existingTodoItem.Id);

            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            _mockTodoRepository.Verify(repo => repo.DeleteAsync(existingTodoItem), Times.Once);
        }

        [Test]
        public async Task Delete_ThrowsNotFoundException_WhenItemDoesNotExist()
        {
            
            var nonExistentId = Guid.NewGuid();

            _mockTodoRepository.Setup(repo => repo.GetByIdAsync(nonExistentId)).ReturnsAsync((TodoItem)null);

            
            var response = await _todoService.Delete(nonExistentId);

            
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            _mockTodoRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            
            var failingId = Guid.NewGuid();
            var exceptionMessage = "Database failure";
            _mockTodoRepository.Setup(repo => repo.GetByIdAsync(failingId)).ThrowsAsync(new Exception(exceptionMessage));

            
            var response = await _todoService.Delete(failingId);

            
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(exceptionMessage, response.Data);
        }

        [Test]
        public async Task GetFilteredTodoItems_ShouldReturnPaginatedListFilteredByTodoItemStatus()
        {
            
            var filter = new TodoItemFilter
            {
                PageIndex = 1,
                PageSize = 10,
                TodoItemStatus = TodoItemStatus.Pending
            };

            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Task 1" },
                new TodoItem { Id = Guid.NewGuid(), Title = "Task 2" }
            }.AsQueryable();

            var paginatedList = new PaginatedList<TodoItem>(
                todoItems.ToList(),
                pageIndex: 1,
                totalPages: 1
            );

            var mappedPaginatedList = new PaginatedList<TodoItemBase>(
                new List<TodoItemBase>
                {
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Task 1" },
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Task 2" }
                },
                pageIndex: 1,
                totalPages: 1
            );

            _mockTodoRepository.Setup(repo => repo.Filter(It.IsAny<TodoItemFilter>())).Returns(todoItems);
            _mockTodoRepository.Setup(repo => repo.GetPaginatedItemsWithFilter(It.IsAny<IQueryable<TodoItem>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);
            _mockMapper.Setup(m => m.Map<PaginatedList<TodoItemBase>>(It.IsAny<PaginatedList<TodoItem>>()))
                .Returns(mappedPaginatedList);

            
            var result = await _todoService.GetFilteredTodoItems(filter);
            var content = result.Data as PaginatedList<TodoItemBase>;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, content.Items.Count);
            _mockTodoRepository.Verify(repo => repo.Filter(filter), Times.Once);
            _mockTodoRepository.Verify(repo => repo.GetPaginatedItemsWithFilter(todoItems, filter.PageIndex, filter.PageSize), Times.Once);
            _mockMapper.Verify(m => m.Map<PaginatedList<TodoItemBase>>(paginatedList), Times.Once);
        }

        [Test]
        public async Task GetFilteredTodoItems_ShouldReturnPaginatedListFilteredByTitle()
        {
            
            var filter = new TodoItemFilter
            {
                PageIndex = 1,
                PageSize = 10,
                Title = "Sample"
            };

            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 1" },
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 2" }
            }.AsQueryable();

            var paginatedList = new PaginatedList<TodoItem>(
                todoItems.ToList(),
                pageIndex: 1,
                totalPages: 1
            );

            var mappedPaginatedList = new PaginatedList<TodoItemBase>(
                new List<TodoItemBase>
                {
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 1" },
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 2" }
                },
                pageIndex: 1,
                totalPages: 1
            );

            _mockTodoRepository.Setup(repo => repo.Filter(It.IsAny<TodoItemFilter>())).Returns(todoItems);
            _mockTodoRepository.Setup(repo => repo.GetPaginatedItemsWithFilter(It.IsAny<IQueryable<TodoItem>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);
            _mockMapper.Setup(m => m.Map<PaginatedList<TodoItemBase>>(It.IsAny<PaginatedList<TodoItem>>()))
                .Returns(mappedPaginatedList);

            
            var result = await _todoService.GetFilteredTodoItems(filter);

            var content = result.Data as PaginatedList<TodoItemBase>;
            
            Assert.IsNotNull(content);
            Assert.AreEqual(2, content.Items.Count);
            _mockTodoRepository.Verify(repo => repo.Filter(filter), Times.Once);
            _mockTodoRepository.Verify(repo => repo.GetPaginatedItemsWithFilter(todoItems, filter.PageIndex, filter.PageSize), Times.Once);
            _mockMapper.Verify(m => m.Map<PaginatedList<TodoItemBase>>(paginatedList), Times.Once);
        }

        [Test]
        public async Task GetFilteredTodoItems_ShouldReturnPaginatedListFilteredByDueDate()
        {
            
            var filter = new TodoItemFilter
            {
                PageIndex = 1,
                PageSize = 10,
                DueDate = DateTime.Now.AddDays(-2)
            };

            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 1", DueDate = DateTime.Now.AddDays(-2) },
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 2", DueDate = DateTime.Now.AddDays(-2) }
            }.AsQueryable();

            var paginatedList = new PaginatedList<TodoItem>(
                todoItems.ToList(),
                pageIndex: 1,
                totalPages: 1
            );

            var mappedPaginatedList = new PaginatedList<TodoItemBase>(
                new List<TodoItemBase>
                {
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 1", DueDate = DateTime.Now.AddDays(-2)},
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 2", DueDate = DateTime.Now.AddDays(-2)}
                },
                pageIndex: 1,
                totalPages: 1
            );

            _mockTodoRepository.Setup(repo => repo.Filter(It.IsAny<TodoItemFilter>())).Returns(todoItems);
            _mockTodoRepository.Setup(repo => repo.GetPaginatedItemsWithFilter(It.IsAny<IQueryable<TodoItem>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);
            _mockMapper.Setup(m => m.Map<PaginatedList<TodoItemBase>>(It.IsAny<PaginatedList<TodoItem>>()))
                .Returns(mappedPaginatedList);

            
            var result = await _todoService.GetFilteredTodoItems(filter);
            var content = result.Data as PaginatedList<TodoItemBase>;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, content.Items.Count);
            _mockTodoRepository.Verify(repo => repo.Filter(filter), Times.Once);
            _mockTodoRepository.Verify(repo => repo.GetPaginatedItemsWithFilter(todoItems, filter.PageIndex, filter.PageSize), Times.Once);
            _mockMapper.Verify(m => m.Map<PaginatedList<TodoItemBase>>(paginatedList), Times.Once);
        }

        [Test]
        public async Task GetFilteredTodoItems_ShouldReturnPaginatedListFilteredByCreatedDate()
        {
            
            var filter = new TodoItemFilter
            {
                PageIndex = 1,
                PageSize = 10,
                CreatedAt = DateTime.Now.AddDays(-2)
            };

            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 1", CreatedAt = DateTime.Now.AddDays(-2) },
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 2", CreatedAt = DateTime.Now.AddDays(-2) }
            }.AsQueryable();

            var paginatedList = new PaginatedList<TodoItem>(
                todoItems.ToList(),
                pageIndex: 1,
                totalPages: 1
            );

            var mappedPaginatedList = new PaginatedList<TodoItemBase>(
                new List<TodoItemBase>
                {
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 1" },
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 2" }
                },
                pageIndex: 1,
                totalPages: 1
            );

            _mockTodoRepository.Setup(repo => repo.Filter(It.IsAny<TodoItemFilter>())).Returns(todoItems);
            _mockTodoRepository.Setup(repo => repo.GetPaginatedItemsWithFilter(It.IsAny<IQueryable<TodoItem>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);
            _mockMapper.Setup(m => m.Map<PaginatedList<TodoItemBase>>(It.IsAny<PaginatedList<TodoItem>>()))
                .Returns(mappedPaginatedList);

            
            var result = await _todoService.GetFilteredTodoItems(filter);
            var content = result.Data as PaginatedList<TodoItemBase>;

            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, content.Items.Count);
            _mockTodoRepository.Verify(repo => repo.Filter(filter), Times.Once);
            _mockTodoRepository.Verify(repo => repo.GetPaginatedItemsWithFilter(todoItems, filter.PageIndex, filter.PageSize), Times.Once);
            _mockMapper.Verify(m => m.Map<PaginatedList<TodoItemBase>>(paginatedList), Times.Once);
        }

        [Test]
        public async Task GetFilteredTodoItems_ShouldReturnPaginatedList()
        {
            
            var filter = new TodoItemFilter
            {
                PageIndex = 1,
                PageSize = 10
            };

            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 1"},
                new TodoItem { Id = Guid.NewGuid(), Title = "Sample Task 2"}
            }.AsQueryable();

            var paginatedList = new PaginatedList<TodoItem>(
                todoItems.ToList(),
                pageIndex: 1,
                totalPages: 1
            );

            var mappedPaginatedList = new PaginatedList<TodoItemBase>(
                new List<TodoItemBase>
                {
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 1"},
                    new TodoItemBase { Id = Guid.NewGuid(), Title = "Sample Task 2"}
                },
                pageIndex: 1,
                totalPages: 1
            );

            _mockTodoRepository.Setup(repo => repo.Filter(It.IsAny<TodoItemFilter>())).Returns(todoItems);
            _mockTodoRepository.Setup(repo => repo.GetPaginatedItemsWithFilter(It.IsAny<IQueryable<TodoItem>>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);
            _mockMapper.Setup(m => m.Map<PaginatedList<TodoItemBase>>(It.IsAny<PaginatedList<TodoItem>>()))
                .Returns(mappedPaginatedList);

            
            var result = await _todoService.GetFilteredTodoItems(filter);
            var content = result.Data as PaginatedList<TodoItemBase>;

            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, content.Items.Count);
            Assert.AreEqual(false, content.HasNextPage);
            Assert.AreEqual(false, content.HasPreviousPage);
            _mockTodoRepository.Verify(repo => repo.Filter(filter), Times.Once);
            _mockTodoRepository.Verify(repo => repo.GetPaginatedItemsWithFilter(todoItems, filter.PageIndex, filter.PageSize), Times.Once);
            _mockMapper.Verify(m => m.Map<PaginatedList<TodoItemBase>>(paginatedList), Times.Once);
        }

        [Test]
        public async Task GetFilteredTodoItems_ShouldReturnErrorResponse_WhenExceptionThrown()
        {
            
            var todoItemFilter = new TodoItemFilter
            {
                PageIndex = 1,
                PageSize = 10
            };

            _mockTodoRepository.Setup(r => r.Filter(It.IsAny<TodoItemFilter>())).Throws(new Exception("Test exception"));

            
            var response = await _todoService.GetFilteredTodoItems(todoItemFilter);

            
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            response.Data.Should().Be("Test exception");
        }


        [Test]
        public async Task Update_UpdatesFields_WhenItemExists()
        {
            var existingItem = CreateTodoItem("Old Title");

            var request = new UpdateTodoItemRequest
            {
                Id = existingItem.Id,
                Title = "New Title",
                Description = "New Description",
                DueDate = DateTime.Now.AddDays(1),
                TodoItemStatus = TodoItemStatus.Completed
            };


            _mockTodoRepository.Setup(x => x.GetByIdAsync(existingItem.Id)).ReturnsAsync(existingItem);
            _mockTodoRepository.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>())).Returns(Task.CompletedTask);

            var result = await _todoService.Update(request);

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            _mockTodoRepository.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenItemDoesNotExist()
        {
            
            var request = new UpdateTodoItemRequest
            {
                Id = Guid.NewGuid(),
                Title = "New Title",
                Description = "New Description",
                DueDate = DateTime.Now.AddDays(1),
                TodoItemStatus = TodoItemStatus.Completed
            };

            _mockTodoRepository.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync((TodoItem)null);

            
            var result = await _todoService.Update(request);

            
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public async Task Update_ShouldReturnBadRequest_WhenTodoItemWithTitleAlreadyExists()
        {
            
            var request = new UpdateTodoItemRequest
            {
                Id = Guid.NewGuid(),
                Title = "Existing Title",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(1),
                TodoItemStatus = TodoList.Domain.Enums.TodoItemStatus.Pending
            };

            var existingTodoItem = new TodoItem
            {
                Id = request.Id,
                Title = "Original Title",
                Description = "Original Description",
                DueDate = DateTime.Now.AddDays(2),
                TodoItemStatus = TodoList.Domain.Enums.TodoItemStatus.Pending,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now
            };

            _mockTodoRepository.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(existingTodoItem);
            _mockTodoRepository.Setup(r => r.ItemExistsAsync(request.Title, request.Id)).ReturnsAsync(true);

            
            var response = await _todoService.Update(request);

            
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Todo item with specified title already exist.", response.Data);
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            
            var request = new UpdateTodoItemRequest
            {
                Id = Guid.NewGuid(),
                Title = "New Title",
                Description = "New Description",
                DueDate = DateTime.Now.AddDays(1),
                TodoItemStatus = TodoItemStatus.Completed
            };

            _mockTodoRepository.Setup(r => r.GetByIdAsync(request.Id)).ThrowsAsync(new Exception("Database error"));

            
            var result = await _todoService.Update(request);

            
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.AreEqual("Database error", result.Data);
        }
    }
}