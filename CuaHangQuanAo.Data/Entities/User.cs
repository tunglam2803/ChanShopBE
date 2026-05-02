namespace CuaHangQuanAo.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int RoleId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime TokenExpires { get; set; }

        // Navigation property
        public Role Role { get; set; } = null!;
    }
}
