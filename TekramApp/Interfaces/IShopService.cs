using TekramApp.Models;

namespace TekramApp.Interfaces
{
    public interface IShopService
    {
        Task<List<Shop>> GetAllShopsAsync();
        Task<Shop?> GetShopByIdAsync(Guid id);
        Task<Shop> CreateShopAsync(Shop shop);
        Task<Shop?> UpdateShopAsync(Guid id, Shop shop);
        Task<bool> DeleteShopAsync(Guid id);
    }
}
