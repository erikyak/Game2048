using UnityEngine;

public class CellNumberLoader : MonoBehaviour
{
    public void Awake()
    {
        CellNumber.cellNumbers.Clear();
        var loadedCells = Resources.LoadAll<CellNumber>("Cells");
        foreach (var cell in loadedCells)
        {
            CellNumber.cellNumbers.Add(cell);
        }
    }
}