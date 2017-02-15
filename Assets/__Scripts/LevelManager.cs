using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    //public Vector3 camOffset1 = new Vector3(0, 0, -15);
    //public Vector3 camOffset2 = new Vector3(15, 0, 0);
    //public Vector3 camYOffset = new Vector3(0, 2, 0);
    public Vector3 speedH1 = new Vector3(4f, 0, 0);
    public Vector3 speedH2 = new Vector3(0, 0, 4f);
    public Quaternion rotation1 = Quaternion.Euler(0, 0, 0);
    public Quaternion rotation2 = Quaternion.Euler(0, -90, 0);

    Bounds boundedBox;

    public bool prevFlag = false;
    public bool flag = false;

    GameObject heroObject, hammerObject;//, cam;
    CameraFollow cam;

    // Use this for initialization
    void Start () {
        cam = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        heroObject = GameObject.Find("Hero").gameObject;
        hammerObject = GameObject.Find("HammerS").gameObject;
        boundedBox = gameObject.GetComponent<BoxCollider>().bounds;
    }
	
	// Update is called once per frame
	void Update () {
        prevFlag = flag;

        if (boundedBox.Contains(Hero.S.transform.position))
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
        print("lv changing");
        cam.isLving = true;
        foreach (Monster mt in FindObjectsOfType<Monster>()) {
            mt.enabled = false;
        }
        heroObject.GetComponent<Hero>().enabled = false;
        RigidbodyConstraints noRot = RigidbodyConstraints.FreezeRotation;
        RigidbodyConstraints noPos = RigidbodyConstraints.FreezePosition;
        hammerObject.GetComponent<Rigidbody>().constraints = noRot | noPos;

        if (cam.GetComponent<CameraFollow>().level == 1) { //from lv1
            Vector3 pos = Hero.S.transform.position;
            pos.x = 10.5f;
            heroObject.GetComponent<Rigidbody>().transform.position = pos;

            heroObject.GetComponent<Rigidbody>().transform.rotation = rotation2;
            heroObject.GetComponent<Rigidbody>().velocity = speedH2;

            heroObject.GetComponent<Rigidbody>().constraints = noRot | RigidbodyConstraints.FreezePositionX;
        }
        else if (cam.GetComponent<CameraFollow>().level == 2) { //from lv1
            Vector3 pos = Hero.S.transform.position;
            pos.z = -10.5f;
            heroObject.GetComponent<Rigidbody>().transform.position = pos;

            heroObject.GetComponent<Rigidbody>().transform.rotation = rotation1;
            heroObject.GetComponent<Rigidbody>().velocity = -speedH1;

            heroObject.GetComponent<Rigidbody>().constraints = noRot | RigidbodyConstraints.FreezePositionZ;
        }
    }

    void ActivateHero() {
        print("leave changed");
        cam.isLving = false;

        if (cam.level == 1) {
            AllRotate.S.transform.rotation = Quaternion.Euler(0, 90, 0);
            cam.level = 2;
        }
        else if (cam.level == 2) {
            cam.level = 1;
            AllRotate.S.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        heroObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        RigidbodyConstraints noRot = RigidbodyConstraints.FreezeRotation;
        heroObject.GetComponent<Rigidbody>().constraints = noRot | RigidbodyConstraints.FreezePositionZ;

        hammerObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

        heroObject.GetComponent<Hero>().enabled = true;
        /*
        Vector3 pos = Hero.S.transform.position;
        pos.x = 10.5f;
        heroObject.GetComponent<Rigidbody>().transform.position = pos;
        */
    }
}
