using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Memory
{
    public class SignatureScan
    {
        //Process to scan.
        private Process process;
        //Module to limit scan region to.
        private ProcessModule targetModule;
        //Whether or not the module's memory has been stored yet.
        private bool moduleDumped = false;
        //The Module's memory buffer.
        private byte[] dumpBuffer;

        public SignatureScan(Process process, ProcessModule targetModule)
        {
            this.process = process;
            this.targetModule = targetModule;
        }

        #region Pinvoke
        [DllImport("kernel32")]
        static extern bool ReadProcessMemory(IntPtr handle, IntPtr address, byte[] buffer, uint size, int bytesRead);
        #endregion

        //Store the entire module's memory into the buffer for faster scanning.
        public void DumpMemory()
        {
            dumpBuffer = new byte[targetModule.ModuleMemorySize];
            ReadProcessMemory(process.Handle, targetModule.BaseAddress, dumpBuffer, (uint)targetModule.ModuleMemorySize, 0);
            moduleDumped = true;
        }

        //Find the pattern index in the passed buffer.
        private int IndexOf(byte[] haystack, byte[] needle)
        {
            int[] lookup = new int[256];
            for (int i = 0; i < lookup.Length; i++) { lookup[i] = needle.Length; }

            for (int i = 0; i < needle.Length; i++)
            {
                lookup[needle[i]] = needle.Length - i - 1;
            }

            int index = needle.Length - 1;
            var lastByte = needle.Last();
            while (index < haystack.Length)
            {
                var checkByte = haystack[index];
                if (haystack[index] == lastByte)
                {
                    bool found = true;
                    for (int j = needle.Length - 2; j >= 0; j--)
                    {
                        if (haystack[index - needle.Length + j + 1] != needle[j])
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return index - needle.Length + 1;
                    else
                        index++;
                }
                else
                {
                    index += lookup[checkByte];
                }
            }
            return -1;
        }

        //Parse the pattern and convert it into a byte array.
        private byte[] ParseBytes(string pattern)
        {
            if (pattern.Length % 2 != 0 || pattern.Length < 2)
                return null;

            byte[] buffer = new byte[pattern.Length / 2];

            for (int i = 0; i < pattern.Length; i += 2)
            {
                buffer[i / 2] = ParseByte(pattern[i], pattern[i + 1]);
            }

            return buffer;
        }

        //Find the byte pattern.
        public IntPtr FindSignature(string pattern, int offset = 0, bool dereference = true, bool addSigLen = true)
        {
            if (pattern.Length % 2 != 0 || pattern.Length < 2)
                return IntPtr.Zero;

            if (!moduleDumped)
                DumpMemory();


            byte[] bPattern = ParseBytes(pattern);

            int index = IndexOf(dumpBuffer, bPattern);

            if (index != -1)
            {
                int len = addSigLen ? pattern.Length / 2 : 0;
                IntPtr addr = (IntPtr)(int)targetModule.BaseAddress + index + len;

                if (dereference)
                {
                    byte[] buff = new byte[4];
                    ReadProcessMemory(process.Handle, addr + offset, buff, 4, 0);
                    int x = BitConverter.ToInt32(buff, 0);
                    return (IntPtr)x;
                }

                return addr;
            }

            return IntPtr.Zero;
        }

        //Parse the byte into a hex string.
        private byte ParseByte(char c1, char c2)
        {
            string s = c1.ToString() + c2.ToString();
            return byte.Parse(s, NumberStyles.HexNumber);
        }
    }
}
