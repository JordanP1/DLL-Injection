using Controller.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.DLL
{
    class InjectDll
    {
        //The default timeout for WaitForSingleObject, which waits for the LoadLibraryA
        //method for loading the DLL into the process to finish.
        private static readonly TimeSpan _defaultTimeout = TimeSpan.FromMilliseconds(30000);

        public static InjectResult Inject(Process process, string dllPath, TimeSpan timeout)
        {
            InjectResult result = InjectResult.None;

            //Get the full path of the DLL.
            string fullPath = Path.GetFullPath(dllPath);
            //Return if the DLL is not found.
            if (!File.Exists(fullPath)) { return InjectResult.Dll_Not_Found; }

            //Process modules aren't automatically updated in a stored process variable.
            //Grab the updated process.
            Process updatedProcess = Process.GetProcessById(process.Id);

            //Sometimes randomly fails due to 64-bit OS accessing 32-bit process ProcessMemory...
            //Try again if fails.
            bool success = false;
            string pathCompare = fullPath.ToLower();
            while (!success)
            {
                try
                {
                    foreach (ProcessModule pm in updatedProcess.Modules)
                    {
                        if (pm.FileName.ToLower() == pathCompare)
                            //Return if the DLL is found to
                            //prevent injecting duplicates.
                        { return InjectResult.Dll_Already_Jnjected; }
                    }
                    success = true;
                }
                catch { }
            }

            //Open handle with all permissions to avoid any unexpected access violation errors...
            IntPtr hProcess = ProcessMemory.OpenProcess(ProcessAccessFlags.All, true, process.Id);
            //Return if the handle is 0. This is an invalid result.
            if (hProcess == IntPtr.Zero) { return InjectResult.Process_Handle_Invalid; }

            //Get the handle for kernel32.dll.
            IntPtr hKernel = ProcessMemory.GetModuleHandle("kernel32.dll");
            //Return if the handle is 0. This is an invalid result.
            if (hKernel == IntPtr.Zero) { return InjectResult.Kernel_Module_Not_Found; }

            //Get the address for LoadLibraryA, which is used to load a process
            //module into a process and calls the DllMain entry point.
            IntPtr hLoadLibraryA = ProcessMemory.GetProcAddress(hKernel, "LoadLibraryA");
            //Return if the address is 0. This is an invalid result.
            if (hLoadLibraryA == IntPtr.Zero) { return InjectResult.Load_Library_A_Not_Found; }

            //Allocation space for the full path of the module and
            //+1 for the null terminator (0x00) of the string.
            int allocationSize = fullPath.Length + 1;
            //Allocate memory space in the process and store the address
            //the allocation was made at.
            IntPtr allocatedAddr = ProcessMemory.VirtualAllocEx(hProcess, IntPtr.Zero, (IntPtr)allocationSize,
                AllocationType.Commit | AllocationType.Reserve,
                MemoryProtection.ExecuteReadWrite);
            //Return if the address is 0. Allocation was not made.
            if (allocatedAddr == IntPtr.Zero) { return InjectResult.Memory_Allocation_Failed; }

            //Convert the full path string into bytes.
            byte[] buffer = Encoding.UTF8.GetBytes(fullPath);
            IntPtr numWritten;
            //Write the bytes to the space we allocated within the process.
            if (!ProcessMemory.WriteProcessMemory(hProcess, allocatedAddr, buffer, buffer.Length, out numWritten))
            {
                //Writing to memory failed if WriteProcessMemory returned false.
                result = InjectResult.Memory_Write_Failed;

                //Free the memory we allocated into the process.
                //dwSize must be 0 to free all pages allocated by VirtualAllocEx.
                if (!ProcessMemory.VirtualFreeEx(hProcess, allocatedAddr, 0, FreeType.Release))
                {
                    //Freeing the allocated memory failed if VirtualFreeEx returned false.
                    result |= InjectResult.Memory_Release_Failed;
                }

                //Return due to failing to write the bytes to ProcessMemory.
                return result;
            }

            //Create a new remote thread, calling the LoadLibraryA function in our target
            //process with the address we allocated our string bytes at as the parameter.
            //This will load the DLL into the process using the full path to the DLL that
            //was specified and call the DLL's DllMain entry point.
            IntPtr threadId;
            IntPtr hThread = ProcessMemory.CreateRemoteThread(hProcess, IntPtr.Zero, 0, hLoadLibraryA, allocatedAddr, 0, out threadId);
            if (hThread == IntPtr.Zero)
            {
                //The remote thread failed to create if the thread handle is is 0.
                result = InjectResult.Thread_Creation_Failed;

                //Free the memory we allocated into the process.
                //dwSize must be 0 to free all pages allocated by VirtualAllocEx.
                if (!ProcessMemory.VirtualFreeEx(hProcess, allocatedAddr, 0, FreeType.Release))
                {
                    //Freeing the allocated memory failed if VirtualFreeEx returned false.
                    result |= InjectResult.Memory_Release_Failed;
                }

                //Return due to failing to create the remote thread.
                return result;
            }

            //Wait for the thread to finish, with specified timeout if it never finishes.
            ProcessMemory.WaitForSingleObject(hThread, (uint)timeout.TotalMilliseconds);

            //Free the memory we allocated into the process.
            //dwSize must be 0 to free all pages allocated by VirtualAllocEx.
            if (!ProcessMemory.VirtualFreeEx(hProcess, allocatedAddr, 0, FreeType.Release))
            {
                //Freeing the allocated memory failed if VirtualFreeEx returned false.
                result |= InjectResult.Memory_Release_Failed;
            }

            //Close the handle created by CreateRemoteThread.
            if (!ProcessMemory.CloseHandle(hThread)) { result |= InjectResult.Thread_Close_Failed; }

            //Close the handle created by OpenProcess.
            if (!ProcessMemory.CloseHandle(hProcess)) { result |= InjectResult.Process_Handle_Close_Failed; }

            return result |= InjectResult.Success;
        }

        //Overload to use the default timeout.
        public static InjectResult Inject(Process process, string dllPath)
        {
            return Inject(process, dllPath, _defaultTimeout);
        }
    }

    [Flags]
    public enum InjectResult
    {
        None =                              0,
        Success =                      1 << 0,
        Process_Handle_Invalid =       1 << 1,
        Dll_Not_Found =                1 << 2,
        Kernel_Module_Not_Found =      1 << 3,
        Load_Library_A_Not_Found =     1 << 4,
        Memory_Allocation_Failed =     1 << 5,
        Memory_Write_Failed =          1 << 6,
        Thread_Creation_Failed =       1 << 7,
        Memory_Release_Failed =        1 << 8,
        Thread_Close_Failed =          1 << 9,
        Process_Handle_Close_Failed =  1 << 10,
        Dll_Already_Jnjected =         1 << 11,
    }
}