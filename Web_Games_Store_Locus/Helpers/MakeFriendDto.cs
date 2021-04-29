using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models;
using Web_Games_Store_Locus.Models.Dto;
using Web_Games_Store_Locus.Models.Entities;

namespace Web_Games_Store_Locus.Helpers
{
    public class MakeFriendDto
    {
        public static FriendDto Make(User user,ApplicationContext _context)
        {
            var userinfo = _context.UserInfos.First(el => el.User == user);
            var result = new FriendDto();
            result.Alias = userinfo.Alias;
            result.Username = user.UserName;
            result.Image = userinfo.Image;
            result.Birth = userinfo.Birth;
            return result;
        }
    }
}
