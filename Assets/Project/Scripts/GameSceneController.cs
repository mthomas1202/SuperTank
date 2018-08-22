using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour {

    public AStar aStar;
    public MovableObject player;

	// Use this for initialization
	void Start () {
       // List<Node> path = aStar.FindPath(player.gameObject, );
        //player.Move(path);
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.collider.gameObject.GetComponent<NavTile>() != null)
                {
                    player.Move(aStar.FindPath(player.gameObject, hit.collider.gameObject));
                    break;
                }
            }
          
        }
    }
	
}
