using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Dto.Users;
using Web_Games_Store_Locus.Models.Entities;
using Web_Games_Store_Locus.Models.Results;
using Web_Games_Store_Locus.Services.Interfaces;

namespace Web_Games_Store_Locus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ApplicationContext _context;
        private UserManager<User> _userManager;
        private IWebHostEnvironment _environment;
        private SignInManager<User> _signInManager;
        private IJwtTokenService _jwtTokenService;
        public AccountController(UserManager<User> userManager,
            ApplicationContext context,
            IWebHostEnvironment environment,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _context = context;
            _environment = environment;
            _jwtTokenService = jwtTokenService;
            _signInManager = signInManager;
        }


        [HttpPost("register")]
        public async Task<ResultDto> Register([FromBody] RegisterDto model)
        {
            User user = new User()
            {
                Email = model.Email,
                UserName = model.Username
            };
            await _userManager.CreateAsync(user, model.Password);
            UserInfo ui = new UserInfo()
            {
                Id = user.Id,
                Birth = model.Birth,
                Username = model.Username,
                IsBanned = false,
                Address="",
                Alias="",
                Image= _environment.WebRootPath+"/Images/UserIcons/defailt-user-image.png"
            };
            await _context.UserInfos.AddAsync(ui);
            await _context.SaveChangesAsync();

            return new ResultDto
            {
                IsSuccess = true
            };

        }
        [HttpPost("login")]
        public async Task<ResultLoginDto> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (!result.Succeeded)
            {
                return new ResultLoginDto
                {
                    IsSuccess = false,
                    Message="Incorrect password or email"
                };
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return new ResultLoginDto
            {
                IsSuccess = true,
                Token = _jwtTokenService.CreateToken(user)
            };

        }
    }
}
