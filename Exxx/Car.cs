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
        public string Name { get; set; }
        int Id { get; set; }
        public void CreateRandomCar()
        {
            Random random = new Random();
            int length = random.Next(5, 20);
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var name=new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            var speed = random.Next(10,120);
            Name = name;
            Speed = speed;
        }
        public override string ToString()
        {
            return "Name: " + Name + " Speed: " + Speed+"\n";
        }
    }
}
