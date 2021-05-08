using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_Games_Store_Locus.Helpers;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Dto;
using Web_Games_Store_Locus.Models.Dto.Users;
using Web_Games_Store_Locus.Models.Entities;
using Web_Games_Store_Locus.Models.Results;
using Web_Games_Store_Locus.Services.Interfaces;

namespace Web_Games_Store_Locus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private ApplicationContext _context;
        private UserManager<User> _userManager;
        private IWebHostEnvironment _environment;
        private SignInManager<User> _signInManager;
        private IJwtTokenService _jwtTokenService;

        public PostController(UserManager<User> userManager,
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

        [HttpGet("getuserposts/{token}")]
        public async Task<ResultDto> GetUsersPosts([FromRoute]string token)
        {
            try
            {
                var stream = token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var jti = tokenS.Claims.First(claim => claim.Type == "email").Value;

                var user = await _userManager.FindByEmailAsync(jti);
                var userinfo = _context.UserInfos.First(el => el.User == user);

                var selected = _context.Posts.Where(el=>el.User==userinfo).Select(el => new PostDto()
                {
                    Id = el.Id,
                    Text=el.Text,
                    Date=el.Date,
                    Image=el.Image,
                    User=new ProfileDto()
                    {
                        Alias = userinfo.Alias,
                        Birth = userinfo.Birth,
                        Email = user.Email,
                        Image = userinfo.Image,
                        Username = userinfo.Username
                    }
                }).ToList();
                var res = new ResultCollectionDto<PostDto>()
                {
                    IsSuccess = true,
                    Message = "200",
                    Data = selected,
                };
                return res;
            }
            catch (Exception ex)
            {
                var err = new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return err;
            }
        }
        [HttpPost("add-post/{token}")]
        public async Task<ResultDto> AddPost([FromBody] PostDto model,[FromRoute]string token)
        {
            try
            {
                var stream = token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var jti = tokenS.Claims.First(claim => claim.Type == "email").Value;

                var user = await _userManager.FindByEmailAsync(jti);
                var userinfo = _context.UserInfos.First(el => el.User == user);

                var post = new Post()
                {
                    Text = model.Text,
                    Date = DateTime.Now,
                    Image = model.Image
                };
                _context.Posts.Add(post);
                post.User = userinfo;
                _context.SaveChanges();
                var res = new ResultDto()
                {
                    IsSuccess = true,
                    Message = post.Id.ToString()
                };
                return res;
            }
            catch (Exception ex)
            {
                var err = new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return err;
            }
        }
        [HttpPut]
        public ResultDto EditPost([FromBody] PostDto model)
        {
            try
            {
                var tag = _context.Posts.Find(model.Id);
                tag.Text = model.Text;
                tag.Image = model.Image;

                _context.Posts.Add(tag);
                _context.SaveChanges();
                var res = new ResultDto()
                {
                    IsSuccess = true,
                    Message = "200"
                };
                return res;
            }
            catch (Exception ex)
            {
                var err = new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return err;
            }
        }
        [HttpGet]
        [Route("remove/{id}")]
        public ResultDto Remove([FromRoute] int id)
        {
            var obj = _context.Posts.Find(id);
            _context.Posts.Remove(obj);
            _context.SaveChanges();
            return new ResultDto()
            {
                IsSuccess = true,
                Message = "Removed succesfuly"
            };
        }
        [HttpPost("uploadPhoto/{id}")]
        public async Task<ResultDto> UploadImageAsync([FromRoute] int id, [FromForm(Name = "file")] IFormFile uploadedImage)
        {
            string filename = Guid.NewGuid().ToString() + ".jpg";
            string path = _environment.WebRootPath + @"\Images\PostsImages";
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
                    var post = _context.Posts.Find(id);
                    if (post.Image != null)
                    {
                        System.IO.File.Delete(path + post.Image);
                    }
                    post.Image = filename;
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