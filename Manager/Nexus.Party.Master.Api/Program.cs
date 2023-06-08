#region Globals
global using Microsoft.AspNetCore.Mvc;
global using Nexus.Party.Master.Api.Controllers.Base;
global using Newtonsoft.Json;
using Nexus.Party.Master.Domain;

#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddHostedService(provider => provider.GetService<SyncService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

app.Run();
