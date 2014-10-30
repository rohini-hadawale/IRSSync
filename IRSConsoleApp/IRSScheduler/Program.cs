using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRSScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            IRSSchedulerService.OFACService obj = new IRSSchedulerService.OFACService();
            obj.Timeout = int.MaxValue;
            obj.StartIRScheduler();
        }
    }
}
