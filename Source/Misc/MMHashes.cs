using KL.Utils;
using MultiMap.Comps;

namespace MultiMap.Misc;

public static class MMHashes
{
    public static readonly int UnbreakableCompHash = GetCompHash<UnbreakableComp>();
    public static readonly int SurfaceCompHash = GetCompHash<SurfaceComp>();

    private static int GetCompHash<T>()
    {
        return Hashes.S(typeof(T).Name.Replace("Comp", ""));
    }
}