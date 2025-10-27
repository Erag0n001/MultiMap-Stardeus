using System;
using System.Diagnostics;
using System.Threading;

namespace MultiMap.Misc.Native;


public static unsafe class NativeConst
{
    private static readonly void* PoisonPointer = (void*)0xDEADDEADDEADDEAD;

    [Conditional("DEBUG")]
    public static unsafe void EnsurePointerIsNotDead<T>(T* pointer) where T : unmanaged
    {
        if (pointer == PoisonPointer)
        {
            throw new Exception($"Tried to access after free!{Thread.CurrentThread.ManagedThreadId}");
        }
    }

    [Conditional("DEBUG")]
    public static unsafe void SetPointerAsDead<T>(ref T* pointer) where T : unmanaged
    {
        pointer = (T*)PoisonPointer;
    }
}
