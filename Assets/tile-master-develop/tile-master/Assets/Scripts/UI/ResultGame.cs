using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultGame : MonoBehaviour
{
    [SerializeField] GameObject popupWin;
    [SerializeField] GameObject popupLose;

    public void WinGame()
    {
        gameObject.SetActive(true);
        popupWin.SetActive(true);
        popupLose.SetActive(false);
    }

    public void LoseGame()
    {
        gameObject.SetActive(true);
        popupLose.SetActive(true);
        popupWin.SetActive(false);
    }

    public void ActionNextLevel()
    {
        int nextGameLevel = BoardManager.instance.GameLevel + 1;
        BoardManager.instance.GameLevel = nextGameLevel;
        if (nextGameLevel > 2)
        {
            nextGameLevel = 1;
            BoardManager.instance.GameLevel = 1;
        }

        BoardManager.instance.StartNewGame(nextGameLevel);
        gameObject.SetActive(false);
    }

    public void ActionReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
