using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;

        public PrrofileController(Db db, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        [HttpPost("image")]
        public async Task<ActionResult> UploadImage([FromForm] IFormFile image)
        {
            if (image is null)
                return BadRequest();

            var userId = _httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _db.Users.FindAsync(userId);

            var folderPath = Path.Combine(_env.ContentRootPath, "Resources");
            DeleteImages(userId, folderPath);

            var imageName = $"{userId}_{image.FileName}";
            var imagePath = Path.Combine(folderPath, imageName);

            using var fileStream = new FileStream(imagePath, FileMode.Create);
            await image.CopyToAsync(fileStream);

            user.PictureUrl = imageName;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> GetMyInfo()
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _db.Users.FindAsync(userId);

            if (Uri.TryCreate(user.PictureUrl, UriKind.Absolute, out _))
                return Ok(new
                {
                    user.Email,
                    user.Name,
                    Picture = user.PictureUrl
                });

            var imagePath = Path.Combine(_env.ContentRootPath, "Resources", user.PictureUrl);
            var image = System.IO.File.OpenRead(imagePath);
            var extension = Path.GetExtension(imagePath);

            using var binaryReader = new BinaryReader(image);
            var imageBytes = binaryReader.ReadBytes((int)image.Length);

            return Ok(new
            {
                user.Email,
                user.Name,
                Picture = $"data:image/{extension};base64,{Convert.ToBase64String(imageBytes)}"
            });
        }

        private void DeleteImages(string userId, string folderPath)
        {
            var images = Directory.GetFiles(folderPath).Where(x => Path.GetFileName(x).StartsWith(userId) || x.Contains("XXXX")).ToList();

            foreach (var image in images)
                System.IO.File.Delete(image);
        }
    }
}
