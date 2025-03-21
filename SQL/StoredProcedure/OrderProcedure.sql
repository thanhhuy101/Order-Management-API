-- Create Stored Procedure to get order list with pagination
CREATE PROCEDURE sp_GetOrders
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total order count
    SELECT COUNT(*) AS TotalCount FROM Orders;
    
    -- Get order list with pagination
    SELECT o.*
    FROM Orders o
    ORDER BY o.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- Create Stored Procedure to add new order
CREATE PROCEDURE sp_CreateOrder
    @CustomerName NVARCHAR(255),
    @Status INT,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Orders (CustomerName, Status, TotalAmount, CreatedAt, UpdatedAt)
    VALUES (@CustomerName, @Status, 0, @CreatedAt, @UpdatedAt);
    
    SELECT SCOPE_IDENTITY() AS OrderId;
END;
GO