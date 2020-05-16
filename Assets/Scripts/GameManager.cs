using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;
    public static GameManager instance = null; // Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManager boardManager; // Store a reference to our BoardManager which will set up the level.
    private int level = 1; // Current level number
    private List<Enemy> enemies;
    private bool enemyMoving;

    // UI
    private GameObject LevelImage;
    private Text LevelText;
    private bool doingSetup;
    private bool waitforStart = true;
    private bool pause = false;

    public bool start = true;
    // Awake is always called before any Start functions.
    void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            // if not, set instance to this.
            instance = this;
        } 
        // If instance already exists and it's not this:
        else if (instance != this)
        {
            // Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager. 
            Destroy(gameObject);
        }

        // Sets this to not be destryed when reloading scene
        DontDestroyOnLoad(gameObject);

        enemies = new List<Enemy>();

        // Get a component reference to the attached BoardManager script.
        boardManager = GetComponent<BoardManager>();

    }

    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }
    public void InitGame ()
    {
        waitforStart = false;

        doingSetup = true;

        LevelImage = GameObject.Find("LevelImage");
        LevelText = GameObject.Find("LevelText").GetComponent<Text>();

        LevelText.text = "Day " + level;

        LevelImage.SetActive(true);

        Invoke("HideLoadingImage", levelStartDelay);

        enemies.Clear();
        // Call the SetupScene function of the BoardManager script, pass it current level number.
        boardManager.SetupScene(level);
    }

    void HideLoadingImage()
    {
        LevelImage.SetActive(false);

        doingSetup = false;
    }

    public void GameOver ()
    {
        LevelText.text = "After " + level + " days you died.";
        LevelImage.SetActive(true);
        enabled = false;
    }

    public int getLevel()
    {
        return level;
    }
    
    void Update ()
    {
        if (!pause) {
            if (playersTurn || enemyMoving || doingSetup || waitforStart) {
                return;
            }

            StartCoroutine(MoveEnemies());
        }
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    IEnumerator MoveEnemies()
    {
        enemyMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        else
            for (int i = 0; i<enemies.Count; i++)
            {
                enemies[i].EnemyMove();

                yield return new WaitForSeconds(enemies[i].moveTime);
            }

        playersTurn = true;
        enemyMoving = false;
    }

}
