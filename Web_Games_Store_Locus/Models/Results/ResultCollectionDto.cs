using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Models.Results
{
    public class ResultCollectionDto<T>:ResultDto
    {
        public ICollection<T> Data { get; set; }
    }
}
