using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour {
    public TextMeshProUGUI valueText;
    
    public Image cellImage;

    private Cell cell;
    
    private bool startMoving;
    private Vector2Int BaseDuration;
    private Vector2 endPosition;
    
    public void Init(Cell initCell)
    {
        StartCoroutine(Spawn());
        cell = initCell;
        cell.CellView = this;
        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;
        UpdateValue(0);
        UpdatePosition(Vector2Int.left, cell.Position);
    }

    public void UpdateValue(int _) {
        if (valueText) {
            valueText.text = cell.CellDescription.number.ToString();
            valueText.color = cell.CellDescription.textColor;
        }
        if (cellImage)
            cellImage.color = cell.CellDescription.color;
    }

    public void UpdatePosition(Vector2Int startPosition, Vector2Int newPosition) {
        Debug.Log(newPosition);
        float cellSize = 166.2f;
        endPosition = new Vector2(newPosition.x * cellSize, newPosition.y * cellSize);
        BaseDuration = newPosition;
        if (startPosition.x == -1)
        {
            transform.localPosition = endPosition;
        }
        else if (!startMoving)
        {
            startMoving = true;
            StartCoroutine(Move(startPosition));
        }
    }

    private IEnumerator Move(Vector2 startPosition)
    {
        float elapsed = 0;
        float duration = 0.1f;

        while (elapsed <= duration)
        {
            Vector2 newPosition = Vector2.Lerp(startPosition * 166.2f, endPosition, elapsed / duration);
            transform.localPosition = newPosition;
            elapsed += Time.deltaTime;
            duration = Mathf.Abs(BaseDuration.x - startPosition.x + BaseDuration.y - startPosition.y) / 20;
            yield return null;
        }
        transform.localPosition = endPosition;

        startMoving = false;
    }

    private IEnumerator Spawn()
    {
        float elapsed = 0;
        float duration = 0.15f;
        while (elapsed <= duration)
        {
            Vector3 newScale = Vector2.Lerp(Vector3.zero, Vector3.one, elapsed / duration);
            transform.localScale = newScale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
    
    private void OnDestroy() {
        if (cell != null) {
            cell.OnValueChanged -= UpdateValue;
            cell.OnPositionChanged -= UpdatePosition;
        }
    }
}