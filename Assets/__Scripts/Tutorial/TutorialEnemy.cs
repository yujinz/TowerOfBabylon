using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEnemy : MonoBehaviour {
    public GameObject text;
    public int count = 0;

	void Start () {
        //Hero = GameObject.Find("Hero").gameObject;
    }

    // Update is called once per frame
    void Update () {
		if (count == 2 && RotateSeq.getRightJQ()!=0) {
            Time.timeScale = 1f;
            text.SetActive(false);
            Destroy(this);
        }
	}

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Hero") return;

        count++;
        if (count == 2) {
            text.SetActive(true);
            Time.timeScale = 0.0001f;
        }
    }
}
