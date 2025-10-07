using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class BlackScreenManager
{
    public static IEnumerator StartBlackScreen(Image blackScreen, float alphaSpeed)
    {
        while (blackScreen.color.a < 0.99f)
        {
            blackScreen.color += new Color(0, 0, 0, alphaSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public static IEnumerator StopBlackScreen(Image blackScreen, float alphaSpeed)
    {
        while (blackScreen.color.a > 0f)
        {
            blackScreen.color -= new Color(0, 0, 0, alphaSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
