using productCatalogAPI.Models;

namespace productCatalogAPI.Services
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product? GetById(int id);
        Product? Create(Product product);
        bool Update(int id,Product product);
        bool Delete(int id);
    }
}
