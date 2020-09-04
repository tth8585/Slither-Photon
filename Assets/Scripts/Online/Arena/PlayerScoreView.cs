using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreView : MonoBehaviourPunCallbacks
{
    public GameObject PlayerOverviewEntryPrefab;

    private Dictionary<int, GameObject> playerListEntries;

    #region UNITY

    public void Awake()
    {
        playerListEntries = new Dictionary<int, GameObject>();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            //GameObject entry = Instantiate(PlayerOverviewEntryPrefab);
            GameObject entry = Instantiate(PlayerOverviewEntryPrefab, gameObject.transform);
            entry.transform.SetParent(gameObject.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<Text>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));//GameData.GetColor(p.GetPlayerNumber());
            entry.GetComponent<Text>().text = string.Format("{0} "+"Score: {1}", p.NickName, p.GetScore());

            playerListEntries.Add(p.ActorNumber, entry);
        }
    }

    private void Start()
    {
        //foreach(Transform obj in gameObject.transform)
        //{
        //    obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
        //    obj.transform.rotation = new Quaternion(90f, 0f, 0f, 0f);
        //}
        //foreach (GameObject obj in gameObject.transform)
        //{
            
        //}
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject entry;

        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            entry.GetComponent<Text>().text = string.Format("{0} "+"Score: {1}", targetPlayer.NickName, targetPlayer.GetScore());
        }
    }

    #endregion
}
