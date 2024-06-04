using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoList.Domain.Enums;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Infra.Hubs;

namespace TodoList.Infra.Services
{
    public class TodoItemStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly NotificationHub _hubContext;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public TodoItemStatusUpdaterService(IServiceScopeFactory scopeFactory, NotificationHub hubContext, IHostApplicationLifetime applicationLifetime)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
            _applicationLifetime = applicationLifetime;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(async () =>
            {
                await RunBackgroundService(cancellationToken);
            });

            return Task.CompletedTask;
        }

        private async Task RunBackgroundService(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var todoRepository = scope.ServiceProvider.GetRequiredService<ITodoRepository>();
                var items = await todoRepository.GetAll().ToListAsync();
                bool updated = false;

                foreach (var item in items)
                {
                    if (item.TodoItemStatus == TodoItemStatus.Pending && item.DueDate.Date < DateTime.Now.Date)
                    {
                        item.TodoItemStatus = TodoItemStatus.Overdue;
                        item.UpdatedAt = DateTime.Now;
                        await todoRepository.UpdateAsync(item);
                        updated = true;
                    }
                }

                if (updated)
                {
                    await _hubContext.SendTodoUpdate("Todo items updated");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }

        }
    }
}
