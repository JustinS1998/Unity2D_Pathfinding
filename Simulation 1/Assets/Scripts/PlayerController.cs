using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb;
    public Boolean isPlayerTurn;
    public LayerMask blockingLayer;
    public float speed;

    private Vector2 newPosition;
    private float changeX;
    private float changeY;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isPlayerTurn = true;
        newPosition = rb.position;
        changeX = 0f; changeY = 0f;
    }


    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if ((isPlayerTurn == true) && (horizontal != 0 || vertical != 0))
        {
            if (horizontal != 0 && vertical == 0)
            {
                changeX = horizontal;

                //Check the new position to see if it is open
                RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2 (0.9f, 0.9f), 0f, new Vector2(changeX, 0f), 1f, blockingLayer);
                if (hit.collider == null)
                {
                    newPosition = rb.position + new Vector2(changeX, 0);
                    isPlayerTurn = false;
                }
                else
                {
                    //If it does collide, no new position
                    isPlayerTurn = true;
                }
            }
            else if (vertical != 0 && horizontal == 0)
            {
                changeY = vertical;
                
                //Check the new position to see if it is open
                RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.9f, 0.9f), 0f, new Vector2(0f, changeY), 1f, blockingLayer);
                if (hit.collider == null)
                {
                    newPosition = rb.position + new Vector2(0, changeY);
                    isPlayerTurn = false;
                }
                else
                {
                    //If it does collide, no new position
                    isPlayerTurn = true;
                }
            }
        }
    }


    void FixedUpdate()
    {
        if (newPosition != rb.position)
            rb.MovePosition(rb.position + new Vector2(changeX, changeY) * speed * Time.deltaTime);
        else if (newPosition == rb.position)
        {
            changeX = 0f; changeY = 0f;
            isPlayerTurn = true;
        }
    }
}
