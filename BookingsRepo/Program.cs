using BookingsGrpcServer.Services;
using Infrastructure.Business.Managers;
using Infrastructure.Business.Service;
using Infrastructure.Data.Context;
using Infrastructure.Data.Models;
using Infrastructure.Data.Repository;
using Infrastructure.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<IBaseRepository<BookingEntity>, BookingRepository>();
builder.Services.AddScoped<BookingManager>();
builder.Services.AddHttpClient<EventServiceClient>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["ServiceBus:ConnectionString"];
    var queueName = config["ServiceBus:QueueName"];

    return new BookingServiceBusSender(connectionString, queueName);
});

var app = builder.Build();

app.MapGrpcService<BookingGrpcService>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
