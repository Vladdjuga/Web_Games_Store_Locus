using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Dto
{
    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductDto? Product { get; set; }
    }
}
