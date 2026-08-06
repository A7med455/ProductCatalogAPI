using productCatalogAPI.Services;
using Serilog;

// ---------- Logging setup ----------
// Configure Serilog: where logs go (console + a new file each day).
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();  // remove default providers, we choose explicitly
builder.Host.UseSerilog();         // route all ILogger calls through Serilog

// ---------- Services ----------
builder.Services.AddControllers();

// Reject requests containing fields that don't exist on the target model
// (e.g. POSTing "hackedField" to /api/products now returns 400 instead of being ignored)
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.UnmappedMemberHandling =
        System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow;
});

builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// Logger for use in the middleware below (fetched once, reused every request)
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// ---------- Global exception handling + request timing middleware ----------
app.Use(async (context, next) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    try
    {
        await next.Invoke(); // run the rest of the pipeline (controllers etc.)
    }
    catch (Exception ex)
    {
        // Unexpected, unhandled failure -> Error level
        logger.LogError("Global Exception Caught: {ErrorMessage}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500; // internal server error, not the caller's fault
        await context.Response.WriteAsync("{\"error\": \"Something went wrong. Please try again later.\"}");
    }
    finally
    {
        // Always runs, success or failure -> Information level (routine, not a problem)
        stopwatch.Stop();
        logger.LogInformation("{Method} {Path} took {ElapsedMs}ms",
            context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
    }
});

app.MapControllers();
app.Run();