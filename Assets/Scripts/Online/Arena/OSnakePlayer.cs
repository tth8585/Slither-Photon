
using CnControls;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSnakePlayer : MonoBehaviour
{
    public float speed = 0;
    public float rotatingSpeed = 0;
    public int points;
    public int speedMultiplier = 0;
    int originalSpeedMultiplier;
    float snakeScale = 1f;
   
    int pieceForPoints = 0; // Each 50p gets 1 piece
    int pointsForScale = 0; // Each 250p the scale increases 

    public GameObject snakePartPrefab;
    public OSnakeHead SnakeHead;
    public List<Transform> snakePieces;
    public ColorTemplate colorTemplate;

    //public static OSnakePlayer player;
    //public bool isPlayer;
    public bool dieing;
    public float referenceScale;
    [SerializeField] string playerID;

    [SerializeField] GameObject namePlayer;
    public Player Owner { get; private set; }

    void Awake()
    {
        GetARandomTemplate();
        dieing = false;
    }
    private void Start()
    {
        namePlayer.GetComponent<TextMesh>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        namePlayer.GetComponent<TextMesh>().text = PhotonNetwork.NickName;

        StartCoroutine(ConsumeSnakeIfSprint());
        
    }
    private void Update()
    {
        //ControlSnakeLenght();
        ControlSnakeScale();
        ControlSprint();
    }
    void GetARandomTemplate()
    {
        ColorTemplate[] colorTemplates = FindObjectsOfType<ColorTemplate>();
        colorTemplate = colorTemplates[Random.Range(0, colorTemplates.Length)];
    }
    IEnumerator ConsumeSnakeIfSprint()
    {
        while (true)
        {
            if (speedMultiplier != 1)
            {
                if (SnakeHead.GetComponent<PhotonView>().IsMine == true)
                {
                    Vector3 spawnPosition = snakePieces[snakePieces.Count - 1].transform.position - snakePieces[snakePieces.Count - 1].forward * 2;

                    object[] foodDatas = new object[] { spawnPosition.x, spawnPosition.y, spawnPosition.z, PhotonNetwork.LocalPlayer.UserId };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                    PhotonNetwork.RaiseEvent(RaiseEventData.FOOD_SPRINT, foodDatas, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendUnreliable);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    void ControlSprint()
    {
        if (SnakeHead.GetComponent<PhotonView>().IsMine == true)
        {
            if (PhotonNetwork.LocalPlayer.GetScore() <= 300) { speedMultiplier = 1; return; }
            {
                if (CnInputManager.GetButton("Jump"))
                {
                    speedMultiplier = originalSpeedMultiplier;
                }
                else
                {
                    speedMultiplier = 1;
                }
            }
        }
    }
    public void ControlGlow(SpriteRenderer glowEffect)
    {
        if (speedMultiplier != 1)
        {
            Color glowEffectColor = glowEffect.color;
            glowEffectColor.a = Mathf.MoveTowards(glowEffectColor.a, 1, 2 * Time.deltaTime);
            glowEffect.color = glowEffectColor;
        }
        else
        {
            Color glowEffectColor = glowEffect.color;
            glowEffectColor.a = Mathf.MoveTowards(glowEffectColor.a, 0, 2 * Time.deltaTime);
            glowEffect.color = glowEffectColor;
        }
    }
    public void SetScale(float scale)
    {
        snakeScale = scale;
    }
    public void ControlSnakeScale()
    {
        referenceScale = Mathf.Lerp(referenceScale, snakeScale, Time.deltaTime * 1);
    }
    public void AddNewPart(int totalNumberPart)
    {
        //check different of number then add
        int dif = totalNumberPart + 1 - snakePieces.Count; //1 for head
        GameObject newPiece;

        if (dif > 0) //add part
        {
            for (int i = 0; i < dif; i++)
            {
                newPiece = (GameObject)Instantiate(snakePartPrefab, snakePieces[snakePieces.Count - 1].transform.position, snakePartPrefab.transform.rotation);

                snakePieces.Add(newPiece.transform);
                newPiece.GetComponent<OPeController>().InitializePiece(snakePieces.Count - 1, this);
            }
        }
        else if (dif < 0) //remove part
        {
            dif *= -1;
            for (int i = 0;i< dif; i++)
            {
                Destroy(snakePieces[snakePieces.Count - 1].gameObject);
                snakePieces.RemoveAt(snakePieces.Count - 1);
            }
        }
    }
    public void SetPlayerID(string id)
    {
        this.playerID = id;
    }
    public string GetPlayerId()
    {
        return this.playerID;
    }
    public void SetPlayerBios(float speed, float rotateSpeed, int speedMulty)
    {
        this.speed = speed;
        this.rotatingSpeed = rotateSpeed;
        this.speedMultiplier = speedMulty;

        originalSpeedMultiplier = speedMultiplier;
    }
    public void AddSnakeHead()
    {
        snakePieces = new List<Transform>();
        snakePieces.Add(SnakeHead.transform);
    }
    public void SpawnDeathRoutine()
    {
        if (dieing) return;
        dieing = true;

        //if (PhotonNetwork.IsMasterClient)
        //{
            
        //}
        Vector3 randomCircle;
        Vector3 spawnPosition;
        List<float> listX = new List<float>();
        List<float> listY = new List<float>();
        List<float> listZ = new List<float>();

        foreach (Transform piece in snakePieces)
        {
            randomCircle = Random.insideUnitSphere * 3;
            randomCircle.y = 0;
            spawnPosition = piece.position + randomCircle;

            listX.Add(spawnPosition.x);
            listY.Add(spawnPosition.y);
            listZ.Add(spawnPosition.z);
        }

        string listStringPosX = FoodRaiseEvent.Instance.ConvertListFloatToString(listX);
        string listStringPosY = FoodRaiseEvent.Instance.ConvertListFloatToString(listY);
        string listStringPosZ = FoodRaiseEvent.Instance.ConvertListFloatToString(listZ);
        string deadId = playerID;

        object[] foodDatas = new object[] { listStringPosX, listStringPosY, listStringPosZ, deadId };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        PhotonNetwork.RaiseEvent(RaiseEventData.SPAWN_FOOD_WHEN_PLAYER_DIE, foodDatas, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendUnreliable);

        Debug.Log("sending food spawn from body");

        //PhotonNetwork.Disconnect();
        //StartCoroutine(FadeToDeathRoutine());
    }

    IEnumerator FadeToDeathRoutine()
    {
        float lerper = 1;
        float lerpTime = 0.5f;

        while (lerper >= 0)
        {
            lerper -= Time.deltaTime / lerpTime;
            foreach (Transform piece in snakePieces)
            {
                Piece pieceParams = piece.gameObject.GetComponent<Piece>();
                if (pieceParams != null)
                {
                    Color spritecol = pieceParams.spriteRenderer.color;
                    spritecol.a = lerper;
                    pieceParams.spriteRenderer.color = spritecol;
                    pieceParams.gameObject.tag = "Untagged";
                }
                Color spriteHeadCol = SnakeHead.spriteRenderer.color;
                spriteHeadCol.a = lerper;
                SnakeHead.spriteRenderer.color = spriteHeadCol;
                SnakeHead.gameObject.tag = "Untagged";

            }

            yield return new WaitForEndOfFrame();
        }

        if(playerID == PhotonNetwork.LocalPlayer.UserId)
        {

        }
        else
        {
            //Destroy(this.gameObject);
        }
    }
}
