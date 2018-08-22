using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour {

    public AStar aStar;
    public MovableObject player;

    public float gameDuration = 30f;
    public float maximumSpawnInterval = 2f;
    public float minimumSpawnInterval = 0.5f;
    public float crateLifeTime = 10f;

    public GameObject cratePrefab;
    public GameObject crateContainer;
    public GameObject navTileContainer;

    private float gameTimer;
    private float spawnTimer;
    private List<NavTile> navigableTiles;

    int score;

	// Use this for initialization
	void Start () {
        spawnTimer = maximumSpawnInterval;

        //Build list of navigable tiles
        navigableTiles = new List<NavTile>();
        foreach(NavTile tile in navTileContainer.GetComponentsInChildren<NavTile>())
        {
            if (tile.navigable)
            {
                navigableTiles.Add(tile);
            }
        }

        player.GetComponent<Player>().onCollect += OnCollectCrate;
	}

    void Update()
    {
        //Game timer logic
        gameTimer += Time.deltaTime;
        float difficulty = Mathf.Min(gameTimer / gameDuration, 1.0f);

        //Spawn Logic
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0.0f)
        {
            float spawnInterval = maximumSpawnInterval - (maximumSpawnInterval - minimumSpawnInterval) * difficulty;
            spawnTimer = spawnInterval;


            Vector3 spawnPosition = navigableTiles[Random.Range(0, navigableTiles.Count)].transform.position;
            GameObject crateInstance = Instantiate(cratePrefab, spawnPosition, Quaternion.identity, crateContainer.transform);

            Destroy(crateInstance, crateLifeTime);
        }
        //Input Logic
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.collider.gameObject.GetComponent<NavTile>() != null)
                {
                    player.Move(aStar.FindPath(player.gameObject, hit.collider.gameObject));

                    break;
                }
            }
          
        }
    }

	private void OnCollectCrate(GameObject crate)
    {
        Destroy(crate);
        score++;

        Debug.Log("Score: " + score);
    }
}
