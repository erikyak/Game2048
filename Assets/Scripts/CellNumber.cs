using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cell", menuName = "Cell", order = 1)]
public class CellNumber : ScriptableObject
{
    public static HashSet<CellNumber> cellNumbers = new();
    public int number;
    public Color color;
    public int fontSize = 50;

    private void OnValidate()
    {
        cellNumbers.Add(this);
    }
}
