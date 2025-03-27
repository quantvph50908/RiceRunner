using Microsoft.AspNetCore.Mvc;
using RiceRunner.Data;
using RiceRunner.Models;
using Microsoft.EntityFrameworkCore;

namespace RiceRunner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly GameDbContext _context;

        public InventoryController(GameDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetInventory(int userId)
        {
            var items = await _context.Inventory
                .Where(i => i.UserId == userId)
                .ToListAsync();
            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] InventoryItem item)
        {
            if (string.IsNullOrEmpty(item.ItemName) || item.UserId <= 0)
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });

            var existingItem = await _context.Inventory
                .FirstOrDefaultAsync(i => i.UserId == item.UserId && i.ItemName == item.ItemName);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _context.Inventory.Add(item);
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm vật phẩm thành công" });
        }
    }
}