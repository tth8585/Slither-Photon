using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;

public class OFood : MonoBehaviour
{
    bool beingEat;
    float timerToBeActive = 2;
    float timerToDestroy = 30;
    bool destruction;
    public int foodIndex;
    public SpriteRenderer spriteRenderer;
    //private PhotonView photonView;
    private void Start()
    {
        beingEat = false;
    }
    private void Update()
    {
        if (timerToBeActive >= 0)
        {
            timerToBeActive -= Time.deltaTime;
        }
        //if (timerToDestroy >= 0)
        //{
        //    timerToDestroy -= Time.deltaTime;
        //    if (timerToDestroy <= 0)
        //    {
        //        destruction = true;
        //    }
        //}

        //if (destruction)
        //{
        //    SendEventDestroyFood();
        //}
    }

    private void SendEventDestroyFood()
    {
        timerToDestroy = 30;
        destruction = false;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(RaiseEventData.FOOD_IS_TOO_OLD, foodIndex, raiseEventOptions, SendOptions.SendUnreliable);

        Destroy(this.gameObject);
    }

    public void SetColor(Color col)
    {
        spriteRenderer.color = col;
        StartCoroutine(fadeStartEffect());
    }
    public void SetSize(float value)
    {
        //foodValue = value;
        transform.localScale = new Vector3(1 + value, 1 + value, 1 + value);
    }

    IEnumerator fadeStartEffect()
    {
        Color col = spriteRenderer.color;
        Color origin = col;
        origin.a = 0;
        float lerper = 0;
        float lerperTime = 1f;
        while (lerper <= 1)
        {
            lerper += Time.deltaTime / lerperTime;
            spriteRenderer.color = Color.Lerp(origin, col, lerper);
            yield return new WaitForEndOfFrame();
        }
    }
    void OnTriggerEnter(Collider obj)
    {
        if (timerToBeActive >= 0) return;
        if (beingEat) return;
       
        if(obj.gameObject == null)
        {
            return;
        }
        if (obj.transform.tag == "Snake")
        {        
            if (obj.GetComponent<PhotonView>().IsMine)
            {
                OSnakePlayer snakeParam = obj.transform.root.GetComponent<OSnakePlayer>();

                object[] eatFood = new object[] { foodIndex, snakeParam.GetPlayerId() };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

                PhotonNetwork.RaiseEvent(RaiseEventData.PLAYER_EAT_FOOD, eatFood, raiseEventOptions, SendOptions.SendUnreliable);
            }

            StartCoroutine(moveAndDisappear(obj.transform));
        }
    }
    IEnumerator moveAndDisappear(Transform targetTransform)
    {
        beingEat = true;

        float lerper = 0;
        float lerperTime = 0.5f;
        Vector3 currentPosition = transform.position;
        while (lerper <= 1)
        {
            lerper += Time.deltaTime / lerperTime;
            try
            {
                transform.position = Vector3.Lerp(currentPosition, targetTransform.position - Vector3.up, lerper);
            }
            catch
            {
                lerper = 1;
            }
            yield return new WaitForEndOfFrame();
        }
      
        beingEat = false;
        Destroy(this.gameObject);
    }
}
