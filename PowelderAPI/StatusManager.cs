using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TShockAPI;

namespace PowelderAPI
{
    public class StatusManager
    {

        public struct Status
        {
            public bool OverrideProxy;
            public string Text;

        }

        public Status[] status = new Status[256];

        public StatusManager(Status status)
        {

            new Thread(StatusUpdate)
            {
                IsBackground = true
            }.Start();
        }


        private void StatusUpdate()
        {
            while (true)
            {
                Thread.Sleep(1000);
                foreach (var player in _ = TShock.Players.Where(x => x != null && x.Active))
                {
                    player.SendData(PacketTypes.Status, $">|{(status[player.Index].OverrideProxy ? "t" : "f")}{status[player.Index].Text}", 2);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
        
    }
}
