using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

    public Vector3 direction;

   public bool sprint;

    void Start() {
        StartCoroutine(changeDirectionRandomlyForRoaming());
        StartCoroutine(sprintRandomly());

    }

    IEnumerator sprintRandomly() {
        float waitTime = 3;
      while (true)  {
            int random = Random.Range(0, 3 + 1);
            if (random == 0) {
                sprint = true;
            } else
            {
                sprint = false;
            }
            yield return new WaitForSeconds(waitTime);

        }
    }

    IEnumerator changeDirectionRandomlyForRoaming() {
        float waitTime = 5;
        Vector3 startingPoint = transform.position;
      
        while (true) {
            Vector3 circle = Random.insideUnitSphere * 250;
            circle.y = startingPoint.y;
            direction = circle - transform.position;
            yield return new WaitForSeconds(waitTime);
        }


    }


    void OnTriggerStay(Collider obj) {

        if (obj.tag == "Snake" && obj.transform.root != transform.root)
        {
            Escape(obj);
        }

  


    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Food")
        {
            Chase(obj);
        }

    }

    void Escape(Collider obj) {

        direction = transform.position - obj.transform.position;
        direction.y = transform.position.y;
        int random = Random.Range(0, 6 + 1);
        if (random == 0)
        {
            sprint = true;
        }
        else
        {
            sprint = false;
        }
    }



    void Chase(Collider obj)
    {
        direction =  obj.transform.position-transform.position;
        direction.y = transform.position.y;
        
    }


}





