#pragma once

namespace pkt
{
	//The packet id, used to determine packets from one another when
	//they arrive as raw bytes.
	//Will always be the first 2 bytes in the raw byte array chunk,
	//since the id is the first listed value in every packet struct.
	enum class PacketType : unsigned short
	{
		Init = 0,
		Printf = 1,
		IncreaseIndex = 2,
		SetIncrease = 3,
		Terminate = 65534
	};

	//Packet header; inherited by every packet.
	struct Packet
	{
		PacketType Id;
	};

	//Packets that are received from the Controller.
	namespace r
	{
		struct Init : Packet
		{
			int Printf;
			int IncreaseIndex;
		};

		struct Printf : Packet
		{
			char Message[512];
		};

		struct SetIncrease : Packet
		{
			int Increase;
		};
	}

	//Packets that get sent to the Controller.
	namespace s
	{
	}
}