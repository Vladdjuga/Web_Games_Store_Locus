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
        [HttpPost("invite")]
        public async Task<ResultDto> Invite([FromBody] InviteDto inviteDto)
        {
            try
            {
                var userFriend1 = await _userManager.FindByNameAsync(inviteDto.Friend1.Username);
                var userFriend2 = await _userManager.FindByNameAsync(inviteDto.Friend2.Username);
                _context.Users.First(el=>el.UserName==userFriend1.UserName).UserInfo.Friends.Add(userFriend2);
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


        [HttpGet("friends/{token}")]
        public async Task<ResultDto> GetFriends([FromRoute]string token)
        {
            try
            {
                var stream = "[encoded jwt]";
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var jti = tokenS.Claims.First(claim => claim.Type == "email").Value;

                var user = await _userManager.FindByEmailAsync(jti);
                var result = new ResultCollectionDto<FriendDto>()
                {
                    IsSuccess = true,
                    Data = user.UserInfo.Friends.Select(el => new FriendDto()
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
