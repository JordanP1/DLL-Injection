using Controller.DLL;
using Controller.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Processes
{
    public class TargetApplication
    {
        //The TargetApplication's process.
        private Process _process;
        //Used to communicate with the DLL.
        private Communication _communication;
        //Initialization variables.
        private bool _isInitialized;
        private bool _isSignaturesInitialized;
        private bool _isHooked;
        private bool _indexMessagePrinted;

        //Signatures for finding the functions in the TargetApplication.
        private const string _printfSignature = "558BEC81ECD80000005356578DBD";
        private const string _increaseIndexSignature = "558BEC81ECC00000005356578DBD40FFFFFFB930000000B8CCCCCCCCF3ABA1";
        //Memory addresses of the functions once found.
        private IntPtr _printfAddress;
        private IntPtr _increaseIndexAddress;

        //The amount the index is increased from IncreaseIndex()
        //within TargetApplication.
        private int _increaseIndex;

        public TargetApplication(Process process)
        {
            this._process = process;
            this._communication = new Communication("targetapplication", process);
            this._communication.SendConnected += _communication_SendConnected;
            this._communication.Received += _communication_Received;
        }

        public Process Process { get { return this._process; } }

        public bool IsInitialized { get { return this._isInitialized; } }

        public bool IsHooked { get { return this._isHooked; } }

        //The amount the index is increased from IncreaseIndex()
        //within TargetApplication.
        public int IncreaseIndex
        {
            get { return this._increaseIndex; }
            set
            {
                this._increaseIndex = value;
                //Reflect the change within the DLL as well.
                this.SetIncreaseAmount(value);
            }
        }

        //The public initialize method, which initializes everything
        //necessary for DLL hooking and communication.
        public void Initialize()
        {
            if (!this._isInitialized)
            {
                this._isInitialized = true;
                this.InitializeSignatures();
                this.Hook();
                this._communication.Start();
            }
        }

        //Unhook the DLL and stop communications.
        //Allow optional unhooking, in the event the application
        //is closing; no need to unhook.
        //(Unhooking while the application is closing may cause a
        //send error due to the pipe being terminated mid-send).
        public void Deinitialize(bool unhook = true)
        {
            if (unhook)
            {
                this.Unhook();
            }

            this._communication.Stop();
            this._isInitialized = false;
        }

        //Scan for the signature pattern and store the addresses so we
        //can send them off to the DLL for invoking.
        private void InitializeSignatures()
        {
            if (!this._isSignaturesInitialized)
            {
                this._isSignaturesInitialized = true;
                SignatureScan signatureScan = new SignatureScan(this._process, this._process.MainModule);
                this._printfAddress = signatureScan.FindSignature(_printfSignature, 0, false, false);
                this._increaseIndexAddress = signatureScan.FindSignature(_increaseIndexSignature, 0, false, false);
            }
        }

        //Inject the DLL into TargetApplication, which gives the DLL full access to the
        //process' internals and allows us to hook and call methods from outside via
        //named pipes communication with the DLL.
        private void Hook()
        {
            if (!this._isHooked)
            {
                this._isHooked = true;
                InjectDll.Inject(this._process, "Hook.dll");
            }
        }

        //Initialize our DLL with the memory addresses we found from the signature scan,
        //which allows the DLL to call the methods via a function pointer.
        private void HookInitialize()
        {
            Packets.Send.Init packet = new Packets.Send.Init(
                this._printfAddress,
                this._increaseIndexAddress);

            this._communication.Send<Packets.Send.Init>(packet);
        }

        //Calls the printf() function within TargetApplication using the message
        //parameter that we pass in.
        public void PrintString(string message)
        {
            Packets.Send.Printf packet = new Packets.Send.Printf(message + Environment.NewLine);
            this._communication.Send<Packets.Send.Printf>(packet);
        }

        //Force-trigger the IncreaseIndex() function within TargetApplication
        //through our DLL.
        public void TriggerIncreaseIndex()
        {
            Packets.Packet packet = new Packets.Packet() { Id = Packets.PacketType.IncreaseIndex };
            this._communication.Send<Packets.Packet>(packet);
        }

        //Set the amount we want the index to increase each time we press enter
        //in TargetApplication, or when we force-trigger it from our controller.
        //The default within TargetApplication is +1, but with our DLL, we can
        //hook and intercept the function, replacing it with our own value.
        private void SetIncreaseAmount(int increase)
        {
            Packets.Send.SetIncrease packet = new Packets.Send.SetIncrease(increase);
            this._communication.Send<Packets.Send.SetIncrease>(packet);

            if (!this._indexMessagePrinted && increase != 1)
            {
                //Notify that the increased index increment also works from
                //directly inside the TargetApplication too.
                this._indexMessagePrinted = true;
                this.PrintString("The index increment value was changed.");
                this.PrintString("The change is also reflected here in the console when pressing enter.");
            }
        }

        //Unhook our DLL from TargetApplication, which restores it back to normal.
        private void Unhook()
        {
            Packets.Packet packet = new Packets.Packet() { Id = Packets.PacketType.Terminate };
            this._communication.Send<Packets.Packet>(packet);
            this._isHooked = false;
        }

        private void _communication_SendConnected()
        {
            //When the send pipe connects successfully with our DLL,
            //do our initialization.
            this.HookInitialize();
            //Update the increase index value once connected.
            this.SetIncreaseAmount(this.IncreaseIndex);
        }

        private void _communication_Received(byte[] data)
        {
            //Invoked when the receive pipe connects successfully with our DLL.
        }

        #region Overrides
        //Compare instances of this class via the process id.
        public override bool Equals(object obj)
        {
            TargetApplication process = obj as TargetApplication;

            if (process != null)
            {
                return process._process.Id == this._process.Id;
            }

            return false;
        }

        //Compare instances of this class' hash code via the process id.
        public override int GetHashCode()
        {
            return this._process.Id;
        }

        //Return the process.exe name and Id as the display name.
        //Useful for displaying in containers such as a ListBox.
        public override string ToString()
        {
            return string.Format("{0}.exe ({1})",
                this._process.ProcessName, this._process.Id);
        }
        #endregion
    }
}
