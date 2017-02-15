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

    Rigidbody rigid;
    public bool isColliding = false;

    void Awake() {
        S = this;
    }

    void Start() {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        //Hero.S.shouldClimb = false;
    }

    public GameObject lastTriggerGo = null;
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            if (Hero.S.isHammer) {
                collision.gameObject.GetComponent<Monster>().Die();
                ScreenShakeEffect.Shake(5, 1, 1);
            }
            return;
        }

        if (!hammerExactLeft() && !hammerExactRight()) isColliding = true;

        //print(gameObject.transform.position.y);
        if (collision.gameObject == lastTriggerGo) return;
        if (transform.position.y <
            collision.gameObject.transform.position.y + collision.collider.bounds.extents.y)
            return;

        lastTriggerGo = collision.gameObject;
        if (Hero.S.grounded) return;
        /*
        print("vel: " + rigid.velocity);
        print(Hero.S.bodyRigid.velocity);
        print(rigid.velocity.x - Hero.S.bodyRigid.velocity.x);
        print("pos: " + transform.position);
        print(Hero.S.transform.position);

        if ((rigid.velocity.x - Hero.S.bodyRigid.velocity.x > 1
            && transform.position.x > Hero.S.transform.position.x)
            || (rigid.velocity.x - Hero.S.bodyRigid.velocity.x < -1
            && transform.position.x < Hero.S.transform.position.x)) {
        }*/

        //print("should climb");
        Hero.S.shouldClimb = true;
        ScreenShakeEffect.Shake(3, 1, 1);
        //rigid.velocity = Vector3.zero; /////
    }

    void OnCollisionExit(Collision collision) {
        isColliding = false;
        //rigid.velocity = Vector3.zero;
    }


    public bool hammerExactLeft() {
        return Vector3.Distance(rigid.gameObject.transform.up, Vector3.up) < 0.2f;
    }

    public bool hammerExactRight() {
        return Vector3.Distance(rigid.gameObject.transform.up, -Vector3.up) < 0.2f;
    }

    public bool hammerExactUp() {
        return Vector3.Distance(rigid.gameObject.transform.up, Vector3.right) < 0.2f;
    }
}
