using CuaHangQuanAo.Data;
using CuaHangQuanAo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/admin/products")]
    [ApiController]
    [Authorize(Roles = "Admin")] // ROLE GUARD: Bắt buộc tài khoản phải có quyền Admin
    public class AdminProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminProductsController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. THÊM SẢN PHẨM MỚI: POST /api/admin/products
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Đã xóa phần gán CreatedAt để khớp với Database của nhóm

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, new { message = "Thêm sản phẩm thành công!", data = product });
        }

        // ==========================================
        // 2. SỬA THÔNG TIN SẢN PHẨM: PUT /api/admin/products/{id}
        // ==========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm cần sửa!" });
            }

            // Cập nhật các trường thông tin
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            // Đã xóa trường StockQuantity để khớp với Database của nhóm
            product.CategoryId = updatedProduct.CategoryId;
            product.ImageUrl = updatedProduct.ImageUrl;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật sản phẩm thành công!", data = product });
        }

        // ==========================================
        // 3. XÓA SẢN PHẨM: DELETE /api/admin/products/{id}
        // ==========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm cần xóa!" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa sản phẩm thành công!" });
        }

        // ==========================================
        // Hàm hỗ trợ để trả về thông tin sản phẩm sau khi Create thành công
        // ==========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}