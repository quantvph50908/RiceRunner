using Microsoft.AspNetCore.Mvc;
using RiceRunner.Data;
using RiceRunner.Models;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using RiceRunner.Services;

namespace RiceRunner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly EmailService _emailService;

        public AuthController(GameDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(new { message = "Username, Password và Email không được để trống" });
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

            if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { message = "Email không hợp lệ" });
            }

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest(new { message = "Username đã tồn tại" });
            }
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest(new { message = "Email đã được sử dụng" });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                UserId = user.Id,
                Score = user.Score,
                Rice = user.Rice,
                Message = "Đăng ký thành công"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { message = "Username và Password không được để trống" });
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (dbUser == null)
            {
                return Unauthorized(new { message = "Tài khoản không tồn tại" });
            }

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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] object request)
        {
            var email = request.GetType().GetProperty("Email")?.GetValue(request)?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email không được để trống" });
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new { message = "Email không hợp lệ" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest(new { message = "Email không tồn tại" });
            }

            var newPassword = GenerateRandomPassword();
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                email,
                "Mật Khẩu Mới - Rice Runner",
                $"Mật khẩu mới của bạn là: {newPassword}\nVui lòng đăng nhập và đổi mật khẩu để bảo mật tài khoản."
            );

            return Ok(new { Message = "Mật khẩu mới đã được gửi đến email của bạn" });
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}