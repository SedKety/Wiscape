using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Counting : MonoBehaviour
{
    public TextMeshProUGUI counterText; // Reference to the TMP Text for live counter
    public TextMeshProUGUI savedText;  // Reference to the TMP Text for saved best time
    private float counter = 0f;
    private bool isCounting = false;
    private float bestTime = float.MaxValue; // Store the lowest time

    void Start()
    {
        // Initialize texts
        counterText.text = "00:00";
        savedText.text = "Best: 00:00";
        // Start counting when the scene loads
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

    // Stop counter when entering a trigger collider
    void OnTriggerEnter(Collider other)
    {
        if (isCounting)
        {
            isCounting = false;
            // Save the count if it's lower than the current best
            if (counter < bestTime)
            {
                bestTime = counter;
                savedText.text = "Best: " + FormatTime(bestTime);
            }
            counter = 0f; // Reset counter
            counterText.text = "00:00";
        }
    }

    // Format time into MM:SS
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
