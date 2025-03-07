using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour {
    public TextMeshProUGUI valueText;
    
    public Image cellImage;

    private Cell cell;
    
    public void Init(Cell initCell) {
        cell = initCell;
        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;
        UpdateValue(cell);
        UpdatePosition(cell);
    }

    public void UpdateValue(Cell updateCell) {
        if (valueText) {
            valueText.text = updateCell.CellDescription.number.ToString();
            valueText.fontSize = updateCell.CellDescription.fontSize;
        }
        if (cellImage)
            cellImage.color = updateCell.CellDescription.color;
    }

    public void UpdatePosition(Cell updateCell) {
        Debug.Log(updateCell.Position);
        float cellSize = 166.2f;
        Vector2 newPosition = new Vector2(updateCell.Position.x * cellSize, updateCell.Position.y * cellSize);
        transform.localPosition = newPosition;
    }

    private void OnDestroy() {
        if (cell != null) {
            cell.OnValueChanged -= UpdateValue;
            cell.OnPositionChanged -= UpdatePosition;
        }
    }
}