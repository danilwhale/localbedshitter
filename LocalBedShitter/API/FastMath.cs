using System.Runtime.CompilerServices;

namespace LocalBedShitter.API;

public static class FastMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Floor(float x) => (int)x > x ? (int)x - 1 : (int)x;
}