using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameField : MonoBehaviour {
    public int rows = 4;
    public int columns = 4;
    public GameObject cellsPrefab;

    private List<Cell> cells = new();

    public void Start()
    {
        CreateCell();
    }

    public Vector2Int GetEmptyPosition() {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                bool occupied = false;
                foreach (var cell in cells) {
                    if (cell.Position == pos) {
                        occupied = true;
                        break;
                    }
                }
                if (!occupied) {
                    emptyPositions.Add(pos);
                }
            }
        }

        if (emptyPositions.Count > 0) {
            int index = Random.Range(0, emptyPositions.Count);
            return emptyPositions[index];
        }

        return new Vector2Int(-1, -1);
    }

    public void CreateCell() {
        Vector2Int pos = GetEmptyPosition();
        if (pos.x == -1) {
            Debug.Log("Нет свободных позиций!");
            return;
        }

        int value = Random.Range(0f, 1f) < 0.9f ? 2 : 4;
        Cell newCell = new Cell(pos, value);
        cells.Add(newCell);

        GameObject cellViewGO = Instantiate(cellsPrefab, transform);
        CellView cellView = cellViewGO.GetComponent<CellView>();
        if (cellView != null) {
            cellView.Init(newCell);
        }
    }
}
