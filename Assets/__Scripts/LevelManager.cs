using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    //public Vector3 camOffset1 = new Vector3(0, 0, -15);
    //public Vector3 camOffset2 = new Vector3(15, 0, 0);
    //public Vector3 camYOffset = new Vector3(0, 2, 0);
    public int startLv = 1;
    //public Vector3 speedH1 = new Vector3(4f, 0, 0);
    public Vector3 speedH2 = new Vector3(0, 0, 4f);
    public Vector3 rot1 = new Vector3(0,0,0);
    public Vector3 rot2 = new Vector3(0, 90, 0);
    Quaternion rotation1 = Quaternion.Euler(0, 0, 0);
    Quaternion rotation2 = Quaternion.Euler(0, 90, 0);

    Bounds boundedBox;

    public bool prevFlag = false;
    public bool flag = false;

    GameObject heroObject, hammerObject;//, cam;
    CameraFollow cam;

    // Use this for initialization
    void Start () {
        rotation1.eulerAngles = rot1;
        rotation2.eulerAngles = rot2;

        cam = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        heroObject = GameObject.Find("Hero").gameObject;
        hammerObject = GameObject.Find("HammerS").gameObject;
        boundedBox = gameObject.GetComponent<BoxCollider>().bounds;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        prevFlag = flag;

        //print(Hero.S.transform.localPosition);
        //print(boundedBox);

        if (boundedBox.Contains(Hero.S.transform.localPosition))
            flag = true;
        else
            flag = false;

        if (flag)
            FreezeHero();
        else if (prevFlag && !flag)
            Invoke("ActivateHero", 0.2f);

        //print(GameObject.Find("Hero").GetComponent<Rigidbody>().velocity);
    }

    void FreezeHero() {
        //print("lv changing");
        cam.isLving = true;
        foreach (Monster mt in FindObjectsOfType<Monster>()) {
            if (mt.lv == cam.level) mt.enabled = false;
        }
        heroObject.GetComponent<Hero>().enabled = false;
        RigidbodyConstraints noRot = RigidbodyConstraints.FreezeRotation;
        RigidbodyConstraints noPos = RigidbodyConstraints.FreezePosition;
        hammerObject.GetComponent<Rigidbody>().constraints = noRot | noPos;

        if (cam.GetComponent<CameraFollow>().level == startLv) { //from lv1
            Vector3 pos = Hero.S.transform.position;
            pos.x = 10.5f;
            heroObject.GetComponent<Rigidbody>().transform.position = pos;

            heroObject.GetComponent<Rigidbody>().transform.rotation = Quaternion.Euler(0, -90, 0);
            heroObject.GetComponent<Rigidbody>().velocity = speedH2;

            heroObject.GetComponent<Rigidbody>().constraints = noRot | RigidbodyConstraints.FreezePositionX;
        }
        else if (cam.GetComponent<CameraFollow>().level == startLv + 1) { //from lv1
            Vector3 pos = Hero.S.transform.position;
            pos.x = -10.5f;
            heroObject.GetComponent<Rigidbody>().transform.position = pos;

            heroObject.GetComponent<Rigidbody>().transform.rotation = Quaternion.Euler(0, 90, 0);
            heroObject.GetComponent<Rigidbody>().velocity = speedH2;

            heroObject.GetComponent<Rigidbody>().constraints = noRot | RigidbodyConstraints.FreezePositionX;
        }
    }

    void ActivateHero() {
        print("leave changed");
        cam.isLving = false;

        if (cam.level == startLv) {
            AllRotate.S.transform.rotation = rotation2;// Quaternion.Euler(0, 90, 0);
            cam.level = startLv + 1;
        }
        else if (cam.level == startLv + 1) {
            cam.level = startLv;
            AllRotate.S.transform.rotation = rotation1;// Quaternion.Euler(0, 0, 0);
        }

        heroObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        RigidbodyConstraints noRot = RigidbodyConstraints.FreezeRotation;
        heroObject.GetComponent<Rigidbody>().constraints = noRot | RigidbodyConstraints.FreezePositionZ;

        hammerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

        heroObject.GetComponent<Hero>().enabled = true;

        foreach (Monster mt in FindObjectsOfType<Monster>()) {
             if (mt.lv == cam.level) mt.enabled = true;
        }
        /*
        Vector3 pos = Hero.S.transform.position;
        pos.x = 10.5f;
        heroObject.GetComponent<Rigidbody>().transform.position = pos;
        */
    }
}
