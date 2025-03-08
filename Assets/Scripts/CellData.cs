using System;
using UnityEngine;

[Serializable]
public class CellData
{
    public int positionX;
    public int positionY;
    public int value;
    
    public CellData(Vector2 pos, int val)
    {
        positionX = (int)pos.x;
        positionY = (int)pos.y;
        value = val;
    }
}