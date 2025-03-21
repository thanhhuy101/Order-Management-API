using Microsoft.EntityFrameworkCore;
using Order_Management.Data;
using Order_Management.DTOs;
using Order_Management.Entities;
using Order_Management.Interfaces;

namespace Order_Management.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context; 

        public async Task<PaginatedResult<Order>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Orders.CountAsync();

            var orders = await _context.Orders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return new PaginatedResult<Order>
            {
                Items = orders,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }
       

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.Now;
            order.UpdatedAt = DateTime.Now;

            // Calculate TotalAmount
            order.TotalAmount = order.OrderDetails.Sum(od => od.Price * od.Quantity);

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
                return null;

            existingOrder.CustomerName = order.CustomerName;
            existingOrder.Status = order.Status;
            existingOrder.UpdatedAt = DateTime.Now;

            // Recalculate TotalAmount
            existingOrder.TotalAmount = existingOrder.OrderDetails.Sum(od => od.Price * od.Quantity);

            await _context.SaveChangesAsync();

            return existingOrder;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
