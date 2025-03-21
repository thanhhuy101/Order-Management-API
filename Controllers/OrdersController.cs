using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Order_Management.DTOs;
using Order_Management.Entities;
using Order_Management.Interfaces;

namespace Order_Management.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;
        private readonly IValidator<CreateOrderDTO> _createOrderValidator;
        private readonly IValidator<UpdateOrderDTO> _updateOrderValidator;

        public OrdersController(IOrderRepository orderRepository, IMapper mapper, ILogger<OrdersController> logger, IValidator<CreateOrderDTO> createOrderValidator, IValidator<UpdateOrderDTO> updateOrderValidator)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
            _createOrderValidator = createOrderValidator;
            _updateOrderValidator = updateOrderValidator;
        }

        //GET: api/Orders
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<OrderDTO>>> GetOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting all orders, page {PageNumber}, size {PageSize}", pageNumber, pageSize);

            var ordersResult = await _orderRepository.GetOrdersAsync(pageNumber, pageSize);

            var result = new PaginatedResult<OrderDTO>
            {
                Items = _mapper.Map<List<OrderDTO>>(ordersResult.Items),
                PageNumber = ordersResult.PageNumber,
                PageSize = ordersResult.PageSize,
                TotalCount = ordersResult.TotalCount
            };

            return Ok(result);
        }

        //GET: api/Orders/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            _logger.LogInformation("Getting order with id {Id}", id);

            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                _logger.LogWarning("Order with id {OrderId} not found", id);
                return NotFound();
            }

            return Ok(_mapper.Map<OrderDTO>(order));
        }

        //POST: api/Orders
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO createOrderDto)
        {
            _logger.LogInformation("Creating a new order for customer {CustomerName}", createOrderDto.CustomerName);

            var validationResult = await _createOrderValidator.ValidateAsync(createOrderDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var order = _mapper.Map<Order>(createOrderDto);
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            var result = _mapper.Map<OrderDTO>(createdOrder);

            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        //PUT: api/Orders/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> UpdateOrder(int id, UpdateOrderDTO updateOrderDto)
        {
            _logger.LogInformation("Updating order with ID {OrderId}", id);

            var validationResult = await _updateOrderValidator.ValidateAsync(updateOrderDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var existingOrder = await _orderRepository.GetOrderByIdAsync(id);
            if (existingOrder == null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return NotFound();
            }

            _mapper.Map(updateOrderDto, existingOrder);
            existingOrder.Id = id;

            var updatedOrder = await _orderRepository.UpdateOrderAsync(existingOrder);

            return Ok(_mapper.Map<OrderDTO>(updatedOrder));
        }

        //DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation("Deleting order with ID {OrderId}", id);

            var result = await _orderRepository.DeleteOrderAsync(id);

            if (!result)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
