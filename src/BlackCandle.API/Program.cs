using BlackCandle.API.Configuration;
using BlackCandle.API.Middleware;
using BlackCandle.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddWebServices();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProjectServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseWebServices();
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

/// <summary>
///     Точка входа в приложение
/// </summary>
public partial class Program { }
