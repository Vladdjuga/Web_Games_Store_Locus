using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_Games_Store_Locus.Hub.Interface;

namespace Web_Games_Store_Locus.Hub
{
    public class BroadcastHub : Hub<IHubClient>
    {
    }
}
