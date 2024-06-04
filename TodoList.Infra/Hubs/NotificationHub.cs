using Microsoft.AspNetCore.SignalR;

namespace TodoList.Infra.Hubs
{
    public class NotificationHub : Hub
    {
        protected IHubContext<NotificationHub> _context;

        public NotificationHub(IHubContext<NotificationHub> context)
        {
            _context = context;
        }

        public async Task SendTodoUpdate(string message)
        {
            await _context.Clients.All.SendAsync("TodoItemUpdated", message);
        }
    }
}
