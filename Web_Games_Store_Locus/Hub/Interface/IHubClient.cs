using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Games_Store_Locus.Hub.Interface
{
    public interface IHubClient
    {
        Task BroadcastMessage();
    }
}
