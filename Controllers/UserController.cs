using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersModel>>> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.Sales)
                .ThenInclude(s => s.Products)
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsersModel>> GetById(int id)
        {
            var user = await _context.Users
                .Include(u => u.Sales)
                .ThenInclude(s => s.Products)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UsersModel>> Create(UsersModel user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UsersModel updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest();
            }

            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Username = updatedUser.Username;
            existing.Email = updatedUser.Email;
            existing.Fullname = updatedUser.Fullname;
            existing.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            _context.Users.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}