# DLL-Injection
A DLL injection example where the Controller injects a DLL hook into the TargetApplication.

######TargetApplication
A basic C++ console application used as the remote process for injecting and hooking the DLL into.

######Controller
A simple C# application that acts as the injector and controller. Once the TargetApplication is selected in the process list (automatically selects the first process), the DLL gets injected and hooked into the process. The controller can then send information to the DLL hook via Named Pipes, which signals the DLL to modify and alter the behavior of the TargetApplication while also having the capability to invoke methods within TargetApplication directly.

######Hook
The DLL hook that gets injected into TargetApplication. The hook is responsible for intercepting method routines and replacing the behavior with its own logic.
