using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmdbApi.DAL.Models
{
    public class CommonResponse
    {
        public bool Status { get; set; }
        public object Response { get; set; }
    }
}
