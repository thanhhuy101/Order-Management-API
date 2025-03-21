using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Order_Management.Controllers;
using Order_Management.DTOs;
using Order_Management.Entities;
using Order_Management.Interfaces;
using Xunit;

namespace Order_Management.Tests
{
    public class OrderControllerTest
    {

        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<OrdersController>> _mockLogger;
        private readonly Mock<IValidator<CreateOrderDTO>> _mockCreateOrderValidator;
        private readonly Mock<IValidator<UpdateOrderDTO>> _mockUpdateOrderValidator;
        private readonly OrdersController _controller;

        public OrderControllerTest()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _mockCreateOrderValidator = new Mock<IValidator<CreateOrderDTO>>();
            _mockUpdateOrderValidator = new Mock<IValidator<UpdateOrderDTO>>();

            _controller = new OrdersController(
                _mockOrderRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockCreateOrderValidator.Object,
                _mockUpdateOrderValidator.Object
            );
        }

        [Fact]
        public async Task GetOrders_ReturnsOkResult_WithPaginatedOrders()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;

            var orders = new List<Order> { new Order { Id = 1, CustomerName = "Test Customer" } };
            var orderDtos = new List<OrderDTO> { new OrderDTO { Id = 1, CustomerName = "Test Customer" } };

            var paginatedResult = new PaginatedResult<Order>
            {
                Items = orders,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 1
            };

            _mockOrderRepository.Setup(repo => repo.GetOrdersAsync(pageNumber, pageSize))
                .ReturnsAsync(paginatedResult);

            _mockMapper.Setup(mapper => mapper.Map<List<OrderDTO>>(orders))
                .Returns(orderDtos);

            // Act
            var result = await _controller.GetOrders(pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PaginatedResult<OrderDTO>>(okResult.Value);
            Assert.Equal(1, returnValue.TotalCount);
            Assert.Equal(pageNumber, returnValue.PageNumber);
            Assert.Equal(pageSize, returnValue.PageSize);
            Assert.Single(returnValue.Items);
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrder_ReturnsOkResult_WhenOrderExists()
        {
            // Arrange
            int orderId = 1;
            var order = new Order { Id = orderId, CustomerName = "Test Customer" };
            var orderDto = new OrderDTO { Id = orderId, CustomerName = "Test Customer" };

            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            _mockMapper.Setup(mapper => mapper.Map<OrderDTO>(order))
                .Returns(orderDto);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<OrderDTO>(okResult.Value);
            Assert.Equal(orderId, returnValue.Id);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var createOrderDto = new CreateOrderDTO { CustomerName = "" };
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("CustomerName", "Customer name is required")
            };

            var validationResult = new ValidationResult(validationFailures);

            _mockCreateOrderValidator.Setup(validator => validator.ValidateAsync(createOrderDto, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.CreateOrder(createOrderDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtAction_WhenOrderIsCreated()
        {
            // Arrange
            var createOrderDto = new CreateOrderDTO { CustomerName = "Test Customer" };
            var order = new Order { CustomerName = "Test Customer" };
            var createdOrder = new Order { Id = 1, CustomerName = "Test Customer" };
            var orderDto = new OrderDTO { Id = 1, CustomerName = "Test Customer" };

            _mockCreateOrderValidator.Setup(validator => validator.ValidateAsync(createOrderDto, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockMapper.Setup(mapper => mapper.Map<Order>(createOrderDto))
                .Returns(order);

            _mockOrderRepository.Setup(repo => repo.CreateOrderAsync(order))
                .ReturnsAsync(createdOrder);

            _mockMapper.Setup(mapper => mapper.Map<OrderDTO>(createdOrder))
                .Returns(orderDto);

            // Act
            var result = await _controller.CreateOrder(createOrderDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<OrderDTO>(createdAtResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("GetOrder", createdAtResult.ActionName);
        }

        [Fact]
        public async Task UpdateOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            var updateOrderDto = new UpdateOrderDTO { CustomerName = "Updated Customer" };

            _mockUpdateOrderValidator.Setup(validator => validator.ValidateAsync(updateOrderDto, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _controller.UpdateOrder(orderId, updateOrderDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNoContent_WhenOrderIsDeleted()
        {
            // Arrange
            int orderId = 1;
            _mockOrderRepository.Setup(repo => repo.DeleteOrderAsync(orderId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            _mockOrderRepository.Setup(repo => repo.DeleteOrderAsync(orderId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
