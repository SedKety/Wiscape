using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenLogic : MonoBehaviour
{

    public static UIScreenLogic Instance;
    [SerializeField] private Image blackScreen;
    [SerializeField] private float alphaSpeed;
    [SerializeField] private float interval;

    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject winScreen;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void LoseGame()
    {
        StartCoroutine(BlackScreenHandling(loseScreen));
    }

    public void WinGame()
    {
        
        StartCoroutine(BlackScreenHandling(winScreen));
    }
    public void ResetGame()
    {
        Time.timeScale = 1;
        StartCoroutine(BlackScreenScenes("Main Scene"));
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(BlackScreenScenes("Menu"));
    }

    private IEnumerator BlackScreenScenes(string scene)
    {
        yield return StartCoroutine(BlackScreenManager.StartBlackScreen(blackScreen, alphaSpeed));
        SceneManager.LoadScene(scene);
    }

    private IEnumerator BlackScreenHandling(GameObject uiScreen)
    {
        Cursor.lockState = CursorLockMode.None;
        yield return StartCoroutine(BlackScreenManager.StartBlackScreen(blackScreen, alphaSpeed));
        
        uiScreen.SetActive(true);
        yield return new WaitForSeconds(interval);
        yield return StartCoroutine(BlackScreenManager.StopBlackScreen(blackScreen, alphaSpeed));
        Time.timeScale = 0;
        
    }
}
