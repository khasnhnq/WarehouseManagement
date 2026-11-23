namespace demoAPI.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int CategoryId { get; set; }

        public int SupplierId { get; set; }
    }
}