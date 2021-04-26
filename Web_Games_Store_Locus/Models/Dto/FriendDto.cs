using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Dto
{
    public class FriendDto
    {
        public string Username { get; set; }
        public string Alias { get; set; }
        public DateTime Birth { get; set; }
        public string Image { get; set; }
    }
}
