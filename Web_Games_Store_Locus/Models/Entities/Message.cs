using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Entities
{
    public class Message
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string File { get; set; }
        public DateTime Date { get; set; }
        public Chat Chat { get; set; }
        public UserInfo User { get; set; }
    }
}
