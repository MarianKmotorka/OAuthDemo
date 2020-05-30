using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OauthDemoApi.Persistance;

namespace OauthDemoApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/profile")]
    public class PrrofileController : ControllerBase
    {
        private readonly Db _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PrrofileController(Db db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("image")]
        public async Task<ActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image is null)
                return BadRequest();

            using var binaryReader = new BinaryReader(image.OpenReadStream());
            var imageBytes = binaryReader.ReadBytes((int)image.Length);

            var userId = _httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _db.Users.FindAsync(userId);

            user.PictureUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetMyInfo()
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _db.Users.FindAsync(userId);

            return Ok(new
            {
                user.Email,
                user.Name,
                Picture = user.PictureUrl
            });
        }
    }
}
