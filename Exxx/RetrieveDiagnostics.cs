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
       public  void FileWrite(DiagnosticView diagview)
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
