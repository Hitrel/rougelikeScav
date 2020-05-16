using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;

    public int hp = 2;

    public AudioClip ChopSound1;
    public AudioClip ChopSound2;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall (int loss)
    {
        spriteRenderer.sprite = dmgSprite;
        SoundMangaer.instance.RandomizeSfx(ChopSound1, ChopSound2);
        hp -= loss;

        if (hp < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
