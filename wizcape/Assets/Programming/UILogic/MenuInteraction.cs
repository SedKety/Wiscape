using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuInteraction : MonoBehaviour
{

    [SerializeField] private Image blackScreen;
    [SerializeField] private float alphaSpeed;
    public void PlayGame()
    {
        StartCoroutine(HandlePlayGame());
    }

    private IEnumerator HandlePlayGame()
    {
        yield return StartCoroutine(HandleBlackScreen());
        SceneManager.LoadScene("Main Scene");
    }

    


    public void QuitGame()
    {
        StartCoroutine(HandleQuitGame());

    }

    private IEnumerator HandleQuitGame()
    {
        yield return StartCoroutine(HandleBlackScreen());
        Application.Quit();
    }

    private IEnumerator HandleBlackScreen()
    {
        yield return StartCoroutine(BlackScreenManager.StartBlackScreen(blackScreen, alphaSpeed));
        yield return new WaitForSeconds(1);
        
    }
}
