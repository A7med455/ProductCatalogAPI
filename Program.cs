using productCatalogAPI.Services;
using Serilog;

// Serilog setup, separate from builder.Logging below
// Configures WHERE logs go: console AND a file, one new file per day.
Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("logs/myapp.txt",rollingInterval:RollingInterval.Day).CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// --- NEW THIS LESSON ---
// By default, ASP.NET Core auto-registers a bunch of logging providers you didn't choose.
// ClearProviders() wipes that default set so YOU decide exactly what's active.
builder.Logging.ClearProviders();

// Adds the Console provider back in: log lines will print to your terminal
// wherever you run "dotnet run".
//builder.Logging.AddConsole();

// Adds the Debug provider: log lines will also show up in your IDE's
// debug/output window when you're running with a debugger attached.
//builder.Logging.AddDebug();


// Register controllers so ASP.NET Core scans for classes like ProductsController
builder.Services.AddControllers();

// Register the service: whenever something asks for IProductService, give it a ProductService
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// app.Services is ASP.NET Core's "warehouse" of everything it knows how to build,
// including ILogger now that we configured providers above.
// GetRequiredService<ILogger<Program>>() = "hand me a logger, tag it as coming from Program.cs"
// We only do this ONCE here (not inside app.Use below), because the logger tool itself
// doesn't change per-request — only the messages we log with it change.
var logger = app.Services.GetRequiredService<ILogger<Program>>();

//(global handler) catches anything wrong after it in the pipeline
app.Use(async(context,next)=>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    try
    {
        await next.Invoke();
    }catch(Exception ex)
    {
        // Old: Console.WriteLine($"Global Exception Caught:{ex.Message}");
        // New: logger.LogError(...) — same idea as Console.WriteLine, but this line
        // is now permanently tagged as "Error" severity, and goes to every provider
        // we registered above (Console AND Debug), not just the terminal.
        // {ErrorMessage} is a placeholder; ex.Message is the value slotted into it.
        logger.LogError("Global Exception Caught: {ErrorMessage}", ex.Message);

        //tell the API "the body i'm sending back is JSON"
        context.Response.ContentType="application/json";
        // 500 = "Internal Server Error"  tells the caller it's our fault, not theirs
        context.Response.StatusCode =500;
        // actually writes that friendly message into the response body sent back to the caller
        // await = don't freeze the server while this gets sent over the network
        await context.Response.WriteAsync("{\"error\": \"Something went wrong. Please try again later.\"}");
    }
    finally
    {
        stopwatch.Stop();

        // Old: Console.WriteLine($"{context.Request.Method} {context.Request.Path} took {stopwatch.ElapsedMilliseconds}ms");
        // New: logger.LogInformation(...) — tagged as "Information" severity (routine,
        // not a problem) since this runs on EVERY request, success or failure alike.
        logger.LogInformation("{Method} {Path} took {ElapsedMs}ms",
            context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
    }
});
// Wires up all your [HttpGet], [HttpPost], etc. routes from your controllers
app.MapControllers();

app.Run();