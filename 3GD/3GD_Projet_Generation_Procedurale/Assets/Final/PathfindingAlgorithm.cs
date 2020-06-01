using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    //Class that contain the node structure which can be used in the A* algorithm
    public class Node
    {
        public Vector2 position;
        public Node parent;

        public float g;
        public float h;
        public float f;

        public Node(int x, int y, Node parent)
        {
            position = new Vector2(x, y);
            this.parent = parent;
        }

        public void CalculateManhattanDistance(Node goal)
        {
            float result = Mathf.Abs(this.position.x - goal.position.x) + Mathf.Abs(this.position.y - goal.position.y);
            h = result;
        }

        public void CalculateEuclidianDistance(Node goal)
        {
            float result = Vector2.Distance(this.position, goal.position); //(int)Mathf.Sqrt(((position.x - goal.position.x) * (position.x - goal.position.x) + (position.y - goal.position.y) * (position.y - goal.position.y)));   
            h = result;
        }

        public void CalculateF()
        {
            f = g + h;
        }
    }

    //Class that contains the A* Algorithm
    public class AStarAlgorithm 
    {
        //Create a path between start node and end node
        public Node[] AStar(Node start, Node end)
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();
            openList.Add(start);
            start.g = 0;
            //start.CalculateManhattanDistance(end);
            start.CalculateEuclidianDistance(end);
            start.f = start.g + start.h;
            while(openList.Count != 0)
            {
                Node current = FindNodeWithLowestF(openList);
                Debug.Log(current.position.ToString() + ", g = " + current.g.ToString() + ", h = " + current.h.ToString() + ", f = " + current.f.ToString());
                if(current.position == end.position)
                {
                    //return closedList.ToArray();
                    return ConstructPath(current);
                }
                openList.Remove(current);
                closedList.Add(current);
                foreach(Node neighbor in FindNeighbors(current))
                {
                    if(!closedList.Exists(nodeIn => { return (nodeIn.position == neighbor.position); }))
                    {
                        //Test 1
                        /*
                        //neighbor.CalculateManhattanDistance(end);
                        neighbor.CalculateEuclidianDistance(end);
                        neighbor.f = neighbor.g + neighbor.h;
                        if (!openList.Exists(nodeIn => { return (nodeIn.position == neighbor.position); }))
                            openList.Add(neighbor);
                        else
                        {
                            Node openNeighbor = openList.Find(nodeIn => { return (nodeIn.position == neighbor.position); });
                            if (openNeighbor != null)
                            {
                                if (neighbor.g < openNeighbor.g)
                                {
                                    openNeighbor.g = neighbor.g;
                                    openNeighbor.parent = neighbor.parent;
                                }
                            }
                        }*/

                        //Test 2
                        neighbor.CalculateEuclidianDistance(end);
                        neighbor.f = neighbor.g + neighbor.h;
                        if (!openList.Exists(nodeIn => { return (nodeIn.position == neighbor.position); }))
                            openList.Add(neighbor);
                        float possibleGScore = current.g + neighbor.g;
                        if (possibleGScore >= neighbor.g)
                            continue;
                        neighbor.g = possibleGScore;
                        neighbor.CalculateEuclidianDistance(end);
                        neighbor.f = neighbor.g + neighbor.h;
                        

                    }
                } 
            }
            return new Node[0];
        }

        //Find the neighbors of a node
        Node[] FindNeighbors(Node node)
        {
            Node[] neighbors = new Node[4];
            neighbors[0] = new Node((int)node.position.x + 1, (int)node.position.y, node);
            neighbors[1] = new Node((int)node.position.x, (int)node.position.y + 1, node);
            neighbors[2] = new Node((int)node.position.x - 1, (int)node.position.y, node);
            neighbors[3] = new Node((int)node.position.x, (int)node.position.y - 1, node);
            foreach(Node neighbor in neighbors)
            {
                neighbor.g = node.g + 1f;
                neighbor.parent = node;
            }
            return neighbors;
        }


        //Construct the path thanks to parent nodes
        Node[] ConstructPath(Node node)
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(node);
            while(node.parent != null)
            {
                node = node.parent;
                nodes.Add(node);
            }
            return nodes.ToArray();
        }

        //Find the node in the List which have the lowest f (or h here because there is no obstacle, thus we can go straight))
        Node FindNodeWithLowestF(List<Node> nodes)
        {
            float minF = float.MaxValue;
            float minH = float.MaxValue;
            Node result = null;
            foreach(Node node in nodes)
            {
                if (node.h <= minH)
                {
                    minH = node.h;
                    minF = node.f;
                    result = node;
                }
            }
            return result;
        }
    }
}
