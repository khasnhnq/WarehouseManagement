using demoAPI.DTOs;
using demoAPI.Models;
using demoAPI.Repositories;

namespace demoAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        // GET ALL
        public List<ProductDto> GetAll()
        {
            var products = _repository.GetAll();

            return products.Select(p => new ProductDto
            {
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                Quantity = p.Quantity,
                CategoryId = p.CategoryId,
                SupplierId = p.SupplierId
            }).ToList();
        }

        // GET BY ID
        public ProductDto? GetById(int id)
        {
            var product = _repository.GetById(id);

            if (product == null)
                return null;

            return new ProductDto
            {
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId
            };
        }

        // CREATE
        public ProductDto Create(ProductDto productDto)
        {
            // DTO → Product
            var product = new Product
            {
                Name = productDto.Name,
                SKU = productDto.SKU,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                CategoryId = productDto.CategoryId,
                SupplierId = productDto.SupplierId
            };

            var createdProduct = _repository.Create(product);

            // Product → DTO
            return new ProductDto
            {
                Name = createdProduct.Name,
                SKU = createdProduct.SKU,
                Price = createdProduct.Price,
                Quantity = createdProduct.Quantity,
                CategoryId = createdProduct.CategoryId,
                SupplierId = createdProduct.SupplierId
            };
        }

        // UPDATE
        public bool Update(int id, ProductDto productDto)
        {
            var product = _repository.GetById(id);

            if (product == null)
                return false;

            // DTO → Product
            product.Name = productDto.Name;
            product.SKU = productDto.SKU;
            product.Price = productDto.Price;
            product.Quantity = productDto.Quantity;
            product.CategoryId = productDto.CategoryId;
            product.SupplierId = productDto.SupplierId;

            return _repository.Update(product);
        }

        // DELETE
        public bool Delete(int id)
        {
            var product = _repository.GetById(id);

            if (product == null)
                return false;

            return _repository.Delete(product);
        }
    }
}