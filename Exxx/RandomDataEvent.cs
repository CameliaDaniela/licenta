using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Exxx
{

    sealed class RandomDataEvent : IObservable<DataEvent>, IDisposable
    {
        private bool _done;
        private readonly List<IObserver<DataEvent>> _observers;
        private List<DataEvent> deList;
        private readonly Random _random;
        private readonly object _sync;
        private readonly Timer _timer;
        private readonly int _timerPeriod;
        private int count;

        /// <summary>
        /// Random observable subject. It produces an integer in regular time periods.
        /// </summary>
        /// <param name="timerPeriod">Timer period (in milliseconds)</param>
        public RandomDataEvent(int timerPeriod)
        {
            deList = Citire();
            _done = false;
            _observers = new List<IObserver<DataEvent>>();
            _random = new Random();
            _sync = new object();
            _timer = new Timer(EmitRandomValue);  //define Timer and delegate
            _timerPeriod = timerPeriod;
            count = 0;
            Schedule(); //call function that resets timer
        }

        //called to register an observer with the observable
        //StreamInsight calls this itself and receives events from the observable
        public IDisposable Subscribe(IObserver<DataEvent> observer)
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
                        DataEvent obj = deList.ElementAt(value);
                        observer.OnNext(obj);
                        count++;
                        if (count == 10)
                            _done = true;
                        //Console.WriteLine(obj);
                        Console.WriteLine(count);
                    }
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
            private readonly RandomDataEvent _subject;
            private IObserver<DataEvent> _observer;

            public Subscription(RandomDataEvent subject, IObserver<DataEvent> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            public void Dispose()
            {
                IObserver<DataEvent> observer = _observer;
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
        List<DataEvent> Citire()
        {
            var lines = System.IO.File.ReadAllLines(@"C:\Users\camel\Desktop\ex\ex\data.txt");
            List<DataEvent> list = new List<DataEvent>();
            foreach (var line in lines)
            {
                DataEvent de = new DataEvent();
                var props = line.Split(',');
                de.StartTime = DateTime.Parse(props[0]);
                de.ActivityCode = int.Parse(props[1]);
                de.Status = props[2];
                list.Add(de);
            }

            return list;
        }
    }
}
