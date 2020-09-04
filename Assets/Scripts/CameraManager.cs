using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
   public Snake playerSnake;
    Camera cam;
    float startOrtographic;
	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
        cam = GetComponent<Camera>();
        startOrtographic = cam.orthographicSize;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (playerSnake != null)
        {
          Follow();
            Zoom();
        }
	}

    void Zoom() {
        float scale = playerSnake.referenceScale;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize,startOrtographic + scale*20,1);


    }

    void Follow() {
        Vector3 playerPosition = playerSnake.SnakeHead.transform.position;
        playerPosition.y = transform.position.y;
        transform.position = Vector3.Lerp(transform.position, playerPosition,  10*Time.deltaTime);

    }
}
