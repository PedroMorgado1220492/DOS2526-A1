using ProductsAPI.Models;

namespace ProductsAPI.SupplierModel
{
    //Suplier Model
    public class SupplierModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Um Supplier fornece vários Products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}