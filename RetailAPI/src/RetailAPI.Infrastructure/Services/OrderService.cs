using Microsoft.Extensions.Logging;
using RetailAPI.Core.DTOs;
using RetailAPI.Core.Entities;
using RetailAPI.Core.Exceptions;
using RetailAPI.Core.Interfaces;

namespace RetailAPI.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new ValidationException("Order must contain at least one item.");

        // Check for duplicate product IDs
        var duplicates = request.Items.GroupBy(i => i.ProductId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (duplicates.Any())
            throw new ValidationException($"Duplicate product IDs in order: {string.Join(", ", duplicates)}");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = (await _unitOfWork.Products.GetByIdsAsync(productIds)).ToDictionary(p => p.Id);

            // Validate all products exist
            foreach (var item in request.Items)
            {
                if (!products.ContainsKey(item.ProductId))
                    throw new NotFoundException("Product", item.ProductId);
            }

            // Validate stock and quantities
            foreach (var item in request.Items)
            {
                var product = products[item.ProductId];
                if (item.Quantity <= 0)
                    throw new ValidationException($"Quantity for product '{product.Name}' must be positive.");
                if (product.StockQuantity < item.Quantity)
                    throw new InsufficientStockException(product.Name, product.StockQuantity, item.Quantity);
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow,
                OrderItems = request.Items.Select(item =>
                {
                    var product = products[item.ProductId];
                    return new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };
                }).ToList()
            };
            order.TotalAmount = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

            // Deduct stock
            foreach (var item in request.Items)
            {
                var product = products[item.ProductId];
                await _unitOfWork.Inventory.UpdateStockAsync(product.Id, product.StockQuantity - item.Quantity);
            }

            var createdOrder = await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Order {OrderId} placed successfully by user {UserId} for total {TotalAmount}.",
                createdOrder.Id, userId, createdOrder.TotalAmount);

            // Reload with relations for response
            var fullOrder = await _unitOfWork.Orders.GetByIdAsync(createdOrder.Id)
                ?? throw new InvalidOperationException("Failed to retrieve created order.");

            return MapToDto(fullOrder);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<OrderDto> GetOrderByIdAsync(int orderId, int userId, string userRole)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
            ?? throw new NotFoundException("Order", orderId);

        if (userRole != "Admin" && order.UserId != userId)
            throw new UnauthorizedException("You are not authorized to view this order.");

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order) => new(
        order.Id,
        order.UserId,
        order.User?.Username ?? "",
        order.TotalAmount,
        order.Status,
        order.CreatedAt,
        order.OrderItems.Select(oi => new OrderItemDto(
            oi.ProductId,
            oi.Product?.Name ?? "",
            oi.Quantity,
            oi.UnitPrice,
            oi.UnitPrice * oi.Quantity
        )).ToList()
    );
}
