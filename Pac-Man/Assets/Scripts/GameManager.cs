using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] float score = 0; //só pra consulta, não alterar na engine
    [SerializeField] GhostMove blinky;
    [SerializeField] GhostMove inky;
    [SerializeField] GhostMove pinky;
    [SerializeField] GhostMove clyde;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI readyText;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] GameObject retryButton;
    [SerializeField] AudioClip initialSound;
    [SerializeField] AudioClip eatPacdotSound;
    [SerializeField] AudioClip eatEnergizerSound;
    [SerializeField] AudioClip eatGhostSound;
    [SerializeField] AudioClip dieSound;
    [SerializeField] AudioClip ghostSound;
    [SerializeField] AudioClip frightenedSound;
    [SerializeField] float startTime = 2f;


    //cached references
    Pacdot[] pacdots;
    Energizer[] energizers;
    Pacman pacman;
    AudioSource GMAudioSource;
    AudioSource CameraAudioSource;

    //public variables
    public static bool scared;
    public float scareLength;

    //private variables
    private float timeToCalm;
    private bool started = false;
    private float timeToStart = 0;
    private bool dead = false;
    private int pacdotCount = 0;



    // Use this for initialization
    void Start()
    {
        pacman = FindObjectOfType<Pacman>();
        readyText.gameObject.SetActive(true);
        winText.gameObject.SetActive(false);
        retryButton.SetActive(false);
        GMAudioSource = GetComponent<AudioSource>();

        energizers = FindObjectsOfType<Energizer>();
        pacdots = FindObjectsOfType<Pacdot>();

        CameraAudioSource = Camera.main.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            timeToStart += Time.deltaTime;
            if (timeToStart >= startTime)
            {
                //cameraAudioSource.enabled = false;
                readyText.gameObject.SetActive(false);
                started = true;
                timeToStart = 0;
            }
            PlayInitialSound();

            GameWin();
        }

        scoreText.text = score.ToString();

        if (scared && timeToCalm <= Time.time)
        {
            CalmGhosts();
        }
    }

    private void GameWin()
    {
        foreach (Pacdot pacdot in pacdots)
        {
            if (!pacdot.enabled)
            {
                pacdotCount++;
                if (pacdotCount == pacdots.Length)
                {
                    winText.gameObject.SetActive(true);
                    GameOver();
                }
            }
            else
            {
                pacdotCount = 0;
            }
        }
    }

    public Pacdot[] GetPacdots()
    {
        return pacdots;
    }

    public void GameOver()
    {
        dead = true;
        timeToStart = 0;
        retryButton.SetActive(true);
    }

    public void ScareGhosts()
    {
        scared = true;

        blinky.Frighten();
        pinky.Frighten();
        inky.Frighten();
        clyde.Frighten();
        timeToCalm = Time.time + scareLength;
    }

    private void CalmGhosts()
    {
        scared = false;

        blinky.Calm();
        pinky.Calm();
        inky.Calm();
        clyde.Calm();

        pacman.ResetKillstreak();
    }

    public void AddScore(float points)
    {
        score += points;
    }

    public float GetScore()
    {
        return score;
    }

    public void Reset()
    {
        pacman.transform.position = new Vector3(15f, 11f, 0);
        blinky.transform.position = new Vector3(15f, 20f, 0);
        pinky.transform.position = new Vector3(14.5f, 17f, 0);
        inky.transform.position = new Vector3(16.5f, 17f, 0);
        clyde.transform.position = new Vector3(12.5f, 17f, 0);

        blinky.ResetCur();
        inky.ResetCur();
        pinky.ResetCur();
        clyde.ResetCur();

        score = 0;
        pacman.ResetLives();
        pacman.gameObject.SetActive(true);

        foreach (Pacdot pacdot in pacdots)
        {
            pacdot.gameObject.SetActive(true);
        }

        foreach (Energizer energizer in energizers)
        {
            energizer.gameObject.SetActive(true);
        }

        started = false;
        dead = false;
        retryButton.SetActive(false);
        winText.gameObject.SetActive(false);
        readyText.gameObject.SetActive(true);
    }

    public bool GetStarted()
    {
        return started;
    }

    public bool GetDead()
    {
        return dead;
    }

    private void PlayInitialSound()
    {
        if (!GMAudioSource.isPlaying && !started)
        {
            GMAudioSource.PlayOneShot(initialSound);
        }
    }

    public void PlayEatPacdotSound()
    {
        if (!CameraAudioSource.isPlaying && started)
        {
            CameraAudioSource.PlayOneShot(eatPacdotSound);
        }
    }

    public void PlayEatEnergizerSound()
    {
        CameraAudioSource.PlayOneShot(eatEnergizerSound);
    }

    public void PlayDieSound()
    {
        GMAudioSource.PlayOneShot(dieSound);
    }

    public void PlayEatGhostSound()
    {
        CameraAudioSource.PlayOneShot(eatGhostSound);
    }

    public void PlayGhostSound()
    {
        if (!GMAudioSource.isPlaying && started)
        {
            GMAudioSource.PlayOneShot(ghostSound);
        }
    }

    public void PlayFrightenedGhostSound()
    {
        if (!GMAudioSource.isPlaying && started)
        {
            GMAudioSource.PlayOneShot(frightenedSound);
        }
    }

    public void isDead(bool toggle)
    {
        dead = toggle;
    }
}
