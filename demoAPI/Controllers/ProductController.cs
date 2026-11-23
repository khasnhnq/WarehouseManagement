using demoAPI.DTOs;
using demoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace demoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // GET: api/product
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _service.GetAll();

            return Ok(products);
        }

        // GET: api/product/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _service.GetById(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            var product = _service.Create(productDto);

            return Ok(product);
        }

        // PUT: api/product/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductDto productDto)
        {
            var result = _service.Update(id, productDto);

            if (!result)
                return NotFound();

            return Ok("Cập nhật sản phẩm thành công");
        }

        // DELETE: api/product/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _service.Delete(id);

            if (!result)
                return NotFound();

            return Ok("Xóa sản phẩm thành công");
        }
    }
}