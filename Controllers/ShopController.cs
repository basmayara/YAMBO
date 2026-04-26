using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAMBO.ShopService.Data;
using YAMBO.ShopService.Dtos;

using YAMBO.ShopService.Models;

namespace YAMBO.ShopService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly YamboDbContext _context;

        public ShopController(YamboDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("items")]
        public async Task<ActionResult<List<ShopItemResponse>>> GetShopItems(
            [FromQuery] int playerId,
            [FromQuery] string itemType = null)
        {
            try
            {
               
                var ownedItemIds = await _context.PlayerInventories
                    .Where(pi => pi.PlayerId == playerId)
                    .Select(pi => pi.ItemId)
                    .ToListAsync();

               
                var query = _context.ShopItems
                    .Where(si => si.IsGlobalAvailable);

                if (!string.IsNullOrEmpty(itemType))
                {
                    query = query.Where(si => si.ItemType == itemType.ToLower());
                }

                var items = await query
                    .OrderBy(si => si.ItemType)
                    .ThenBy(si => si.Price)
                    .ToListAsync();

                var response = items.Select(item => new ShopItemResponse
                {
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    ItemType = item.ItemType,
                    Price = item.Price,
                    Description = item.Description,
                    SpriteUrl = item.SpriteUrl,
                    
                    IsAvailable = !ownedItemIds.Contains(item.ItemId)
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error fetching items: {ex.Message}" });
            }
        }

        
        [HttpPost("purchase")]
        public async Task<ActionResult<PurchaseResponse>> PurchaseItem([FromBody] PurchaseRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var item = await _context.ShopItems
                    .FirstOrDefaultAsync(si => si.ItemId == request.ItemId);

                if (item == null)
                {
                    return NotFound(new { error = "Item not found" });
                }

                
                var alreadyOwned = await _context.PlayerInventories
                    .AnyAsync(pi => pi.PlayerId == request.PlayerId && pi.ItemId == request.ItemId);

                if (alreadyOwned)
                {
                    return BadRequest(new { error = "Item already owned" });
                }

                var wallet = await _context.PlayerWallets
                    .FirstOrDefaultAsync(pw => pw.PlayerId == request.PlayerId);

                if (wallet == null)
                {
                    return NotFound(new { error = "Player wallet not found" });
                }

                if (wallet.Balance < item.Price)
                {
                    return BadRequest(new
                    {
                        error = $"Insufficient balance. Need {item.Price}, have {wallet.Balance}"
                    });
                }

               
                int balanceBefore = wallet.Balance;
                wallet.Balance -= item.Price;
                wallet.TotalSpent += item.Price;
                wallet.UpdatedAt = DateTime.UtcNow;

                
                var inventory = new PlayerInventory
                {
                    PlayerId = request.PlayerId,
                    ItemId = request.ItemId,
                    AcquiredAt = DateTime.UtcNow
                };
                _context.PlayerInventories.Add(inventory);

               
                var log = new TransactionLog
                {
                    PlayerId = request.PlayerId,
                    ItemId = request.ItemId,
                    TransactionType = "purchase",
                    Amount = item.Price,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Timestamp = DateTime.UtcNow
                };
                _context.TransactionLogs.Add(log);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new PurchaseResponse
                {
                    Success = true,
                    Message = $"Successfully purchased {item.ItemName}",
                    NewBalance = wallet.Balance,
                    ItemName = item.ItemName
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = $"Purchase failed: {ex.Message}" });
            }
        }

       
        [HttpGet("balance/{playerId}")]
        public async Task<ActionResult<BalanceResponse>> GetBalance(int playerId)
        {
            try
            {
                var wallet = await _context.PlayerWallets
                    .FirstOrDefaultAsync(pw => pw.PlayerId == playerId);

                if (wallet == null)
                {
                    return NotFound(new { error = "Player not found" });
                }

                return Ok(new BalanceResponse
                {
                    PlayerId = wallet.PlayerId,
                    Username = wallet.Username,
                    Balance = wallet.Balance,
                    TotalEarned = wallet.TotalEarned,
                    TotalSpent = wallet.TotalSpent
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error fetching balance: {ex.Message}" });
            }
        }

       
        [HttpGet("inventory/{playerId}")]
        public async Task<ActionResult<List<ShopItemResponse>>> GetPlayerInventory(int playerId)
        {
            try
            {
                var inventory = await _context.PlayerInventories
                    .Where(pi => pi.PlayerId == playerId)
                    .Include(pi => pi.Item)
                    .Select(pi => new ShopItemResponse
                    {
                        ItemId = pi.Item.ItemId,
                        ItemName = pi.Item.ItemName,
                        ItemType = pi.Item.ItemType,
                        Price = pi.Item.Price,
                        Description = pi.Item.Description,
                        SpriteUrl = pi.Item.SpriteUrl,
                        IsAvailable = false 
                    })
                    .ToListAsync();

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error fetching inventory: {ex.Message}" });
            }
        }

       
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                service = "YAMBO Shop Service (C# ASP.NET Core)",
                status = "running",
                timestamp = DateTime.UtcNow
            });
        }
        // POST /api/Shop/sell
        [HttpPost("sell")]
        public async Task<ActionResult<SellResponse>> SellItem([FromBody] SellRequest request)
        {
            var inventoryEntry = await _context.PlayerInventories
                .Include(i => i.Item)
                .FirstOrDefaultAsync(i => i.PlayerId == request.PlayerId
                                       && i.ItemId == request.ItemId);

            if (inventoryEntry == null)
                return NotFound(new SellResponse
                {
                    Success = false,
                    Message = "Item not found in inventory"
                });

            int refundAmount = (int)(inventoryEntry.Item!.Price * 0.5f);

            var wallet = await _context.PlayerWallets
                .FirstOrDefaultAsync(w => w.PlayerId == request.PlayerId);

            if (wallet == null)
                return NotFound(new SellResponse
                {
                    Success = false,
                    Message = "Player wallet not found"
                });

            int balanceBefore = wallet.Balance;
            wallet.Balance += refundAmount;
            wallet.TotalSpent -= refundAmount;
            wallet.UpdatedAt = DateTime.UtcNow;

            _context.Entry(wallet).Property(w => w.TotalEarned).IsModified = false;


            _context.PlayerInventories.Remove(inventoryEntry);

            _context.TransactionLogs.Add(new TransactionLog
            {
                PlayerId = request.PlayerId,        // int ✅
                ItemId = request.ItemId,            // int? ✅
                TransactionType = "refund",
                Amount = refundAmount,
                BalanceBefore = balanceBefore,      // ✅ champ existant
                BalanceAfter = wallet.Balance,      // ✅ champ existant
                Timestamp = DateTime.UtcNow         // ✅ était CreatedAt → Timestamp
            });

            await _context.SaveChangesAsync();

            return Ok(new SellResponse
            {
                Success = true,
                Message = $"Item sold for {refundAmount} coins (50% refund)",
                RefundAmount = refundAmount,        // ✅ corrigé
                NewBalance = wallet.Balance
            });
        }

        // PUT /api/Shop/inventory/favorite
        [HttpPut("inventory/favorite")]
        public async Task<ActionResult<FavoriteResponse>> ToggleFavorite([FromBody] FavoriteRequest request)
        {
            var inventoryEntry = await _context.PlayerInventories
                .FirstOrDefaultAsync(i => i.PlayerId == request.PlayerId
                                       && i.ItemId == request.ItemId);

            if (inventoryEntry == null)
                return NotFound(new FavoriteResponse
                {
                    Success = false,
                    Message = "Item not found in inventory"
                });

            inventoryEntry.IsFavorite = request.IsFavorite;
            await _context.SaveChangesAsync();

            return Ok(new FavoriteResponse
            {
                Success = true,
                Message = request.IsFavorite ? "Added to favorites" : "Removed from favorites",
                IsFavorite = request.IsFavorite
            });
        }
    }
    
}