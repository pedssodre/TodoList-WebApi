using AutoMapper;
using System.Net;
using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Interfaces.Repositories;
using TodoList.Domain.Interfaces.Services;

namespace TodoList.Domain.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;

        public TodoService(ITodoRepository todoRepository, IMapper mapper)
        {
            _todoRepository = todoRepository;
            _mapper = mapper;
        }

        public async Task<CreateTodoItemResponse> Create(CreateTodoItemRequest request)
        {
            try
            {
                if (await _todoRepository.ItemExistsAsync(request.Title))
                {
                    return new()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Data = "Todo item with specified title already exist."
                    };
                }

                var todoItem = _mapper.Map<TodoItem>(request);

                todoItem.TodoItemStatus = TodoItemStatus.Pending;
                todoItem.CreatedAt = DateTime.Now;

                await _todoRepository.AddAsync(todoItem);

                var createdItem = _mapper.Map<TodoItemBase>(todoItem);

                return new CreateTodoItemResponse(createdItem);
            }
            catch (Exception ex)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = ex.Message
                };
            }
        }

        public async Task<ApiResponse> Delete(Guid id)
        {
            try
            {
                var todoItem = await _todoRepository.GetByIdAsync(id);

                if (todoItem != null)
                {
                    await _todoRepository.DeleteAsync(todoItem);
                    return new()
                    {
                        StatusCode = HttpStatusCode.OK
                    };
                }

                return new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Data = "Todo item not found"
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = ex.Message
                };
            }
        }

        public async Task<GetAllTodoItemsResponse> GetFilteredTodoItems(TodoItemFilter todoItemFilter)
        {
            try
            {
                var todoItemList =  _todoRepository.Filter(todoItemFilter);

                var paginatedList = await _todoRepository.GetPaginatedItemsWithFilter(todoItemList, todoItemFilter.PageIndex, todoItemFilter.PageSize);

                var paginatedListTodoItemBase = _mapper.Map<PaginatedList<TodoItemBase>>(paginatedList);

                return new GetAllTodoItemsResponse(paginatedListTodoItemBase);
            }
            catch (Exception ex)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = ex.Message
                };
            }
        }

        public async Task<UpdateTodoItemResponse> Update(UpdateTodoItemRequest request)
        {
            try
            {
                var todoItem = await _todoRepository.GetByIdAsync(request.Id);

                if (todoItem != null && await _todoRepository.ItemExistsAsync(request.Title, request.Id))
                {
                    return new()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Data = "Todo item with specified title already exist."
                    };
                }

                if (todoItem != null)
                {
                    todoItem.Title = request.Title;
                    todoItem.Description = request.Description;
                    todoItem.DueDate = request.DueDate;
                    todoItem.TodoItemStatus = request.TodoItemStatus;
                    todoItem.UpdatedAt = DateTime.Now;

                    await _todoRepository.UpdateAsync(todoItem);

                    return new()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Data = todoItem
                    };
                }

                return new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Data = "Todo item not found"
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = ex.Message
                };
            }
        }
    }
}
