using CnControls;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSnakeHead : MonoBehaviour
{
    public OSnakePlayer snakeParameters;
    private PhotonView photonView;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer glow;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        
        SetColorBasedOnTemplate();
    }
    void SetColorBasedOnTemplate()
    {
        spriteRenderer.color = snakeParameters.colorTemplate.colors[0];
    }
    private void Update()
    {
        if (snakeParameters.dieing) return;

        snakeParameters.ControlGlow(glow);
        transform.localScale = new Vector3(snakeParameters.referenceScale, snakeParameters.referenceScale, snakeParameters.referenceScale);

        Move();

        if (Vector3.Distance(transform.position, Vector3.zero) <= 500)
        {
            Rotate();
        }
        else
        {
            RotateTowardsCenter();
        }
    }

    void Move()
    {
        transform.position += transform.forward * snakeParameters.speed * Time.deltaTime * snakeParameters.speedMultiplier;
    }

    void Rotate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Vector3 axis = new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical"));

        if (axis != Vector3.zero)
        {
            Quaternion targetLook = Quaternion.LookRotation(axis);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, snakeParameters.rotatingSpeed * Time.deltaTime);
        }
    }
    void RotateTowardsCenter()
    {
        Quaternion targetLook = Quaternion.LookRotation(Vector3.zero - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, snakeParameters.rotatingSpeed * Time.deltaTime);
    }
    void OnTriggerEnter(Collider obj)
    {
        if (snakeParameters.dieing) return;
        if(obj.transform.tag == "Food")
        {
            return;
        }
        string myId = this.transform.parent.GetComponent<OSnakePlayer>().GetPlayerId();
        string otherId = obj.transform.parent.GetComponent<OSnakePlayer>().GetPlayerId();

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

        if (obj.transform.tag == "Part" && obj.transform.root != transform.root)
        {
            PhotonNetwork.RaiseEvent(RaiseEventData.PLAYER_DIES, myId, raiseEventOptions, SendOptions.SendUnreliable);
        }
        else if (obj.transform.tag == "Snake")
        {
            // Collided with an other head
            object[] twoId = new object[] { myId, otherId };

            PhotonNetwork.RaiseEvent(RaiseEventData.CHECK_HEAD_COLLIDER, twoId, raiseEventOptions, SendOptions.SendUnreliable);
        }
    }
    public void Die()
    {
        snakeParameters.SpawnDeathRoutine();
    }
    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
