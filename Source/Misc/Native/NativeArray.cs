using System;
using System.Runtime.InteropServices;

namespace MultiMap.Misc.Native;

public unsafe struct NativeArray<T> where T : unmanaged
{
    public T* Buffer;
    public int Length { get; private set; }

    #if DEBUG
    private static int currentID;
    private static int NextId => ++currentID;

    public int Id;
    #endif
    
    public NativeArray(int length)
    {
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

        Length = length;
        Buffer = (T*)Marshal.AllocHGlobal(length * sizeof(T));
        #if DEBUG
        Id = NextId;
        #endif
    }

    public NativeArray(T* buffer, int length)
    {
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
        Length = length;
        Buffer = buffer;
    }
    
    public ref T this[int index]
    {
        get
        {
            NativeConst.EnsurePointerIsNotDead(Buffer);
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
            return ref Buffer[index];
        }
    }

    public void SetAtUnsafe(int index, in T value)
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        #if DEBUG
            if ((uint)index >= (uint)Length)
                throw new IndexOutOfRangeException();
        #endif
        Buffer[index] = value;
    }

    public ref T GetAtUnsafe(int index)
    {
#if DEBUG
        NativeConst.EnsurePointerIsNotDead(Buffer);
        if ((uint)index >= (uint)Length)
            throw new IndexOutOfRangeException();
#endif
        return ref Buffer[index];
    }

    public void Dispose()
    {
        if (Buffer != null)
        {
            Marshal.FreeHGlobal((IntPtr)Buffer);;
            NativeConst.SetPointerAsDead(ref Buffer);
            Length = 0;
        }
    }

    public void GrowBuffer(int newSize)
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        if (newSize <= Length) return; 
        var newBuffer = (T*)(T*)Marshal.AllocHGlobal(newSize * sizeof(T));
        
        long bytesToCopy = (long)Length * sizeof(T);
        System.Buffer.MemoryCopy(Buffer, newBuffer, newSize * sizeof(T), bytesToCopy);
        Marshal.FreeHGlobal((IntPtr)Buffer);
        NativeConst.SetPointerAsDead(ref Buffer);
        Buffer = newBuffer;
        Length = newSize;
    }
    
    public Span<T> AsSpan(int length) => new Span<T>(Buffer, length);
    public ReadOnlySpan<T> AsReadOnlySpan(int length) => new(Buffer, length);
    public Span<T> AsSpan() => new Span<T>(Buffer, Length);
    public ReadOnlySpan<T> AsReadOnlySpan() => new(Buffer, Length);

    public T[] AsManagedArray()
    {
        T[] bytes = new T[Length];
        fixed (T* p = bytes)
        {
            System.Buffer.MemoryCopy(Buffer, p, Length * sizeof(T), Length * sizeof(T));
        }
        return bytes;
    }
}