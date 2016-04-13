#include <iostream>
#include <Windows.h>

int index = -1;
bool pressed = false;

void increaseIndex(int amount);

int main()
{
	printf("Press enter to increase the index.\n");

	while (true)
	{
		//Increment the index by 1.
		increaseIndex(1);

		while (true)
		{
			//Only process key if window has focus.
			if (GetForegroundWindow() == GetConsoleWindow())
			{
				bool state = (GetKeyState(0x0D) & 0x8000) > 0;
				if (state) //If enter is being pressed.
				{
					//If last input wasn't enter, to prevent
					//multiple calls to increase.
					if (!pressed)
					{
						pressed = true;
						break;
					}
				}
				else //If enter is not being pressed.
				{
					//If last input was enter.
					if (pressed)
					{
						pressed = false;
					}
				}
			}

			//Let CPU rest to prevent CPU throttling.
			Sleep(1);
		}
	}
	return 0;
}

//Increase index by the specified amount and print the new index.
void increaseIndex(int amount)
{
	index += amount;
	printf("Index: %d\n", index);
}