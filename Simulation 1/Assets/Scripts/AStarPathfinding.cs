using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AStarPathfinding : MonoBehaviour {

    private struct Node
    {
        public Vector2 position;
        public Vector2 parentPosition;

        //The estimated distance to end using Manhattan method
        public int H; //H = 10*(abs(startX-endX) + abs(startY-endY))
        //Movement cost from the starting point
        public int G;
        public int F; //F = H + G
    }

    public List<Vector2> Pathfinding(Vector2 start, Vector2 end, LayerMask blockingLayer)
    {
        //Check if the path needs to be generated at all
        //If start and end are the same
        List<Vector2> noPath = new List<Vector2>();
        noPath.Add(start);
        if (start == end)
            return noPath;

        //If end is unwalkable
        RaycastHit2D hit = Physics2D.BoxCast(end, new Vector2(0.9f, 0.9f), 0, new Vector2(0, 0), 0, blockingLayer);
        if (hit.collider != null)
            return noPath;

        //Set that has not yet been explored
        List<Node> openSet = new List<Node>();
        //Set that has been explored
        List<Node> closedSet = new List<Node>();

        //Start with the starting position
        int newH = 10 * (int)(Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y)); //Manhattan method to find heuristic
        openSet.Add(new Node { position = start, parentPosition = start, F = newH, G = 0, H = newH });

        //Do until the path is found or deemed nonexistent
        while (openSet.Count != 0)
        {
            //Use the cell with the lowest F score
            int tempIndex = FindLowestFIndex(openSet);

            //Check adjacent squares and add them to open set if they are viable
            AddToOpenSet(openSet, closedSet, new Vector2(0, 1), end, tempIndex, blockingLayer); //North
            AddToOpenSet(openSet, closedSet, new Vector2(0, -1), end, tempIndex, blockingLayer); //South
            AddToOpenSet(openSet, closedSet, new Vector2(1, 0), end, tempIndex, blockingLayer); //East
            AddToOpenSet(openSet, closedSet, new Vector2(-1, 0), end, tempIndex, blockingLayer); //West
            AddToOpenSet(openSet, closedSet, new Vector2(1, 1), end, tempIndex, blockingLayer); //Northeast
            AddToOpenSet(openSet, closedSet, new Vector2(1, -1), end, tempIndex, blockingLayer); //Southeast
            AddToOpenSet(openSet, closedSet, new Vector2(-1, -1), end, tempIndex, blockingLayer); //Southwest
            AddToOpenSet(openSet, closedSet, new Vector2(-1, 1), end, tempIndex, blockingLayer); //Northwest

            //Move the cell to the closed list
            closedSet.Add(openSet[tempIndex]);
            openSet.RemoveAt(tempIndex);

            //Check if the item we added to the closed list is the destination
            Node tempNode = closedSet[closedSet.Count - 1];
            Vector2 checkDifference = tempNode.position - end;
            if (Math.Abs(checkDifference.x) < .1 && Math.Abs(checkDifference.y) < .1) //Close enough?
            {
                //Node tempNode = closedSet[closedSet.Count - 1];
                closedSet.RemoveAt(closedSet.Count - 1);
                closedSet.Add(new Node { position = end, parentPosition = tempNode.parentPosition, F = tempNode.F, G = tempNode.G, H = tempNode.H });
            }

            if (tempNode.position == end)
            {
                return (ReconstructPath(closedSet, start));
            }
        }

        //No path is found
        return noPath;
    }

    //Finds where the lowest F score is
    int FindLowestFIndex(List<Node> set)
    {
        int lowestFScore = set[0].F;
        int lowestFIndex = 0;
        for (int i = 1; i < set.Count; i++)
        {
            if (set[i].F < lowestFScore)
            {
                lowestFScore = set[i].F;
                lowestFIndex = i;
            }
        }
        return lowestFIndex;
    }

    //Checks that the space is walkable and not already in a set then adds it to the open set if it is
    void AddToOpenSet(List<Node> openSet, List<Node> closedSet, Vector2 direction, Vector2 end, int index, LayerMask blockingLayer)
    {
        Vector2 tempPosition = openSet[index].position;
        //Check if the space is walkable
        RaycastHit2D hit = Physics2D.BoxCast(tempPosition, new Vector2(0.9f, 0.9f), 0, direction, 1, blockingLayer);
        //Check if the space is already in open or closed set
        if (hit.collider == null && !openSet.Exists(x => x.position == tempPosition + direction) && !closedSet.Exists(x => x.position == tempPosition + direction))
        {
            //Manhattan method to find heuristic
            int newH = 10 * (int)(Math.Abs(tempPosition.x + direction.x - end.x) + Math.Abs(tempPosition.y + direction.y - end.y));
            //Add 10 for orthogonal, 14 for diagonal (because sqrt(2) is about 1.4)
            int newG;
            if (direction.x != 0 && direction.y != 0)
                newG = openSet[index].G + 14;
            else
                newG = openSet[index].G + 10;

            openSet.Add(new Node { position = tempPosition + direction, parentPosition = tempPosition, H = newH, G = newG, F = newH + newG });
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
                    openSet.Add(new Node { position = openSet[index].position, parentPosition = betterPath.position, G = newG, H = openSet[index].H, F = newG + openSet[index].H });
                    openSet.RemoveAt(index);
                }
            }
            //Orthogonal
            else
            {
                int newG = betterPath.G + 10;
                if (newG < openSet[index].G)
                {
                    openSet.Add(new Node { position = openSet[index].position, parentPosition = betterPath.position, G = newG, H = openSet[index].H, F = newG + openSet[index].H });
                    openSet.RemoveAt(index);
                }
            }
        }
    }

    //Starting from the end, reconstructs the path
    List<Vector2> ReconstructPath(List<Node> closedSet, Vector2 start)
    {
        List<Vector2> path = new List<Vector2>();

        //Start with the last element (presumably at the end position)
        Node current = closedSet[closedSet.Count - 1];

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

    Vector2 FindDirection(Vector2 changePos)
    {
        Vector2 direction;
        float multiplier;
        multiplier = (float)(1 / Math.Sqrt((changePos.x * changePos.x) + (changePos.y * changePos.y)));
        direction = new Vector2((changePos.x * multiplier), (changePos.y * multiplier));
        return direction;
    }
}
