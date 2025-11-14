using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        // Simulação de uma base de dados em memória
        private static List<SalesModel> sales = new List<SalesModel>();

        // GET: api/sales
        [HttpGet]
        public ActionResult<IEnumerable<SalesModel>> GetSales()
        {
            return Ok(sales);
        }

        // GET: api/sales/{id}
        [HttpGet("{id}")]
        public ActionResult<SalesModel> GetSale(int id)
        {
            var sale = sales.FirstOrDefault(s => s.Id == id);
            if (sale == null)
                return NotFound($"Sale with ID {id} not found.");
            
            return Ok(sale);
        }

        // POST: api/sales
        [HttpPost]
        public ActionResult<SalesModel> CreateSale([FromBody] SalesModel sale)
        {
            sale.Id = sales.Count > 0 ? sales.Max(s => s.Id) + 1 : 1;
            sales.Add(sale);
            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }

        // PUT: api/sales/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateSale(int id, [FromBody] SalesModel updatedSale)
        {
            var sale = sales.FirstOrDefault(s => s.Id == id);
            if (sale == null)
                return NotFound($"Sale with ID {id} not found.");

            sale.Description = updatedSale.Description;
            sale.TotalPrice = updatedSale.TotalPrice;
            sale.Products = updatedSale.Products;

            return NoContent();
        }

        // DELETE: api/sales/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteSale(int id)
        {
            var sale = sales.FirstOrDefault(s => s.Id == id);
            if (sale == null)
                return NotFound($"Sale with ID {id} not found.");

            sales.Remove(sale);
            return NoContent();
        }
        private static List<SalesModel> Sales = new List<SalesModel>();
    }
}
