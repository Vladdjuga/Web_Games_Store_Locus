using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models.Entities;

namespace Web_Games_Store_Locus.Models.Dto
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string File { get; set; }
        public ChatDto Chat { get; set; }
    }
}
