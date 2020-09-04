using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {
    public int PieceIndex;
    public Snake snakeParameters;
    Transform reference;
    public SpriteRenderer spriteRenderer;
    float pieceDistanceOffset = 1f;
    float pieceYDistanceOffset = 0.01f;
   public SpriteRenderer glow;
   public void InitializePiece(int index, Snake parameters) {
        PieceIndex = index;
        snakeParameters = parameters;
        transform.parent = parameters.transform;
        reference = snakeParameters.snakePieces[index - 1];
        transform.position = reference.transform.position - reference.transform.forward * (pieceDistanceOffset * snakeParameters.transform.localScale.x) - Vector3.up* pieceYDistanceOffset;
    }

    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);

        SetColorBasedOnTemplate();
    }

    void SetColorBasedOnTemplate()
    {
        int colorIndex = PieceIndex;
        while (colorIndex >= snakeParameters.colorTemplate.colors.Length) {
            colorIndex -= snakeParameters.colorTemplate.colors.Length;
        }
        spriteRenderer.color = snakeParameters.colorTemplate.colors[colorIndex];
    }





    void Update() {
        Move();
        Rotate();
        snakeParameters.ControlGlow(glow);
        transform.localScale = new Vector3(snakeParameters.referenceScale, snakeParameters.referenceScale, snakeParameters.referenceScale);

    }



    void Rotate()
    {
        Vector3 referencePosition = reference.transform.position-reference.transform.forward* (pieceDistanceOffset * snakeParameters.transform.localScale.x);
        referencePosition.y = transform.position.y;

        Vector3 direction = referencePosition - transform.position;

     

        if (direction != Vector3.zero)
        {
            Quaternion targetLook = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, snakeParameters.rotatingSpeed*Time.deltaTime*30*snakeParameters.speedMultiplier);
        }

    }
        float MOVlerpTime = 0.50f;
    void Move()
    {


        float frameRateCorrection = Time.deltaTime*10;
        MOVlerpTime = Mathf.Lerp(0.3f, 0.95f, frameRateCorrection);
        Vector3 referencePosition = reference.transform.position - reference.transform.forward* ((pieceDistanceOffset  * snakeParameters.transform.localScale.x));
        referencePosition.y = transform.position.y;
        transform.position = Vector3.Lerp(transform.position, referencePosition, MOVlerpTime);
       
    }


}
