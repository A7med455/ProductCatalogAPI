using Microsoft.AspNetCore.Mvc;
using productCatalogAPI.Models;
using productCatalogAPI.Services;
namespace productCatalogAPI.Controllers
{
    //Label says treat this as an API Controller
    [ApiController]
    //Base URL
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly  IProductService ProductService;
        public ProductsController(IProductService productService)
        {
            ProductService = productService;   
        }

        //ActionResult<T> means "this can return either an HTTP result (like Ok(), NotFound()) 
        // or raw data of type T." It's flexible on purpose, since different code paths return different things.
        [HttpGet]
        public ActionResult<List<Product>> GetAll()
        {
            return Ok(ProductService.GetAll());
        }
        
        [HttpGet("{id}")]
        public ActionResult<Product> GetById(int id)
        {
            var product = ProductService.GetById(id);
            if(product == null)
            {
                return NotFound($"Product With ID {id} not found");
            }
            return Ok(product);
        }

        [HttpPost]
        public ActionResult AddProduct(Product Product)
        {
            var product = ProductService.Create(Product);
            if(product == null)
            {
                return BadRequest("Could not create product");
            }
            return Ok($"Product With ID:{product.Id} Added");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteById(int id)
        {
            var success = ProductService.Delete(id);
            if(!success)
            {
                return NotFound($"Product With ID {id} not found");
            }
            return Ok($"Product with ID:{id} Deleted");
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct(int id,Product product)
        {
            var success = ProductService.Update(id,product);
            if(!success)
            {
                return NotFound($"Product With ID:{id} not found or cannot be updated");
            }
            return Ok($"Product With ID{id} Updated");
        }
    }
}