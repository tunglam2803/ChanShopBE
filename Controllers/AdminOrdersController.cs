using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        // ==========================================
        // 1. XEM TOÀN BỘ ĐƠN HÀNG (DỮ LIỆU GIẢ CHỜ DATABASE)
        // ==========================================
        [HttpGet("orders")]
        public IActionResult GetAllOrders()
        {
            // Trả về một mảng rỗng tạm thời để không bị lỗi code
            return Ok(new string[] { "Chưa có database cho Đơn hàng, hãy nhắc team cập nhật AppDbContext!" });
        }

        // ==========================================
        // 2. THỐNG KÊ TỔNG QUAN (DASHBOARD - DỮ LIỆU GIẢ)
        // ==========================================
        [HttpGet("dashboard")]
        public IActionResult GetDashboardStats()
        {
            var dashboardData = new
            {
                TotalOrders = 0, // Chờ DB
                TotalProducts = 8, // Số lượng cứng dựa trên seed data
                TotalUsers = 0, // Chờ DB
                TotalRevenue = 0 // Chờ DB
            };

            return Ok(dashboardData);
        }
    }
}