using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.DTO;

namespace WebApplication
{
    public class ClientSubscriber : IClientSubscriber
    {
        public SubscribeDTO Subscribe(string callback)
        {
            var uri = new Uri(string.Format("http://localhost:51396/api/clients/subscribe?date=2016-03-10&callback={0}", callback));
            var response = uri.ToString().WithHeader("Accept-Client", "Fourth-Monitor").GetJsonAsync<SubscribeDTO>().Result;
            return response;
        }
    }
}