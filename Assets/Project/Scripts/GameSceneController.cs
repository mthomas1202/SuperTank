using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour {
    [Header("Gameplay")]
    public AStar aStar;
    public MovableObject player;

    public float gameDuration = 30f;
    public float maximumSpawnInterval = 2f;
    public float minimumSpawnInterval = 0.5f;
    public float crateLifeTime = 10f;

    [Header("Audio")]
    public AudioSource collectSound;

    [Header("UI")]
    public GameObject startGroup;
    public GameObject gameplayGroup;
    public GameObject gameOverGroup;
    public Text scoreText;
    public Text gameOverText;
    public Text timeText;
    public Text highScoreText;
    public Text newHighScoreText;

    public GameObject cratePrefab;
    public GameObject crateContainer;
    public GameObject navTileContainer;

    private float gameTimer;
    private float spawnTimer;
    private List<NavTile> navigableTiles;
    private bool isPlaying;
    private bool isGameOver;


    int score;

    public int Score
    {
        get {
            return score;
         }
        set{
            score = value;
            scoreText.text = "Score: " + score;
        }
    }
	// Use this for initialization
	void Start () {
        spawnTimer = maximumSpawnInterval;
        Score = 0;

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

        startGroup.SetActive(true);
        gameplayGroup.SetActive(false);
        gameOverGroup.SetActive(false);

        int currentHighScore = PlayerPrefs.GetInt("highscore");
        highScoreText.text = string.Format(highScoreText.text, currentHighScore);
	}

    void Update()
    {
        if (isPlaying)
        {
            //Game timer logic
            gameTimer += Time.deltaTime;
            float difficulty = Mathf.Min(gameTimer / gameDuration, 1.0f);

            timeText.text = string.Format("Time: {0}s", Mathf.CeilToInt(gameDuration - gameTimer));
            //Spawn Logic
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0.0f)
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
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.gameObject.GetComponent<NavTile>() != null)
                    {
                        player.Move(aStar.FindPath(player.gameObject, hit.collider.gameObject));

                        break;
                    }
                }
            }

            //Check for Game Over
            if(gameTimer > gameDuration && !isGameOver)
            {
                OnGameOver();
            }
        }
    }

	private void OnCollectCrate(GameObject crate)
    {
        Destroy(crate);
        Score++;
        collectSound.Play();
    }

    public void OnPlay()
    {
        startGroup.SetActive(false);
        gameplayGroup.SetActive(true);
        gameOverGroup.SetActive(false);

        isPlaying = true;
    }

    private void OnGameOver()
    {
        startGroup.SetActive(false);
        gameplayGroup.SetActive(false);
        gameOverGroup.SetActive(true);

        isGameOver = true;
        isPlaying = false;

        gameOverText.text = string.Format(gameOverText.text, score);

        //Save the High Score
        newHighScoreText.gameObject.SetActive(false);
        int currentHighScore = PlayerPrefs.GetInt("highscore");
        if(score > currentHighScore)
        {
            PlayerPrefs.SetInt("highscore", score);
            newHighScoreText.gameObject.SetActive(true);
        }
    }

    public void OnReplay()
    {
        SceneManager.LoadScene("Game");
    }
}
