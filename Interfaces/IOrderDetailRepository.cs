using Order_Management.Entities;

namespace Order_Management.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<OrderDetail> GetOrderDetailByIdAsync(int id);
        Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail);
        Task<bool> DeleteOrderDetailAsync(int id);
    }
}
