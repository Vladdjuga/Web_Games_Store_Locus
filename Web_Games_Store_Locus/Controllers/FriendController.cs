using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class FriendController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;
        public FriendController(UserManager<User> userManager,
            ApplicationContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpPost("inviteaccept")]
        public async Task<ResultDto> InviteAccept([FromBody] InviteDto inviteDto)
        {
            try
            {
                var userFriend1 = await _userManager.FindByNameAsync(inviteDto.Friend1.Username);
                var userFriend2 = await _userManager.FindByNameAsync(inviteDto.Friend2.Username);
                var userinfo1 = _context.UserInfos.First(el=>el.User== userFriend1);
                var userinfo2 = _context.UserInfos.First(el=>el.User== userFriend2);
                if (userinfo1.Friends == null)
                {
                    userinfo1.Friends = new List<User>();
                }
                if (userinfo2.Friends == null)
                {
                    userinfo2.Friends = new List<User>();
                }
                userinfo1.Friends.Add(userFriend2);
                userinfo2.Friends.Add(userFriend1);
                _context.SaveChanges();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "Successfuly added"
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
        [HttpPost("inviteaccept")]
        public async Task<ResultDto> Invite([FromBody] InviteDto inviteDto)
        {
            try
            {
                var userFriend1 = await _userManager.FindByNameAsync(inviteDto.Friend1.Username);
                var userFriend2 = await _userManager.FindByNameAsync(inviteDto.Friend2.Username);
                var invite = new Invite()
                {
                    User1 = userFriend1.UserName,
                    User2 = userFriend2.UserName
                };
                _context.Invites.Add(invite);
                _context.SaveChanges();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "Successfuly added"
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
        [HttpGet("getinvites/{token}")]
        public async Task<ResultDto> GetInvites([FromRoute]string token)
        {
            try
            {
                var stream = token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var jti = tokenS.Claims.First(claim => claim.Type == "email").Value;

                var user = await _userManager.FindByEmailAsync(jti);
                var userinfo = _context.UserInfos.First(el=>el.User==user);
                var result = new ResultCollectionDto<InviteDto>()
                {
                    IsSuccess = true,
                    Data = _context.Invites.Where(el=>el.User1==user.UserName).Select(el => new InviteDto()
                    {
                        Friend1=new FriendDto()
                        {
                            Alias = userinfo.Alias,
                            Birth = userinfo.Birth,
                            Image = userinfo.Image,
                            Username = user.UserName
                        },
                        Friend2 = Helpers.MakeFriendDto.Make(user,_context)
                    }).ToList(),
                    Message = "success"
                };
                return result;
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


        [HttpGet("friends/{token}")]
        public async Task<ResultDto> GetFriends([FromRoute]string token)
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
                if (userinfo.Friends == null)
                {
                    userinfo.Friends = new List<User>();
                }
                var result = new ResultCollectionDto<FriendDto>()
                {
                    IsSuccess = true,
                    Data = userinfo.Friends.Select(el => new FriendDto()
                    {
                        Alias = el.UserInfo.Alias,
                        Birth = el.UserInfo.Birth,
                        Image = el.UserInfo.Image,
                        Username = el.UserName
                    }).ToList(),
                    Message = "success"
                };
                return result;
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
