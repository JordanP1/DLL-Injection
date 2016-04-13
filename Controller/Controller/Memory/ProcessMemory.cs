using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Memory
{
    class ProcessMemory
    {
        #region Pinvoke
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int nSize,
            out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
             ProcessAccessFlags processAccess,
             bool bInheritHandle,
             int processId
        );

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
           IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           int dwSize, FreeType dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
           IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
           IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        #endregion

        //ReadProcessMemory wrapper to read memory with additional offsets.
        public static T ReadByte<T>(IntPtr processHandle, IntPtr address,
            params int[] offset) where T : struct
        {
            //Read through all offsets.
            for (int i = 0; i < offset.Length; i++)
            {
                byte[] buffer;
                IntPtr numRead;
                bool finalOffset = i >= offset.Length - 1 ? true : false;

                if (finalOffset) //Reached the last offset.
                {
                    //The size of the genetic type.
                    buffer = new byte[Marshal.SizeOf<T>()];
                }
                else
                {
                    //The size of a pointer.
                    buffer = new byte[IntPtr.Size];
                }

                //Read the memory from the specified address + offset
                //into the buffer.
                ReadProcessMemory(processHandle, address + offset[i],
                    buffer, buffer.Length, out numRead);

                if (finalOffset)
                {
                    //Convert the buffer bytes into generic type and return.
                    return GetStruct<T>(buffer);
                }
                else
                {
                    //Convert the buffer bytes into a readable address
                    //for the next read.
                    address = GetStruct<IntPtr>(buffer);
                }
            }

            //Return a new empty generic type if something went wrong
            //since we cannot return null structs.
            return new T();
        }

        //Overload for a single memory address read with no offset.
        public static T ReadByte<T>(IntPtr processHandle, IntPtr address) where T : struct
        {
            return ReadByte<T>(processHandle, address, 0);
        }

        //Convert/fill a struct from the array of bytes passed in.
        public static T GetStruct<T>(byte[] buffer)
        {
            //Allocate the passed in byte array to memory.
            //Pin the allocation to prevent garbage collection.
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            //Create the generic struct and fill it with the byte data
            //that we just allocated.
            T t = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());

            //Free the memory allocation.
            handle.Free();

            return t;
        }

        //Convert a generic struct into an array of bytes.
        public static byte[] GetBytes<T>(T structObj) where T : struct
        {
            //Allocate unmanaged memory.
            int size = Marshal.SizeOf(structObj);
            byte[] buffer = new byte[size];
            IntPtr allocPtr = Marshal.AllocHGlobal(size);

            //Allocate the generic struct to the unmanaged portion of
            //memory that we allocated.
            Marshal.StructureToPtr<T>(structObj, allocPtr, false);
            //Copy the raw struct bytes into the buffer.
            Marshal.Copy(allocPtr, buffer, 0, buffer.Length);

            //Free the allocated memory.
            Marshal.FreeHGlobal(allocPtr);

            return buffer;
        }
    }

    #region Pinvoke Flags
    [Flags]
    public enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }

    [Flags]
    public enum FreeType
    {
        Decommit = 0x4000,
        Release = 0x8000,
    }

    [Flags]
    public enum AllocationType
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Reset = 0x80000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        LargePages = 0x20000000
    }

    [Flags]
    public enum MemoryProtection
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        GuardModifierflag = 0x100,
        NoCacheModifierflag = 0x200,
        WriteCombineModifierflag = 0x400
    }
    #endregion
}
