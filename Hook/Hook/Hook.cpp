#include "Hook.h"
#include "Packet.h"

//Called from the DllMain entry point when attached to a process.
void attach(HMODULE hModule)
{
	//Store the hModule so we can detach the DLL later when needed.
	m_hModule = hModule;
	//Run the DLL on a separate thread via the main() function.
	HANDLE h = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)main, 0, 0, NULL);
	//Don't need to manage thread handle, close it right away without consequence
	CloseHandle(h);
}

//Ran on a separate thread from attach().
void main()
{
	initialize();
	communicate();
	detach();
}

//Initialize variables and pipe names for communication.
void initialize()
{
	//Get the process Id to utilize for the unique pipe name.
	int pId = GetCurrentProcessId();
	//Initialize the pipe names with the unique process id.
	m_receivePipeName = m_pipeNamePrepend + L"receive__targetapplication__" + std::to_wstring(pId);
	m_sendPipeName = m_pipeNamePrepend + L"send__targetapplication__" + std::to_wstring(pId);
}

//Initialize pipes and begin communication.
void communicate()
{
	while (m_running)
	{
		//Create the receiving pipe to receive information from the Controller.
		createReceivePipe();

		//Wait for the Controller to connect to the receive pipe.
		//This method will block until a connection is made.
		if (ConnectNamedPipe(m_receivePipe, NULL))
		{
			//Create the sending pipe to send information to the Controller.
			createSendPipe();

			//A buffer used to store information received from the Controller.
			char* buffer = 0;
			DWORD numRead;

			//Wait for information to be sent from the Controller.
			//This method will block until information is sent.
			//If ReadFile returns false, the connection between the
			//Controller was closed.
			while (ReadFile(
				m_receivePipe,
				(buffer = new char[m_receiveBufferSize]),
				m_receiveBufferSize,
				&numRead,
				NULL) && numRead > 0)
			{
				//Pass the buffer and number of bytes contained
				//in the buffer to a handler to process them.
				handleReceivePacket(buffer, numRead);
				//Clear and delete the buffer.
				//Unusual behavior occurs if the buffer is not deleted
				//and reinitialized.
				delete[] buffer;
			}

			//Delete the buffer if the ReadFile returned false mid-way.
			if (buffer) { delete[] buffer; }

			//Close the sending pipe since the connection was closed off.
			closeSendPipe();
		}

		//Close the receiving pipe since the connection was closed off.
		closeReceivePipe();
	}
}

//Detach the DLL from the process.
void detach()
{
	//Frees the DLL from the process and exits the running thread.
	//Effectively detaches the DLL from the process, freeing memory.
	FreeLibraryAndExitThread(m_hModule, 0);
}

//Hook methods in TargetApplication to reroute and intercept their calls.
//This allows us to effectively replace the method with our own,
//or modify parameters before triggering the original method.
void hook()
{
	//Set up the detour transactions.
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());

	//Methods to replace with our own code.
	DetourAttach(&(PVOID&)m_increaseIndex, increaseIndexOverride);

	//Commit our changes.
	DetourTransactionCommit();
}

//Unhook methods that we previously hooked within TargetApplication,
//restoring them back to normal.
void unhook()
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());

	//Methods to restore back to normal.
	DetourDetach(&(PVOID&)m_increaseIndex, increaseIndexOverride);

	//Commit our changes.
	DetourTransactionCommit();
}

//Creates the receive pipe for receiving information from the Controller.
void createReceivePipe()
{
	//Keep trying to create until successful.
	while (!m_receivePipe || m_receivePipe == INVALID_HANDLE_VALUE)
	{
		m_receivePipe = CreateNamedPipe(
			m_receivePipeName.c_str(),
			PIPE_ACCESS_INBOUND | FILE_FLAG_FIRST_PIPE_INSTANCE,
			PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_WAIT | PIPE_REJECT_REMOTE_CLIENTS,
			1,
			0,
			m_receiveBufferSize,
			0,
			NULL);
	}
}

//Creates the send pipe for sending information to the Controller.
void createSendPipe()
{
	//Keep trying to create until successful.
	while (!m_sendPipe || m_sendPipe == INVALID_HANDLE_VALUE)
	{
		m_sendPipe = CreateFile(
			m_sendPipeName.c_str(),
			GENERIC_WRITE,
			0,
			NULL,
			OPEN_EXISTING,
			0,
			NULL);
	}
}

//Close the receive pipe.
void closeReceivePipe()
{
	if (m_receivePipe)
	{
		//Disconnect the pipe if it was connected to the Controller.
		DisconnectNamedPipe(m_receivePipe);
		//Close the pipe.
		CloseHandle(m_receivePipe);
		//Set the handle back to null.
		m_receivePipe = 0;
	}
}

//Close the send pipe.
void closeSendPipe()
{
	if (m_sendPipe)
	{
		//Close the pipe.
		CloseHandle(m_sendPipe);
		//Set the handle back to null.
		m_sendPipe = 0;
	}
}

//Receive and process incoming information from the Controller.
void handleReceivePacket(const char* buffer, int size)
{
	//Verify if packet size is valid.
	if (size >= sizeof(pkt::Packet))
	{
		//Get the packet id from the first two bytes in the buffer.
		pkt::PacketType id = (pkt::PacketType)(buffer[1] << 8 | buffer[0]);

		//Handle the packet based on the id.
		switch (id)
		{
		//Initialization of method addresses and hooking.
		case pkt::PacketType::Init:
			//Only initialize once.
			if (!m_initialized)
			{
				m_initialized = true;
				//Initialize packet.
				pkt::r::Init init;
				//Copy the data in the buffer to the packet.
				memcpy(&init, buffer, size);

				//Initialize the methods with the memory addresses.
				m_printStrng = (printStringFunc)init.Printf;
				m_increaseIndex = (increaseIndexFunc)init.IncreaseIndex;

				//Hook methods.
				hook();

				//Print out to TargetApplication that the hooking was successful.
				printString("Hook successfully attached.\n");
			}
			break;
		//Prints the sent message to the TargetApplication's console.
		case pkt::PacketType::Printf:
			//Printf/PrintString packet.
			pkt::r::Printf print;
			//Copy the data in the buffer to the packet.
			memcpy(&print, buffer, size);

			//Print the message to the TargetApplication's console.
			printString(print.Message);
			break;
		case pkt::PacketType::IncreaseIndex:
			//Call the IncreaseIndex() method via a function pointer, which
			//will trigger the method within TargetApplication.
			//Pass our increase amount variable, which can be changed from
			//the Controller.
			increaseIndex(m_increaseAmount);
			break;
		case pkt::PacketType::SetIncrease:
			//Increase Index packet.
			pkt::r::SetIncrease setInc;
			//Copy the data in the buffer to the packet.
			memcpy(&setInc, buffer, size);

			//Set the increase amount variable from the value in the packet,
			//which will be used when IncreaseIndex() is called.
			m_increaseAmount = setInc.Increase;
			break;
		//Close pipe connections, unhook methods, and unhook the DLL.
		case pkt::PacketType::Terminate:
			m_running = false;
			//Close the receive pipe.
			closeReceivePipe();
			//Close the send pipe.
			closeSendPipe();
			//Unhook methods that were hooked.
			unhook();
			//Print out to TargetApplication that the unhooking was successful.
			printString("Hook successfully detached.\n");
			break;
		default:
			break;
		}
	}
}

//PrintString method wrapper to ensure m_printStrng has been
//initialized before calling.
void printString(const char* string)
{
	if (m_printStrng)
	{
		//Call the printString method, triggering it within TargetApplication.
		m_printStrng(string);
	}
}

//IncreaseIndex method wrapper to ensure m_increaseIndex has been
//initialized before calling.
void increaseIndex(int amount)
{
	if (m_increaseIndex)
	{
		//Call the increaseIndex method, triggering it within TargetApplication.
		m_increaseIndex(amount);
	}
}

//Replace the original function with ours.
void increaseIndexOverride(int amount)
{
	//Increase from our increase amount rather than from the parameter.
	m_increaseIndex(m_increaseAmount);
}