using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Counting : MonoBehaviour
{

    public static Counting Instance;
    public TextMeshProUGUI counterText; // Reference to the TMP Text for live counter
    public TextMeshProUGUI savedText;  // Reference to the TMP Text for saved best time
    private float counter = 0f;
    private bool isCounting = false;
    private float bestTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Initialize texts
        counterText.text = "00:00";

        if (PlayerPrefs.HasKey("BestTime"))
        {
            savedText.text = PlayerPrefs.GetString("BestTime");
            bestTime = PlayerPrefs.GetFloat("BestTimeNumber");

        }

        else
        {
            savedText.text = "Best: 00:00";
        }

        isCounting = true;
    }

    void Update()
    {
        if (isCounting)
        {
            // Increment counter based on time
            counter += Time.deltaTime;
            counterText.text = FormatTime(counter);
        }
    }

    // Format time into MM:SS
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopCounter()
    {
        isCounting = false;
    }
    // can was here 
}
