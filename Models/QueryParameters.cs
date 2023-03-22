using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImagesApi.Models
{
    public class QueryParameters
    {
        public int PageSize { get; set; } = 15;
        //public int StartIndex { get; set; }
        public int PageNumber { get; set; }
    }
}