using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Domain.Interfaces.Services;
using TodoList.Infra.Context;
using TodoList.Infra.Hubs;
using TodoList.Infra.Services;
using TodoList_WebApi.Configuration;

namespace TodoList.Tests.Api
{
    public class DependencyInjectionConfigTests
    {
        private IServiceCollection _services;
        private IConfiguration _configuration;
        private DbContextOptions<TodoContext> _dbContextOptions;


        [SetUp]
        public void Setup()
        {
            _services = new ServiceCollection();
            var configurationMock = new Mock<IConfiguration>();
            _configuration = configurationMock.Object;

            _dbContextOptions = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoListTest")
                .Options;

            _services.AddSingleton(_dbContextOptions);

            _services.AddAutoMapper(typeof(AutoMapperConfig));

            var hostApplicationLifetimeMock = new Mock<IHostApplicationLifetime>();
            _services.AddSingleton(hostApplicationLifetimeMock.Object);
        }

        [Test]
        public void ResolveDependencies_ShouldRegisterAllDependecies()
        {
            _services.ResolveDependencies(_configuration);
            var serviceProvider = _services.BuildServiceProvider();

            // HttpClient
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            Assert.IsNotNull(httpClientFactory);

            // IHttpContextAccessor
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
            Assert.IsNotNull(httpContextAccessor);

            // Signal R
            var signalRHub = serviceProvider.GetService<NotificationHub>();
            Assert.IsNotNull(signalRHub);

            // HostedService
            var hostedService = serviceProvider.GetService<IHostedService>() as TodoItemStatusUpdaterService;
            Assert.IsNotNull(hostedService);

            // DbContext
            var dbContext = serviceProvider.GetService<TodoContext>();
            Assert.IsNotNull(dbContext);

            // Services
            var todoService = serviceProvider.GetService<ITodoService>();
            Assert.IsNotNull(todoService);

            // Repositories
            var todoRepository = serviceProvider.GetService<ITodoRepository>();
            Assert.IsNotNull(todoRepository);
        }

        [Test]
        public void ResolveDependencies_DbContext_ShouldBeInMemoryDatabase()
        {
            _services.ResolveDependencies(_configuration);
            var serviceProvider = _services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<TodoContext>();
            Assert.IsNotNull(dbContext);
            Assert.IsTrue(dbContext.Database.IsInMemory());
        }
    }
}
