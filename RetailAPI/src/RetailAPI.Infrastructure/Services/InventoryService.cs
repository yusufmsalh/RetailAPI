using RetailAPI.Core.DTOs;
using RetailAPI.Core.Entities;
using RetailAPI.Core.Exceptions;
using RetailAPI.Core.Interfaces;

namespace RetailAPI.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<InventoryItemDto>> GetInventoryAsync()
    {
        var products = await _unitOfWork.Inventory.GetAllWithStockAsync();
        return products.Select(p => new InventoryItemDto(p.Id, p.Name, p.StockQuantity, p.Price));
    }

    public async Task<InventoryItemDto> AdjustStockAsync(int productId, StockAdjustmentRequest request)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new NotFoundException("Product", productId);

        var newQuantity = product.StockQuantity + request.Adjustment;
        if (newQuantity < 0)
            throw new ValidationException($"Stock adjustment would result in negative stock ({newQuantity}). Current stock: {product.StockQuantity}.");

        await _unitOfWork.Inventory.UpdateStockAsync(productId, newQuantity);

        return new InventoryItemDto(product.Id, product.Name, newQuantity, product.Price);
    }
}
