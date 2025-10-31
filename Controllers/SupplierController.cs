using Microsoft.AspNetCore.Mvc;
using SupplierModels;
using System.Collections.Generic;
using System.Linq;

namespace SuppliersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
    // Para exemplo simples: usar uma lista em memória
             private static readonly List<Supplier> _suppliers = new()
             {
                 new Supplier { Id = 1, Name = "ISEP1 Ltda" },
                 new Supplier { Id = 2, Name = "ISEP2 Corp" }
             };

             [HttpGet]
             public ActionResult<IEnumerable<Supplier>> GetSuppliers()
             {
                 return Ok(_suppliers);
             }

             [HttpGet("{id}")]
             public ActionResult<Supplier> GetSupplier(int id)
             {
                 var supplier = _suppliers.FirstOrDefault(s => s.Id == id);
                 if (supplier == null)
                     return NotFound();
                 return Ok(supplier);
             }

             [HttpPost]
             public ActionResult<Supplier> CreateSupplier(Supplier supplier)
             {
                 // definir novo Id (simples)
                 supplier.Id = _suppliers.Any() ? _suppliers.Max(s => s.Id) + 1 : 1;
                 _suppliers.Add(supplier);
                 // retornamos 201 Created com localização
                 return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
             }

             [HttpPut("{id}")]
             public IActionResult UpdateSupplier(int id, Supplier updatedSupplier)
             {
                 var existing = _suppliers.FirstOrDefault(s => s.Id == id);
                 if (existing == null)
                     return NotFound();

                 existing.Name = updatedSupplier.Name;
                 return NoContent();  // 204
             }

             [HttpDelete("{id}")]
             public IActionResult DeleteSupplier(int id)
             {
                 var existing = _suppliers.FirstOrDefault(s => s.Id == id);
                 if (existing == null)
                     return NotFound();

                 _suppliers.Remove(existing);
                 return NoContent();
             }
         
    }
}