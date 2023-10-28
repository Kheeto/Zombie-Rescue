using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator transition;

    public static LevelLoader instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void LoadLevel(int index)
    {
        StartCoroutine(Transition(index));
    }

    public void LoadNextLevel()
    {
        StartCoroutine(Transition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator Transition(int index)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(index);
    }
}
