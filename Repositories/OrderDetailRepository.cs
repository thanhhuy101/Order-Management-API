using Microsoft.EntityFrameworkCore;
using Order_Management.Data;
using Order_Management.Entities;
using Order_Management.Interfaces;

namespace Order_Management.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly OrderDbContext _context;

        public OrderDetailRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<OrderDetail> GetOrderDetailByIdAsync(int id)
        {
            return await _context.OrderDetails
                .FindAsync(id);
        }

        public async Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);

            // Update order total amount
            var order = await _context.Orders.FindAsync(orderDetail.OrderId);
            if (order != null)
            {
                order.TotalAmount += (orderDetail.Price * orderDetail.Quantity);
                order.UpdatedAt = System.DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return orderDetail;
        }

        public async Task<bool> DeleteOrderDetailAsync(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
                return false;

            // Update order total amount
            var order = await _context.Orders.FindAsync(orderDetail.OrderId);
            if (order != null)
            {
                order.TotalAmount -= (orderDetail.Price * orderDetail.Quantity);
                order.UpdatedAt = System.DateTime.Now;
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
