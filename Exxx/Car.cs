using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exxx
{
    public class Car
    {
        public int Speed { get; set; }
        public string RoadSegment { get; set; }
        int Id { get; set; }
        public void CreateRandomCar(int noSegments)
        {
            Random random = new Random();
            int rs = random.Next(1, noSegments);
            //const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            //var name=new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            RoadSegment = rs.ToString();
            var speed = random.Next(10, 120);
            Speed = speed;
        }
        public override string ToString()
        {
            return "RoadSegment: " + RoadSegment + " Speed: " + Speed+"\n";
        }
    }
}
