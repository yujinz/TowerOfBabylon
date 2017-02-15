using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleTile : MonoBehaviour {

    public float resetTime = 7f;
    public bool ____private____;

    public bool destroy = false;

    public void Update() {
        if (destroy) {
            Vanish();
            Invoke("Reset", resetTime);
        }
    }
    public void Vanish() {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        //Instantiate(Effects.S.TileBreak, transform.position, transform.rotation);
        destroy = false;
    }

    public void Reset() {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
