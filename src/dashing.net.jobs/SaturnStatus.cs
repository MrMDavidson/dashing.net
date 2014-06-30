using System;
using System.ComponentModel.Composition;
using System.Threading;
using dashing.net.common;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    [Export(typeof(IJob))]
    public class SaturnStatus : IJob
    {
        public Lazy<Timer> Timer { get; private set; }


        public SaturnStatus()
        {
            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)));
        }

        private void SendMessage(object state)
        {
            Random r = new Random();
            if (r.Next(0, 2) == 1)
            {
                Dashing.SendMessage(
                    new
                    {
                        id = "saturn-status",
                        text = "OK",
                        is_okay = true,
                    }
                );
            }
            else
            {
                Dashing.SendMessage(
                    new
                    {
                        id = "saturn-status",
                        text = "ERROR",
                        is_okay = false,
                    }
                );
            }
        }
    }
}