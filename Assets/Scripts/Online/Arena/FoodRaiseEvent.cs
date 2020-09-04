using Photon.Pun;

using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FoodRaiseEvent : MonoBehaviourPunCallbacks
{
    public static FoodRaiseEvent Instance = null;
    [SerializeField] GameObject foodPrefab;

    public Color[] foodColorRandomList;

    //player
    public List<GameObject> listPlayer;// = new List<GameObject>();

    #region UNITY
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        listPlayer = new List<GameObject>();
    }
    private void Update()
    {
        if (listPlayer.Count != PhotonNetwork.PlayerList.Length)
        {
            GetListPlayer();
            for (int i = 0; i < listPlayer.Count; i++)
            {
                if (listPlayer[i].GetComponent<OSnakePlayer>().snakePieces.Count == 0)
                {
                    listPlayer[i].GetComponent<OSnakePlayer>().AddSnakeHead();
                }
            }
        }
    }
    #endregion

    #region NETWORK
    private void NetworkingClient_EventReceived(EventData obj)
    {
        if(obj.Code == RaiseEventData.RESPAWN_FOOD)
        {
            HandleRespawnFood(obj);
        }
        else if(obj.Code == RaiseEventData.GAME_START)
        {
            HandleGameStart(obj);
        }
        else if (obj.Code == RaiseEventData.SPAWN_FOOD_FIRST_TIME)
        {
            HandleSpawnFoodFirstTime(obj);
        }
        else if (obj.Code == RaiseEventData.PLAYER_UPDATE_SCORE)
        {
            HandleUpdateScore(obj);
        }
        else if(obj.Code == RaiseEventData.ADD_PLAYER_PART)
        {
            HandleUpdatePlayerPart(obj);
        }
        else if (obj.Code == RaiseEventData.UPDATE_PLAYER_PART_AND_SCORE)
        {
            HandleUpdatePlayerPartAndScore(obj);
        }
        else if(obj.Code == RaiseEventData.PLAYER_DIES)
        {
            HandlePlayerDie(obj);
        }
        else if(obj.Code == RaiseEventData.SPAWN_FOOD_WHEN_PLAYER_DIE)
        {
            HandleSpawnFoodFromBody(obj);
        }
    }


    #endregion

    #region handle NETWORK

    private void HandlePlayerDie(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;
        object[] valueData;

        string deadId = "";
        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            deadId = (string)valueData[0];
        }

        //PlayerDie(userId);
        SnakePlayerDie(deadId);
    }
    private void HandleUpdatePlayerPartAndScore(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;
        object[] valueData;
        int score = 0;
        int newNumberOfPart = 0;
        string userId = "";

        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            score = (int)valueData[0];
            newNumberOfPart = (int)valueData[1];
            userId = (string)valueData[2];
        }

        UpdatePlayerPart(newNumberOfPart, userId);
        UpdateScore(score, userId);
        UpdateScale(score, userId);
    }
    private void HandleUpdatePlayerPart(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;
        object[] valueData;
        int newNumberOfPart = 0;
        string userId = "";

        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            newNumberOfPart = (int)valueData[0];
            userId = (string)valueData[1];
        }
        UpdatePlayerPart(newNumberOfPart, userId);
    }

    private void HandleUpdateScore(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;

        object[] valueData;
        int score = 0;
        string userId = "";

        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            score = (int)valueData[0];
            userId = (string)valueData[1];
        }

        UpdateScore(score, userId);
        UpdateScale(score, userId);
    }
    private void HandleSpawnFoodFirstTime(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;

        List<float> listPower = new List<float>();
        List<float> listPosX = new List<float>();
        List<float> listPosY = new List<float>();
        List<float> listPosZ = new List<float>();
        List<int> listIndexFood = new List<int>();

        object[] valueData;

        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            listPower = ConvertStringToListFloat((string)valueData[0]);
            listPosX = ConvertStringToListFloat((string)valueData[1]);
            listPosY = ConvertStringToListFloat((string)valueData[2]);
            listPosZ = ConvertStringToListFloat((string)valueData[3]);
            listIndexFood = ConvertStringToListInt((string)valueData[4]);
        }

        for (int i = 0; i < listPower.Count; i++)
        {
            SpawnFood(listPower[i], new Vector3(listPosX[i], listPosY[i], listPosZ[i]), listIndexFood[i]);
        }
    }
    private void HandleSpawnFoodFromBody(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;

        List<float> listPower = new List<float>();
        List<float> listPosX = new List<float>();
        List<float> listPosY = new List<float>();
        List<float> listPosZ = new List<float>();
        List<int> listIndexFood = new List<int>();
        string userId = "";

        object[] valueData;

        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            listPower = ConvertStringToListFloat((string)valueData[0]);
            listPosX = ConvertStringToListFloat((string)valueData[1]);
            listPosY = ConvertStringToListFloat((string)valueData[2]);
            listPosZ = ConvertStringToListFloat((string)valueData[3]);
            listIndexFood = ConvertStringToListInt((string)valueData[4]);
            userId = (string)valueData[5];
        }

        if (userId == PhotonNetwork.LocalPlayer.UserId)
        {
            PhotonNetwork.Disconnect();
        }

        for (int i = 0; i < listPower.Count; i++)
        {
            SpawnFood(listPower[i], new Vector3(listPosX[i], listPosY[i], listPosZ[i]), listIndexFood[i]);
        }
    }

    private void HandleGameStart(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;

        int point;
        float spawnRange;
        float speed;
        float rotateSpeed;
        int speedMultiplier;

        object[] valueData;

        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            point = (int)valueData[0];
            spawnRange = (float)valueData[1];
            speed = (float)valueData[2];
            rotateSpeed = (float)valueData[3];
            speedMultiplier = (int)valueData[4];

            SpawnPlayer(point, spawnRange,
                        speed, rotateSpeed, speedMultiplier);
        }
    }

    private void HandleRespawnFood(EventData obj)
    {
        Dictionary<byte, object> data = obj.Parameters;

        object[] valueData;

        float power = 0;
        float posX = 0;
        float posY = 0;
        float posZ = 0;
        int foodIndex = 0;

        foreach (KeyValuePair<byte, object> kvp in data)
        {
            valueData = (object[])kvp.Value;

            power = (float)valueData[0];
            posX = (float)valueData[1];
            posY = (float)valueData[2];
            posZ = (float)valueData[3];
            foodIndex = (int)valueData[4];
            SpawnFood(power, new Vector3(posX, posY, posZ), foodIndex);
        }
    }

    #endregion

    #region CONVERT FUNC
    private List<float> ConvertStringToListFloat(string data)
    {
        var ins = new MemoryStream(Convert.FromBase64String(data)); //Create an input stream from the string
        var bf = new BinaryFormatter(); //Create a formatter 
        return (List<float>)bf.Deserialize(ins); //Read back the data
    }
    private List<int> ConvertStringToListInt(string data)
    {
        var ins = new MemoryStream(Convert.FromBase64String(data)); //Create an input stream from the string
        var bf = new BinaryFormatter(); //Create a formatter 
        return (List<int>)bf.Deserialize(ins); //Read back the data
    }
    public string ConvertListStringToString(List<string> list)
    {
        var o = new MemoryStream(); //Create something to hold the data
        var bf = new BinaryFormatter(); //Create a formatter
        bf.Serialize(o, list); //Save the list
        return Convert.ToBase64String(o.GetBuffer());
    }
    public string ConvertListFloatToString(List<float> list)
    {
        var o = new MemoryStream(); //Create something to hold the data
        var bf = new BinaryFormatter(); //Create a formatter
        bf.Serialize(o, list); //Save the list
        return Convert.ToBase64String(o.GetBuffer());
    }
    #endregion

    #region NO IDEAL

    public Vector3 RandomTransformOnCircle(float spawnRange)
    {
        Vector3 rangeVector = UnityEngine.Random.onUnitSphere * spawnRange;
        rangeVector.y = 0;
        return Vector3.zero + rangeVector;
    }
    private void UpdateScale(int score, string userId)
    {
        float scale = 0;
        int pointsForScale = 250;
        float scaleOffset = 0.05f;

        for (int i = 0; i < listPlayer.Count; i++)
        {
            if (listPlayer[i].GetComponent<OSnakePlayer>().GetPlayerId() == userId)
            {
                scale = (float)score / pointsForScale;
                scale = 1 + scale * scaleOffset;
                //referenceScale = Mathf.Lerp(referenceScale, scale, Time.deltaTime * 1);
                listPlayer[i].GetComponent<OSnakePlayer>().SetScale(scale);
            }
        }
    }

    void UpdateScore(int score, string userId)
    {
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if(PhotonNetwork.LocalPlayer.UserId == userId)
            {
                PhotonNetwork.LocalPlayer.SetScore(score);
                break;
            }
        }
    }
    private void UpdatePlayerPart(int newNumberOfPart, string userId)
    {
        for (int i=0;i< listPlayer.Count; i++)
        {
            if(listPlayer[i].GetComponent<OSnakePlayer>().GetPlayerId() == userId)
            {
                listPlayer[i].GetComponent<OSnakePlayer>().AddNewPart(newNumberOfPart);
                break;
            }
        }
    }
    private void SpawnPlayer(int point, float spawnRange,
        float speed, float rotateSpeed, int speedMulty)
    {
        Quaternion rotation = Quaternion.identity;
        rotation.z = rotation.y;
        rotation.y = 0;
        Vector3 posSpawn = RandomTransformOnCircle(spawnRange);
        GameObject obj = PhotonNetwork.Instantiate("OSnakePlayer", posSpawn, rotation, 0);
        obj.GetComponent<OSnakePlayer>().AddSnakeHead();
        obj.GetComponent<OSnakePlayer>().SetPlayerID(PhotonNetwork.LocalPlayer.UserId);
        obj.GetComponent<OSnakePlayer>().SetPlayerBios(speed, rotateSpeed, speedMulty);
        PhotonNetwork.LocalPlayer.SetScore(point);
    }

    public void SpawnFood(float power, Vector3 spawningPosition, int foodIndex)
    {
        GameObject newFood = (GameObject)Instantiate(foodPrefab, spawningPosition, foodPrefab.transform.rotation);
        OFood newFoodParameter = newFood.GetComponent<OFood>();
        newFoodParameter.foodIndex = foodIndex;
        newFoodParameter.SetSize(power);
        Color color = foodColorRandomList[UnityEngine.Random.Range(0, foodColorRandomList.Length)];
        newFoodParameter.SetColor(color);
        newFood.transform.parent = this.transform;
    }
    void GetListPlayer()
    {
        listPlayer.Clear();

        string ownerId;
        GameObject[] playerSpawnObjs = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerSpawnObjs.Length; i++)
        {
            if(playerSpawnObjs[i].GetComponent<OSnakePlayer>().SnakeHead == null)
            {
                return;
            }
            ownerId = playerSpawnObjs[i].GetComponent<OSnakePlayer>().SnakeHead.GetComponent<PhotonView>().Owner.UserId;
            playerSpawnObjs[i].GetComponent<OSnakePlayer>().SetPlayerID(ownerId);
            listPlayer.Add(playerSpawnObjs[i]);
        }
    }
    void SnakePlayerDie(string userId)
    {
        if (userId == "")
        {
            return;
        }

        if(PhotonNetwork.LocalPlayer.UserId == userId)
        {
            for (int i = 0; i < listPlayer.Count; i++)
            {
                if (listPlayer[i].GetComponent<OSnakePlayer>().GetPlayerId() == userId)
                {
                    listPlayer[i].GetComponent<OSnakePlayer>().SnakeHead.GetComponent<OSnakeHead>().Die();
                    break;
                }
            }
        }
    }

    #endregion
}