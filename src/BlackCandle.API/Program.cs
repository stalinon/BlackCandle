using BlackCandle.API.Configuration;
using BlackCandle.API.Middleware;
using BlackCandle.Web;
using BlackCandle.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddWebAdmin();

builder.Services.AddAuthorization();
builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProjectServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwaggerDocumentation();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();

/// <summary>
///     Точка входа в приложение
/// </summary>
public partial class Program { }
