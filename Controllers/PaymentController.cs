using CuaHangQuanAo.Data;
using CuaHangQuanAo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            // Lấy giỏ hàng hiện tại
            var cart = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest(new { message = "Giỏ hàng của bạn đang trống, không thể chốt đơn!" });
            }

            // Tạo hóa đơn mới
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Paid", // Mặc định giả lập đã thanh toán thành công
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
            };

            // Chuyển sản phẩm từ Giỏ hàng sang Hóa đơn
            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product.Price
                });
            }

            _context.Orders.Add(order);

            // Xóa sạch giỏ hàng sau khi chốt đơn
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Thanh toán thành công! Đơn hàng đã được tạo.", orderId = order.Id });
        }
    }
}