using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Helpers;
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
                Address = "",
                Alias = model.Alias,
                Image = model.Image,
                Posts=new List<Post>()
            };
            user.UserInfo = ui;
            await _context.UserInfos.AddAsync(ui);
            ui.User = user;
            await _context.SaveChangesAsync();

            return new ResultLoginDto
            {
                IsSuccess = true,
                Token= user.Id
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
        [HttpGet("profile/{token}")]
        public async Task<ResultCollectionDto<ProfileDto>> Profile([FromRoute] string token)
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == "email").Value;

            var user = await _userManager.FindByEmailAsync(jti);
            var userInfo=_context.UserInfos.Find(user.Id);
            var profile = new ProfileDto()
            {
                Alias = userInfo.Alias,
                Birth = userInfo.Birth,
                Email = user.Email,
                Image = userInfo.Image,
                Username = userInfo.Username
            };
            return new ResultCollectionDto<ProfileDto>()
            {
                IsSuccess = true,
                Message = $"{token} users profile returned",
                Data = new List<ProfileDto>()
                {
                    profile
                }
            };
        }
        [HttpGet("profile_username/{username}")]
        public async Task<ResultCollectionDto<ProfileDto>> ProfileByUsername([FromRoute] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var userInfo = _context.UserInfos.Find(user.Id);
            var profile = new ProfileDto()
            {
                Alias = userInfo.Alias,
                Birth = userInfo.Birth,
                Email = user.Email,
                Image = userInfo.Image,
                Username = userInfo.Username
            };
            return new ResultCollectionDto<ProfileDto>()
            {
                IsSuccess = true,
                Message = $"{username} users profile returned",
                Data = new List<ProfileDto>()
                {
                    profile
                }
            };
        }
        [HttpPost("uploadPhoto/{id}")]
        public async Task<ResultDto> UploadImageAsync([FromRoute] string id, [FromForm(Name = "file")] IFormFile uploadedImage)
        {
            string filename = Guid.NewGuid().ToString() + ".jpg";
            string path = _environment.WebRootPath + @"\Images\UsersIcons";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + @"\" + filename;
            using (Bitmap bmp = new Bitmap(uploadedImage.OpenReadStream()))
            {
                var saveImage = ImageWorker.CreateImage(bmp, 400, 365);
                if (saveImage != null)
                {
                    saveImage.Save(path, ImageFormat.Jpeg);
                    var user = await _userManager.FindByIdAsync(id);
                    var userinfo = _context.UserInfos.Find(user.Id);
                    if (userinfo.Image != null && userinfo.Image != "default-user-image.png")
                    {
                        System.IO.File.Delete(path + userinfo.Image);
                    }
                    _context.UserInfos.Find(id).Image = filename;
                    _context.SaveChanges();
                }
            }
            return new ResultDto()
            {
                IsSuccess = true,
                Message = "ok"
            };
        }
    }
}
