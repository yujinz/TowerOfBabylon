using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMove : MonoBehaviour {
    public GameObject text;
    public int count = 0;

	void Start () {
        //Hero = GameObject.Find("Hero").gameObject;
    }

    // Update is called once per frame
    void Update () {
        //print(getLeftJQ());		
	}

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Hero") return;

        count++;
        if (count == 2) {
            text.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag != "Hero") return;

        if (count == 2) {
            text.SetActive(false);
            Destroy(this);
        }
    }

    int getLeftJQ() {
        float x = Input.GetAxis("LeftJoystickX");
        float y = Input.GetAxis("LeftJoystickY");
        return ClockwiseSeq.getQuadrant(x, y);
    }
}
