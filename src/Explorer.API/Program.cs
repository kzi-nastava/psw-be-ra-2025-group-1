using Explorer.API.Adapters.Tours;
using Explorer.API.Demo;
using Explorer.API.Middleware;
using Explorer.API.Startup;
using Explorer.Payments.Core.Domain.External;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Repositories;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.UseCases;
using Explorer.Tours.Core.Adapters;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

// Create wwwroot for images
var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
if (!Directory.Exists(wwwroot))
    Directory.CreateDirectory(wwwroot);

var builder = WebApplication.CreateBuilder(args);

// Set test database environment variables in development - COMMENTED OUT FOR MAIN DB ACCESS
if (builder.Environment.IsDevelopment())
{
    // Only set test database if we're actually running tests
    var isRunningTests = Environment.GetEnvironmentVariable("RUNNING_TESTS") == "true";

    if (isRunningTests)
    {
        Environment.SetEnvironmentVariable("DATABASE_SCHEMA", "explorer-v1-test");
        Environment.SetEnvironmentVariable("DATABASE_HOST", "localhost");
        Environment.SetEnvironmentVariable("DATABASE_PORT", "5432");
        Environment.SetEnvironmentVariable("DATABASE_USERNAME", "postgres");
        Environment.SetEnvironmentVariable("DATABASE_PASSWORD", "root");
    }
}

builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);
const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<Explorer.Tours.Core.Domain.RepositoryInterfaces.IUserLocationRepository, UserLocationAdapter>(); // So there is no link between modules
builder.Services.AddScoped<DemoSeeder>();
builder.Services.AddScoped<ITourInfoService,TourInfoServiceAdapter>();



builder.Services.RegisterModules();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// Check for seed argument
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DemoSeeder>();
    seeder.Seed();
    return;
}

// Added for UploadController
app.UseStaticFiles();

app.UseRouting();
app.UseCors(corsPolicy);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Required for automated tests
namespace Explorer.API
{
    public partial class Program { }
}