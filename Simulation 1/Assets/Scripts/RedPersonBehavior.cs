using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RedPersonBehavior : MonoBehaviour {

    public Rigidbody2D rb;
    public Vector2 newPosition;
    public LayerMask blockingLayer;
    public float speed;
    public Camera cam;
    public GameObject selectionBox;

    private List<Vector2> path = new List<Vector2>();
    private int currentIndex;
    private Vector2 changeInPosition;
    private Boolean atPosition;

   // private GameObject selectionBox;

    public AStarPathfinding pathfindingScriptA;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        atPosition = true;

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
        if (atPosition && Input.GetButtonUp("Fire2"))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            //Round to the nearest X
            float roundingNumber = Math.Abs(mousePos.x - (int)mousePos.x);
            float boxPositionX;
            if (roundingNumber < 0.5)
                boxPositionX = (int)mousePos.x;
            else
                boxPositionX = (int)mousePos.x + 1;
            //Round to the nearest Y
            roundingNumber = Math.Abs(mousePos.y - (int)mousePos.y);
            float boxPositionY;
            if (roundingNumber < 0.5)
                boxPositionY = (int)mousePos.y;
            else
                boxPositionY = (int)mousePos.y + 1;
                
            Vector3 boxPosition = new Vector3(boxPositionX, boxPositionY, 0f);

            //Create selection box
            selectionBox.SetActive(true);
            selectionBox.transform.position = boxPosition;

            //Pathfind to it
            newPosition = new Vector2(boxPositionX, boxPositionY);
            path = new List<Vector2>(pathfindingScriptA.Pathfinding(rb.position, newPosition, blockingLayer));
            currentIndex = 0;
        }
    }

    /*void Update()
    {
        if (atPosition)
        {
            selectionBox = GameObject.Find("SelectionBox(Clone)");
            if (selectionBox != null)
            {
                Vector2 selectionBox2D = new Vector2(selectionBox.transform.position.x, selectionBox.transform.position.y);
                if (selectionBox2D != newPosition)
                {
                    newPosition = selectionBox2D;
                    path.Clear();
                    path = new List<Vector2>(pathfindingScriptA.Pathfinding(rb.position, newPosition, blockingLayer));
                    currentIndex = 0;
                }
                if (selectionBox2D == rb.position)
                    Destroy(selectionBox);
            }
        }
    }*/

    Vector2 FindDirection(Vector2 changePos)
    {
        Vector2 direction;
        float multiplier;
        multiplier = (float)(1 / Math.Sqrt((changePos.x * changePos.x) + (changePos.y * changePos.y)));
        direction = new Vector2((changePos.x * multiplier), (changePos.y * multiplier));
        return direction;
    }
}
