using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuObject;
    [SerializeField] private GameObject deadObject;

    public bool isLocked;
    public bool isPaused;

    private void Awake()
    {
        isLocked = false;
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isLocked)
        {
            if (menuObject.activeSelf) DisableMenu();
            else EnableMenu();
        }
    }

    public void EnableMenu()
    {
        isPaused = true;
        menuObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void DisableMenu()
    {
        isPaused = false;
        menuObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
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

    public void PlayerDied()
    {
        isLocked = true;
        deadObject.SetActive(true);
        EnableMenu();
    }
}
