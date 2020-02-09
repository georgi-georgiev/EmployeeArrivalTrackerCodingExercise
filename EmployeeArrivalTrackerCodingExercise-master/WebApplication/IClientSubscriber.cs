using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DTO;

namespace WebApplication
{
    public interface IClientSubscriber
    {
        SubscribeDTO Subscribe(string callback);
    }
}
