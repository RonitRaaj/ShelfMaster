using ShelfMaster.Application.Interfaces;
using ShelfMaster.Application.DTOs;
using ShelfMaster.Domain.Entities;
using ShelfMaster.Domain.Exceptions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
namespace ShelfMaster.Application.Services;
public class StockTransactionService
{
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IEmailRepository _emailRepository;
    private readonly ILogger<StockTransactionService> _logger;

    public StockTransactionService(IStockTransactionRepository stockTransactionRepository, IUserRepository userRepository, IInventoryRepository inventoryRepository, IEmailRepository emailRepository , ILogger<StockTransactionService> logger)
    {
        _stockTransactionRepository = stockTransactionRepository;
        _userRepository = userRepository;
        _inventoryRepository = inventoryRepository;
        _emailRepository = emailRepository;
        _logger = logger;

    }

    public async Task<StockTransactionResponseDTO> AddStockTransactionAsync(StockTransactionRecordDTO dto , string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found");

        var inventoryItem = await _inventoryRepository.GetInventoryItemByIdAsync(dto.InventoryItemId);
        if (inventoryItem == null) throw new NotFoundException($"Inventory item with ID {dto.InventoryItemId} not found");

        var transaction = new StockTransaction(dto.QuantityChange, dto.Note, user.Id, inventoryItem.Id);
        await _stockTransactionRepository.AddStockTransactionAsync(transaction);

        if(transaction.QuantityChanged < 0)
        {
            var withdrawQuantity = -transaction.QuantityChanged;
            inventoryItem.WithdrawQuantity(withdrawQuantity);
            if(inventoryItem.Quantity < inventoryItem.LowStockThreshold)
        {
            var email = user.Email;
            if (!string.IsNullOrEmpty(email))
            {
                var subject = $"Low Stock Alert: {inventoryItem.Name}";
                var body = $"The inventory item '{inventoryItem.Name}' (SKU: {inventoryItem.SKU}) has a low stock level of {inventoryItem.Quantity} units. Please restock soon.";

                try
                {
                    await _emailRepository.SendEmailAsync(email, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send low stock alert email");
                }
            }
        }
        }
        else if (transaction.QuantityChanged > 0)
        {
            inventoryItem.RestockQuantity(transaction.QuantityChanged);
        }

        await _inventoryRepository.UpdateInventoryItemAsync(inventoryItem);

        return new StockTransactionResponseDTO(
            transaction.Id,
            transaction.QuantityChanged,
            transaction.Timestamp,
            transaction.Notes,
            transaction.UserId,
            transaction.InventoryItemId,
            user.Username, // Include user name for easier client display
            inventoryItem.Name // Include item name for easier client display
        );
    }

    public async Task<StockTransactionResponseDTO?> GetStockTransactionByIdAsync(string id)
    {
        var transaction = await _stockTransactionRepository.GetStockTransactionByIdAsync(id);
        if (transaction == null) throw new NotFoundException($"Stock transaction with ID {id} not found");

        return MapToResponseDTO(transaction);
    }

    public async Task<IEnumerable<StockTransactionResponseDTO>> GetAllStockTransactionsAsync()
    {
        var transactions = await _stockTransactionRepository.GetAllStockTransactionsAsync();
        return transactions.Select(MapToResponseDTO);
    }

    public async Task<IEnumerable<StockTransactionResponseDTO>> GetStockTransactionsByUserIdAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) throw new NotFoundException($"User with ID {userId} not found");
        var transactions = await _stockTransactionRepository.GetStockTransactionByUserIdAsync(userId);
        return transactions.Select(MapToResponseDTO);
    }

    public async Task<IEnumerable<StockTransactionResponseDTO>> GetStockTransactionsByInventoryItemIdAsync(string inventoryItemId)
    {
        var inventoryItem = await _inventoryRepository.GetInventoryItemByIdAsync(inventoryItemId);
        if (inventoryItem == null) throw new NotFoundException($"Inventory item with ID {inventoryItemId} not found");
        var transactions = await _stockTransactionRepository.GetStockTransactionByInventoryItemIdAsync(inventoryItemId);
        return transactions.Select(MapToResponseDTO);
    }

    public async Task DeleteStockTransactionAsync(string id)
    {
        var transaction = await _stockTransactionRepository.GetStockTransactionByIdAsync(id);
        if (transaction == null) throw new NotFoundException($"Stock transaction with ID {id} not found");
        await _stockTransactionRepository.DeleteStockTransactionAsync(id);
    }

    private static StockTransactionResponseDTO MapToResponseDTO(StockTransaction transaction)
    {
        return new StockTransactionResponseDTO(
            transaction.Id,
            transaction.QuantityChanged,
            transaction.Timestamp,
            transaction.Notes,
            transaction.UserId,
            transaction.InventoryItemId,
            transaction.User?.Username, // Include user name if available
            transaction.InventoryItem?.Name // Include item name if available
        );
    }
}