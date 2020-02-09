using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.DTO
{
    public class SubscribeDTO
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}