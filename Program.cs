using productCatalogAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers so ASP.NET Core scans for classes like ProductsController
builder.Services.AddControllers();

// Register the service: whenever something asks for IProductService, give it a ProductService
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();
//catches anything wrong after it in the pipeline
app.Use(async(context,next)=>
{
    try
    {
        await next.Invoke();
    }catch(Exception ex)
    {
        Console.WriteLine($"Global Exception Caught:{ex.Message}");
        //tell the API "the body i'm sending back is JSON"
        context.Response.ContentType="application/json";
        // 500 = "Internal Server Error"  tells the caller it's our fault, not theirs
        context.Response.StatusCode =500;
        // actually writes that friendly message into the response body sent back to the caller
        // await = don't freeze the server while this gets sent over the network
        await context.Response.WriteAsync("{\"error\": \"Something went wrong. Please try again later.\"}");
    }
});
// Wires up all your [HttpGet], [HttpPost], etc. routes from your controllers
app.MapControllers();

app.Run();