using Microsoft.EntityFrameworkCore;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;

namespace TekramApp.Services.Shops
{
    public class ProductService : IProductService
    {
        private readonly TekramDbContext _context;

        public ProductService(TekramDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<List<Product>> GetProductsByShopIdAsync(Guid shopId)
        {
            return await _context.Products
                .Where(p => p.ShopId == shopId)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _context.Products.Include(p => p.Shop)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Guid id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return null;

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Availability = product.Availability;
            existing.ImageUrl = product.ImageUrl;
            existing.ShopId = product.ShopId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
