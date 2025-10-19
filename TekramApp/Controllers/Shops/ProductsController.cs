using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TekramApp.Interfaces;
using TekramApp.Models;
using TekramApp.ViewModels;

namespace TekramApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/products/shop/{shopId}
        [HttpGet("shop/{shopId:guid}")]
        public async Task<IActionResult> GetByShop(Guid shopId)
        {
            var products = await _productService.GetProductsByShopIdAsync(shopId);
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found." });

            return Ok(product);
        }

        // POST: api/products (supports FormData)
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fileName = "";
            // Handle image upload
            if (dto.Image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                Directory.CreateDirectory(uploadsFolder);

                fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }
            }

            var newProduct = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Availability = dto.Availability,
                ImageUrl = fileName != "" ? $"/images/products/{fileName}" : null,
                ShopId = dto.ShopId
            };

            var created = await _productService.CreateProductAsync(newProduct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] Product product, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                product.ImageUrl = $"/images/products/{fileName}";
            }

            var updated = await _productService.UpdateProductAsync(id, product);
            if (updated == null)
                return NotFound(new { message = "Product not found." });

            return Ok(updated);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound(new { message = "Product not found." });

            return NoContent();
        }
    }
}
