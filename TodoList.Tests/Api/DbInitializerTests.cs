using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TodoList_WebApi.Configuration;
using TodoList.Infra.Context;
using TodoList.Domain.Entities;
using Moq;

namespace TodoList.Tests.Api
{
    public class DbInitializerTests
    {
        private ServiceProvider _serviceProvider;
        private DbContextOptions<TodoContext> _dbContextOptions;

        [SetUp]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEntityFrameworkInMemoryDatabase();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoListTest")
                .Options;

            serviceCollection.AddSingleton(_dbContextOptions);
            serviceCollection.AddScoped<TodoContext>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Test]
        public void Initialize_ShouldSeedDatabaseWithSampleData()
        {
            using var context = new TodoContext(_dbContextOptions);
            context.Database.EnsureCreated();
            DbInitializer.Initialize(_serviceProvider);

            Assert.AreEqual(9, context.TodoItems.Count());

            var sampleTask1 = context.TodoItems.FirstOrDefault(t => t.Title == "Sample Task 1");
            Assert.IsNotNull(sampleTask1);
            Assert.AreEqual("This is sample task 1 description.", sampleTask1.Description);
        }

        [Test]
        public void Initialize_ShouldNotSeedSamplesWhenDataAlreadyExists()
        {
            var context = new TodoContext(_dbContextOptions);
            context.Database.EnsureCreated();

            DbInitializer.Initialize(_serviceProvider);
            Assert.AreEqual(9, context.TodoItems.Count());

            DbInitializer.Initialize(_serviceProvider);

            Assert.AreEqual(9, context.TodoItems.Count());
        }

        [TearDown]
        public void TearDown()
        {
            using var context = new TodoContext(_dbContextOptions);
            context.Database.EnsureDeleted();

            _serviceProvider.Dispose();
        }
    }
}
