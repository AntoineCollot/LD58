using UnityEngine;

public class CursorMode : MonoBehaviour
{
    public CursorLockMode mode;
    public void OnGameStart()
    {
        Cursor.lockState = mode;
        Cursor.visible = false;
    }

    public void OnGameEnd()
    {

    }
}
