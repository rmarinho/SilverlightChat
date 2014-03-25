using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Silverlight.PolicyServers;
using System.Net;

namespace SilverlightChat.PolicyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            MulticastPolicyConfiguration config = new MulticastPolicyConfiguration();
            config.AnySourceConfiguration.Add("*", new MulticastResource(
              //  IPAddress.Parse("224.0.0.1"), 3000)); 
      IPAddress.Parse("239.0.0.5"), 45678));
            using (MulticastPolicyServer server = new MulticastPolicyServer(config))
            {
                server.Start();

                Console.Write("Hit enter to exit...");
                Console.ReadLine();

                server.Stop();
            }
        }
    }
}
