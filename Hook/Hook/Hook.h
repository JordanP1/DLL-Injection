#pragma once
#include "Functions.h"
#include <Windows.h>
#include <string>
#include "detours.h"

//The DLL's HMODULE handle.
//Used for unhooking the DLL and freeing memory.
HMODULE m_hModule;

//Named pipes for communicating with Controller.
HANDLE m_receivePipe = 0;
HANDLE m_sendPipe = 0;
//The syntax that every pipe must start with.
std::wstring m_pipeNamePrepend = L"\\\\.\\pipe\\";
//Names for the receive/send pipes.
std::wstring m_receivePipeName;
std::wstring m_sendPipeName;
//The receive pipe's buffer size.
int m_receiveBufferSize = 1 << 11; //2048

bool m_running = true;
bool m_initialized = false;

//Function pointers to call/hook methods within TargetApplication.
printStringFunc m_printStrng = 0;
increaseIndexFunc m_increaseIndex = 0;

//The amount to increase the index by each
//time IncreaseIndex() is called.
int m_increaseAmount = 1;

//Stores the hModule and creates a new thread to run main().
void attach(HMODULE hModule);

//Calls initialize(), communicate(), and detach().
//communicate() will block until m_running becomes false
//and connection with the Controller is terminated.
void main();

//Initializes necessary variables such as the names
//for the pipes utilizing the unique process id.
void initialize();

//Begins communication with the Controller utilizing named pipes.
//This method will block until m_running becomes false and
//connection with the Controller is terminated.
//This is where all incoming packets and information sent from
//the Controller is handled.
void communicate();

//Detaches the DLL from the process and frees memory while
//exiting the thread.
void detach();

//Hooks the function pointers, replacing TargetApplication's
//methods with our own.
void hook();

//Unhooks the function pointers, restoring TargetApplication's
//methods back to normal.
void unhook();

//Creates the receive pipe to receive information from the Controller.
void createReceivePipe();

//Creates the send pipe to send information to the Controller.
void createSendPipe();

//Closes the receive pipe and ends receiving communication.
void closeReceivePipe();

//Closes the send pipe and ends sending communication.
void closeSendPipe();

//Handle all incoming data from the Controller.
//Allows communication with the Controller and
//TargetApplication through the Hook.
void handleReceivePacket(const char* buffer, int size);

//Calls printf() within TargetApplication, printing the
//passed string to theconsole.
void printString(const char* string);

//Calls IncreaseIndex() within TargetApplication, increasing
//the index by the passed amount.
void increaseIndex(int amount);

//Used for overriding the IncreaseIndex() method in
//TargetApplication, calling this one instead with
//our own custom code and behavior.
void increaseIndexOverride(int amount);