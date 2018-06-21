using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exxx
{

    sealed class RandomObject<T> : IObservable<T>, IDisposable
    {
        private bool _done;
        private readonly List<IObserver<T>> _observers;
        private List<T> deList;
        private readonly Random _random;
        private readonly object _sync;
        private readonly Timer _timer;
        private readonly int _timerPeriod;
        private int count;
        private int NoValues;
        private int NoSegments;
        private int CountMax;
        private int TimeStmp;
        private int cntTS;
       

        /// <summary>
        /// Random observable subject. It produces an integer in regular time periods.
        /// </summary>
        /// <param name="timerPeriod">Timer period (in milliseconds)</param>
        public RandomObject(int timerPeriod,int noValues, int countMax, int noSegments,int TS)
        {
            deList = Citire();
            _done = false;
            _observers = new List<IObserver<T>>();
            _random = new Random();
            _sync = new object();
            _timer = new Timer(EmitRandomValue);  //define Timer and delegate
            _timerPeriod = timerPeriod;
            count = 0;
            CountMax = countMax;//number maxim of events in flow
            NoSegments = noSegments;//number of road segments
            NoValues = noValues;//number of values/second
            TimeStmp = TS;
     
            Schedule(); //call function that resets timer

        }

        //called to register an observer with the observable
        //StreamInsight calls this itself and receives events from the observable
        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_sync)
            {
                Console.WriteLine("StreamInsight calling [Subscribe]");
                _observers.Add(observer);
            }
            return new Subscription(this, observer);
        }

        //sends data to each observer (in this case, StreamInsight)
        public void OnNext(T value)
        {
            lock (_sync)
            {
                if (!_done)
                {
                    foreach (var observer in _observers)
                    {
                        //T obj = deList.ElementAt(value);
                        observer.OnNext(value);

                       
                        //if (cntTS == NoValues)
                        //{
                        //    TimeStmp++;
                        //    cntTS = 0;
                        //}
                        if (count == CountMax)
                        {
                            _done = true;
                            observer.OnCompleted();
                            //DBRead();
                            break;
                        }
  
                        DBWrite(value);
                        Console.WriteLine(value);
                        DateTime now = DateTime.UtcNow;
                        Console.WriteLine(now);
                      
                        count++;
                        cntTS++;
                        //Console.WriteLine(count);
                    }
                }
            }
        }
       
        private void DBWrite(T obj)
        {
            DB dbWrite = new DB();
            var v = new List<T>
            {
                obj
            };
            if (typeof(T) == typeof(DataEvent))
            {
                DataEvent dataEvent = new DataEvent();
                List<DataEvent> de = (List<DataEvent>)v.AsEnumerable();
                dataEvent = de.ElementAt(0);
                dbWrite.WriteToDB(dataEvent.ActivityCode, dataEvent.StartTime, dataEvent.Status);
            }else if (typeof(T) == typeof(Car))
            {
                Car car = new Car();
                List<Car> de = (List<Car>)v.AsEnumerable();
                car = de.ElementAt(0);
                dbWrite.WriteToDB(TimeStmp,car.Speed, car.RoadSegment);
            }
           
        }

        //fires on error
        public void OnError(Exception e)
        {
            Console.WriteLine("error encountered");
            lock (_sync)
            {
                foreach (var observer in _observers)
                {
                    //give error to each observer
                    observer.OnError(e);
                }
                _done = true;
            }
        }

        //runs when completed
        public void OnCompleted()
        {
            lock (_sync)
            {
                foreach (var observer in _observers)
                {
                    observer.OnCompleted();
                }
                _done = true;
            }
        }

        void IDisposable.Dispose()
        {
            Console.WriteLine("timer disposed");
            _timer.Dispose();
        }

        //function that resets the timer; when timer period reached, the delegate (EmitRandomValue) fires
        private void Schedule()
        {
            lock (_sync)
            {
                if (!_done)
                {
                    //triggers EmitRandomValue
                    _timer.Change(_timerPeriod, Timeout.Infinite);
                    //Console.WriteLine("Time----" + _timer.ToString() + "Timer period----" + _timerPeriod.ToString());
                }
            }
        }

        //emits a value and calls "schedule" again
        private void EmitRandomValue(object _)
        {
            
            //Console.WriteLine("[Observable]\t" + value);
            if(typeof(T)==typeof(Car))
            {
                Car c = new Car();
                c.CreateRandomCar(NoSegments);
                List<Car> list = new List<Car>
                {
                    c
                };
                List<T> l = new List<T>((IEnumerable<T>)list);
                OnNext(l.Last());
            }
            else
            {
                var value = (int)(_random.NextDouble() * 10);
                OnNext(deList.ElementAt(value));
            }
           
            Schedule();
        }

        //class within the other class that accesses the parent's private member variables
        //returned from subscribe command
        private sealed class Subscription : IDisposable
        {
            private readonly RandomObject<T> _subject;
            private IObserver<T> _observer;

            public Subscription(RandomObject<T> subject, IObserver<T> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            public void Dispose()
            {
                IObserver<T> observer = _observer;
                if (null != observer)
                {
                    lock (_subject._sync)
                    {
                       // Console.WriteLine("observer removed");
                        _subject._observers.Remove(observer);
                    }
                    _observer = null;
                }
            }

        }
        public List<T> Citire()
        {
            if (typeof(T) == typeof(DataEvent))
            {
                var lines = System.IO.File.ReadAllLines("data.txt");
                List<DataEvent> list = new List<DataEvent>();
                foreach (var line in lines)
                {
                    DataEvent de = new DataEvent();
                    var props = line.Split(',');
                    de.StartTime = DateTime.Parse(props[0]);
                    de.ActivityCode = Int32.Parse(props[1]);
                    de.Status = props[2];
                    list.Add(de);
                }
                List<T> l = new List<T>((IEnumerable<T>)list);
                return l;
            }
            return null;
        }
        

    }
}
