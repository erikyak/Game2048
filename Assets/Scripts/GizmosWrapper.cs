using System;
using UnityEngine;

public static class GizmosWrapper
{
    public static Action<Vector3, Vector3> DrawCubeAction = Gizmos.DrawCube;
    
    public static void DrawCube(Vector3 center, Vector3 size)
    {
        DrawCubeAction(center, size);
    }
}