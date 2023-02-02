using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TransformUtil : MonoBehaviour
{
    public readonly static float2 innerBorder = new float2
    {
        x = 3.45f,
        y = 4.65f,
    };

    static public float3 RotateVector(float3 vector, float rad)
    {
        return new float3
        {
            x = vector.x * math.cos(rad) - vector.y * math.sin(rad),
            y = vector.x * math.sin(rad) + vector.y * math.cos(rad),
        };
    }

    public static bool IsInBarrier(float3 pos)
    {
        float x = math.abs(pos.x);
        float y = math.abs(pos.y);

        return x < innerBorder.x && y < innerBorder.y;
    }
}
