using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefinitiveScript;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool previouslyLockedCursor;

    public void Resume()
    {
        if(GameManager.Instance.LocalPlayer != null) GameManager.Instance.LocalPlayer.playerOff = false;

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        if(previouslyLockedCursor) GameManager.Instance.CursorController.LockCursor();
        previouslyLockedCursor = false;
    }

    public void Pause()
    {
        if(GameManager.Instance.LocalPlayer != null) GameManager.Instance.LocalPlayer.playerOff = true;
        previouslyLockedCursor = GameManager.Instance.CursorController.LockedCursor();

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.CursorController.UnlockCursor();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SceneController.BackToMenu();
    }
}
