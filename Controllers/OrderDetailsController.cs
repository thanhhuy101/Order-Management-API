using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Order_Management.DTOs;
using Order_Management.Entities;
using Order_Management.Interfaces;

namespace Order_Management.Controllers
{
    [ApiController]
    [Route("api")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderDetailsController> _logger;
        private readonly IValidator<CreateOrderDetailDTO> _createOrderDetailValidator;

        public OrderDetailsController(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IMapper mapper, ILogger<OrderDetailsController> logger, IValidator<CreateOrderDetailDTO> createOrderDetailValidator)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _logger = logger;
            _createOrderDetailValidator = createOrderDetailValidator;
        }

        //GET: api/orders/{orderId}/order-details
        [HttpGet("orders/{id}/order-details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderDetailDTO>>> GetOrderDetails(int id)
        {
            _logger.LogInformation("Getting order details for order ID {OrderId}", id);

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return NotFound();
            }

            var orderDetails = await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(id);

            return Ok(_mapper.Map<IEnumerable<OrderDetailDTO>>(orderDetails));
        }

        //POST: api/orders/{orderId}/order-details
        [HttpPost("orders/{id}/order-details")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDetailDTO>> AddOrderDetail(int orderId, CreateOrderDetailDTO createOrderDetailDto)
        {
            _logger.LogInformation("Adding order detail to order ID {OrderId}", orderId);

            var validationResult = await _createOrderDetailValidator.ValidateAsync(createOrderDetailDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", orderId);
                return NotFound();
            }

            var orderDetail = _mapper.Map<OrderDetail>(createOrderDetailDto);
            orderDetail.OrderId = orderId;

            var createdOrderDetail = await _orderDetailRepository.CreateOrderDetailAsync(orderDetail);

            var result = _mapper.Map<OrderDetailDTO>(createdOrderDetail);

            return CreatedAtAction(nameof(GetOrderDetails), new { orderId = orderId }, result);
        }

        //DELETE: api/order-details/{id}
        [HttpDelete("order-details/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            _logger.LogInformation("Deleting order detail with ID {OrderDetailId}", id);

            var result = await _orderDetailRepository.DeleteOrderDetailAsync(id);

            if (!result)
            {
                _logger.LogWarning("Order detail with ID {OrderDetailId} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
