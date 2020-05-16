using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MovingObject
{
    public float restartLevelDelay = 1f;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public int wallDamage = 1;
    public Text foodText;

    [Header("Sounds Effect")]
    public AudioClip moveClip1;
    public AudioClip moveClip2;
    public AudioClip eatClip1;
    public AudioClip eatClip2;
    public AudioClip drinkClip1;
    public AudioClip drinkClip2;
    public AudioClip gameOverClip;


    private Animator animator;
    private int foods;

    private bool pause;
    // Start is called before the first frame update
    new void Start()
    {
        animator = GetComponent<Animator>();
        foods = GameManager.instance.playerFoodPoints;

        foodText.text = "Food: " + foods;
        base.Start();
    }

    public void OnDisable()
    {
        GameManager.instance.playerFoodPoints = foods;

        
    }

    protected override void AttemptMove <T>(int xDir, int yDir)
    {
        foods -= 1;
        foodText.text = "-1 Food: " + foods;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundMangaer.instance.RandomizeSfx(moveClip1, moveClip2);
        }
        CheckIfGameOver();
        
        GameManager.instance.playersTurn = false;
    }


    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;

        hitWall.DamageWall(wallDamage);

        animator.SetTrigger("playerChop");

    }
    
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");

        foods -= loss;
        foodText.text = "-" + loss + " Food: " + foods;
        CheckIfGameOver();
    }
    
    private void CheckIfGameOver()
    {
        if (foods <= 0)
        {
            SoundMangaer.instance.PlaySingle(gameOverClip);
            SoundMangaer.instance.MusicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
        }
        
        else if (collision.tag == "Food")
        {
            SoundMangaer.instance.RandomizeSfx(eatClip1, eatClip2);
            foods += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + foods;
            collision.gameObject.SetActive(false);
        }

        else if (collision.tag == "Soda")
        {
            SoundMangaer.instance.RandomizeSfx(drinkClip1, drinkClip2);
            foods += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + foods;
            collision.gameObject.SetActive(false);
        }
    }
    private void Restart()
    {
        SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        if (!pause) {
            if (!GameManager.instance.playersTurn) {
                return;
            }

            int horizontal = 0;

            int vertical = 0;


            horizontal = (int)Input.GetAxisRaw("Horizontal");

            vertical = (int)Input.GetAxisRaw("Vertical");

            if (horizontal != 0)
                vertical = 0;


            if (horizontal != 0 || vertical != 0) {
                AttemptMove<Wall>(horizontal, vertical);
            }
        }
    }
    
}
