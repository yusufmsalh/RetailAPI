using RetailAPI.Core.DTOs;

namespace RetailAPI.Core.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request);
}

public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderRequest request);
    Task<OrderDto> GetOrderByIdAsync(int orderId, int userId, string userRole);
}

public interface IInventoryService
{
    Task<IEnumerable<InventoryItemDto>> GetInventoryAsync();
    Task<InventoryItemDto> AdjustStockAsync(int productId, StockAdjustmentRequest request);
}
