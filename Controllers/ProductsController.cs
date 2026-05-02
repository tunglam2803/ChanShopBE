using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CuaHangQuanAo.Data;
using CuaHangQuanAo.Data.Entities;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm kèm phân trang và lọc theo giá, danh mục
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Lọc theo Category
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            // Lọc theo giá
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .OrderByDescending(p => p.Id) // Mới nhất lên đầu
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Success = true,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Data = products
            });
        }

        /// <summary>
        /// Lấy chi tiết 1 sản phẩm kèm hình ảnh
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { Success = false, Message = "Không tìm thấy sản phẩm" });

            return Ok(new { Success = true, Data = product });
        }

        /// <summary>
        /// Lấy danh sách Áo, Quần, Phụ kiện
        /// </summary>
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(new { Success = true, Data = categories });
        }

        /// <summary>
        /// Lấy 4 sản phẩm nổi bật hiển thị trang chủ
        /// </summary>
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            var products = await _context.Products
                .OrderBy(x => Guid.NewGuid()) // Lấy ngẫu nhiên 4 món cho mới mẻ
                .Take(4)
                .ToListAsync();
            return Ok(new { Success = true, Data = products });
        }

        /// <summary>
        /// Upload ảnh sản phẩm và lưu vào wwwroot/images
        /// </summary>
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "File ảnh không hợp lệ" });

            // Kiểm tra thư mục wwwroot/images
            var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imagesPath = Path.Combine(rootPath, "images");

            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            // Tạo tên file chống trùng
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(imagesPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Success = true, ImageUrl = $"/images/{fileName}" });
        }
    }
}