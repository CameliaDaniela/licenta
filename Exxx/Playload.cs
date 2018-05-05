using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exxx
{
   
    public class Payload
    {
        public DataEvent Value { get; set; }

        public override string ToString()
        {
            return "[StreamInsight]\tValue: " + Value.ToString();
        }
    }
}
