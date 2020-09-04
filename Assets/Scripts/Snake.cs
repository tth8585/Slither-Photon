using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CnControls;

public class Snake : MonoBehaviour {
    public float speed = 5;
    public float rotatingSpeed = 0.7f;
    public int points;
    public int speedMultiplier = 2;
    int originalSpeedMultiplier;
    int startingPoints = 250; // You can't go lower than this value
    int pieceForPoints = 25; // Each 50p gets 1 piece

    int pointsForScale = 250; // Each 250p the scale increases 
    float scaleOffset = 0.05f; // by this value

    public GameObject snakePartPrefab;
    public SnakeHead SnakeHead;
    public List<Transform> snakePieces;
    public ColorTemplate colorTemplate;
    public AI aiModule;
    public static Snake player;
    public bool isPlayer;
    public bool dieing;
    public float referenceScale;
    void Awake() {
        GetARandomTemplate();

    }






    void Start() {
        originalSpeedMultiplier = speedMultiplier;
        snakePieces = new List<Transform>();
        snakePieces.Add(SnakeHead.transform);
        isPlayer = SnakeHead.IsPlayer;
        if (isPlayer) { player = this;
        }
        StartCoroutine(consumeSnakeIfSprint());


    }

    void GetARandomTemplate()
    {
        ColorTemplate[] colorTemplates = FindObjectsOfType<ColorTemplate>();
        colorTemplate = colorTemplates[Random.Range(0, colorTemplates.Length)];

    }

    void Update() {
        ControlSnakeLenght();
        ControlSnakeScale();
        ControlSprint();
 
    }

    IEnumerator consumeSnakeIfSprint() {
        while (true){
            if (speedMultiplier != 1) {
                if (isPlayer)
                {
                    points -= 5; // Consume only the main player
                }
                Vector3 spawnPosition = snakePieces[snakePieces.Count - 1].transform.position - snakePieces[snakePieces.Count - 1].forward*2;
                FoodManager.instance.SpawnFood(Random.Range(3,6), FoodManager.instance.foodColorRandomList[Random.Range(0, FoodManager.instance.foodColorRandomList.Length)], spawnPosition, false);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    void ControlSprint()
    {
        if (points <= 300) { speedMultiplier = 1; return; }
        // IF IS PLAYER
        if (isPlayer)
        {
            if (CnInputManager.GetButton("Jump"))
            {
                speedMultiplier = originalSpeedMultiplier;
            }
            else
            {
                speedMultiplier = 1;
            }
        }
        // IF IS BOT
        if (!isPlayer)
        {
            if (aiModule.sprint)
            {
                speedMultiplier = originalSpeedMultiplier;

            }
            else
            {
                speedMultiplier = 1;
            }
        }


    }

  public  void ControlGlow(SpriteRenderer glowEffect) {
            if (speedMultiplier != 1)
            {
            Color glowEffectColor = glowEffect.color;
            glowEffectColor.a = Mathf.MoveTowards(glowEffectColor.a, 1, 2 * Time.deltaTime);
            glowEffect.color = glowEffectColor;

        }

        else
            {
            Color glowEffectColor = glowEffect.color;
            glowEffectColor.a = Mathf.MoveTowards(glowEffectColor.a, 0, 2 * Time.deltaTime);
            glowEffect.color = glowEffectColor;
        }


    }

    void ControlSnakeScale() {
        float scale =(float) points / pointsForScale;

        scale = 1 + scale * scaleOffset;

        referenceScale = Mathf.Lerp(referenceScale, scale,Time.deltaTime*1);
    }


    void ControlSnakeLenght() {
        if (points < startingPoints) points = startingPoints;

        int snakeParts = Mathf.RoundToInt(points / pieceForPoints);
        if (snakePieces.Count < snakeParts) {
            AddNewPart();
        }
        if (snakePieces.Count > snakeParts)
        {
            RemovePart();
        }

    }
    void RemovePart() {
        Destroy(snakePieces[snakePieces.Count - 1].gameObject);
        snakePieces.RemoveAt(snakePieces.Count - 1);
    }
    void AddNewPart() {
        GameObject newPiece = (GameObject)Instantiate(snakePartPrefab, snakePieces[snakePieces.Count - 1].transform.position, snakePartPrefab.transform.rotation);
        
        snakePieces.Add(newPiece.transform);
        newPiece.GetComponent<Piece>().InitializePiece(snakePieces.Count - 1, this);
    }



   public void DeathRoutine() {
        if (dieing) return;
        dieing = true;
        foreach(Transform piece in snakePieces) {
            Vector3 randomCircle = Random.insideUnitSphere*3;
            randomCircle.y = 0;
            if (Vector3.Distance(SnakeHead.transform.position, Camera.main.transform.position) <= 250)
            {
                int value = Random.Range(Mathf.RoundToInt(points / 2 / snakePieces.Count), Mathf.RoundToInt(points / 3 / snakePieces.Count));
                value = Mathf.Clamp(value, 1, 20);
                FoodManager.instance.SpawnFood(value, FoodManager.instance.foodColorRandomList[Random.Range(0, FoodManager.instance.foodColorRandomList.Length)], piece.position + randomCircle, false);
            }

        }
       Population.instance.SpawnSnake(Random.Range(250, 1000));
        StartCoroutine(FadeToDeathRoutine());
    }

    IEnumerator FadeToDeathRoutine() {
        float lerper = 1;
        float lerpTime = 0.5f;
        if (Vector3.Distance(SnakeHead.transform.position, Camera.main.transform.position) <= 250)
        {

            while (lerper >= 0)
            {
                lerper -= Time.deltaTime / lerpTime;
                foreach (Transform piece in snakePieces)
                {
                    Piece pieceParams = piece.gameObject.GetComponent<Piece>();
                    if (pieceParams != null)
                    {
                        Color spritecol = pieceParams.spriteRenderer.color;
                        spritecol.a = lerper;
                        pieceParams.spriteRenderer.color = spritecol;
                        pieceParams.gameObject.tag = "Untagged";
                    }
                    Color spriteHeadCol = SnakeHead.spriteRenderer.color;
                    spriteHeadCol.a = lerper;
                    SnakeHead.spriteRenderer.color = spriteHeadCol;
                    SnakeHead.gameObject.tag = "Untagged";

                }

                yield return new WaitForEndOfFrame();
            }

        }
        if (isPlayer) {
            yield return new WaitForSeconds(1f);
            GameManagerOffline.instance.OnGameOver_Event();
        }

        Destroy(this.gameObject);

    }


}
