using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exxx
{
    class Count<T>
    {
        public int NoOptimal { get; set; }
        List<T> list=new List<T>();
        public Count(double media)
        {
           NoOptimal= MediumAvreage(media);
            Afisare(list);
        }
        double AvreageList(List<T> list)
        {
            if (typeof(T) == typeof(Car))
            {
                List<Car> carList = (List<Car>)list.AsEnumerable();
                Double avg = carList.Average(x => x.Speed);
                return avg;
            }
            return 0;

        }
       
        List<T> DBRead(int noValues)
        {
            DB dbRead = new DB();
            if (typeof(T) == typeof(DataEvent))
            {
                DataEvent dataEvent = new DataEvent();
                List<DataEvent> de = dbRead.ReadFromDB(noValues);
                //foreach (var ob in de)
                //{
                //    Console.Write(ob);
                //}
                //Console.WriteLine(de.Count);
                return new List<T>((IEnumerable<T>)de);

            }
            if (typeof(T) == typeof(Car))
            {
                Car dataEvent = new Car();
                List<Car> car = dbRead.ReadCarsDB(noValues);
                //foreach (var ob in de)
                //{
                //    Console.Write(ob);
                //}
                //Console.WriteLine(de.Count);
                return new List<T>((IEnumerable<T>)car);
            }
            else return null;


        }
        int MediumAvreage(double media)
        {
            int noValues = 1;
            double min = double.MaxValue,avg=0;
           // list = DBRead(noValues);
            
            while(Math.Abs(avg-media)<min)
            {
                min = Math.Abs(avg - media);
                noValues++;
                list = DBRead(noValues);
                avg = AvreageList(list);

            }
            return noValues-1;

        }
        void Afisare(List<T> list)
        {
            foreach (var ob in list)
            {
                Console.Write(ob);
            }
            Console.WriteLine(list.Count);

        }

    }
}
