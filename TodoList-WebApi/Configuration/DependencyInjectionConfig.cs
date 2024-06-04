using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Domain.Interfaces.Services;
using TodoList.Domain.Services;
using TodoList.Infra.Context;
using TodoList.Infra.Hubs;
using TodoList.Infra.Repositories;
using TodoList.Infra.Services;

namespace TodoList_WebApi.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSignalR();

            services.AddSingleton<NotificationHub>();
            
            services.AddHostedService<TodoItemStatusUpdaterService>();

            services.AddDbContext<TodoContext>(options =>
                options.UseInMemoryDatabase("TodoDatabase"));

            #region services
            services.AddScoped<ITodoService, TodoService>();
            #endregion


            #region repositories
            services.AddScoped<ITodoRepository, TodoRepository>();
            #endregion

            return services;
        }
    }
}
