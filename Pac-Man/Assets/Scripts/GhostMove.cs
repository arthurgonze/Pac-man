using UnityEngine;
using System.Collections;
using System;

public class GhostMove : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed = 0.3f;

    //cached references
    GameManager gameManager;
    Pacman pacman;

    private int points = 200;
    private int cur = 0;

    //scare variables
    private bool isWhite = false;
    public float scatterLength = 5f;
    private float toggleInterval;
    private float timeToWhite;
    private float timeToToggleWhite;
    private bool runAway = false;

    //respawn variables
    private bool respawn = false;
    [SerializeField] float respawnTime = 2f;
    private float respawnTimer = 0;

    private void Start()
    {
        pacman = FindObjectOfType<Pacman>();
        gameManager = FindObjectOfType<GameManager>();
        toggleInterval = gameManager.scareLength * 0.33f * 0.20f;//magic numbers
        GetComponent<Animator>().SetBool("Run", false);
    }

    void FixedUpdate()
    {
        if(!gameManager.GetDead())
        {
            respawnTimer += Time.deltaTime;
            RespawnGhost();

            if (gameManager.GetStarted())
            {
                
                Movement();
                MovementAnimation();

                if (runAway)
                {
                    RunAway();
                    //gameManager.playFrightenedGhostSound();
                }else
                {
                    //gameManager.playGhostSound();
                }
            }
        }
    }

    private void RespawnGhost()
    {
        if (respawn)
        {
            if (respawnTimer >= respawnTime)
            {
                isWhite = false;
                runAway = false;
                GetComponent<Animator>().SetBool("Run", false);
                this.transform.position = new Vector3(14.5f, 17f, 0);
                this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
                cur = 0;
                respawnTimer = 0;
                respawn = false;
            }
        }
    }

    private void Movement()
    {
        // Waypoint not reached yet? then move closer
        if (transform.position != waypoints[cur].position)
        {
            Vector2 p = Vector2.MoveTowards(transform.position, waypoints[cur].position, speed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }
        // Waypoint reached, select next one
        else
        {
            cur = (cur + 1) % waypoints.Length;
        }
    }

    private void MovementAnimation()
    {
        // Animation
        Vector2 dir = waypoints[cur].position - transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
        
    }

    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name == "pacman" && !runAway)
        {
            pacman.LoseLife();
        }

        if (co.name == "pacman" && runAway)
        {
            gameManager.PlayEatGhostSound();

            pacman.AddKillStreak();
            gameManager.AddScore(points*pacman.GetKillStreak());

            //respawn ghost
            respawn = true;
            respawnTimer = 0;
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
    }
    
    void RunAway()
    {
        GetComponent<Animator>().SetBool("Run", true);

        if (Time.time >= timeToWhite && Time.time >= timeToToggleWhite)
        {
            ToggleBlueWhite();
        }

    }

    public void Frighten()
    {
        runAway = true;

        timeToWhite = Time.time + gameManager.scareLength * 0.66f;//magic number
        timeToToggleWhite = timeToWhite;
        GetComponent<Animator>().SetBool("Run_White", false);
    }

    public void Calm()
    {
        runAway = false;
        timeToToggleWhite = 0;
        timeToWhite = 0;
        GetComponent<Animator>().SetBool("Run_White", false);
        GetComponent<Animator>().SetBool("Run", false);
    }
    
    public void ToggleBlueWhite()
    {
        isWhite = !isWhite;
        GetComponent<Animator>().SetBool("Run_White", isWhite);
        timeToToggleWhite = Time.time + toggleInterval;
    }

    public void ResetCur()
    {
        cur = 0;
    }
}