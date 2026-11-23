using demoAPI.Models;

namespace demoAPI.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAll();

        Product? GetById(int id);

        Product Create(Product product);

        bool Update(Product product);

        bool Delete(Product product);
    }
}