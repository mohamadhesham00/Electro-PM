using API.Extensions;
using API.Middlewares;
using Application;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register the global exception handler middleware and ProblemDetails for standardized error responses
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); 

// Register services from the Application and Infrastructure layers
builder.Services.AddInfrastructure(builder.Configuration)
    .AddApplication();

// 3. Swagger Interface Documentation Setup
builder.Services.AddSwaggerDocumentation();

// Authorization policies layer activation
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseSwaggerDocumentation(app.Environment);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();