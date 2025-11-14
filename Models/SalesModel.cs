using System.Collections.Generic;

namespace ProductsAPI.Models
{
    public class SalesModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public double TotalPrice { get; set; }
        public List<Product> Products { get; set; }
    }

}