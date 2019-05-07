using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathfinding : MonoBehaviour {

    private struct Node
    {
        public Vector2 position;
        public Vector2 parentPosition;

        public int G;
    }

    public List<Vector2> Pathfinding(Vector2 start, GameObject objective, LayerMask blockingLayer)
    {
        //List of unexplored spaces
        List<Node> openSet = new List<Node>();
        //List of explored spaces
        List<Node> closedSet = new List<Node>();

        //Start with the starting position
        openSet.Add(new Node { position = start, parentPosition = start, G = 0 });

        //Do until all spaces have been checked
        while (openSet.Count != 0)
        {
            //Check the space with the lowest G
            int tempIndex = FindLowestG(openSet);

            //Check all adjacent spaces and add them to the open set if they are viable
            AddToOpenSet(openSet, closedSet, new Vector2(0, 1), tempIndex, blockingLayer); //North
            AddToOpenSet(openSet, closedSet, new Vector2(0, -1), tempIndex, blockingLayer); //South
            AddToOpenSet(openSet, closedSet, new Vector2(1, 0), tempIndex, blockingLayer); //East
            AddToOpenSet(openSet, closedSet, new Vector2(-1, 0), tempIndex, blockingLayer); //West
            AddToOpenSet(openSet, closedSet, new Vector2(1, 1), tempIndex, blockingLayer); //Northeast
            AddToOpenSet(openSet, closedSet, new Vector2(1, -1), tempIndex, blockingLayer); //Southeast
            AddToOpenSet(openSet, closedSet, new Vector2(-1, -1), tempIndex, blockingLayer); //Southwest
            AddToOpenSet(openSet, closedSet, new Vector2(-1, 1), tempIndex, blockingLayer); //Northwest

            //Move the cell to the closed list
            closedSet.Add(openSet[tempIndex]);
            openSet.RemoveAt(tempIndex);

            //Check if the item we added to the closed list is the destination
            Node tempNode = closedSet[closedSet.Count - 1];
            LayerMask objectiveLayer = LayerMask.GetMask(LayerMask.LayerToName(objective.layer));
            RaycastHit2D hit = Physics2D.BoxCast(tempNode.position, new Vector2(0.9f, 0.9f), 0f, new Vector2(0,0), 0f, objectiveLayer);
            if (hit)
            {
                return (ReconstructPath(closedSet, start));
            }
        }

        //If no path to target
        Debug.Log("No path");
        List<Vector2> noPath = new List<Vector2>();
        noPath.Add(start);
        return noPath;
    }

    int FindLowestG (List<Node> set)
    {
        //Start at 0
        int lowestScore = set[0].G;
        int lowestIndex = 0;

        for (int i = 1; i < set.Count; i++)
        {
            if (set[i].G < lowestScore)
            {
                lowestScore = set[i].G;
                lowestIndex = i;
            }
        }
        return lowestIndex;
    }

    //Checks that the space is walkable and not already in a set then adds it to the open set if it is
    void AddToOpenSet(List<Node> openSet, List<Node> closedSet, Vector2 direction, int index, LayerMask blockingLayer)
    {
        Vector2 tempPosition = openSet[index].position;
        //Check if the space is walkable
        RaycastHit2D hit = Physics2D.BoxCast(tempPosition, new Vector2(0.9f, 0.9f), 0, direction, 1, blockingLayer);
        //Check if the space is already in open or closed set
        if (hit.collider == null && !openSet.Exists(x => x.position == tempPosition + direction) && !closedSet.Exists(x => x.position == tempPosition + direction))
        {
            int newG;
            if (direction.x != 0 && direction.y != 0)
                newG = openSet[index].G + 14;
            else
                newG = openSet[index].G + 10;

            openSet.Add(new Node { position = tempPosition + direction, parentPosition = tempPosition, G = newG});
        }

        //If it is already on the open list, check to see if this path to the square is a better one
        if (openSet.Exists(x => x.position == tempPosition + direction))
        {
            Node betterPath = openSet.Find(x => x.position == tempPosition + direction);

            //See if the G value is better and change path if it is
            //Diagonal
            if (direction.x != 0 && direction.y != 0)
            {
                int newG = betterPath.G + 14;
                if (newG < openSet[index].G)
                {
                    openSet.Add(new Node { position = openSet[index].position, parentPosition = betterPath.position, G = newG});
                    openSet.RemoveAt(index);
                }
            }
            //Orthogonal
            else
            {
                int newG = betterPath.G + 10;
                if (newG < openSet[index].G)
                {
                    openSet.Add(new Node { position = openSet[index].position, parentPosition = betterPath.position, G = newG});
                    openSet.RemoveAt(index);
                }
            }
        }
    }

    //Starting from the end, reconstructs the path
    List<Vector2> ReconstructPath(List<Node> closedSet, Vector2 start)
    {
        List<Vector2> path = new List<Vector2>();

        //Start with the second to last element which one away from the destination
        Node current = closedSet[closedSet.Count - 1];
        //current = closedSet.Find(x => x.position == current.parentPosition);

        while (current.position != start)
        {
            //Add it to the path
            path.Add(current.position);

            //Find the next cell and make that the current cell
            current = closedSet.Find(x => x.position == current.parentPosition);
        }

        //Reverse it since it started at the end
        path.Reverse();
        return path;
    }
}
