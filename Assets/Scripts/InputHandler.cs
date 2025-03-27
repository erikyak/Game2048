using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Game2048Input game2048Input;

    private Vector2 pointerStartPosition;

    private void Awake()
    {
        game2048Input = new Game2048Input();
        game2048Input.Play.KeyboardMove.performed += ctx => ProcessSwipe(ctx.ReadValue<Vector2>());
        
        game2048Input.Play.Click.performed += _ => SwipeStart();
        game2048Input.Play.Click.canceled += _ => SwipeEnd();
    }
    private void SwipeStart()
    {
        pointerStartPosition = Pointer.current.position.ReadValue();
    }
    
    private void SwipeEnd()
    {
        Vector2 direction = Pointer.current.position.ReadValue() - pointerStartPosition;
        ProcessSwipe(direction);
    }
    
    public void ProcessSwipe(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                Debug.Log("Движение вправо");
                GameManager.Instance.ProcessMove(Vector2Int.right);
            }
            else
            {
                Debug.Log("Движение влево");
                GameManager.Instance.ProcessMove(Vector2Int.left);
            }
        }
        else
        {
            if (direction.y > 0)
            {
                Debug.Log("Движение вверх");
                GameManager.Instance.ProcessMove(Vector2Int.up);
            }
            else
            {
                Debug.Log("Движение вниз");
                GameManager.Instance.ProcessMove(Vector2Int.down);
            }
        }
    }

    private void OnEnable()
    {
        Enable();
    }

    public void Enable()
    {
        game2048Input.Enable();
    }
    private void OnDisable()
    {
        Disable();
    }

    public void Disable()
    {
        game2048Input.Disable();
    }
    

}
