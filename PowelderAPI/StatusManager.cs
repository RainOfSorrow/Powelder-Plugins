using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowelderAPI
{
    public class StatusManager
    {
        public struct Status
		{
			public bool overrideProxy;
            public TShockAPI.TSPlayer plr;
			public string text
            {
                get
                {
                    return text;
                }
                set
                {
                    textOld = text;
                    text = value;
                }
            }
            public string textOld;

            public int interval;
		}

        public Status status;

        public StatusManager(Status status)
        {
            this.status = status;


            new Thread(StatusUpdate)
            {
                IsBackground = true
            }.Start();
        }


        private void StatusUpdate()
        {
            while (true)
            {
                Thread.Sleep(status.interval);
                if (status.interval == 0 || status.text == status.textOld)
                {
                    Thread.Sleep(5);
                }
                else
                {
                    status.plr.SendData(PacketTypes.Status, "/>" + status.text);           
                }

            }
		}

    }
}
