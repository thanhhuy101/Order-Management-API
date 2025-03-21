using Order_Management.DTOs;
using Order_Management.Entities;

namespace Order_Management.Interfaces
{
    public interface IOrderRepository
    {
        Task<PaginatedResult<Order>> GetOrdersAsync(int pageNumber, int pageSize);
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
    }
}
