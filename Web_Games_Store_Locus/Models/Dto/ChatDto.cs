using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Dto
{
    public class ChatDto
    {
        public int Id { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public ICollection<MessageDto> Messages { get; set; }
    }
}
