namespace ProductsAPI.Models
{
    public class SalesModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }

        public int UserId { get; set; }
        public UsersModel? User { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}