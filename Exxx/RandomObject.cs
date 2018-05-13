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

        /// <summary>
        /// Random observable subject. It produces an integer in regular time periods.
        /// </summary>
        /// <param name="timerPeriod">Timer period (in milliseconds)</param>
        public RandomObject(int timerPeriod)
        {
            deList = Citire();
            _done = false;
            _observers = new List<IObserver<T>>();
            _random = new Random();
            _sync = new object();
            _timer = new Timer(EmitRandomValue);  //define Timer and delegate
            _timerPeriod = timerPeriod;
            count = 0;
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
        public void OnNext(int value)
        {
            lock (_sync)
            {
                if (!_done)
                {
                    foreach (var observer in _observers)
                    {
                        T obj = deList.ElementAt(value);
                        observer.OnNext(obj);
                        count++;
                        if (count == 10)
                            _done = true;
                        if (typeof(T) == typeof(DataEvent))
                        {
                            FileWrite(obj);
                        }
                        else
                        {
                            Console.WriteLine(obj);
                        }
                        //Console.WriteLine(count);
                    }
                }
            }
        }

        private void FileWrite(T obj)
        {
            DataEvent dataEvent = new DataEvent();
            var v = new List<T>
            {
                obj
            };
            List<DataEvent> de = (List<DataEvent>)v.AsEnumerable();
            SqlDataAdapter da = new SqlDataAdapter();
            dataEvent = de.ElementAt(0);
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "Data source=DESKTOP-F3CIUE8\\SQLEXPRESS;Initial Catalog=Licenta;Integrated Security=true";
               
                string saveDataEv = "INSERT into DataEvent (ActivityCode,StartTime,StatusEvent ) VALUES (@actCode,@stT,@status)";

                using ( da.InsertCommand = new SqlCommand(saveDataEv,conn))
                {

                    da.InsertCommand.Parameters.Add("@actCode", SqlDbType.Int).Value =dataEvent.ActivityCode ;
                    da.InsertCommand.Parameters.Add("@stT", SqlDbType.DateTime).Value = dataEvent.StartTime;
                    da.InsertCommand.Parameters.Add("@status", SqlDbType.VarChar,50).Value = dataEvent.Status;
                    conn.Open();
                    da.InsertCommand.ExecuteNonQuery();
                }
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
                }
            }
        }

        //emits a value and calls "schedule" again
        private void EmitRandomValue(object _)
        {
            var value = (int)(_random.NextDouble() * 10);
            //Console.WriteLine("[Observable]\t" + value);
            OnNext(value);
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
                        Console.WriteLine("observer removed");
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
