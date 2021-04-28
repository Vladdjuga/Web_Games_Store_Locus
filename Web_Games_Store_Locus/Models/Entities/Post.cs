using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Entities
{
    public class Post
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public UserInfo User { get; set; }
        public ICollection<PostUser> Likes { get; set; }
    }
}
