using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;
using TekramApp.ViewModels;

namespace TekramApp.Services.Shops
{
    public class ShopService : IShopService
    {
        private readonly TekramDbContext _context;

        public ShopService(TekramDbContext context)
        {
            _context = context;
        }

        public async Task<List<Shop>> GetAllShopsAsync()
        {
            return await _context.Shops.ToListAsync();
        }

        public async Task<Shop?> GetShopByIdAsync(Guid id)
        {
            return await _context.Shops.FindAsync(id);
        }

        public async Task<Shop> CreateShopAsync(ShopCreateDto dto)
        {
            var shop = new Shop
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                IsOpen = dto.IsOpen,
                Currency = dto.Currency,
                CreatedAt = DateTime.UtcNow
            };

            // Handle file upload
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "shops");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                shop.ImageUrl = $"/images/shops/{fileName}";
            }

            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            return shop;
        }


        public async Task<Shop?> UpdateShopAsync(Guid id, Shop shop)
        {
            var existing = await _context.Shops.FindAsync(id);
            if (existing == null) return null;

            existing.Name = shop.Name;
            existing.ImageUrl = shop.ImageUrl;
            existing.IsOpen = shop.IsOpen;
            existing.Currency = shop.Currency;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteShopAsync(Guid id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null) return false;

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    

}
