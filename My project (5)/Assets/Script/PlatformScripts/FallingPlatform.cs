using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    //Falling variables
    public float fallDelay = 1f;
    public float destroyDelay = 1f;
    private bool isFalling = false;
    private float fallTimer = 1f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isFalling)
        {
            fallTimer -= Time.deltaTime;

            if (fallTimer <= 0f)
            {
                //If the state of falling is true, we change the rb to dynamic and destroy it after the timer is up
                rb.bodyType = RigidbodyType2D.Dynamic;
                Destroy(gameObject, destroyDelay);
                isFalling = false; 
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Player"))
        {
            isFalling = true;
            fallTimer = fallDelay;
        }
    }
}
