using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    [SerializeField] float speed = 0.4f;
    [SerializeField] int lives = 3;
    [SerializeField] GameObject[] pointSprites;
    [SerializeField] GameObject[] lifeSprites;
    [Range(0, 4)] [SerializeField] int killCount = 0;

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
    void FixedUpdate()
    {
        if(gameManager.GetStarted() && !gameManager.GetDead())
        {
            Movement();
        }
    }

    private void Movement()
    {
        // Move closer to Destination
        Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        MovementInput();
        MovementAnimation();
    }

    private void MovementAnimation()
    {
        // Animation Parameters
        Vector2 dir = dest - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    private void MovementInput()
    {
        // Check for Input if not moving
        if ((Vector2)transform.position == dest)
        {
            //up
            if (Input.GetAxis("Vertical") > 0 && Valid(Vector2.up))
            {
                dest = (Vector2)transform.position + Vector2.up;
            }
            //right
            if (Input.GetAxis("Horizontal") > 0 && Valid(Vector2.right))
            {
                dest = (Vector2)transform.position + Vector2.right;
            }
            //down
            if (Input.GetAxis("Vertical") < 0 && Valid(-Vector2.up))
            {
                dest = (Vector2)transform.position - Vector2.up;
            }
            //left
            if (Input.GetAxis("Horizontal") < 0 && Valid(-Vector2.right))
            {
                dest = (Vector2)transform.position - Vector2.right;
            }
        }
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

        //Note: I simply casted the Line from the point next to Pac-Man (pos + dir) to Pac-Man himself (pos).
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
            gameManager.GameOver();
        }
        
    }

    public int GetLives()
    {
        return lives;
    }

    public void ResetLives()
    {
        lives=3;

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
        gameManager.isDead(false);
        ResetPos();
    }
}
