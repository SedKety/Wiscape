using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuLogic : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool _isPaused;
    public void PauseInput(InputAction.CallbackContext context)
    {
        if (context.started && !_isPaused)
        {
            PauseGame();
        }
    }
    private void PauseGame()
    {
        if (!pauseMenu) { return; }
        Cursor.lockState = CursorLockMode.None;
        _isPaused = true;
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(true);
    }

    public void ContinueGame()
    {
        if (!pauseMenu) { return; }
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        _isPaused = false;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
