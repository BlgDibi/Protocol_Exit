using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;
    public AudioSource musicSource;
    public Slider volumeSlider;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.Save();
        }

        LoadVolume();

        if (SceneManager.GetActiveScene().name == "MENU")
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
        else
        {
            musicSource.Stop();
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MENU" && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void ChangeVolume()
    {
        if (volumeSlider != null)
        {
            float volume = volumeSlider.value;
            musicSource.volume = volume;
            PlayerPrefs.SetFloat("musicVolume", volume);
            PlayerPrefs.Save();
            ApplyVolume();
        }
    }

    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("musicVolume");

        if (musicSource != null)
            musicSource.volume = volume;

        if (volumeSlider != null)
            volumeSlider.value = volume;
    }

    private void ApplyVolume()
    {
        if (musicSource != null)
            musicSource.volume = PlayerPrefs.GetFloat("musicVolume");
    }

    public void AssignSlider(Slider slider)
    {
        volumeSlider = slider;
        LoadVolume();
    }
}
