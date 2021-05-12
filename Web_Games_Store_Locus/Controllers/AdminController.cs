using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Entities;
using Web_Games_Store_Locus.Models.Results;
using Web_Games_Store_Locus.Services.Interfaces;

namespace Web_Games_Store_Locus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private ApplicationContext _context;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IWebHostEnvironment _environment;
        private SignInManager<User> _signInManager;
        private IJwtTokenService _jwtTokenService;

        public AdminController(UserManager<User> userManager,
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
        [HttpGet("ban/{username}")]
        [Authorize(Roles="Admin")]
        public async Task<ResultDto> BanUser([FromRoute]string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                var userInfo = _context.UserInfos.Find(user.Id);
                await _userManager.AddToRoleAsync(user, "Ban");
                await _context.SaveChangesAsync();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "User has been banned!"
                };
            }
            catch(Exception ex)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}