using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Entities
{
    public class User : IdentityUser
    {
        public UserInfo UserInfo { get; set; }
    }
}
