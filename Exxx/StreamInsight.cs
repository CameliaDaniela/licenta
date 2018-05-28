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
    class StreamInsight
    {
        int NoSeconds { get; set; }
        List<String> List { get; set; }
        public StreamInsight(int noSec)
        {
            NoSeconds = noSec;
            List = new List<string>();
        }
        public void App()
        {
            Console.WriteLine("Starting observable source...");
            using (var source = new RandomObject<Car>(NoSeconds))//genereaza o data la fiecare 500 milisecunde
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
                        e => PointEvent.CreateInsert(DateTime.Now, new Payload<Car> { Value = e }),
                        AdvanceTimeSettings.StrictlyIncreasingStartTime,
                        "Observable Stream");



                    //query that sums of events within 2 second tumbling windows
                    //var query = from ob in stream.TumblingWindow(TimeSpan.FromSeconds(2), HoppingWindowOutputPolicy.ClipToWindowEnd)
                    //            select new
                    //            {
                    //                sum = ob.Sum(e => e.Value.ActivityCode),
                    //                max =ob.Max(e=>e.Value.StartTime)
                    //            };
                    //List<double> list;
                    var query = from ob in stream.HoppingWindow(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2))
                                select new
                                {
                                    avreage = ob.Avg(e => e.Value.Speed),
                                    // max = ob.Max(e => e.Value.StartTime)

                                };



                    //IDisposable subscription = query.ToObservable().Subscribe(Console.WriteLine);
                    //  subscription.Dispose();
                    //**ienumerable sink
                    var enumerator = query.ToPointEnumerable().GetEnumerator();
                    while (enumerator.MoveNext())
                    {

                        if (enumerator.Current.EventKind == EventKind.Insert)
                        {
                           var s =enumerator.Current.Payload.ToString();
                            Console.WriteLine(s);
                            if(s!=null)
                                List.Add(s);


                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")));
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/PlanManager")));
                            // RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/MyStreaminsigtApp/serverApp/Query/query")), Console.Out);

                            DiagnosticSettings settings = new DiagnosticSettings(DiagnosticAspect.GenerateErrorReports, DiagnosticLevel.Always);
                            server.SetDiagnosticSettings(new Uri("cep:/Server"), settings);
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")));
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/PlanManager")));
                            RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/Query")));



                        }
                    }



                }
                Count<Car> count = new Count<Car>(50);
                Console.WriteLine("-------------" + count.NoOptimal);
                Console.WriteLine("The end");
               
                source.OnCompleted();
                
            }
            Console.WriteLine("Stopped observable source.");
            foreach (var ob in List)
            {
                char[] c = { '{', '}', ' ', '=' };
               string[] str = ob.Split(c);
                Console.WriteLine(str[0]);
                Console.WriteLine(str[1]);
                Console.WriteLine(str[2]);
            }
            Console.ReadLine();

        }
        private static void RetrieveDiagnostics(DiagnosticView diagview)
        {
            using (StreamWriter sw = new StreamWriter("logs.txt", append: true))
            {

                // Display diagnostics for diagnostic view object  
                sw.WriteLine("Diagnostic View for '" + diagview.ObjectName + "':");

                foreach (KeyValuePair<string, object> diagnostics in diagview)
                {
                    sw.WriteLine(" " + diagnostics.Key + ": " + diagnostics.Value);
                }
                sw.WriteLine("--------------------------------------------");
            }
        }
        
    }
}
