using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public GameObject outerWallTile;
    public GameObject floorTile;
    public GameObject rock;
    public GameObject tree;
    public GameObject objective;

    public int columns = 8;
    public int rows = 8;
    public int rocks = 10;
    public int trees = 20;
    public int objectives = 20;

    public LayerMask blockingLayer;

    private Transform boardHolder;

    private void Start()
    {
        boardSetup();
        placeRocks();
        placeTrees();
        placeObjective();
    }

    void boardSetup ()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x <= columns; x++)
        {
            for (int y = -1; y <= rows; y++)
            {
                GameObject toInstantiate = floorTile;

                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTile;
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity);

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    void placeRocks()
    {
        for (int i = 0; i < rocks; i++)
        {

            Vector3 position = new Vector3(Random.Range(1, columns - 2), Random.Range(1, rows - 2), 0f);

            //Check that the position is open
            RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(0.9f, 0.9f), 0f, new Vector2(0, 0), 0f, blockingLayer);
            if (hit.collider == null)
            {
                GameObject instance = Instantiate(rock, position, Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
            else
                //To keep the same numRocks when none is added
                rocks++;
        }
    }

    void placeTrees()
    {
        for (int i = 0; i < trees; i++)
        {
            Vector3 position = new Vector3(Random.Range(1, columns - 2), Random.Range(1, rows - 2), 0f);

            //Check the position is open
            RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(0.9f, 0.9f), 0f, new Vector2(0, 0), 0f, blockingLayer);
            if (hit.collider == null)
            {
                GameObject instance = Instantiate(tree, position, Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
            else
                trees++;
        }
    }

    void placeObjective()
    {
        for (int i = 0; i < objectives; i++)
        {
            Vector3 position = new Vector3(Random.Range(1, columns - 2), Random.Range(1, rows - 2), 0f);

            //Check the position is open
            RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(0.9f, 0.9f), 0f, new Vector2(0, 0), 0f, blockingLayer);
            if (hit.collider == null)
            {
                GameObject instance = Instantiate(objective, position, Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
            else
                objectives++;
        }
    }
}
