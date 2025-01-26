using System.Runtime.CompilerServices;
using UnityEngine;

public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 WithX(this Vector3 source, float xValue)
    {
        return new(xValue, source.y, source.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 WithY(this Vector3 source, float yValue)
    {
        return new(source.x, yValue, source.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 WithZ(this Vector3 source, float zValue)
    {
        return new(source.x, source.y, zValue);
    }
}