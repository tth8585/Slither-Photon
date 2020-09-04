using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerUIOnline : MonoBehaviourPunCallbacks
{
    [Header("Selection Mode Panel")]
    public GameObject SelectionModePanel;
    [Header("Selection Panel")]
    public GameObject SelectionPanel;
    [Header("Room List Panel")]
    public GameObject RoomListPanel;
    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomPanel;
    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;
    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public InputField RoomNameInputField;
    public InputField MaxPlayersInputField;

    public Button StartGameButton;
    public GameObject PlayerWaitingEntryPrefab;
    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    #region Unity
    private void Awake()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");

        if (playerName.Equals(""))
        {
            playerName = "Trump";
            Debug.Log("Player Name are default " + playerName);
        }

        PhotonNetwork.LocalPlayer.NickName = playerName;
    }
    #endregion

    #region PUN callback
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }
    public override void OnJoinedRoom()
    {
        //tuy` game design

        SetActivePanel(InsideRoomPanel.name);

        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(PlayerWaitingEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerWaitingController>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;

            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerWaitingController>().SetPlayerReady((bool)isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable
        {
            {GameData.PLAYER_LOADED_LEVEL, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("left room");
        SetActivePanel(SelectionPanel.name);

        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("joint random room falied, create new room");

        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions { MaxPlayers = 8, PublishUserId = true };
        //RoomOptions options = new RoomOptions { MaxPlayers = 8 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("joint room falied");
        SetActivePanel(SelectionPanel.name);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(PlayerWaitingEntryPrefab);
        entry.transform.SetParent(InsideRoomPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerWaitingController>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        playerListEntries.Add(newPlayer.ActorNumber, entry);
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerWaitingController>().SetPlayerReady((bool)isPlayerReady);
            }
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("create room failed");
        SetActivePanel(SelectionPanel.name);
    }
    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();

        ClearRoomListView();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("disconect from master");
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region UI button click
    public void OnCreateRoomButtonClicked()
    {
        string roomName = RoomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        byte maxPlayers;
        byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers ,PublishUserId = true};

        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    public void OnJoinRandomRoomButtonClicked()
    {
        SetActivePanel(JoinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }
    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(RoomListPanel.name);
    }
    public void OnBackButtonLobbyClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(SelectionPanel.name);
    }
    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("SceneArea");
    }
    public void OnLogOutButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }
    public void OnGlobalBtnClick()
    {

    }
    public void OnArenaBtnClick()
    {

    }
    #endregion

    #region other
    public void SetActivePanel(string activePanel)
    {
        SelectionModePanel.SetActive(activePanel.Equals(SelectionModePanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
        RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
        InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    }
    public void LocalPlayerPropertiesUpdated()
    {
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }
    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }
    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomController>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, entry);
        }
    }
    #endregion
}
