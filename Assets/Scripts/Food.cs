using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

    float foodValue;
    bool beingEat;
    float timerToBeActive = 2;
    float timerToDestroy = 30;
    bool destruction;
    public SpriteRenderer spriteRenderer;
    public void SetColor(Color col) {
        spriteRenderer.color = col;
        StartCoroutine(fadeStartEffect());
    }
    public void SetSize(float value)
    {
        foodValue = value;
        transform.localScale = new Vector3(1 + value, 1 + value, 1 + value);
    }
    IEnumerator fadeStartEffect() {
        Color col = spriteRenderer.color;
        Color origin = col;
        origin.a = 0;
        float lerper = 0;
        float lerperTime = 1f;
        while (lerper <= 1) {
            lerper += Time.deltaTime / lerperTime;
            spriteRenderer.color = Color.Lerp(origin, col, lerper);
            yield return new WaitForEndOfFrame();
        }
    }

    void Update() {


        if (timerToBeActive >= 0)
        {
            timerToBeActive -= Time.deltaTime;
        }
        if (timerToDestroy >= 0)
        {
            timerToDestroy -= Time.deltaTime;
            if (timerToDestroy <= 0) {
                destruction = true;
            }
        }

        if (destruction)
        {            
                if (Vector3.Distance(Camera.main.transform.position, this.transform.position) > 100)
                {
                    Destroy(this.gameObject);

                }            
            
        }

    }
    void OnTriggerEnter(Collider obj) {
        if (timerToBeActive >= 0) return;
        if (beingEat) return;

        if (obj.transform.tag == "Snake") {

            Snake snakeParam = obj.transform.root.GetComponent<Snake>();
            if (snakeParam.isPlayer)
            {
                snakeParam.points += Mathf.RoundToInt(foodValue * 2);
            }
     


            StartCoroutine(moveAndDisappear(obj.transform));
        }
    }

    IEnumerator moveAndDisappear(Transform targetTransform) {
        beingEat = true;

        float lerper = 0;
        float lerperTime = 0.5f;
        Vector3 currentPosition = transform.position;
        while (lerper <=1) {
            lerper += Time.deltaTime / lerperTime;
            try
            {
                transform.position = Vector3.Lerp(currentPosition, targetTransform.position-Vector3.up, lerper);
            }
            catch {
                lerper = 1;
            }
            yield return new WaitForEndOfFrame();
        }
       // FoodManager.instance.SpawnFood(Random.Range(1, 2.5f), FoodManager.instance.foodColorRandomList[Random.Range(0, FoodManager.instance.foodColorRandomList.Length)], Vector3.zero);
        beingEat = false;
        Destroy(this.gameObject);
    }


}
