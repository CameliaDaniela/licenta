using System;
using System.ServiceModel;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.ManagementService;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.ComplexEventProcessing.Linq;

namespace Exxx
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //StreamInsight streamInsight = new StreamInsight(4,10);
             StreamInsight streamInsight = new StreamInsight(1,10,10);
            //no values/ minute, no events in the flow, no road segments
            streamInsight.App();
            
        }
       
    }
}


