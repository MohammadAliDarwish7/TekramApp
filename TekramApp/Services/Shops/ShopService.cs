using Microsoft.EntityFrameworkCore;
using System;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;

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

        public async Task<Shop> CreateShopAsync(Shop shop)
        {
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
