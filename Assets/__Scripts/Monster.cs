using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Monster : MonoBehaviour {

    public int lv = 1;
    public int health;
    public float resetTime = 0.2f;
    public float reviveTime = 10f;
    public int damage;

    public bool ________private________;
    public bool recovery = false;
    public float tileSize = 0.5f; // Default tile size.
    public Transform startTrans;
    public Vector3 startPos;

    public void Awake() {
        startTrans = transform;
        startPos = transform.localPosition;
    }
    
    public virtual void Die() {
        print("Enemy " + gameObject.name + " dies.");
            Vanish();
            Invoke("Revive", reviveTime);
    }

    public void Vanish() {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.enabled = false;
        }
    }

    public void Revive() {
        //transform.position = startTrans.position;
        transform.localPosition = startPos;
        transform.eulerAngles = startTrans.eulerAngles;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.enabled = true;
        }
    }
        
    public GameObject Probe(Vector3 direction, float length) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, length, 1<<LayerMask.NameToLayer("Ground")))
            return hit.collider.gameObject;
        else
            return null;
    }

}
