using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;

    [Header("Audio")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text soundText;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayButton()
    {
        LevelLoader.instance?.LoadNextLevel();
    }

    public void OptionsButton()
    {
        if (!optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(true);
            mainMenu.SetActive(false);
        }
        else
        {
            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void MusicButton()
    {
        mixer.GetFloat("MusicVolume", out float volume);
        if (volume == 0f)
        {
            mixer.SetFloat("MusicVolume", -80f);
            musicText.text = "MUSIC: OFF";
        }
        else if (volume == -80f)
        {
            mixer.SetFloat("MusicVolume", 0f);
            musicText.text = "MUSIC: ON";
        }
    }

    public void SoundButton()
    {
        mixer.GetFloat("SfxVolume", out float volume);
        if (volume == 0f)
        {
            mixer.SetFloat("SfxVolume", -80f);
            soundText.text = "SOUND: OFF";
        }
        else if (volume == -80f)
        {
            mixer.SetFloat("SfxVolume", 0f);
            soundText.text = "SOUND: ON";
        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
