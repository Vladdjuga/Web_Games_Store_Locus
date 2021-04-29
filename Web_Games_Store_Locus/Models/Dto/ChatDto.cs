using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Dto
{
    public class ChatDto
    {
        public int Id { get; set; }
        public FriendDto User1 { get; set; }
        public FriendDto User2 { get; set; }
        public ICollection<MessageDto> Messages { get; set; }
    }
}
