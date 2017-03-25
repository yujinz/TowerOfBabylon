using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hopper : Monster {
    public Vector3 direction;
    public float speedX = 2f;
    public float speedJump = 10f;
    public float hTime = 3f;
    public float vTime = 5f;
    public bool nextLeft = true;
    public float nextChangeTime;
    Rigidbody rigid;

    void Start () {
        direction = Vector3.zero;
        nextChangeTime = Time.time;
        rigid = GetComponent<Rigidbody>();
    }
	
	void Update () {
        if (transform.position == startPos) {
            direction = Vector3.zero;
            nextChangeTime = Time.time;
            nextLeft = true;
        }

        if (Time.time > nextChangeTime) {
            RigidbodyConstraints noRot = RigidbodyConstraints.FreezeRotation;
            rigid.constraints = noRot | RigidbodyConstraints.FreezePositionZ;
            if (direction == Vector3.zero) {
                if (nextLeft) {
                    direction = Vector3.left;
                    nextLeft = false;
                }
                else {
                    direction = Vector3.right;
                    nextLeft = true;
                }
                nextChangeTime += hTime;
                rigid.constraints = noRot | RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezePositionY;
            }
            else {
                direction = Vector3.zero;
                Vector3 vel = rigid.velocity;
                vel.y = speedJump;
                rigid.velocity = vel;

                nextChangeTime += vTime;
                rigid.constraints = noRot | RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezePositionX;
            }
        }
	}

    public void FixedUpdate() {
        transform.position += speedX * direction;
    }
}
