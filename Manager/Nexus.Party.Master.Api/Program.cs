#region Globals
global using Microsoft.AspNetCore.Mvc;
global using Nexus.Party.Master.Api.Controllers.Base;
global using Nexus.Party.Master.Domain.Helpers;
global using Newtonsoft.Json;
using Nexus.Party.Master.Domain;
using Nexus.Party.Master.Domain.Services;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomService<ISyncService, SyncService>();
builder.Services.AddCustomService<CategorizerService>();

var app = builder.Build();
var confg = app.Configuration;

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseRouting();

app.UseCors(builder =>
      builder
          .WithOrigins(confg.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>())
          .WithHeaders(confg.GetSection("Cors:Headers").Get<string[]>() ?? Array.Empty<string>())
          .AllowAnyMethod()
          .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();