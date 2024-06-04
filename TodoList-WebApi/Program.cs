using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using TodoList.Domain.Validations.TodoItem;
using TodoList.Infra.Hubs;
using TodoList_WebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateTodoItemRequestValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<UpdateTodoItemRequestValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<TodoItemFilterValidator>();
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo List Api", Version = "v1" });
    options.UseInlineDefinitionsForEnums();
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ResolveDependencies(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbInitializer.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo List Api V1");
    });
}

app.MapHub<NotificationHub>("/notificationhub", options =>
{
    options.Transports= HttpTransportType.WebSockets;
});

app.UseCors("AllowAllHeaders");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
