using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exxx
{
    public class DataEvent
    {
        public DataEvent()
        {

        }

        public DateTime StartTime { get; set; }
        public int ActivityCode { get; set; }
        public String Status { get; set; }

        public override string ToString()
        {
            return StartTime + " " + ActivityCode + " " + Status + "\n";
        }

        
    }
}
