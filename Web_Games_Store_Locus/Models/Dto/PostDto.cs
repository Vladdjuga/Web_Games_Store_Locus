using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models.Entities;

namespace Web_Games_Store_Locus.Models.Dto
{
    public class PostDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public UserInfo User { get; set; }
    }
}
