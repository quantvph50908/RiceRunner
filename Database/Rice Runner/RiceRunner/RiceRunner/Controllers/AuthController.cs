using Microsoft.AspNetCore.Mvc;
using RiceRunner.Data;
using RiceRunner.Models;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace RiceRunner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _context;

        public AuthController(GameDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { message = "Username và Password không được để trống" });
            }

            if (user.Username.Length < 3)
            {
                return BadRequest(new { message = "Username phải có ít nhất 3 ký tự" });
            }
            if (user.Password.Length < 6)
            {
                return BadRequest(new { message = "Password phải có ít nhất 6 ký tự" });
            }

            if (!Regex.IsMatch(user.Username, @"^[a-zA-Z0-9]+$"))
            {
                return BadRequest(new { message = "Username chỉ được chứa chữ cái và số" });
            }
            if (user.Password.Contains(" "))
            {
                return BadRequest(new { message = "Password không được chứa khoảng trắng" });
            }

            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest(new { message = "Username đã tồn tại" });
            }

            // Mã hóa mật khẩu trước khi lưu
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(new { Message = "Đăng ký thành công", UserId = user.Id });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { message = "Username và Password không được để trống" });
            }

            var dbUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (dbUser == null)
            {
                return Unauthorized(new { message = "Tài khoản không tồn tại" });
            }

            // Kiểm tra mật khẩu đã mã hóa
            if (!BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
            {
                return Unauthorized(new { message = "Mật khẩu không đúng" });
            }

            return Ok(new
            {
                UserId = dbUser.Id,
                Score = dbUser.Score,
                Rice = dbUser.Rice,
                Message = "Đăng nhập thành công"
            });
        }
    }
}
