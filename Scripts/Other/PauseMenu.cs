using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuObject;

    public bool isPaused;

    private void Awake()
    {
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuObject.activeSelf)
            {
                isPaused = false;
                menuObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
            }
            else
            {
                isPaused = true;
                menuObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
        }
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;
        LevelLoader.instance?.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitButton()
    {
        Time.timeScale = 1f;
        LevelLoader.instance?.LoadLevel(0);
    }
}
