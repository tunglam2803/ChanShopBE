using CuaHangQuanAo.Data;
using CuaHangQuanAo.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cua Hang API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập Token theo cú pháp: Bearer [token_của_bạn]",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// 2. JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 3. Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 4. Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 6. Auto-seed Roles + Admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Đảm bảo Role Admin Id=1 tồn tại
    if (!db.Roles.Any(r => r.Id == 1))
    {
        db.Database.ExecuteSqlRaw(
            "SET IDENTITY_INSERT Roles ON; " +
            "INSERT INTO Roles (Id, Name) VALUES (1, N'Admin'); " +
            "SET IDENTITY_INSERT Roles OFF;");
        Console.WriteLine("✅ Role Admin created");
    }

    // Đảm bảo Role User Id=2 tồn tại
    if (!db.Roles.Any(r => r.Id == 2))
    {
        db.Database.ExecuteSqlRaw(
            "SET IDENTITY_INSERT Roles ON; " +
            "INSERT INTO Roles (Id, Name) VALUES (2, N'User'); " +
            "SET IDENTITY_INSERT Roles OFF;");
        Console.WriteLine("✅ Role User created");
    }

    // Dùng SQL trực tiếp để tránh đọc DateTime NULL
    var adminExists = db.Database.ExecuteSqlRaw(
        "SELECT 1 FROM Users WHERE Username = 'admin'") >= 0
        && db.Users.Any(u => u.Username == "admin");

    if (!adminExists)
    {
        // Insert thẳng bằng SQL, set TokenExpires = GETDATE() tránh NULL
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        db.Database.ExecuteSqlRaw(
            "INSERT INTO Users (Username, Email, PasswordHash, RoleId, TokenExpires) " +
            "VALUES ({0}, {1}, {2}, {3}, {4})",
            "admin", "admin@chanshop.com", hash, 1, DateTime.Now.AddYears(1));
        Console.WriteLine("✅ Admin account created: admin / Admin@123");
    }
    else
    {
        // Fix RoleId nếu sai, cũng fix TokenExpires NULL
        db.Database.ExecuteSqlRaw(
            "UPDATE Users SET RoleId = 1, " +
            "TokenExpires = CASE WHEN TokenExpires IS NULL THEN GETDATE() ELSE TokenExpires END " +
            "WHERE Username = 'admin'");
        Console.WriteLine("✅ Admin RoleId verified");
    }
}

// 7. Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html", permanent: false);
    return Task.CompletedTask;
});

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();