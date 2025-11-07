namespace SupplierModels
{
    //Suplier Model
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Um Supplier fornece vários Products
        public ICollection<Product> Products { get; set; } = new List<Products>();
    }
}