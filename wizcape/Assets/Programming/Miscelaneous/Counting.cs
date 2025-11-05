using TMPro;
using UnityEngine;

public class Counting : MonoBehaviour
{
    public static Counting Instance;
    public TextMeshProUGUI counterText; // Reference to the TMP Text for live counter
    public TextMeshProUGUI savedText;   // Reference to the TMP Text for saved best time

    private float counter = 0f;
    private bool isCounting = false;
    private float bestTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        counterText.text = "00:00";

        if (PlayerPrefs.HasKey("BestTime"))
        {
            savedText.text = PlayerPrefs.GetString("BestTime");
            bestTime = PlayerPrefs.GetFloat("BestTimeNumber");
        }
        else
        {
            savedText.text = "Best: 00:00";
            bestTime = float.MaxValue; // Initialize to max so any time will be better
        }

        isCounting = true;
    }

    void Update()
    {
        if (isCounting)
        {
            counter += Time.deltaTime;
            counterText.text = FormatTime(counter);
        }
    }

    private void SaveHighscore()
    {
        if (counter < bestTime)
        {
            bestTime = counter;
            PlayerPrefs.SetFloat("BestTimeNumber", bestTime);
            PlayerPrefs.SetString("BestTime", "Best: " + FormatTime(bestTime));
            PlayerPrefs.Save();
            savedText.text = PlayerPrefs.GetString("BestTime");
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopCounter()
    {
        isCounting = false;
        SaveHighscore();
    }
}
