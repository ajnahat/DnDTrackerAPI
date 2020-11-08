using DnDTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DnDTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DnDTrackerContext _context;

        public AuthenticationController(DnDTrackerContext context)
        {
            _context = context;
        }

        [HttpPost()]
        public async Task<ActionResult<User>> AuthenticateLogin([FromBody] AuthenticateLoginPayload request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(o => o.UserName.ToUpper() == request.UserName.ToUpper());

            if (user == null)
            {
                return Unauthorized("There is no user by that name.");
            }

            return Ok(user);
        }
    }
}