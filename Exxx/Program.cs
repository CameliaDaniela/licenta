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
            
            StreamInsight streamInsight = new StreamInsight(2,10);
            streamInsight.App();
            
        }
       
    }
}


