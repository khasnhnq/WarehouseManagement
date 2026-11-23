using demoAPI.DTOs;

namespace demoAPI.Services
{
    public interface IProductService
    {
        List<ProductDto> GetAll();
        ProductDto? GetById(int id);
        ProductDto Create(ProductDto productDto);
        bool Update(int id, ProductDto productDto);
        bool Delete(int id);
    }
}