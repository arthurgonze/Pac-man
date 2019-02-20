using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] float points = 42;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name == "pacman")
        {
            gameManager.PlayEatPacdotSound();
            gameManager.AddScore(points);
            gameObject.SetActive(false);
        }
    }
}