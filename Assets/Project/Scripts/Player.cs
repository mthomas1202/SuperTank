using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public delegate void CollectHandler(GameObject crate);
    public event CollectHandler onCollect;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
       if(otherCollider.gameObject.tag == "Crate")
        {
            if(onCollect != null)
            {
                onCollect(otherCollider.gameObject);
            }
        }
    }
}
