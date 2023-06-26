#region Globals
global using Microsoft.AspNetCore.Mvc;
global using Newtonsoft.Json;
global using Nexus.Party.Master.Api.Controllers.Base;
global using Nexus.Tools.Validations.Middlewares.Authentication.Attributes;
#endregion

using Nexus.Party.Master.Domain;
using Nexus.Party.Master.Domain.Spotify;
using Nexus.Tools.Validations.Middlewares.Authentication;

var builder = WebApplication.CreateBuilder(args);
var authHelper = new AuthenticationHelper(new(BaseController.AuthConnString));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddHostedService(provider => provider.GetService<SyncService>());

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

// Use Nexus Middleware for control clients authentications
app.UseAuthentication(authHelper.ValidAuthenticationAsync);

app.MapControllers();

app.Run();
