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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Application.Quit();
    }

    private IEnumerator BlackScreenHandling(GameObject uiScreen)
    {
        Cursor.lockState = CursorLockMode.None;
        yield return StartCoroutine(StartBlackScreen());
        uiScreen.SetActive(true);
        yield return new WaitForSeconds(interval);
        yield return StartCoroutine(StopBlackScreen());
        
    }

    private IEnumerator StartBlackScreen()
    {
        while (blackScreen.color.a < 0.99f)
        {
            blackScreen.color  += new Color(0, 0, 0, alphaSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator StopBlackScreen()
    {
        while (blackScreen.color.a > 0f)
        {
            blackScreen.color -= new Color(0, 0, 0, alphaSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
