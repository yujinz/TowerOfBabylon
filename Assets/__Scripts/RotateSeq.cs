using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSeq : MonoBehaviour {
    public bool ans = false;
    public const int len = 10;
    public int hitLen = 3;
    public int jumpLen = 6;
    public float dtMax = 0.1f;
    public int[] seq = new int[len];
    public int idx = 0;
    public float tStart;

    virtual public void Awake () {
        tStart = Time.time;
	}

    virtual public void Update () {
        int curr = getRightJQ();
        if (Hero.S.isTakingOff) curr = 0;
        if (Hammer.S.isColliding && idx > hitLen - 1) idx = 0;
        
        if (curr != 0) {
            //print("curr" + curr);
            //print("idx" + idx);
            if (idx == 0) {
                setSeq(curr);
                //print(string.Format("{0}, {1}, {2}, {3}, {4}, {5}", seq[0], seq[1], seq[2], seq[3], seq[4], seq[5]));
                tStart = Time.time;
                idx++;
            }
            else if (curr == seq[idx - 1]) { }
            else if (curr == seq[idx]) {
                float dt = Time.time - tStart;
                tStart = Time.time; 
                if (dt < dtMax) {
                    ++idx;
                    setHammerDir();
                    if (idx <= hitLen) {
                        if (idx == hitLen) {
                            callHit();
                        }
                    }
                    else if (idx == jumpLen) {
                            callJump();
                        
                    }
                    else if (idx == len) {
                        idx = 0;
                    }
                }
                else  idx = 0;
            }
            else  idx = 0;
        }
        else   idx = 0;
    }

    public static int getRightJQ() {
        float x = Input.GetAxis("RightJoystickX");
        float y = Input.GetAxis("RightJoystickY");
        return getQuadrant(x, y);
    }

    public static int getQuadrant(float x, float y) {
        if (Mathf.Abs(x) < 0.2 && Mathf.Abs(y) < 0.2) return 0;

        if (x > 0 && y <= 0) {
            if (Mathf.Abs(x / y) > 1) return 1;
            else return 2;
        }
        else if (x <= 0 && y <= 0) {
            if (Mathf.Abs(x / y) < 1) return 3;
            else return 4;
        }
        else if (x <= 0 && y > 0) {
            if (Mathf.Abs(x / y) > 1) return 5;
            else return 6;
        }
        else if (x > 0 && y > 0) {
            if (Mathf.Abs(x / y) < 1) return 7;
            else return 8;
        }
        return 0;
    }

    virtual public void setSeq(int num) {
    }

    virtual public void setHammerDir() {
    }

    virtual public void callHit() {
    }

    virtual public void callJump() {
    }
}
