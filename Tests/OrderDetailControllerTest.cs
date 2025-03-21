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
    public class OrderDetailControllerTest
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IOrderDetailRepository> _mockOrderDetailRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<OrderDetailsController>> _mockLogger;
        private readonly Mock<IValidator<CreateOrderDetailDTO>> _mockCreateOrderDetailValidator;
        private readonly OrderDetailsController _controller;

        public OrderDetailControllerTest()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<OrderDetailsController>>();
            _mockCreateOrderDetailValidator = new Mock<IValidator<CreateOrderDetailDTO>>();

            _controller = new OrderDetailsController(
                _mockOrderRepository.Object,
                _mockOrderDetailRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockCreateOrderDetailValidator.Object
            );
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _controller.GetOrderDetails(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsOkResult_WithOrderDetails()
        {
            // Arrange
            int orderId = 1;
            var order = new Order { Id = orderId, CustomerName = "Test Customer" };
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Id = 1, OrderId = orderId, ProductName = "Product 1" },
                new OrderDetail { Id = 2, OrderId = orderId, ProductName = "Product 2" }
            };
            var orderDetailDtos = new List<OrderDetailDTO>
            {
                new OrderDetailDTO { Id = 1, ProductName = "Product 1" },
                new OrderDetailDTO { Id = 2, ProductName = "Product 2" }
            };

            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            _mockOrderDetailRepository.Setup(repo => repo.GetOrderDetailsByOrderIdAsync(orderId))
                .ReturnsAsync(orderDetails);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<OrderDetailDTO>>(orderDetails))
                .Returns(orderDetailDtos);

            // Act
            var result = await _controller.GetOrderDetails(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<OrderDetailDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task AddOrderDetail_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            int orderId = 1;
            var createOrderDetailDto = new CreateOrderDetailDTO { ProductName = "" };
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("ProductName", "Product name is required")
            };

            var validationResult = new ValidationResult(validationFailures);

            _mockCreateOrderDetailValidator.Setup(validator => validator.ValidateAsync(createOrderDetailDto, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.AddOrderDetail(orderId, createOrderDetailDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddOrderDetail_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            var createOrderDetailDto = new CreateOrderDetailDTO { ProductName = "Product 1" };

            _mockCreateOrderDetailValidator.Setup(validator => validator.ValidateAsync(createOrderDetailDto, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _controller.AddOrderDetail(orderId, createOrderDetailDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddOrderDetail_ReturnsCreatedAtAction_WhenOrderDetailIsCreated()
        {
            // Arrange
            int orderId = 1;
            var createOrderDetailDto = new CreateOrderDetailDTO { ProductName = "Product 1", Quantity = 1, Price = 10.99M };
            var order = new Order { Id = orderId, CustomerName = "Test Customer" };
            var orderDetail = new OrderDetail { ProductName = "Product 1", Quantity = 1, Price = 10.99M };
            var createdOrderDetail = new OrderDetail { Id = 1, OrderId = orderId, ProductName = "Product 1", Quantity = 1, Price = 10.99M };
            var orderDetailDto = new OrderDetailDTO { Id = 1, ProductName = "Product 1", Quantity = 1, Price = 10.99M };

            _mockCreateOrderDetailValidator.Setup(validator => validator.ValidateAsync(createOrderDetailDto, CancellationToken.None))
                .ReturnsAsync(new ValidationResult());

            _mockOrderRepository.Setup(repo => repo.GetOrderByIdAsync(orderId))
                .ReturnsAsync(order);

            _mockMapper.Setup(mapper => mapper.Map<OrderDetail>(createOrderDetailDto))
                .Returns(orderDetail);

            _mockOrderDetailRepository.Setup(repo => repo.CreateOrderDetailAsync(It.IsAny<OrderDetail>()))
                .ReturnsAsync(createdOrderDetail);

            _mockMapper.Setup(mapper => mapper.Map<OrderDetailDTO>(createdOrderDetail))
                .Returns(orderDetailDto);

            // Act
            var result = await _controller.AddOrderDetail(orderId, createOrderDetailDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<OrderDetailDTO>(createdAtResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("GetOrderDetails", createdAtResult.ActionName);
        }

        [Fact]
        public async Task DeleteOrderDetail_ReturnsNoContent_WhenOrderDetailIsDeleted()
        {
            // Arrange
            int orderDetailId = 1;
            _mockOrderDetailRepository.Setup(repo => repo.DeleteOrderDetailAsync(orderDetailId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOrderDetail(orderDetailId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteOrderDetail_ReturnsNotFound_WhenOrderDetailDoesNotExist()
        {
            // Arrange
            int orderDetailId = 1;
            _mockOrderDetailRepository.Setup(repo => repo.DeleteOrderDetailAsync(orderDetailId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteOrderDetail(orderDetailId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
