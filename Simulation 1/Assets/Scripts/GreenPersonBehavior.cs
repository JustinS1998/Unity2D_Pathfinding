using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GreenPersonBehavior : MonoBehaviour {

    public Rigidbody2D rb;
    public LayerMask blockingLayer;
    public float speed = 1;
    public DijkstraPathfinding pathfindingScriptD;
    public GameObject objective;
    public Boolean findNext;

    private List<Vector2> path = new List<Vector2>();
    private int currentIndex;
    private Vector2 changeInPosition;
    private Boolean atPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        atPosition = true;
        findNext = false;

        //Prevent range error in FixedUpdate
        currentIndex = 0;
        path.Add(rb.position);
    }

        void FixedUpdate()
        {
            if (path[currentIndex] != rb.position)
            {
                atPosition = false;

                changeInPosition = path[currentIndex] - rb.position;
                //Is close enough?
                if (Math.Abs(changeInPosition.x) < .01 && Math.Abs(changeInPosition.y) < .01)
                    rb.MovePosition(path[currentIndex]);

                //Move slower on diagonal
                else
                {
                    if (changeInPosition.x != 0 && changeInPosition.y != 0)
                        rb.MovePosition(rb.position + FindDirection(changeInPosition) * .71f * speed * Time.deltaTime);
                    else
                        rb.MovePosition(rb.position + FindDirection(changeInPosition) * speed * Time.deltaTime);
                }
            }
            else if (path[currentIndex] == rb.position)
            {
                atPosition = true;
                if (currentIndex < path.Count - 1) //WHY -1?! Can't figure this out but -0 gives a range error
                    currentIndex++;
            }
        }

        void Update()
        {
            if (Input.GetButtonDown("Jump"))
                findNext = true;
            if (findNext && atPosition)
            {
                path.Clear();
                path = pathfindingScriptD.Pathfinding(rb.position, objective, blockingLayer);
                currentIndex = 0;
                findNext = false;
            }
        }

        Vector2 FindDirection(Vector2 changePos)
        {
            Vector2 direction;
            float multiplier;
            multiplier = (float)(1 / Math.Sqrt((changePos.x * changePos.x) + (changePos.y * changePos.y)));
            direction = new Vector2((changePos.x * multiplier), (changePos.y * multiplier));
            return direction;
        }
}
