using Order_Management.Entities;

namespace Order_Management.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }

    public class CreateOrderDTO
    {
        public string CustomerName { get; set; }
        public OrderStatus Status { get; set; }
        //public List<CreateOrderDetailDTO> OrderDetails { get; set; }
    }

    public class UpdateOrderDTO
    {
        public string CustomerName { get; set; }
        public OrderStatus Status { get; set; }

    }
}
