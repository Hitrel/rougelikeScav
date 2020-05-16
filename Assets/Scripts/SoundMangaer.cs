using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMangaer : MonoBehaviour
{
    [Header("Source")]
    public AudioSource EfxSource;
    public AudioSource MusicSource;
    [HideInInspector] public static SoundMangaer instance = null;
    [Header("RandomPitch")]
    public float MaxPitch = 1.05f;
    public float MinPitch = 0.95f;



    // Start is called before the first frame update
    void Awake()
    {
        if (SoundMangaer.instance == null)
            SoundMangaer.instance = this;

        else if (SoundMangaer.instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

    }

    public void PlaySingle(AudioClip clip)
    {
        EfxSource.clip = clip;
        EfxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);

        float randomPitch = Random.Range(MinPitch, MaxPitch);

        EfxSource.pitch = randomPitch;

        EfxSource.clip = clips[randomIndex];

        EfxSource.Play();
    }
}
