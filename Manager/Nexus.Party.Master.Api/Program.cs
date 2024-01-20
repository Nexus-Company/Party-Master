#region Globals
global using Microsoft.AspNetCore.Mvc;
global using Newtonsoft.Json;
global using Nexus.Party.Master.Api.Controllers.Base;
global using Nexus.Party.Master.Domain.Helpers;
global using Nexus.Party.Master.Domain.Services;
global using Nexus.Tools.Validations.Middlewares.Authentication.Attributes;
using Microsoft.EntityFrameworkCore;
using Nexus.OAuth.Libary;
using Nexus.Party.Master.Dal;
using Nexus.Stock.Domain.Helpers;
using Nexus.Tools.Validations.Middlewares.Authentication;
#endregion

var builder = WebApplication.CreateBuilder(args);
var oAuthApp = new Application(builder.Configuration["OAuth:ClientId"]!, builder.Configuration["OAuth:Secret"]!);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuthenticationContext>(options =>
{
    options.UseSqlite(@$"Data Source={AppDomain.CurrentDomain.BaseDirectory}\Databases\Authentication.db");
});
builder.Services.AddSingleton(oAuthApp);
builder.Services.AddScoped<IAuthenticationContextFactory, AuthenticationHelper>();
builder.Services.AddCustomService<ISyncService, SyncService>();
builder.Services.AddCustomService<CategorizerService>();

var app = builder.Build();
var config = app.Configuration;

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
          .WithOrigins(config.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>())
          .WithHeaders(config.GetSection("Cors:Headers").Get<string[]>() ?? Array.Empty<string>())
          .AllowAnyMethod()
          .AllowCredentials());

app.UseHttpsRedirection();

// Use Nexus Middleware for control clients authentications
app.UseAuthentication<AuthenticationContext, Application>(AuthenticationHelper.CheckAuthenticationAsync);

app.MapControllers();

app.Run();