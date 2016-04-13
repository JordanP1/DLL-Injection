using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Packets
{
    //The packet id, used to determine packets from one another when
    //they arrive as raw bytes.
    //Will always be the first 2 bytes in the raw byte array chunk,
    //since the id is the first listed value in every packet struct.
    public enum PacketType : ushort
    {
        Init = 0,
        Printf = 1,
        IncreaseIndex = 2,
        SetIncrease = 3,
        Terminate = 65534
    }

    //LayoutKind.Sequential is used to ensure that the data layout
    //allocates variables in the exact order as listed.

    //Packet header; included as the first element in each packet.
    [StructLayout(LayoutKind.Sequential)]
    struct Packet
    {
        public PacketType Id;
    }

    //Packets that get sent to the DLL.
    namespace Send
    {
        [StructLayout(LayoutKind.Sequential)]
        struct Init
        {
            public Packet Header;
            public IntPtr Printf;
            public IntPtr IncreaseIndex;

            public Init(
                IntPtr printf,
                IntPtr increaseIndex)
                : this()
            {
                this.Header.Id = PacketType.Init;
                this.Printf = printf;
                this.IncreaseIndex = increaseIndex;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Printf
        {
            public Packet Header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] Message;

            public Printf(
                string message)
                : this()
            {
                this.Header.Id = PacketType.Printf;
                this.Message = new byte[512];
                byte[] msg = Encoding.UTF8.GetBytes(message);
                Array.Copy(msg, this.Message, msg.Length);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SetIncrease
        {
            public Packet Header;
            public int Increase;

            public SetIncrease(
                int increase)
                : this()
            {
                this.Header.Id = PacketType.SetIncrease;
                this.Increase = increase;
            }
        }
    }

    //Packets that are received from the DLL.
    namespace Receive
    {

    }
}
