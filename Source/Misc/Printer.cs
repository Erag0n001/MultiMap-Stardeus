using KL.Utils;

namespace MultiMap.Misc;

public static class Printer
{
    public const string Prefix = "[MT]> ";

    // ReSharper disable once ChangeFieldTypeToSystemThreadingLock
    private static readonly object PrintLock = new object();

    public static void Warn(object toLog)
    {
        lock (PrintLock)
        {
            D.Warn(Prefix + (toLog?.ToString() ?? "null"));
        }
    }

    public static void Error(object toLog)
    {
        lock (PrintLock)
        {
            D.Err(Prefix + (toLog?.ToString() ?? "null"));
        }
    }
}