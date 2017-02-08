using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// action: left/right, hit, jump, long/short

// turn left/right: hit 0.5 then turn
// jump no hit : hit 1, give vel.y, keep vel.x, stop at hit 1.25
// jump while hit: finish current hit, then jump
// air no hit: control left/right speed
// air hit: keep vel.x, hit 1 (circle)

// long/short while hit: finish current hit, then change
// long/short in air:



public class Hammer : MonoBehaviour {
    static public Hammer S;

    public bool HammerLeft = true;
    public int jumpForce = 5;
    Rigidbody rigid;
    public Vector3 up;
    public float dist;
    public bool APressed = false;

    void Awake() {
        S = this;
    }

    void Start() {
        rigid = GetComponent<Rigidbody>();
        
    }

    void Update() {
        up = transform.up;
        dist = Vector3.Distance(transform.up, Vector3.right);

        if (Input.GetKeyDown(KeyCode.A)) {
            this.gameObject.layer = LayerMask.NameToLayer("HammerJump");
            APressed = true;
            if (HammerLeft) {
                rigid.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                HammerLeft = false;
            }
            else {
                rigid.AddForce(-transform.up * jumpForce, ForceMode.Impulse);
                HammerLeft = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.A)) {
            this.gameObject.layer = LayerMask.NameToLayer("Hammer");
        }

        if (APressed && Vector3.Distance(transform.up, Vector3.right) < 0.1f) {
            transform.up = Vector3.right;
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
            //APressed = false;
        }

        


        

    }
 
    
}
