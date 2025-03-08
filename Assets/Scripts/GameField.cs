using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameField : MonoBehaviour {
    public int rows = 4;
    public int columns = 4;
    public GameObject cellsPrefab;

    
    [NonSerialized] public Cell[,] cellGrid;

    private void Awake()
    {
        cellGrid = new Cell[rows,columns];
    }

    public Vector2Int GetEmptyPosition() {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < rows; x++) {
            for (int y = 0; y < columns; y++) {
                if (cellGrid[x,y] == null) {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (emptyPositions.Count > 0) {
            int index = Random.Range(0, emptyPositions.Count);
            return emptyPositions[index];
        }

        return new Vector2Int(-1, -1);
    }

    public void ChangeCellPosition(Vector2Int oldPosition, Vector2Int newPosition)
    {
        cellGrid[newPosition.x, newPosition.y] = cellGrid[oldPosition.x,oldPosition.y];
        cellGrid[oldPosition.x, oldPosition.y] = null;
    }

    public void RemoveCell(Cell cell)
    {
        cellGrid[cell.Position.x, cell.Position.y] = null;
        Destroy(cell.CellView.gameObject);
    }
    public void CreateInRandomPosition()
    {
        Vector2Int pos = GetEmptyPosition();
        if (pos.x == -1)
        {
            Debug.Log("Нет свободных позиций!");
            return;
        }

        int value = Random.Range(0f, 1f) < 0.8f ? 2 : 4;
        var newCell = new Cell(pos, value);
        CreateCell(newCell, pos);
    }

    public void CreateCell(Cell cell, Vector2Int position)
    {
        cellGrid[position.x, position.y] = cell;
        GameObject cellViewGO;
        if (cell.CellView == null) cellViewGO = Instantiate(cellsPrefab, transform);
        else cellViewGO = cell.CellView.gameObject;
        CellView cellView = cellViewGO.GetComponent<CellView>();
        if (cellView != null)
        {
            cellView.Init(cell);
            cell.OnPositionChanged += ChangeCellPosition;
        }
    }
    private void OnDrawGizmos()
    {
        if (cellGrid == null)
            return;

        // Задаем цвет для отрисовки заполненных клеток
        Gizmos.color = Color.green;

        // Перебираем все позиции в поле
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                // Если клетка занята (не равна null)
                if (cellGrid[x, y] != null)
                {
                    // Вычисляем мировую позицию клетки с учетом позиции игрового объекта
                    Vector3 cellPosition = transform.position + new Vector3(x*70, y *70, 0);
                    // Рисуем куб, представляющий заполненную клетку (размер можно подогнать, например, чуть меньше единицы)
                    Gizmos.DrawCube(cellPosition, Vector3.one * 60);
                }
            }
        }
    }
}
