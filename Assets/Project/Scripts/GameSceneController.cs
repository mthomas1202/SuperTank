using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour {

    public AStar aStar;
    public GameObject player;

	// Use this for initialization
	void Start () {
        List<Node> path = aStar.FindPath();
        StartCoroutine(MoveRoutine(path));
	}
	
    private IEnumerator MoveRoutine(List<Node> path)
    {
        foreach(Node node in path)
        {
            player.transform.position = node.value.transform.position;
            yield return new WaitForSeconds(0.5f);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
