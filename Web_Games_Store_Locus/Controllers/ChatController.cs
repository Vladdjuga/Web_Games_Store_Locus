using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Dto;
using Web_Games_Store_Locus.Models.Entities;
using Web_Games_Store_Locus.Models.Results;

namespace Web_Games_Store_Locus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;
        public ChatController(UserManager<User> userManager,
            ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("chats/{token}")]
        public async Task<ResultDto> GetChats([FromRoute] string token)
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
                var chats = _context.Chats.Where(el => el.User1 == userinfo.Username || el.User2 == userinfo.Username).ToList();

                var list_of_chats = new List<ChatDto>();
                foreach (var item in chats)
                {
                    var messages = _context.Messages.Where(el => el.Chat == item);

                    var temp1 = new FriendDto();
                    var temp2 = new FriendDto();
                    var f1 = new UserInfo();
                    var f2 = new UserInfo();
                    if (item.User1 == userinfo.Username)
                    {
                        f1 = _context.UserInfos.First(el => el.Username == item.User1);
                        f2 = _context.UserInfos.First(el => el.Username == item.User2);
                    }
                    else
                    {
                        f1 = _context.UserInfos.First(el => el.Username == item.User2);
                        f2 = _context.UserInfos.First(el => el.Username == item.User1);
                    }

                    temp1.Alias = f1.Alias;
                    temp1.Username = f1.Username;
                    temp1.Image = f1.Image;
                    temp1.Birth = f1.Birth;
                    //
                    temp2.Alias = f2.Alias;
                    temp2.Username = f2.Username;
                    temp2.Image = f2.Image;
                    temp2.Birth = f2.Birth;
                    var chat = new ChatDto()
                    {
                        Id=item.Id,
                        User1 = temp1,
                        User2 = temp2,
                        Messages = messages.Select(el => new MessageDto() { File = el.File, Id = el.Id, Text = el.Text,
                            Date = el.Date,
                            User = new FriendDto() { 
                            Alias=el.User.Alias,
                            Birth=el.User.Birth,
                            Image=el.User.Image,
                            Username=el.User.Username
                        } }).OrderBy(el=>el.Date).ToList()
                    };
                    list_of_chats.Add(chat);
                }
                return new ResultCollectionDto<ChatDto>()
                {
                    IsSuccess = true,
                    Message = "success",
                    Data = list_of_chats
                };
            }
            catch (Exception ex)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        [HttpGet("add-chat/{token}&{friend}")]
        public async Task<ResultDto> AddChat([FromRoute]string token, [FromRoute]string friend)
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
                if(_context.Chats.Any(el=>(el.User1==userinfo.Username&&el.User2==friend)||(el.User2 == userinfo.Username && el.User1 == friend)))
                {
                    return new ResultDto()
                    {
                        IsSuccess = true,
                        Message = $"You already have chat with {friend}!"
                    };
                }
                var chat = new Chat()
                {
                    User1 = userinfo.Username,
                    User2 = friend,
                    Messages = new List<Message>()
                };
                _context.Chats.Add(chat);
                _context.SaveChanges();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = $"Write something to {friend}!"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        [HttpPost("add-message/{token}&{id}")]
        public async Task<ResultDto> AddMessage([FromRoute]string token, [FromRoute]string id,[FromBody]MessageDto model)
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
                var chat = _context.Chats.Find(int.Parse(id));
                var message = new Message()
                {
                    Id= Guid.NewGuid().ToString(),
                    File = model.File,
                    Chat = chat,
                    Text = model.Text,
                    User = userinfo,
                    Date=DateTime.Now
                };
                _context.Messages.Add(message);
                _context.SaveChanges();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = $"Message sent!"
                };
            }
            catch (Exception ex)
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