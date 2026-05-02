using CuaHangQuanAo.API.Dtos;
using CuaHangQuanAo.Data;          // Để nhận diện UserDto và AppDbContext
using CuaHangQuanAo.Data.Entities; // Để nhận diện class User và Role
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            // Kiểm tra user tồn tại
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Tên đăng nhập đã tồn tại.");

            // Hash mật khẩu
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Tạo User mới theo Schema của Huy (RoleId là int, Role là Object)
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                RoleId = 2 // Gán ID số 2 (mặc định cho Role User trong bảng Roles)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Đăng ký thành công!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            // Quan trọng: Phải .Include(u => u.Role) thì mới lấy được thông tin từ bảng Role
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return BadRequest("Tài khoản hoặc mật khẩu không chính xác.");

            return Ok(new { Token = CreateToken(user) });
        }

        // GET: api/auth/profile
        [HttpGet("profile")]
        [Authorize] // Chỉ người đã đăng nhập mới xem được
        public async Task<IActionResult> GetProfile()
        {
            // Lấy UserId từ Token mà Huy đã dán vào Authorize
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Token không hợp lệ");

            var userId = int.Parse(userIdClaim.Value);
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound("Không thấy người dùng");

            return Ok(new
            {
                user.Username,
                user.Email,
                Role = user.Role.Name
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.TokenExpires < DateTime.Now)
                return Unauthorized("Token hết hạn hoặc không hợp lệ.");

            string token = CreateToken(user);
            return Ok(new { Token = token });
        }
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                // Lấy Name từ bảng Role (nhờ có .Include ở trên nên mới lấy được)
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}