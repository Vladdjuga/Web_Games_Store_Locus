using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
