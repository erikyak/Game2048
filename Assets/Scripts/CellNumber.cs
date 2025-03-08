using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Cell", menuName = "Cell", order = 1)]
public class CellNumber : ScriptableObject
{
    public static HashSet<CellNumber> cellNumbers = new();
    public int number;
    public Color color;
    public Color textColor;
}
