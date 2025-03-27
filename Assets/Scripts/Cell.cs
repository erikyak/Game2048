using System;
using System.Linq;
using UnityEngine;

public class Cell {
    public Vector2Int Position { get; private set; }
    public CellNumber CellDescription { get; private set; }

    public CellView CellView { get; set; }
    
    public event Action<int> OnValueChanged;
    public event Action<Vector2Int, Vector2Int> OnPositionChanged;

    public Cell(Vector2Int startPosition, int initialValue) {
        Position = startPosition;
        CellDescription = CellNumber.cellNumbers.FirstOrDefault(cell => cell.number == initialValue);
    }

    public void SetValue(int newValue) {
        if (CellDescription.number != newValue)
        {
            CellDescription = CellNumber.cellNumbers.FirstOrDefault(cell => cell.number == newValue);
            OnValueChanged?.Invoke(newValue);
        }
    }

    // Изменение позиции клетки с вызовом события
    public void SetPosition(Vector2Int newPosition, bool telepot) {
        if (Position != newPosition) {
            if(telepot) OnPositionChanged?.Invoke(Vector2Int.left, newPosition);
            else OnPositionChanged?.Invoke(Position, newPosition);
            Position = newPosition;
            
        }
    }
}