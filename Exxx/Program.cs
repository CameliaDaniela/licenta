using System;
using System.ServiceModel;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using Microsoft.ComplexEventProcessing.ManagementService;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Exxx
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting observable source...");
            using (var source = new RandomDataEvent(500))
            {
                Console.WriteLine("Started observable source.");
                using (var server = Server.Create("Default"))
                {
                    // Create a local end point for the server embedded in this program  
                    var host = new ServiceHost(server.CreateManagementService());
                    host.AddServiceEndpoint(typeof(IManagementService), new WSHttpBinding(SecurityMode.Message), "http://8080/MyStreamInsightApp");
                    host.Open();
                    var myApp = server.CreateApplication("serverApp");
                    Console.WriteLine("convert source to stream");
                    var stream = source.ToPointStream(myApp,
                        e => PointEvent.CreateInsert(DateTime.Now, new Payload { Value = e }),
                        AdvanceTimeSettings.StrictlyIncreasingStartTime,
                        "Observable Stream");

                    //query that sums of events within 2 second tumbling windows
                    var query = from ob in stream.TumblingWindow(TimeSpan.FromSeconds(2), HoppingWindowOutputPolicy.ClipToWindowEnd)
                                select new
                                {
                                    sum = ob.Sum(e => e.Value.ActivityCode),
                                    max =ob.Max(e=>e.Value.StartTime)
                                };
                    Console.ReadLine();


                    //IDisposable sink; subscription hooks into running event stream
                    //could wrap this up in using block and not have to call Dispose explicitly
                    IDisposable subscription = query.ToObservable().Subscribe(Console.WriteLine);
                    Console.WriteLine("Started query ...");
                    Console.ReadLine();
                    Console.WriteLine("Stopping query ...");
                    subscription.Dispose();

                    //**ienumerable sink
                    var enumerator = query.ToPointEnumerable().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.EventKind == EventKind.Insert)
                            Console.WriteLine(enumerator.Current.Payload.ToString());
                    }
                }

                Console.ReadLine();
                Console.WriteLine("Stopping observable source...");
                source.OnCompleted();
            }
            Console.WriteLine("Stopped observable source.");

        }
    }
}
