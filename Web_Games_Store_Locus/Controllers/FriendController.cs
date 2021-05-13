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
    [Authorize(Roles = "User,Admin")]
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
                if (inviteDto.Friend1.Username == inviteDto.Friend2.Username)
                {
                    return new ResultDto()
                    {
                        IsSuccess = true,
                        Message = "That is you"
                    };
                }
                var userinfo1 = _context.UserInfos.First(el => el.User == userFriend1);
                var userinfo2 = _context.UserInfos.First(el => el.User == userFriend2);
                _context.Invites.Remove(_context.Invites.First(el => el.User1 == userinfo1.Username && el.User2 == userinfo2.Username));
                var friend = new Friend()
                {
                    User1 = userinfo1.Username,
                    User2 = userinfo2.Username
                };
                _context.Friends.Add(friend);
                _context.SaveChanges();
                return new ResultDto()
                {
                    IsSuccess = true,
                    Message = "Friend added!"
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
        [HttpPost("invite")]
        public async Task<ResultDto> Invite([FromBody] InviteDto inviteDto)
        {
            try
            {
                var userFriend1 = await _userManager.FindByNameAsync(inviteDto.Friend1.Username);
                var userFriend2 = await _userManager.FindByNameAsync(inviteDto.Friend2.Username);
                if(inviteDto.Friend1.Username == inviteDto.Friend2.Username)
                {
                    return new ResultDto()
                    {
                        IsSuccess = true,
                        Message = "That is you"
                    };
                }
                if (!_context.Invites.Any(el => el.User2 == userFriend2.UserName))
                {
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
                else
                {
                    return new ResultDto()
                    {
                        IsSuccess = false,
                        Message = "That user already invited!"
                    };
                }
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
                var userinfo = _context.UserInfos.First(el => el.User == user);
                var invites = _context.Invites.Where(el => el.User2 == user.UserName).ToList();
                var list_of_friends = new List<FriendDto>();
                foreach (var item in invites)
                {
                    var userinf_temp = _context.UserInfos.First(el => el.Username == item.User1);
                    var temp = new FriendDto();

                    temp.Alias = userinf_temp.Alias;
                    temp.Username = userinf_temp.Username;
                    temp.Image = userinf_temp.Image;
                    temp.Birth = userinf_temp.Birth;
                    list_of_friends.Add(temp);
                }
                var result = new ResultCollectionDto<FriendDto>()
                {
                    IsSuccess = true,
                    Data = list_of_friends,
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
                var friends_table = _context.Friends.Where(el => el.User1 == userinfo.Username || el.User2 == userinfo.Username).ToList();
                var friends = new List<FriendDto>();
                foreach (var item in friends_table)
                {
                    var f1 = new UserInfo();
                    if (item.User1 == userinfo.Username)
                    {
                        f1 = _context.UserInfos.First(el => el.Username == item.User2);
                    }
                    else
                    {
                        f1 = _context.UserInfos.First(el => el.Username == item.User1);
                    }

                    friends.Add(new FriendDto()
                    {
                        Alias = f1.Alias,
                        Birth = f1.Birth,
                        Image = f1.Image,
                        Username = f1.Username
                    });
                }
                var result = new ResultCollectionDto<FriendDto>()
                {
                    IsSuccess = true,
                    Data = friends,
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
        [HttpGet("friendsusername/{username}")]
        public async Task<ResultDto> GetFriendsUsername([FromRoute] string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                var userinfo = _context.UserInfos.First(el => el.User == user);
                var friends_table = _context.Friends.Where(el => el.User1 == userinfo.Username || el.User2 == userinfo.Username).ToList();
                var friends = new List<FriendDto>();
                foreach (var item in friends_table)
                {
                    var f1 = new UserInfo();
                    if (item.User1 == userinfo.Username)
                    {
                        f1 = _context.UserInfos.First(el => el.Username == item.User2);
                    }
                    else
                    {
                        f1 = _context.UserInfos.First(el => el.Username == item.User1);
                    }

                    friends.Add(new FriendDto()
                    {
                        Alias = f1.Alias,
                        Birth = f1.Birth,
                        Image = f1.Image,
                        Username = f1.Username
                    });
                }
                var result = new ResultCollectionDto<FriendDto>()
                {
                    IsSuccess = true,
                    Data = friends,
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
        [HttpGet("find-friends/{token}&{str}")]
        public async Task<ResultDto> GetStringFriends([FromRoute]string token,[FromRoute] string str)
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
                var data = _context.UserInfos.Where(el => el.Username.Contains(str)&&el.Username!=userinfo.Username).Select(el => new FriendDto()
                {
                    Alias = el.Alias,
                    Birth = el.Birth,
                    Image = el.Image,
                    Username = el.User.UserName
                }).ToList();

                var result = new ResultCollectionDto<FriendDto>()
                {
                    IsSuccess = true,
                    Data = data,
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
        [HttpGet("all-friends")]
        [Authorize(Roles = "Admin")]
        public ResultDto GetAllFriends()
        {
            try
            {
                var data = _context.UserInfos.Select(el => new FriendDto()
                {
                    Alias = el.Alias,
                    Birth = el.Birth,
                    Image = el.Image,
                    Username = el.User.UserName
                }).ToList();

                var result = new ResultCollectionDto<FriendDto>()
                {
                    IsSuccess = true,
                    Data = data,
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


    }
}
