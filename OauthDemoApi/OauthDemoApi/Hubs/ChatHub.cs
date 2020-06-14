using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using OauthDemoApi.Persistance;

namespace OauthDemoApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly Db _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatHub(Db db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendMessage(string message)
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _db.Users.FindAsync(userId);

            await Clients.All.SendAsync("ReceiveMessage", user.Name, message);
        }
    }
}
