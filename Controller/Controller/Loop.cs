using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Controller
{
    class Loop
    {
        //Default tick interval.
        private static readonly TimeSpan _defaultInterval = TimeSpan.FromMilliseconds(100);

        //Run loop on a separate thread to prevent blocking.
        private Thread _loopThread;
        //Interval to trigger the tick event.
        private TimeSpan _interval;
        //The last time the tick was triggered.
        private DateTime _lastTick;
        //Keeps the loop running.
        private bool _isRunning;
        //Prevent thread-race scenarios in multi-threading.
        private object _sync;

        //Delegates for the events.
        public delegate void StartedDelegate();
        public delegate void StoppedDelegate();
        public delegate void TickDelegate();

        //Triggers when the loop is started.
        public event StartedDelegate Started;
        //Triggers when the loop has stopped.
        public event StoppedDelegate Stopped;
        //Triggers each time the interval has passed.
        public event TickDelegate Tick;

        public Loop(TimeSpan interval)
        {
            this._interval = interval;
            this._sync = new object();
        }

        public Loop() :
            this(_defaultInterval)
        {
        }

        //Reports if the loop is currently running.
        public bool IsRunning
        {
            get
            {
                return this._isRunning ||
                  (this._loopThread != null && this._loopThread.IsAlive);
            }
        }

        //If true, it means that Stop() was called but the thread
        //has not yet exited.
        public bool IsStopping
        {
            get
            {
                return !this._isRunning &&
                  (this._loopThread != null && this._loopThread.IsAlive);
            }
        }

        //Start the loop if not already running.
        public void Start()
        {
            lock (this._sync)
            {
                if (!this.IsRunning)
                {
                    this._isRunning = true;
                    this._loopThread = new Thread(this.MainLoop);
                    this._loopThread.Start();
                }
            }
        }

        //Stop the loop.
        public void Stop()
        {
            this._isRunning = false;
        }

        private void MainLoop()
        {
            //Loop has started; call the Started event.
            this.Started?.Invoke();

            //Continue looping until stopped.
            while (this._isRunning)
            {
                if (DateTime.UtcNow >= this._lastTick.Add(this._interval))
                {
                    //Invoke the Tick event since the interval has elapsed.
                    this.Tick?.Invoke();
                    this._lastTick = DateTime.UtcNow;
                }

                //Give the CPU a break to prevent throttling.
                Thread.Sleep(1);
            }

            //Loop has finished, call the Stopped event.
            this.Stopped?.Invoke();
        }
    }
}
