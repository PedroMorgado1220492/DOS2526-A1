namespace ProductsAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int SupplierId { get; set; }
        public SupplierModel? Supplier { get; set; }

        public ICollection<SalesModel> Sales { get; set; } = new List<SalesModel>();
    }
}