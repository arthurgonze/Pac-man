using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    [SerializeField] float speed = 0.4f;
    [SerializeField] int lives = 3;
    [SerializeField] GameObject[] pointSprites;
    [SerializeField] GameObject[] lifeSprites;
    [Range(0, 4)][SerializeField] int killCount = 0;

    Vector2 dest = Vector2.zero;
    GameManager gameManager;
    private bool deadPlaying = false;


    // Use this for initialization
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        dest = transform.position;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     MovementInput();
    //     MovementAnimation();
    // }

    private void Update()
    {
        if (gameManager.GetStarted() && !gameManager.GetDead() && !gameManager.isGameOver())
            Movement();
    }

    private void Movement()
    {
        // Move closer to Destination
        // Vector2 p = Vector2.MoveTowards(transform.position, dest, speed*Time.deltaTime);
        // GetComponent<Rigidbody2D>().MovePosition(p);

        if (MovementInput())
            MovementAnimation();
        else
            IdleAnimation();
    }

    private void MovementAnimation()
    {
        // Animation Parameters
        Vector2 dir = (Vector2)transform.position - dest;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    private void IdleAnimation()
    {
        GetComponent<Animator>().SetFloat("DirX", 1);
        GetComponent<Animator>().SetFloat("DirY", 0);
    }

    private bool MovementInput()
    {
        // Check for Input if not moving
        // if ((Vector2)transform.position == dest)
        // {
        dest = (Vector2)transform.position;
        if (Input.GetAxis("Vertical") > 0 && Valid(Vector2.up * Time.deltaTime * speed))
        {
            transform.position = (Vector2)transform.position + (Vector2.up * Time.deltaTime * speed);
            return true;
        }

        if (Input.GetAxis("Horizontal") > 0 && Valid(Vector2.right * Time.deltaTime * speed))
        {
            transform.position = (Vector2)transform.position + (Vector2.right * Time.deltaTime * speed);
            return true;
        }


        if (Input.GetAxis("Vertical") < 0 && Valid(-Vector2.up * Time.deltaTime * speed))
        {
            transform.position = (Vector2)transform.position - (Vector2.up * Time.deltaTime * speed);
            return true;
        }


        if (Input.GetAxis("Horizontal") < 0 && Valid(-Vector2.right * Time.deltaTime * speed))
        {
            transform.position = (Vector2)transform.position - (Vector2.right * Time.deltaTime * speed);
            return true;
        }
        
        return false;
    }

    /**
     * A way to find out if Pac-Man can move into a certain direction or if there is a wall. 
     */
    bool Valid(Vector2 dir)
    {
        // Cast Line from 'next to Pac-Man' to 'Pac-Man'
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
        return (hit.collider == GetComponent<Collider2D>());
    }

    public void LoseLife()
    {
        gameManager.PlayDieSound();
        gameManager.isDead(true);

        StartCoroutine("PlayDeadAnimation");

        lives--;
        lifeSprites[lives].SetActive(false);
        if (lives <= 0)
        {
            gameManager.isDead(true);
            gameManager.GameOver();
        }

    }

    public int GetLives()
    {
        return lives;
    }

    public void ResetLives()
    {
        lives = 3;

        foreach (GameObject life in lifeSprites)
        {
            life.SetActive(true);
        }
    }

    public void ResetKillstreak()
    {
        killCount = 0;
    }

    public int GetKillStreak()
    {
        return killCount;
    }

    public void AddKillStreak()
    {
        killCount++;
        Instantiate(pointSprites[killCount - 1], transform.position, Quaternion.identity);
    }

    private void ResetPos()
    {
        this.transform.position = new Vector2(15f, 11f);
        dest = new Vector2(15f, 11f);
        GetComponent<Animator>().SetFloat("DirX", 1);
        GetComponent<Animator>().SetFloat("DirY", 0);
    }

    IEnumerator PlayDeadAnimation()
    {
        deadPlaying = true;
        GetComponent<Animator>().SetBool("Die", true);
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("Die", false);
        deadPlaying = false;
        if (!gameManager.isGameOver())
        {
            gameManager.isDead(false);
        }
        ResetPos();
    }
}
