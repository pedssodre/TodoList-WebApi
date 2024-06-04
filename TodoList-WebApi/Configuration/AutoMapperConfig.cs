using AutoMapper;
using TodoList.Domain.DTOs.Common;
using TodoList.Domain.DTOs.TodoItemDTO;
using TodoList.Domain.Entities;

namespace TodoList_WebApi.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CreateTodoItemRequest, TodoItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.TodoItemStatus, opt => opt.Ignore());
            CreateMap<TodoItem, TodoItemBase>();
            CreateMap<PaginatedList<TodoItem>, PaginatedList<TodoItemBase>>();
            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>));
        }
    }
}
