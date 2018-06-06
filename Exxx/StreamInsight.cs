﻿using System;
using System.ServiceModel;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.ManagementService;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.ComplexEventProcessing.Linq;
using System.Linq;

namespace Exxx
{
    class StreamInsight
    {
        const int FromSec = 1000;
        int NoSeconds { get; set; }
        //number of values /second
        int NoValues { get; set; }
        //max number of events in the flow
        int CountMax { get; set; }
        List<String> List { get; set; }
        int NoSegments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="noValues"> number of values per second</param>
        /// <param name="countM"> maxim number of events in the data flow </param>
        /// <param name="noSegments">number of car segments</param>
        public StreamInsight(int noValues, int countM, int noSegments)
        {
            CountMax = countM;
            NoValues = noValues;
            NoSeconds = FromSec/noValues;
            NoSegments = noSegments;
            List = new List<string>();
        }
        public StreamInsight(int noValues, int countM)
        {
            CountMax = countM;
            NoValues = noValues;
            NoSeconds = FromSec / noValues;
            NoSegments = 10;
            List = new List<string>();
        }
        /// <summary>
        /// analyze data object event flow
        /// </summary>
        public void App()
        {
            Console.WriteLine("Starting observable source...");
            using (var source = new RandomObject<Car>(NoSeconds,NoValues,CountMax,NoSegments))//genereaza o data la fiecare 500 milisecunde
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
                            RetrieveDiagnostics rD = new RetrieveDiagnostics();

                            rD.FileWrite(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")));
                            rD.FileWrite(server.GetDiagnosticView(new Uri("cep:/Server/PlanManager")));
                            // RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/MyStreaminsigtApp/serverApp/Query/query")), Console.Out);

                            DiagnosticSettings settings = new DiagnosticSettings(DiagnosticAspect.GenerateErrorReports, DiagnosticLevel.Always);
                            server.SetDiagnosticSettings(new Uri("cep:/Server"), settings);
                            rD.FileWrite(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")));
                            rD.FileWrite(server.GetDiagnosticView(new Uri("cep:/Server/PlanManager")));
                            rD.FileWrite(server.GetDiagnosticView(new Uri("cep:/Server/Query")));



                        }
                    }



                }
                Count<Car> count = new Count<Car>(50);
                Console.WriteLine("-------------" + count.NoOptimal);
                Console.WriteLine("The end");
               
                source.OnCompleted();
                
            }
            Console.WriteLine("Stopped observable source.");
            Console.ReadLine();

        }
       
        
    }
}
