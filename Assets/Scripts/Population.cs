using UnityEngine;
using System.Collections;

public class Population : MonoBehaviour {
    public int small = 20;
    public int medium = 10;
    public static Population instance;
    public int big = 5;
  public   float spawnCircleLenght = 1000;
    public GameObject snakePrefab;

    void Awake() {
        instance = this;

    }
    // Use this for initialization
    void Start () {
        SpawnPopulation();
	}
	
public void SpawnPopulation()
    {
        for (int i = 0; i < small; i++) {

            SpawnSnake(Random.Range(250, 1000));
        }
        for (int i = 0; i < medium; i++)
        {
            SpawnSnake(Random.Range(2500, 4000));


        }
        for (int i = 0; i < big; i++)
        {
            SpawnSnake(Random.Range(6500, 12500));


        }
     

    }

  public  void SpawnSnake(int points)
    {
        GameObject newsnake = (GameObject)Instantiate(snakePrefab, snakePrefab.transform.position, snakePrefab.transform.rotation);
        Snake snakeparams = newsnake.GetComponent<Snake>();
        snakeparams.points = points;
        snakeparams.SnakeHead.IsPlayer = false;
        snakeparams.isPlayer = false;
        Vector2 randomSpawnCircleVector2 = Random.insideUnitCircle*spawnCircleLenght;
        Vector3 randomSpawnCircle = new Vector3(randomSpawnCircleVector2.x, newsnake.transform.position.y, randomSpawnCircleVector2.y);
        newsnake.transform.position = newsnake.transform.position + randomSpawnCircle;

    }

}
