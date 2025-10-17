using Microsoft.EntityFrameworkCore;
using System;
using TekramApp.Context;
using TekramApp.Interfaces;
using TekramApp.Models;

namespace TekramApp.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly TekramDbContext _context;

        public OrderService(TekramDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems)
        {
            order.Id = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Pending";
            order.TotalAmount = orderItems.Sum(i => i.Price * i.Quantity);

            _context.Orders.Add(order);

            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
                _context.OrderItems.Add(item);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
