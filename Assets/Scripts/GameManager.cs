using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public GameObject playerSnake;
    public GameObject mainMenuCanvas;
    public GameObject InGameCanvas;
    public GameObject InGameJoystickCanvas;
    public GameObject gameOverCanvas;

    public Text InGameLenghtText;
    public Text GameOverLenghtText;
    string inGameLTString, GameOverLTstring;

    // Use this for initialization
    void Awake() {
        inGameLTString = InGameLenghtText.text;
        GameOverLTstring = GameOverLenghtText.text;
        instance = this;
    }
    void Update() {
        if (Snake.player != null) {
            InGameLenghtText.text = inGameLTString +  Snake.player.points;
            GameOverLenghtText.text = GameOverLTstring + Snake.player.points;

        }
    }

    public void OnGameOver_Event() {
        //AdNetworks.instance.ShowInterstitial();
        gameOverCanvas.SetActive(true);
        InGameJoystickCanvas.SetActive(false);
        InGameCanvas.SetActive(false);
    }

    public void OnPlayButton()
    {
        InGameJoystickCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        playerSnake.SetActive(true);
        InGameCanvas.SetActive(true);

}


public void OnGameOverPlayButton() {

        Application.LoadLevel(Application.loadedLevel);
    }
}
