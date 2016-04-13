#pragma once

//Function pointers to call and hook for TargetApplication.

//Used for TargetApplication's printf() function.
//Prints a string to the console window.
typedef void(__cdecl *printStringFunc)(const char* string);

//Used for TargetApplication's IncreaseIndex() function.
//Increases the index by the amount parameter.
typedef void(_cdecl *increaseIndexFunc)(int amount);