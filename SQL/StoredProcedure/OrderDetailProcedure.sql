-- Tạo Stored Procedure để lấy chi tiết đơn hàng
CREATE PROCEDURE sp_GetOrderDetails
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT od.*
    FROM OrderDetails od
    WHERE od.OrderId = @OrderId;
END;
GO

-- Create Stored Procedure to add new order detail
CREATE PROCEDURE sp_CreateOrderDetail
    @OrderId INT,
    @ProductName NVARCHAR(255),
    @Quantity INT,
    @Price DECIMAL(18, 2)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Add new order detail
    INSERT INTO OrderDetails (OrderId, ProductName, Quantity, Price)
    VALUES (@OrderId, @ProductName, @Quantity, @Price);
    
    -- Update total amount of order
    UPDATE Orders
    SET TotalAmount = (
        SELECT SUM(Quantity * Price)
        FROM OrderDetails
        WHERE OrderId = @OrderId
    ),
    UpdatedAt = GETDATE()
    WHERE Id = @OrderId;
    
    SELECT SCOPE_IDENTITY() AS OrderDetailId;
END;
GO