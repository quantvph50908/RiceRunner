using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiceRunner.Data;
using RiceRunner.Models;

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
        public IActionResult GetInventory(int userId)
        {
            var items = _context.Inventory.Where(i => i.UserId == userId).ToList();
            return Ok(items);
        }

        [HttpPost("add")]
        public IActionResult AddItem([FromBody] InventoryItem item)
        {
            if (string.IsNullOrEmpty(item.ItemName) || item.UserId <= 0)
                return BadRequest("Dữ liệu không hợp lệ");

            var existingItem = _context.Inventory
                .FirstOrDefault(i => i.UserId == item.UserId && i.ItemName == item.ItemName);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _context.Inventory.Add(item);
            }
            _context.SaveChanges();
            return Ok("Thêm vật phẩm thành công");
        }
    }
}
