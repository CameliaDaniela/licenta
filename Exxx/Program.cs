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
using System.IO;

namespace Exxx
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting observable source...");
            using (var source = new RandomDataEvent(500))//genereaza o data la fiecare 500 milisecunde
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
                    var query = from ob in stream.HoppingWindow(TimeSpan.FromHours(2), TimeSpan.FromSeconds(2))
                                select new
                                {
                                    sum = ob.Sum(e => e.Value.ActivityCode),
                                    max =ob.Max(e=>e.Value.StartTime)
                                };
                    //Console.ReadLine();


                    //IDisposable sink; subscription hooks into running event stream
                    //could wrap this up in using block and not have to call Dispose explicitly
                    
                    IDisposable subscription = query.ToObservable().Subscribe(Console.WriteLine);
                   
                    subscription.Dispose();
                   
                    //**ienumerable sink
                    var enumerator = query.ToPointEnumerable().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.EventKind == EventKind.Insert)
                        {
                            Console.WriteLine(enumerator.Current.Payload.ToString());
                           

                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")));
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/PlanManager")));
                            // RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/MyStreaminsigtApp/serverApp/Query/query")), Console.Out);

                            DiagnosticSettings settings = new DiagnosticSettings(DiagnosticAspect.GenerateErrorReports, DiagnosticLevel.Always);
                            server.SetDiagnosticSettings(new Uri("cep:/Server"), settings);
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")));
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/PlanManager")));
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/Query")));

                            //Console.WriteLine("Summary Query Diagnostics");
                            //RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/MyStreamInsightApp/TrafficJoinSample/Query/TrafficSensorQuery")), Console.Out);
                            //Console.WriteLine("Operator Diagnostics");
                            //RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/MyStreamInsightApp/TrafficJoinSample/Query/TrafficSensorQuery/Operator/sensorInput")), Console.Out);
                            
                        }
                    }


                }

                Console.ReadLine();
                Console.WriteLine("Stopping observable source...");
                source.OnCompleted();
            }
            Console.WriteLine("Stopped observable source.");

        }
        private static void RetrieveDiagnostics(DiagnosticView diagview )
        {
            using (StreamWriter sw = new StreamWriter("logs.txt", append: true))
            {

                // Display diagnostics for diagnostic view object  
                sw.WriteLine("Diagnostic View for '" + diagview.ObjectName + "':");
                foreach (KeyValuePair<string, object> diagprop in diagview)
                {
                    sw.WriteLine(" " + diagprop.Key + ": " + diagprop.Value);
                }
                sw.WriteLine("--------------------------------------------");
            }
        }
    }
}
