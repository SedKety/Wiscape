using UnityEngine;

public class SoundTriggerScript : MonoBehaviour
{
    public static SoundTriggerScript Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetSound(string audioName)
    {
        if (transform.Find(audioName) == null) return;
        transform.Find(audioName).GetComponent<AudioSource>().Play();
    }
}
