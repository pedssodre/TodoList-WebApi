using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Infra.Context;
using TodoList.Infra.Repositories;

namespace TodoList.Tests.Data
{
    [TestFixture]
    public class TodoRepositoryTests
    {
        private TodoContext _context;
        private TodoRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoDatabase")
                .Options;

            _context = new TodoContext(options);
            _repository = new TodoRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {

            var todoItems = new List<TodoItem>
            {
                new() { 
                    Id = Guid.NewGuid(), 
                    Title = "Task 1", 
                    CreatedAt = DateTime.Now.AddDays(-1), 
                    DueDate = DateTime.Now.AddDays(1), 
                    TodoItemStatus = TodoItemStatus.Pending 
                },
                new() { 
                    Id = Guid.NewGuid(), 
                    Title = "Task 2", 
                    CreatedAt = DateTime.Now.AddDays(-2), 
                    DueDate = DateTime.Now.AddDays(2), 
                    TodoItemStatus = TodoItemStatus.Completed 
                },
                new() { 
                    Id = Guid.NewGuid(), 
                    Title = "Task 3", CreatedAt = DateTime.Now.AddDays(-3), 
                    DueDate = DateTime.Now.AddDays(3), 
                    TodoItemStatus = TodoItemStatus.Pending 
                },
                new() { 
                    Id = Guid.NewGuid(), 
                    Title = "Task 4", 
                    CreatedAt = DateTime.Now.AddDays(-4), 
                    DueDate = DateTime.Now.AddDays(4), 
                    TodoItemStatus = TodoItemStatus.Overdue 
                },
                new() { 
                    Id = Guid.NewGuid(), 
                    Title = "Task 5", 
                    CreatedAt = DateTime.Now.AddDays(-5), 
                    DueDate = DateTime.Now.AddDays(5), 
                    TodoItemStatus = TodoItemStatus.Pending 
                }
            };

            _context.TodoItems.AddRange(todoItems);
            _context.SaveChanges();
        }

        [Test]
        public void GetAll_ShouldReturnAllItems()
        {
            var items = _repository.GetAll().ToList();

            Assert.AreEqual(5, items.Count);
        }

        [Test]
        public async Task GetPaginatedItemsWithFilter_ShouldReturnPaginatedList()
        {
            var itemsFiltered = _context.TodoItems.AsQueryable();
            int pageIndex = 1;
            int qt = 5;

            var result = await _repository.GetPaginatedItemsWithFilter(itemsFiltered, pageIndex, qt);

            Assert.IsNotNull(result);
            Assert.AreEqual(pageIndex, result.PageIndex);
            Assert.AreEqual((int)(5 / qt) + 1, result.TotalPages);
            Assert.AreEqual(qt, result.Items.Count);
        }

        [Test]
        public void Filter_ShouldReturnFilteredItemsByStatus()
        {
            var filter = new TodoItemFilter
            {
                TodoItemStatus = TodoItemStatus.Pending
            };

            var result = _repository.Filter(filter).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(t => t.TodoItemStatus == TodoItemStatus.Pending));
        }

        [Test]
        public void Filter_ShouldReturnFilteredItemsByCreatedAt()
        {
            var filter = new TodoItemFilter
            {
                CreatedAt = DateTime.Now.AddDays(-3)
            };

            var result = _repository.Filter(filter).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(t => t.CreatedAt <= DateTime.Now.AddDays(-3)));
        }

        [Test]
        public void Filter_ShouldReturnFilteredItemsByOverDueDate()
        {
            var filter = new TodoItemFilter
            {
                DueDate = DateTime.Now.AddDays(3)
            };

            var result = _repository.Filter(filter).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(t => t.DueDate <= DateTime.Now.AddDays(3)));
        }

        [Test]
        public void Filter_ShouldReturnFilteredItemsByTitle()
        {
            var filter = new TodoItemFilter
            {
                Title = "Task 1"
            };

            var result = _repository.Filter(filter).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.All(t => t.Title.Contains("Task 1")));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnItem_WhenItemExists()
        {
            var existingItem = _context.TodoItems.First();

            var item = await _repository.GetByIdAsync(existingItem.Id);

            Assert.IsNotNull(item);
            Assert.AreEqual(existingItem.Id, item.Id);
        }

        [Test]
        public async Task AddAsync_ShouldAddNewItem()
        {
            var newItem = new TodoItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 3",
                Description = "Description 3",
                DueDate = DateTime.Now.AddDays(3),
                TodoItemStatus = TodoItemStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.AddAsync(newItem);
            var item = await _repository.GetByIdAsync(newItem.Id);

            Assert.IsNotNull(item);
            Assert.AreEqual(newItem.Title, item.Title);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateExistingItem()
        {
            var existingItem = _context.TodoItems.First();
            existingItem.Title = "Updated Task";

            await _repository.UpdateAsync(existingItem);
            var item = await _repository.GetByIdAsync(existingItem.Id);

            Assert.IsNotNull(item);
            Assert.AreEqual("Updated Task", item.Title);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveItem()
        {
            var existingItem = _context.TodoItems.First();

            await _repository.DeleteAsync(existingItem);
            var item = await _repository.GetByIdAsync(existingItem.Id);

            Assert.IsNull(item);
        }

        [Test]
        public async Task ItemExistsAsync_ShouldReturnTrue_WhenItemExists()
        {
            var existingItem = _context.TodoItems.First();

            var exists = await _repository.ItemExistsAsync(existingItem.Title);

            Assert.IsTrue(exists);
        }

        [Test]
        public async Task ItemExistsAsync_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            var exists = await _repository.ItemExistsAsync("Non-Existent Task");

            Assert.IsFalse(exists);
        }

        [Test]
        public async Task ItemExistsAsync_ShouldReturnTrue_WhenItemWithTitleExists()
        {
            var title = "Task 1";
            var excludeId = Guid.NewGuid();

            var result = await _repository.ItemExistsAsync(title, excludeId);

            result.Should().BeTrue();
        }

        [Test]
        public async Task ItemExistsAsync_ShouldReturnFalse_WhenItemWithTitleDoesNotExist()
        {
            var title = "Non-Existent Task";
            var excludeId = Guid.NewGuid();

            var result = await _repository.ItemExistsAsync(title, excludeId);

            result.Should().BeFalse();
        }

        [Test]
        public async Task ItemExistsAsync_ShouldReturnFalse_WhenItemWithTitleExistsButIdMatches()
        {
            var title = "Task 1";
            var existingItem = await _context.TodoItems.FirstAsync(x => x.Title == title);

            var result = await _repository.ItemExistsAsync(title, existingItem.Id);

            result.Should().BeFalse();
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
