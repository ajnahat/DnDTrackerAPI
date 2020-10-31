using DnDTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DnDTrackerContext _context;

        public UsersController(DnDTrackerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users
                .Include(o => o.Encounters)
                    .ThenInclude(o => o.Waves)
                        .ThenInclude(o => o.Monsters)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<User>> AuthenticateLogin([FromBody] AuthenticateLoginPayload request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(o => o.UserName.ToUpper() == request.UserName.ToUpper());

            if (user == null)
            {
                return BadRequest("There is no user by that name.");
            }

            return Ok(user);
        }

        [HttpGet("authenticate/{userName}")]
        public ActionResult<bool> UserNameExists(string userName)
        {
            return Ok(UserExists(userName));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("{userName}")]
        public async Task<ActionResult<User>> PostUser(string userName)
        {
            var user = new User() { UserName = userName };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUser", new { id = user.UserId }, user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private bool UserExists(string userName)
        {
            return _context.Users.Any(e => e.UserName.ToUpper() == userName.ToUpper());
        }

        [HttpGet("{id}/encounters")]
        public async Task<ActionResult<IEnumerable<Encounter>>> GetEncounters(int id)
        {
            return await _context.Encounters.Where(o => o.UserId == id)
                .Include(o => o.Waves)
                    .ThenInclude(o => o.Monsters)
                .ToListAsync();
        }
    }
}