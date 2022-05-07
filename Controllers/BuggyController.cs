using API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BuggyController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }
        [HttpGet("server-error")]
        public async Task<IActionResult> ServerError()
        {
            var user = await _applicationDbContext.Users!.FindAsync(-1);
            var think = user!.ToString();
            return Ok(think);
        }
        [HttpGet("not-found")]
        public async Task<IActionResult> NotFoundError()
        {
            var user = await _applicationDbContext.Users!.FindAsync(-1);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
        [HttpGet("bad-request")]
        public ActionResult<string> BadRequestError()
        {
            return BadRequest("Bad Request");
        }
        [HttpGet("get-me")]
        public ActionResult GetMe(){
            return Ok("My Name is dotnet");
        }
    }
}