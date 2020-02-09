using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DTO;

namespace WebApplication.Tests
{
    public class TestClientSubscriber : IClientSubscriber
    {
        public SubscribeDTO Subscribe(string callback)
        {
            return new SubscribeDTO
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddHours(8)
            };
        }
    }
}
