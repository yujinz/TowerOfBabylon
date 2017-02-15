using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialEnd : MonoBehaviour {
    public GameObject text;
    public int count = 0;

	void Start () {
        //Hero = GameObject.Find("Hero").gameObject;
    }

    // Update is called once per frame
    void Update () {
		if (count == 2 && Input.GetButton("AButton")) {
            Time.timeScale = 1f;
            text.SetActive(false);
            Destroy(this);
            SceneManager.LoadScene("_Scene_Main");
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
