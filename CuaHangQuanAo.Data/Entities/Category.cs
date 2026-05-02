namespace CuaHangQuanAo.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
