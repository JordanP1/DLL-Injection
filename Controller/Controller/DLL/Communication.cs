using Controller.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controller.DLL
{
    class Communication
    {
        //Prepends for each named pipe.
        private const string _sendPrepend = "receive__";
        private const string _receivePrepend = "send__";
        //The pipe name.
        private readonly string _name;
        //The id to append to the end of the pipe name.
        private readonly string _id;
        //The full name of the pipe.
        private readonly string _sendName;
        private readonly string _receiveName;

        //The named pipes for communication with the DLL hook.
        private NamedPipeClientStream _sendPipe;
        private NamedPipeServerStream _receivePipe;

        //Timeout delay until reattempting to connect
        //the send pipe.
        private TimeSpan _sendTimeout;
        private DateTime _lastSendTimeout;

        //The receiving pipe's buffer size to store incoming data.
        private int _receiveBufferSize = 1 << 11; //2048

        //A loop to keep communication flowing.
        private Loop _communicationLoop;

        //Delegates for the communication events.
        public delegate void ReceiveConnectedDelegate();
        public delegate void SendConnectedDelegate();
        public delegate void ReceivedDelegate(byte[] data);

        //Communication-related events to subscribe to.
        public event ReceiveConnectedDelegate ReceiveConnected;
        public event SendConnectedDelegate SendConnected;
        public event ReceivedDelegate Received;

        public Communication(string name, string id)
        {
            this._name = name;
            this._id = "__" + id;
            //Full send name.
            this._sendName = _sendPrepend + this._name + this._id;
            //Full receive name.
            this._receiveName = _receivePrepend + this._name + this._id;
            //Default timeout of 1000 milliseconds (1 second).
            this._sendTimeout = TimeSpan.FromMilliseconds(1000);
            this._communicationLoop = new Loop();
            //Subscribe to loop events.
            this._communicationLoop.Started += _communicationLoop_Started;
            this._communicationLoop.Tick += _communicationLoop_Tick;
            this._communicationLoop.Stopped += _communicationLoop_Stopped;
        }

        public Communication(string name, Process process) :
            this(name, process.Id.ToString())
        {
        }

        //Start communications with the DLL.
        public void Start()
        {
            this._communicationLoop.Start();
        }

        //Stop communications with the DLL.
        public void Stop()
        {
            this._communicationLoop.Stop();
        }

        //Send data that has already been converted into bytes to the DLL.
        public void Send(byte[] data)
        {
            //Make sure the pipe has been connected before sending.
            if (this._sendPipe != null && this._sendPipe.IsConnected)
            {
                try
                {
                    this._sendPipe.Write(data, 0, data.Length);
                }
                catch { } //Pipe was disconnected or there was a sending error.
            }
        }

        //Send a generic type (usually a packet) to the DLL.
        public void Send<T>(T packet) where T : struct
        {
            //Make sure the pipe has been connected before sending.
            if (this._sendPipe != null && this._sendPipe.IsConnected)
            {
                //Convert the struct into bytes for sending.
                byte[] bytes = ProcessMemory.GetBytes<T>(packet);
                this.Send(bytes);
            }
        }

        private void _communicationLoop_Started()
        {
            //Pipe for sending information to the DLL.
            //Uses default localhost server since we don't need to utilize networking.
            this._sendPipe = new NamedPipeClientStream(".", this._sendName, PipeDirection.Out);

            //Loop and continue to attempt connecting until we are successful or
            //we end communications. (IE: application closing)
            //This will prevent the thread from being blocked if we need to close
            //the application before a successful connection.
            while (!this._communicationLoop.IsStopping && !this._sendPipe.IsConnected)
            {
                //Add a timeout, since the connect function throttles the CPU
                //if left to run continuously, even if the connect function
                //itself is given a high timeout value.
                if (DateTime.UtcNow >= this._lastSendTimeout.Add(this._sendTimeout))
                {
                    try
                    {
                        this._sendPipe.Connect(1);
                    }
                    catch { } //Failed to connect, allow loop to retry after timeout.

                    this._lastSendTimeout = DateTime.UtcNow;
                }

                Thread.Sleep(1);
            }

            //If the send pipe was connected successfully,
            //invoke the SendConnected event.
            if (this._sendPipe.IsConnected)
            {
                this.SendConnected?.Invoke();
            }

            //Pipe for receiving information from the DLL.
            this._receivePipe = new NamedPipeServerStream(
                this._receiveName,
                PipeDirection.In,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous,
                this._receiveBufferSize,
                0);

            //Allow asynchronous waiting so we don't block the thread from exiting in the
            //event we need to close the application before a successful connection.
            IAsyncResult result = this._receivePipe.BeginWaitForConnection(null, null);

            //Wait for pipe to connect, or until we end communications
            //Ex: application closing.
            while (!this._communicationLoop.IsStopping && !result.IsCompleted)
            {
                Thread.Sleep(1);
            }

            if (!this._communicationLoop.IsStopping)
            {
                //If communications are still running, end the waiting
                //as the connecting process has finished.
                //Otherwise, the pipe will close itself during the stop event.
                //Calling the EndWaitForConnection before a successful connection
                //will end up locking the thread.
                this._receivePipe.EndWaitForConnection(result);

                //If the receive pipe was successfully connected,
                //invoke the ReceiveConnected event.
                if (this._receivePipe.IsConnected)
                {
                    this.ReceiveConnected?.Invoke();
                }
            }
        }

        private void _communicationLoop_Tick()
        {
            //The receive buffer for when the DLL sends us information.
            byte[] buffer = new byte[this._receiveBufferSize];
            int numRead = this._receivePipe.Read(buffer, 0, this._receiveBufferSize);

            //If the number of bytes read was 0, we were disconnected
            //from the DLL.
            if (numRead == 0)
            {
                this.Stop();
                return;
            }

            //Invoke the Received event, and send over the information we
            //received from the DLL.
            if (this.Received != null)
            {
                byte[] data = new byte[numRead];
                Array.Copy(buffer, data, numRead);
                this.Received(data);
            }
        }

        private void _communicationLoop_Stopped()
        {
            //Only disconnect if we were already connected,
            //otherwise we will receive an exception.
            if (this._receivePipe.IsConnected)
            {
                this._receivePipe.Disconnect();
            }

            //Close and dispose the pipes.

            this._receivePipe.Close();
            this._receivePipe.Dispose();
            this._receivePipe = null;

            this._sendPipe.Close();
            this._sendPipe.Dispose();
            this._sendPipe = null;
        }
    }
}
