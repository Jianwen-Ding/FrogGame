using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject gameMenu;

    public static bool isPaused = false;

    CursorLockMode cursorModeOnPause;

    bool cursorVisibilityOnPause;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        cursorModeOnPause = Cursor.lockState;
        cursorVisibilityOnPause = Cursor.visible;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameMenu.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Cursor.lockState = cursorModeOnPause;
        Cursor.visible = cursorVisibilityOnPause;
        gameMenu.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
