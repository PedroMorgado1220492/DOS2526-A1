using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private static readonly List<UsersModel> _users = new()
        {
            new UsersModel
            { 
                Id = 1, Username = "jdoe", Email = "jdoe@email.com", 
                Fullname = "John Doe", Role = "User",
                Sales = new List<SalesModel>() 
                {
                    new SalesModel { Id = 1, Description = "First Sale", TotalPrice = 100 },
                    new SalesModel { Id = 2, Description = "Second Sale", TotalPrice = 150 }
                }
            },
            new UsersModel 
            { 
                Id = 2, Username = "admin", Email = "admin@email.com",
                Fullname = "Administrator", Role = "Admin" ,
                Sales = new List<SalesModel>() 
                {
                    new SalesModel { Id = 1, Description = "First Sale", TotalPrice = 100.00 },
                    new SalesModel { Id = 2, Description = "Second Sale", TotalPrice = 150.50 }
                }
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<UsersModel>> GetAll()
        {
            return Ok(_users);
        }

        [HttpGet("{id}")]
        public ActionResult<UsersModel> GetById(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            
            return Ok(user);
        }

        [HttpPost]
        public ActionResult<UsersModel> Create(UsersModel user)
        {
            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UsersModel updatedUser)
        {
            var existing = _users.FirstOrDefault(u => u.Id == id);
            if (existing == null)
                return NotFound();

            existing.Username = updatedUser.Username;
            existing.Email = updatedUser.Email;
            existing.Fullname = updatedUser.Fullname;
            existing.Role = updatedUser.Role;
            existing.Sales = updatedUser.Sales;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _users.FirstOrDefault(u => u.Id == id);
            if (existing == null)
                return NotFound();

            _users.Remove(existing);
            return NoContent();
        }
        private static List<UsersModel> Users = new List<UsersModel>();
    }
}
