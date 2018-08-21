using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that holds information about a certain position
// sot that it is used in a pathfinding algorithm
class Node
{
    //every node may have different values
    //according to your game/application
    public enum Value
    {
        FREE,
        BLOCKED
    }


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
    public Value value;

    //Constructor
    public Node(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;

        value = Value.FREE;
    }

}
public class AStar : MonoBehaviour
{

    //Constant
    private const int MAP_SIZE = 6;

    //Variables
    private List<string> map;
    private Node[,] nodeMap;
    // Use this for initialization
    void Start()
    {
        map = new List<string>();
        map.Add("G-----");
        map.Add("XXX-XX");
        map.Add("S-X-X-");
        map.Add("--X-X-");
        map.Add("--X-X-");
        map.Add("------");

        //parse the map
        nodeMap = new Node[MAP_SIZE, MAP_SIZE];
        Node start = null;
        Node goal = null;

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                Node node = new Node(x, y);
                char currentChar = map[y][x];
                if (currentChar == 'X')
                {
                    node.value = Node.Value.BLOCKED;
                }
                else if (currentChar == 'G')
                {
                    goal = node;
                }
                else if (currentChar == 'S')
                {
                    start = node;
                }
                nodeMap[x, y] = node;
            }

        }

        //Execute AStar algorithm
        List<Node> nodePath = ExecuteAStar(start, goal);

        //Burn the path in the map

        foreach(Node node in nodePath)
        {
            char[] charArray = map[node.posY].ToCharArray();
            charArray[node.posX] = '@';
            map[node.posY] = new string(charArray);
        }
        //Print the map
        string mapString = "";
        foreach (string mapRow in map)
        {
            mapString += mapRow + "\n";
        }
        Debug.Log(mapString);
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
            if (candidate.value != Node.Value.BLOCKED)
            {
                neighbors.Add(candidate);
            }
        }

        if (node.posX + 1 <= MAP_SIZE - 1)
        {
            Node candidate = nodeMap[node.posX + 1, node.posY];
            if (candidate.value != Node.Value.BLOCKED)
            {
                neighbors.Add(candidate);
            }
        }

        if(node.posY -1 >= 0)
        {
            Node candidate = nodeMap[node.posX, node.posY - 1];
            if(candidate.value != Node.Value.BLOCKED)
            {
                neighbors.Add(candidate);
            }
        }

        if(node.posY + 1 <= MAP_SIZE - 1)
        {
            Node candidate = nodeMap[node.posX, node.posY + 1];
            if(candidate.value != Node.Value.BLOCKED)
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
