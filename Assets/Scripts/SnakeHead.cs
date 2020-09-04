using UnityEngine;
using System.Collections;
using CnControls;
public class SnakeHead : MonoBehaviour {
    // Controls the snake movement
    public Snake snakeParameters;
    public bool IsPlayer;
    public SpriteRenderer spriteRenderer;
    public AI aimodule;
    public SpriteRenderer glow;

    void Start() {
       
        SetColorBasedOnTemplate();
    }

    void SetColorBasedOnTemplate() {
        spriteRenderer.color = snakeParameters.colorTemplate.colors[0];
    }


    void OnTriggerEnter(Collider obj){

        if (snakeParameters.dieing) return;


        if (obj.transform.tag == "Snake" && obj.transform.root != transform.root) {

            SnakeHead snakeHead = obj.transform.GetComponent<SnakeHead>();
            if (snakeHead != null) {
                // Collided with an other head
            
                if (snakeHead.snakeParameters.points < snakeParameters.points ) { // Little is stronger
                    Die();
                }
            }
            else
            {
                Die();
            }

        }
    }
    void Die() {
        snakeParameters.DeathRoutine();
    }



    void Update()
    {
        if (snakeParameters.dieing) return;

        snakeParameters.ControlGlow(glow);

        Move();
        transform.localScale = new Vector3(snakeParameters.referenceScale, snakeParameters.referenceScale, snakeParameters.referenceScale);


        if (Vector3.Distance(transform.position, Vector3.zero) <= Population.instance.spawnCircleLenght )
        {

            if (IsPlayer)
            {
                Rotate();
            }
            else
            {
                IaRotate();

            }
        }
        else {
            RotateTowardsCenter();
        }
    }

    void RotateTowardsCenter() {
        Quaternion targetLook = Quaternion.LookRotation(Vector3.zero-transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, snakeParameters.rotatingSpeed * Time.deltaTime);

    }


    void IaRotate()
    {


        if (aimodule.direction != Vector3.zero)
        {
            Quaternion targetLook = Quaternion.LookRotation(aimodule.direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, snakeParameters.rotatingSpeed * Time.deltaTime);
        }
    }

    void Rotate()
    {
    
        Vector3 axis = new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical"));

        if (axis != Vector3.zero)
        {
            Quaternion targetLook = Quaternion.LookRotation(axis);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, snakeParameters.rotatingSpeed * Time.deltaTime);
        }

        // limit rotation
 
    }

    void Move()
    {
       
        transform.position += transform.forward * snakeParameters.speed*Time.deltaTime*snakeParameters.speedMultiplier;

    }


}
