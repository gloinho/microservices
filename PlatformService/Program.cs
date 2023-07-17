using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Data.Interfaces;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region AutoMapper

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

#endregion

#region Database

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("--> Using In Memory Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMemoryDb"));
}
else
{
    Console.WriteLine("--> Using SQL Server Db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection")));
}

#endregion

#region Injection

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

#endregion

var app = builder.Build();

#region Db Preparation

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

#endregion

Console.WriteLine($"--> CommandService Endpoint: {builder.Configuration["CommandsService"]}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();



