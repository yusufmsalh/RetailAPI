using System.ComponentModel.DataAnnotations;

namespace RetailAPI.Core.DTOs;

public record ProductDto(int Id, string Name, string Description, decimal Price, int StockQuantity, DateTime UpdatedAt);

public record CreateProductRequest(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string Description,
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")] decimal Price,
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")] int StockQuantity
);

public record UpdateProductRequest(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string Description,
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")] decimal Price,
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")] int StockQuantity
);

public record StockAdjustmentRequest(
    [Range(int.MinValue, int.MaxValue)] int Adjustment,
    string Reason
);

public record InventoryItemDto(int ProductId, string Name, int StockQuantity, decimal Price);
