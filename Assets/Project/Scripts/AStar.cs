using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that holds information about a certain position
// sot that it is used in a pathfinding algorithm
class Node
{
 
    //Nodes have x and y positions (horizontal & vertical)
    public int posX;
    public int posY;

    //cost to travel from one Node to another
    public int g = int.MaxValue;
    //Hueristic that *estimates* cost of closest path
    public int f = int.MaxValue;

    //Nodes have references to other nodes in order to build a path
    public Node parent;

    //value of the node
    public NavTile value = null;

    //Constructor
    public Node(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
    }

}
public class AStar : MonoBehaviour
{
    //public variables
    public GameObject player;
    public GameObject enemy;
    public GameObject backgroundContainer;

    //private variables
    private int mapWidth;
    private int mapHeight;
    private Node[,] nodeMap;
    // Use this for initialization
    void Start()
    {
        //Preload values
        mapHeight = backgroundContainer.transform.childCount;
        mapWidth = backgroundContainer.transform.GetChild(0).childCount;

        //Parse the map
        nodeMap = new Node[mapWidth, mapHeight];
        Node start = null;
        Node goal = null;

        for(int y = 0; y < mapHeight; y++)
        {
            Transform backgroundRow = backgroundContainer.transform.GetChild(y);

            for(int x = 0; x < mapWidth; x++)
            {
                NavTile tile = backgroundRow.GetChild(x).GetComponent<NavTile>();

                Node node = new Node(x, y);
                node.value = tile;
                nodeMap[x, y] = node;
            }
        }

        start = FindNode(player);
        goal = FindNode(enemy);

        Debug.Log("Player is on " + start.posX + ", " + start.posY);
        Debug.Log("Enemy is on " + goal.posX + ", " + goal.posY);
        //Execute AStar algorithm
        //List<Node> nodePath = ExecuteAStar(start, goal);
    }

    //find the node the obj is on
    private Node FindNode(GameObject obj)
    {
        Collider2D[] collidingObjects = Physics2D.OverlapCircleAll(obj.transform.position, 0.2f);
        foreach (Collider2D collidingObject in collidingObjects)
        {
            if (collidingObject.gameObject.GetComponent<NavTile>() != null)
            {
                //tile obj is on
                NavTile tile = collidingObject.gameObject.GetComponent<NavTile>();
                //find node that contains the tile
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Node node = nodeMap[x, y];
                        if (node.value == tile)
                        {
                            return node;
                        }
                    }
                }
            }
        }

        return null;
    }
    private List<Node> ExecuteAStar(Node start, Node goal)
    {
        //holds potential best path nodes that should be visited
        //always starts from origin
        List<Node> openList = new List<Node>() { start };

        //keeps track of all nodes that have been visited
        List<Node> closedList = new List<Node>();

        //Initialize start node
        start.g = 0;
        start.f = CalculateHeuristicValue(start, goal);

        //Main algorithm
        while (openList.Count > 0)
        {
            //get the node with the lowest estimated cost
            //to reach the target

            Node current = openList[0];
            foreach (Node node in openList)
            {
                if (node.f < current.f)
                {
                    current = node;
                }
            }
            //check if the target has been reached
            if (current == goal)
            {
                return BuildPath(goal);
            }

            //make sure current node is not revisited
            openList.Remove(current);
            closedList.Add(current);

            //execute algorithm in current node's neighbors
            List<Node> neighbors = GetNeighborNodes(current);
            foreach (Node neighbor in neighbors)
            {
                //if neighbor has been visited, ignore it
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                //if neighbor has not been schedule for visit, add it
                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }

                //calculate a new G value and verify if this value
                //is better than whatever is stored in the neighbor
                int candidateG = current.g + 1;

                if (candidateG >= neighbor.g)
                {
                    //if the g value is greater than or equal,
                    //there is a better path already stored
                    continue;
                }

                else
                {
                    //otherwise we found a better path
                    neighbor.parent = current;
                    neighbor.g = candidateG;
                    neighbor.f = neighbor.g + CalculateHeuristicValue(neighbor, goal);

                }

            }
        }

        //No more nodes to check, no valid path
        return new List<Node>();
    }

    private List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighbors = new List<Node>();

        if (node.posX - 1 >= 0)
        {
            Node candidate = nodeMap[node.posX - 1, node.posY];
            if (candidate.value != false)
            {
                neighbors.Add(candidate);
            }
        }

        if (node.posX + 1 <= mapWidth - 1)
        {
            Node candidate = nodeMap[node.posX + 1, node.posY];
            if (candidate.value != false)
            {
                neighbors.Add(candidate);
            }
        }

        if(node.posY -1 >= 0)
        {
            Node candidate = nodeMap[node.posX, node.posY - 1];
            if(candidate.value != false)
            {
                neighbors.Add(candidate);
            }
        }

        if(node.posY + 1 <= mapHeight - 1)
        {
            Node candidate = nodeMap[node.posX, node.posY + 1];
            if(candidate.value != false)
            {
                neighbors.Add(candidate);
            }
        }
        return neighbors;
    }
    //A simple estimate of the distance (manhattan distance)
    private int CalculateHeuristicValue(Node node1, Node node2)
    {
        return Mathf.Abs(node1.posX - node2.posX) + Mathf.Abs(node1.posY - node2.posY);
    }

    private List<Node> BuildPath(Node node)
    {
        List<Node> path = new List<Node>() { node };
        
        while(node.parent != null)
        {
            node = node.parent;
            path.Add(node);
        }

        return path;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
