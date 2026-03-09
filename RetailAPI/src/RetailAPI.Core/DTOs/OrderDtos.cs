using System.ComponentModel.DataAnnotations;

namespace RetailAPI.Core.DTOs;

public record OrderItemRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Product ID must be valid.")] int ProductId,
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")] int Quantity
);

public record PlaceOrderRequest(
    [Required, MinLength(1, ErrorMessage = "Order must contain at least one item.")] List<OrderItemRequest> Items
);

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Subtotal);

public record OrderDto(int Id, int UserId, string Username, decimal TotalAmount, string Status, DateTime CreatedAt, List<OrderItemDto> Items);
