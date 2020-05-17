using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OauthDemoApi.Entities;
using OauthDemoApi.Models;
using OauthDemoApi.Options;
using OauthDemoApi.Persistance;

namespace OauthDemoApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly Db _db;
        private readonly OAuthOptions _oAuthOptions;
        private readonly JwtOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient;

        public AuthController(Db db, OAuthOptions oAuthOptions, JwtOptions jwtOptions,
            IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _oAuthOptions = oAuthOptions;
            _jwtOptions = jwtOptions;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("google-code-callback")]
        public async Task<ActionResult> GoogleCodeCallback(string code)
        {
            var request = new
            {
                code,
                client_id = _oAuthOptions.GoogleClientId,
                client_secret = _oAuthOptions.GoogleClientSecret,
                grant_type = "authorization_code",
                redirect_uri = "http://localhost:3000/google-login-callback"
            };

            var response = await _httpClient.PostAsJsonAsync(_oAuthOptions.TokenEndpoint, request);
            var authResponse = await response.Content.ReadAsAsync<GoogleAuthResponse>();

            var userInfoRequest = new HttpRequestMessage()
            {
                Method = new HttpMethod("GET"),
                Headers = { { HttpRequestHeader.Authorization.ToString(), $"{authResponse.TokenType} {authResponse.AccessToken}" } },
                RequestUri = new Uri(_oAuthOptions.UserInfoEndpoint)
            };

            var userInfoResponse = await _httpClient.SendAsync(userInfoRequest);
            var userInfoModel = await userInfoResponse.Content.ReadAsAsync<GoogleUserInfoModel>();

            var jwt = await CreateJwt(userInfoModel);

            return Ok(new { Token = jwt });
        }

        [Authorize]
        [HttpGet("my-info")]
        public async Task<ActionResult> GetProtected()
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _db.Users.SingleAsync(x => x.Id == userId);

            return Ok(new
            {
                user.Email,
                user.Name,
                Picture = user.PictureUrl
            });
        }

        private async Task<string> CreateJwt(GoogleUserInfoModel model)
        {

            var userDbId = await RegisterIfNotAlready(model);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDbId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, model.Email),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer)
            };

            var jwtObject = new JwtSecurityToken(
                _jwtOptions.Issuer,
                null,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(_jwtOptions.TokenLifeTime),
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );

            return tokenHandler.WriteToken(jwtObject);
        }

        private async Task<string> RegisterIfNotAlready(GoogleUserInfoModel model)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower());

            if (user is object)
                return user.Id;

            var newUser = new AuthUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = model.IsEmailVerified,
                Name = model.Name,
                PictureUrl = model.PictureUrl
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return newUser.Id;
        }
    }
}
