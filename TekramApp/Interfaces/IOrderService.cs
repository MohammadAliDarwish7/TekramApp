using TekramApp.Models;

namespace TekramApp.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems);
        Task<bool> UpdateOrderStatusAsync(Guid id, string status);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
