using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("MusicSave"))
        {
            LoadAudio();
        }

        else
        {
            SetMusic();
            SetSFX();
        }
    }
    public void SetMusic()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicSave", volume);
    }

    public void SetSFX()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("Sounds", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SoundSave", volume);
    }

    private void LoadAudio()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicSave");
        sfxSlider.value = PlayerPrefs.GetFloat("SoundSave");

        SetMusic();
        SetSFX();
    }
}
