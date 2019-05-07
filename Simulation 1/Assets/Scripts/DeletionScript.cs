using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletionScript : MonoBehaviour {

    public Collider2D collider2D;
    public GameObject self;

	// Use this for initialization
	void Start () {
        collider2D = GetComponent<BoxCollider2D>();
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            Destroy(self);
        }
    }
}
