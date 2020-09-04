using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviourPunCallbacks
{
    public void OnOnlineBtnClick()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");

        if (playerName.Equals(""))
        {
            playerName = "Trump";
            Debug.Log("Player Name are default " + playerName);
        }

        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }
    public void OnOfflineBtnClick()
    {
        SceneManager.LoadScene("SceneOffline");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to master");
        SceneManager.LoadScene("SceneOnline");
    }
    public void OnQuitApp()
    {
        Application.Quit();
    }
}
