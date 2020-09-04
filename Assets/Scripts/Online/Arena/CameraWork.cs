using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWork : MonoBehaviour
{
    Camera cam;
    bool isFollowing;
    [SerializeField] private bool followOnStart = false;
    PhotonView photonView;
    public OSnakePlayer playerSnake;
    float startOrtographic;
    void Start()
    {
        if(cam == null)
        {
            cam = Camera.main;
        }

        Application.targetFrameRate = 60;
        photonView = GetComponent<PhotonView>();
        startOrtographic = cam.orthographicSize;

        if (followOnStart)
        {

        }
        if (photonView.IsMine)
        {
            OnStartFollowing();
        }
    }
    private void LateUpdate()
    {
        if (cam == null && isFollowing)
        {
            OnStartFollowing();
        }

        if (isFollowing)
        {
            Follow();
            Zoom();
        }
    }
    void Follow()
    {
        Vector3 playerPosition = playerSnake.SnakeHead.transform.position;
        playerPosition.y = 50;
        cam.transform.position = Vector3.Lerp(cam.transform.position, playerPosition, 10 * Time.deltaTime);
    }
    void Zoom()
    {
        float scale = playerSnake.referenceScale;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, startOrtographic + scale * 20, 1);
    }
    public void OnStartFollowing()
    {
        //cameraTransform = Camera.main.transform;
        isFollowing = true;
    }
}
