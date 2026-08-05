using productCatalogAPI.Models;
namespace productCatalogAPI.Services
{
    public class ProductService : IProductService
    {
        private const double MaxPrice = 1000000;
        private List<Product> products = new()
        {
            new Product { Id = 1, Name = "Laptop", Description = "Gaming laptop", Price = 25000 },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 300 }
        };

        public List<Product> GetAll()
        {
            return products;    
        }
        //FirstOrDefault scans the list and returns the first product whose Id matches, or null if none found
        public Product? GetById(int id)=>products.FirstOrDefault(p => p.Id == id);
        public Product? Create(Product product)
        {
            if(product.Price < 0||product.Price > MaxPrice)
            {
                throw new ArgumentException("Price cannot be negative or exceed pricing limits.");
            }
            if(string.IsNullOrEmpty(product.Name))
            {
                throw new ArgumentException("Product Name is Empty or null");
            }
            product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);
            return product;
        }
        public bool Update(int id,Product UpdatedProduct)
        {
            if(id < 0)
            {
                throw new ArgumentException("id cannot be negative.");
            }
            var product = products.FirstOrDefault(p => p.Id == id);
            if(product == null)
            {
                return false;
            }
            if(UpdatedProduct.Price < 0 ||UpdatedProduct.Price > MaxPrice)
            {
                throw new ArgumentException("Price cannot be negative or exceed pricing limits.");
            }
            if(string.IsNullOrEmpty(UpdatedProduct.Name))
            {
                throw new ArgumentException("Product Name is Empty or null");
            }
            product.Name = UpdatedProduct.Name;
            product.Description = UpdatedProduct.Description;
            product.Price = UpdatedProduct.Price;
            return true;
        }
        public bool Delete(int id)
        {
            if(id < 0)
            {
                throw new ArgumentException("id cannot be negative.");
            }
            var product = products.FirstOrDefault(p => p.Id== id);
            if(product == null)
            {
                return false;
            }
            products.Remove(product);
            return true;
        }

    }
}