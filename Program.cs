using productCatalogAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers so ASP.NET Core scans for classes like ProductsController
builder.Services.AddControllers();

// Register the service: whenever something asks for IProductService, give it a ProductService
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// Wires up all your [HttpGet], [HttpPost], etc. routes from your controllers
app.MapControllers();

app.Run();