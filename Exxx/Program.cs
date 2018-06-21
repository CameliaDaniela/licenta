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
            //StreamInsight streamInsight = new StreamInsight(10, 100, 10, 1);
            //for (int i=1;i<=10; i++)
            //streamInsight.App(1);
            //no values/ minute, no events in the flow, no road segments, window size
            double result = 0;
            for (int i = 0; i < 100000; i++)
            {
                result += GeneratePoisson(1 / 45f);
            }
            Console.WriteLine(result);
            Console.ReadLine();

        }
        public static double GeneratePoisson(float rate)
            {
                Random rnd = new System.Random();
                float res = ((float)rnd.Next(100) / 101.0f);

                var a = -Math.Log(1.0f - res) / rate;

                return a;
            }

    }
}


