using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlusScore:MonoBehaviour
{
    public GameObject plusScoreUI;
    public void Start()
    {
        GameManager.Instance.OnScoreChanged += ChangeScore;
    }

    public void ChangeScore(int score)
    {
        GameObject plusScoreGO = Instantiate(plusScoreUI, transform);
        plusScoreGO.GetComponent<TextMeshProUGUI>().text = $"+{score.ToString()}";
        StartCoroutine(FlowAway(plusScoreGO));
    }

    private IEnumerator FlowAway(GameObject scoreObject)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPos = Vector3.zero;
        Vector3 targetPos = startPos + new Vector3(0, 50f, 0);
        
        TextMeshProUGUI textComp = scoreObject.GetComponent<TextMeshProUGUI>();
        Color startColor = textComp.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsedTime < duration)
        {
            scoreObject.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            textComp.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        scoreObject.transform.position = targetPos;
        textComp.color = targetColor;
#if UNITY_EDITOR
        if (!Application.isPlaying)
            DestroyImmediate(scoreObject);
        else
            Destroy(scoreObject);
#else
    Destroy(scoreObject);
#endif
    }
}
