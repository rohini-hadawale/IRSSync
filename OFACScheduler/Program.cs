using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFACScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            OFACSchedulerService.OFACService objSchedulerService = new OFACSchedulerService.OFACService();
            objSchedulerService.Timeout = int.MaxValue;
            objSchedulerService.StartOFACScheduler();
        }
    }
}
