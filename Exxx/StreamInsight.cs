using System;
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
        int WinSize { get; set; }
       
        int NoSegments { get; set; }
        DB DB = new DB();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="noValues"> number of values per second</param>
        /// <param name="countM"> maxim number of events in the data flow </param>
        /// <param name="noSegments">number of road segments</param>
        public StreamInsight(int noValues, int countM, int noSegments, int winSize)
        {
            CountMax = countM;
            NoValues = noValues;
            NoSeconds = FromSec/noValues;
            NoSegments = noSegments;
            WinSize = winSize;
        
          
        }
        public StreamInsight(int noValues, int countM)
        {
            CountMax = countM;
            NoValues = noValues;
            NoSeconds = FromSec / noValues;
            NoSegments = 10;
           
        }
        /// <summary>
        /// analyze data object event flow
        /// </summary>
        public void App(int TimeStamp)
        {
            Console.WriteLine("Starting observable source...");
            using (var source = new RandomObject<Car>(NoSeconds,NoValues,CountMax,NoSegments,TimeStamp))//genereaza o data la fiecare 500 milisecunde
            {
                Console.WriteLine("Started observable source.");
                using (var server = Server.Create("Default"))
                {

                    var host = new ServiceHost(server.CreateManagementService());
                    host.AddServiceEndpoint(typeof(IManagementService), new WSHttpBinding(SecurityMode.Message), "http://localhost/MyStreamInsightApp");
                    host.Open(); 
                    var myApp = server.CreateApplication("serverApp");
                    Console.WriteLine("convert source to stream");
                    var stream = source.ToPointStream(myApp,
                        e => PointEvent.CreateInsert(DateTime.Now, new Payload<Car> { Value = e }),
                        AdvanceTimeSettings.StrictlyIncreasingStartTime,
                        "Observable Stream");

                    //Console.ReadLine()
;
                    //query that sums of events within 2 second tumbling windows
                    var thumblingResult = from ob in stream.TumblingWindow(TimeSpan.FromSeconds(2), HoppingWindowOutputPolicy.ClipToWindowEnd)
                                 select new
                                 {
                                     avreageT= ob.Avg(e => e.Value.Speed),

                                 };
                    //List<double> list;
                    var query = from ob in stream
                                where ob.Value.Speed < 50
                                select ob.Value;
                    //var query = from cars in stream.SnapshotWindow()
                    //                     select new {
                    //                         avreage = cars.Avg(e => e.Value.Speed),
                    //                         groupId=1
                    //                    };
                    //var query = from ob in stream.HoppingWindow(TimeSpan.FromSeconds(WinSize), TimeSpan.FromSeconds(1))
                    //            select new
                    //            {
                    //                avreage = ob.Avg(e => e.Value.Speed),
                    //                groupId = 1
                    //            };
                    //var query = from rs in stream
                    //            group rs by rs.Value.RoadSegment into roadSeg
                    //            from ob in roadSeg.HoppingWindow(TimeSpan.FromSeconds(WinSize), TimeSpan.FromSeconds(1))
                    //            select new
                    //            {
                    //                avreage = ob.Avg(e => e.Value.Speed),
                    //                groupId = roadSeg.Key
                    //            };
                    //var query = from rs in stream
                    //            group rs by rs.Value.RoadSegment into roadSeg
                    //            from ob in roadSeg.SnapshotWindow()
                    //            select new
                    //            {
                    //                avreage = ob.Avg(e => e.Value.Speed),
                    //                groupId = roadSeg.Key
                    //            };



                    var enumerator = query.ToPointEnumerable().GetEnumerator();

                    while (enumerator.MoveNext())
                    {

                        if (enumerator.Current.EventKind == EventKind.Insert)
                        {
                           var s =enumerator.Current.Payload.ToString();
                            //if (s.Length > 0)
                            //    DB.Write(s);
                            //Console.WriteLine(s);
                            DB.Write(TimeStamp,s);

                            RetrieveDiagnostics rD = new RetrieveDiagnostics();

                            DiagnosticSettings settings = new DiagnosticSettings(DiagnosticAspect.GenerateErrorReports, DiagnosticLevel.Always);
                            server.SetDiagnosticSettings(new Uri("cep:/Server"), settings);
                            rD.FileWrite(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")),WinSize);
                            rD.FileWrite(server.GetDiagnosticView(new Uri("cep:/Server/Query")),WinSize);

                        }
                    }

                    host.Close();
                   


                }
                //Count<Car> count = new Count<Car>(50);
                //Console.WriteLine("-------------" + count.NoOptimal);
                Console.WriteLine("The end");
               
                source.OnCompleted();
                
            }
            Console.WriteLine("Stopped observable source.");
            //Console.ReadLine();

        }
       
        
    }
}
