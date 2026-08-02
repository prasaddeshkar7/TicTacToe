using TicTacToe.Api.Exceptions;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register dependencies for Dependency Injection
builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddSingleton<IScoreboardRepository, ScoreboardRepository>();
builder.Services.AddSingleton<IComputerStrategy, ComputerStrategy>();
builder.Services.AddSingleton<IGameService, GameService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseCors("Angular");

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
