using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Entities
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Alias { get; set; }
        public DateTime Birth { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public bool IsBanned { get; set; }
        public User User { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
