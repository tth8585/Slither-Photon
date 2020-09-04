using UnityEngine;
using System.Collections;
public class FoodManager : MonoBehaviour {

    public int foodQuantity = 1500;
    float spawnRange;
     float staticFoodMinSize = 2f;
     float staticFoodMaxSize = 6f;
    public static FoodManager instance;
    public GameObject foodPrefab;
    public Color[] foodColorRandomList;
    
    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start () {
        spawnRange = Population.instance.spawnCircleLenght;
        SpawnAllFood();
	}


    void SpawnAllFood() {
        for (int i = 0; i < foodQuantity; i++) {
            SpawnFood(Random.Range(staticFoodMinSize,staticFoodMaxSize), foodColorRandomList[Random.Range(0, foodColorRandomList.Length)],Vector3.zero);
        }
    }

    public void SpawnFood(float power,Color foodColor, Vector3 spawningPosition,bool randomPosition = true ) {

        foodColor.a = 1f;

        GameObject newFood = (GameObject)Instantiate(foodPrefab, Vector3.zero, foodPrefab.transform.rotation);
        Food newFoodParameter = newFood.GetComponent<Food>();
        newFoodParameter.SetSize(power);
        newFoodParameter.SetColor(foodColor);

        if (randomPosition)
        {
            MoveTransformOnCircle(newFood.transform);
        }
        else
        {
            newFood.transform.position = spawningPosition;

        }

        newFood.transform.parent = this.transform;
    }

   public void MoveTransformOnCircle(Transform foodTransform) {
        Vector3 rangeVector = Random.onUnitSphere * spawnRange;
        rangeVector.y = 0;
        foodTransform.position = Vector3.zero + rangeVector;
    }
}
