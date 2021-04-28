using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Entities
{
    public class PostUser
    {
        public string Id { get; set; }
        public UserInfo User { get; set; }
        public Post Post { get; set; }
        public bool IsLiked { get; set; }
    }
}
