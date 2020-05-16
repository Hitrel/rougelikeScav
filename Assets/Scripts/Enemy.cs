using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    [Header("Sound Effect")]
    public AudioClip attackSound;

    private Animator animator;
    private Transform player;
    private bool skipTurn;
    // Start is called before the first frame update
    new void Start()
    {
        GameManager.instance.AddEnemyToList(this);

        playerDamage = (int) (Mathf.Log(GameManager.instance.getLevel()+1)*playerDamage);

        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        base.Start();

    }

    // Update is called once per frame

    protected override void  AttemptMove<T>(int xDir, int yDir)
    {
        if (skipTurn)
        {
            skipTurn = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipTurn = true;
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        
        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("enemyAttack");
        SoundMangaer.instance.PlaySingle(attackSound);

    }

    public void EnemyMove()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(player.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = player.position.y > transform.position.y ? 1 : -1;
        }

        else
        {
            xDir = player.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);

    }

}
