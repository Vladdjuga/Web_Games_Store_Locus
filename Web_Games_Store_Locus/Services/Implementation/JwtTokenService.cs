using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Entities;
using Web_Games_Store_Locus.Services.Interfaces;

namespace Web_Games_Store_Locus.Services.Implementation
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        public JwtTokenService(ApplicationContext context,
            IConfiguration configuration,
            UserManager<User> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }
        public string CreateToken(User user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            var fullName = _context.UserInfos.FirstOrDefault(t => t.Id == user.Id).Username;
            var claims = new List<Claim>()
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Id)
                new Claim("id", user.Id.ToString()),
                //new Claim("name", fullName),
                new Claim("email", user.Email)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }
            string jwtTokenSecretKey = this._configuration.GetValue<string>("SecretPhrase");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSecretKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                expires: DateTime.Now.AddYears(1));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
