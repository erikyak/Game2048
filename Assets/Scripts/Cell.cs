using System;
using System.Linq;
using UnityEngine;

public class Cell {
    public Vector2Int Position { get; private set; }
    public CellNumber CellDescription { get; private set; }
    
    
    public event Action<Cell> OnValueChanged;
    public event Action<Cell> OnPositionChanged;

    public Cell(Vector2Int startPosition, int initialValue) {
        Position = startPosition;
        CellDescription = CellNumber.cellNumbers.First(cell => cell.number == initialValue);
    }

    public void SetValue(int newValue) {
        if (CellDescription.number != newValue)
        {
            CellDescription = CellNumber.cellNumbers.First(cell => cell.number == newValue);
            OnValueChanged?.Invoke(this);
        }
    }

    // Изменение позиции клетки с вызовом события
    public void SetPosition(Vector2Int newPosition) {
        if (Position != newPosition) {
            Position = newPosition;
            OnPositionChanged?.Invoke(this);
        }
    }
}