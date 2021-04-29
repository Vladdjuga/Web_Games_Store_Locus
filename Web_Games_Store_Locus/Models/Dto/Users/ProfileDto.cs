using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Dto.Users
{
    public class ProfileDto
    {
        public string Image { get; set; }
        public string Username { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public DateTime Birth { get; set; }
    }
}
