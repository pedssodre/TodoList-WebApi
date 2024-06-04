using Microsoft.AspNetCore.Mvc;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Interfaces.Services;

namespace TodoList_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateTodoItemRequest request)
        {
            CreateTodoItemResponse createTodoItemResponse = await _todoService.Create(request);
            return StatusCode(createTodoItemResponse.StatusCode.GetHashCode(), createTodoItemResponse.Data);
        }

        [HttpPut]
        public async Task<ActionResult> Update(UpdateTodoItemRequest request)
        {
            var response = await _todoService.Update(request);
            return StatusCode(response.StatusCode.GetHashCode(), response.Data);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Guid id)
        {
            var response = await _todoService.Delete(id);
            return StatusCode(response.StatusCode.GetHashCode(), response.Data);
        }

        [HttpGet]
        public async Task<ActionResult<GetAllTodoItemsResponse>> GetAllTodoItemsWithFilter([FromQuery] TodoItemFilter filter)
        {
            var response = await _todoService.GetFilteredTodoItems(filter);
            return StatusCode(response.StatusCode.GetHashCode(), response.Data);
        }
    }
}
