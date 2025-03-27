using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RestartProcess : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public InputHandler inputHandler;


    private void Start()
    {
        GameManager.Instance.OnGameOver += ShowGameOverScreen;
        Debug.Log(GameManager.Instance.OnGameOver.GetInvocationList().Length);
        
        gameObject.SetActive(false);
    }

    private void ShowGameOverScreen()
    {
        gameObject.SetActive(true);
        Debug.Log("Game Over");
        inputHandler.Disable();
        scoreText.text = $"Score: {GameManager.Instance.score}";
    }

    public void RestartGame()
    {
        inputHandler.Enable();
        GameManager.Instance.ResetGame();
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
