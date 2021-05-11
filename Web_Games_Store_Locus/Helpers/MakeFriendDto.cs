using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
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
        public static FriendDto Make(string user, DbSet<UserInfo> infos)
        {
            var userinfo = infos.First(el => el.Username == user);
            var result = new FriendDto();
            result.Alias = userinfo.Alias;
            result.Username = userinfo.Username;
            result.Image = userinfo.Image;
            result.Birth = userinfo.Birth;
            return result;
        }
    }
}
