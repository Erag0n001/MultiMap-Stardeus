using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

// ReSharper disable ConvertToAutoProperty
// ReSharper disable InconsistentNaming

namespace MultiMap.Misc.Native;

public unsafe struct NativeList<T> where T : unmanaged
{
    public T* Buffer;
    private int _length;
    private int _capacity;
    
    public int Count => _length;
    public int Capacity => _capacity;
    
    public NativeList(int initialCapacity = 4)
    {
        if (initialCapacity < 0) throw new ArgumentOutOfRangeException(nameof(initialCapacity));

        _capacity = initialCapacity;
        _length = 0;
        
        Buffer = (T*)Marshal.AllocHGlobal(_capacity * sizeof(T));
    }
    
    public NativeList(){}
    
    public void Add(T item)
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        if (_length >= _capacity)
            Grow();

        Buffer[_length++] = item;
    }

    public bool Remove(T item)
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        for (int i = 0; i < _length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(Buffer[i], item))
            {
                for (int j = i; j < _length - 1; j++)
                {
                    Buffer[j] = Buffer[j + 1];
                }

                _length--;
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        _length = 0;
    }

    public void AddRange(IEnumerable<T> collection)
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    public void AddRange(Span<T> collection)
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        if (Capacity < Count + collection.Length)
        {
            Grow();
        }
        T* dest = Buffer + _length;
        fixed (T* src = collection)
            System.Buffer.MemoryCopy(src, dest, collection.Length * sizeof(T), (long)collection.Length * sizeof(T));
        _length += collection.Length;
    }

    public void EnsureCapacity(int newCapacity)
    {
        if (newCapacity > Capacity)
        {
            Grow();
        }
    }

    /// <summary>
    /// Increases length, and grows if necessary
    /// </summary>
    /// <param name="amount">Amount to "add"</param>
    public void SimulateAdd(int amount)
    {
        _length += amount;
        if (_length >= _capacity)
        {
            Grow();
        }
    }
    
    private void Grow()
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        int newCapacity = _capacity > 0 ? _capacity * 2 : 4;
        Console.WriteLine($"Growing buffer from {_capacity}, {_length} to {newCapacity} in {Thread.CurrentThread.ManagedThreadId}");
        T* newBuffer = (T*)Marshal.AllocHGlobal(Capacity * sizeof(T));

        if (_length > 0)
        {
            System.Buffer.MemoryCopy(
                source: Buffer,
                destination: newBuffer,
                destinationSizeInBytes: (long)newCapacity * sizeof(T),
                sourceBytesToCopy: (long)_length * sizeof(T));
        }

        if (Buffer != null)
        {
            Marshal.FreeHGlobal((IntPtr)Buffer);
            NativeConst.SetPointerAsDead(ref Buffer);
        }

        Buffer = newBuffer;
        _capacity = newCapacity;
    }
    
    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            NativeConst.EnsurePointerIsNotDead(Buffer);
            if ((uint)index >= (uint)_length)
                throw new IndexOutOfRangeException();
            return ref Buffer[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetAtUnsafe(int index)
    {
        #if DEBUG
        NativeConst.EnsurePointerIsNotDead(Buffer);
        if ((uint)index >= (uint)_length)
            throw new IndexOutOfRangeException();
        #endif
        return ref Buffer [index];
    }
    
    public void Dispose()
    {
        if (Buffer != null)
        {
            Marshal.FreeHGlobal((IntPtr)Buffer);
            NativeConst.SetPointerAsDead(ref Buffer);
            _length = 0;
            _capacity = 0;
        }
    }

    public void CopyFromExistingBuffer(T* buffer, int length)
    {
        NativeConst.EnsurePointerIsNotDead(Buffer);
        System.Buffer.MemoryCopy(buffer, Buffer, _length * sizeof(T), length * sizeof(T));
    }
    
    public Span<T> AsSpan(int length) => new Span<T>(Buffer, length);
    public ReadOnlySpan<T> AsReadOnlySpan(int length) => new(Buffer, length);
    public Span<T> AsSpan() => new Span<T>(Buffer, _length);
    public ReadOnlySpan<T> AsReadOnlySpan() => new(Buffer, _length);
    
    public NativeArray<T> AsArray()
    {
        T* buffer = (T*)Marshal.AllocHGlobal(_length * sizeof(T));
        System.Buffer.MemoryCopy(Buffer, buffer, _length *  sizeof(T), _length *  sizeof(T));
        return new NativeArray<T>(buffer, _length);
    }
}