using Microsoft.ComplexEventProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exxx
{
    class RetrieveDiagnostics
    {
       public  void FileWrite(DiagnosticView diagview, int dimFer)
        {
            using (StreamWriter sw = new StreamWriter("logs.txt", append: true))
            {

                // Display diagnostics for diagnostic view object  
                //sw.WriteLine("Diagnostic View for '" + diagview.ObjectName + "':");
                DB dB = new DB();
                foreach (KeyValuePair<string, object> diagnostics in diagview)
                {
                    if(diagnostics.Key.Contains("Cpu") || diagnostics.Key.Contains("QueryTotalConsumedEventLatency") || diagnostics.Key.Contains("AllEventsMemory"))
                        //sw.WriteLine(" " + diagnostics.Key + ": " + diagnostics.Value);
                        dB.WriteToDB(dimFer,diagnostics.Key.ToString(), diagnostics.Value.ToString());
                }
                //sw.WriteLine("--------------------------------------------");
            }
        }
    }
}
