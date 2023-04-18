#define WIN32_LEAN_AND_MEAN
#include <windows.h>

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
    )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        // A new process is loading the DLL.
        break;
    case DLL_THREAD_ATTACH:
        // A new thread is creating a new instance of the DLL.
        break;
    case DLL_THREAD_DETACH:
        // A thread is exiting cleanly.
        break;
    case DLL_PROCESS_DETACH:
        // The calling process has called FreeLibrary or
        // TerminateProcess, or a thread has exited
        // cleanly while this DLL was loaded.
        break;
    }
    return TRUE;
}