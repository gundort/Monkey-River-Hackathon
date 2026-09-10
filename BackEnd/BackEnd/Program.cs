using BackEnd.Extensions;
using BackEnd.Models;
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddCustomCors()
    .AddCustomSwagger()
    .AddMongoDb(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddAppDependencies();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

await CheckMongoDbConnectionAsync(app);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseExceptionHandler("/error");

app.Use(async (context, next) =>
{
    try { await next.Invoke(); }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception occurred.");
        throw;
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

async Task CheckMongoDbConnectionAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var connected = await mongoContext.PingAsync();
        if (connected)
            logger.LogInformation("Connected to MongoDB successfully.");
        else
            logger.LogError("Failed to connect to MongoDB.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Exception while connecting to MongoDB.");
    }
}
