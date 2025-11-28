using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesModel>>> GetSales()
        {
            var sales = await _context.Sales
                .Include(s => s.Products)
                .Include(s => s.User)
                .ToListAsync();

            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesModel>> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Products)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult<SalesModel>> CreateSale([FromBody] SalesModel sale)
        {
            if (sale.Products != null && sale.Products.Any())
            {
                var productIds = sale.Products.Select(p => p.Id).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                sale.Products = products;
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] SalesModel updatedSale)
        {
            if (id != updatedSale.Id)
            {
                return BadRequest();
            }

            var sale = await _context.Sales
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            sale.Description = updatedSale.Description;
            sale.TotalPrice = updatedSale.TotalPrice;
            sale.UserId = updatedSale.UserId;

            sale.Products.Clear();
            if (updatedSale.Products != null && updatedSale.Products.Any())
            {
                var productIds = updatedSale.Products.Select(p => p.Id).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var product in products)
                {
                    sale.Products.Add(product);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
