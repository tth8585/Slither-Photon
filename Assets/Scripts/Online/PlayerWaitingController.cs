using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class PlayerWaitingController : MonoBehaviour
{
    [Header("UI References")]
    public Text PlayerNameText;

    public Image PlayerColorImage;
    public Button PlayerReadyButton;
    public Image PlayerReadyImage;

    private int ownerId;
    private bool isPlayerReady;

    public void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }
    public void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }
    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            PlayerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            Hashtable initialProps = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_READY, isPlayerReady }, { GameData.PLAYER_LIVES, GameData.PLAYER_MAX_LIVES } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);
            PlayerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);
                Hashtable props = new Hashtable() { { GameData.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<GameManagerUIOnline>().LocalPlayerPropertiesUpdated();
                }
            });
        }
    }
    private void OnPlayerNumberingChanged()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == ownerId)
            {
                PlayerColorImage.color = GameData.GetColor(p.GetPlayerNumber());
            }
        }
    }
    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }
    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
        PlayerReadyImage.gameObject.SetActive(playerReady);
        //PlayerReadyButton.gameObject.SetActive(playerReady);
    }
}
