using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PubScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            PubSchedulerService.OFACService obj = new PubSchedulerService.OFACService();
            obj.Timeout = int.MaxValue;
            obj.StartPubcheduler();
        }
    }
}
