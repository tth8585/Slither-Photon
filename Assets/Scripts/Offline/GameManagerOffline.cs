using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerOffline : MonoBehaviour
{
    public static GameManagerOffline instance;

    public GameObject playerSnake;
    public GameObject InGameCanvas;
    public GameObject InGameJoystickCanvas;
    public GameObject gameOverCanvas;

    public Text InGameLenghtText;
    public Text GameOverLenghtText;
    string inGameLTString, GameOverLTstring;
    void Awake()
    {
        inGameLTString = InGameLenghtText.text;
        GameOverLTstring = GameOverLenghtText.text;
        instance = this;
    }
    private void Start()
    {
        playerSnake.SetActive(true);
    }
    void Update()
    {
        if (Snake.player != null)
        {
            InGameLenghtText.text = inGameLTString + Snake.player.points;
            GameOverLenghtText.text = GameOverLTstring + Snake.player.points;
        }
    }

    public void OnGameOver_Event()
    {
        gameOverCanvas.SetActive(true);
        InGameJoystickCanvas.SetActive(false);
        InGameCanvas.SetActive(false);
    }
    public void OnQuitBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
