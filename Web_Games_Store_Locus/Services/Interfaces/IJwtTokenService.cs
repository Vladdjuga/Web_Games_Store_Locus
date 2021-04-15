using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Models.Entities;

namespace Web_Games_Store_Locus.Services.Interfaces
{
    public interface IJwtTokenService
    {
        public string CreateToken(User user);
    }
}
