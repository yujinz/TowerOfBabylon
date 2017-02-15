using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectJoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.localPosition != Vector3.zero)
            transform.localPosition = Vector3.zero;
    }
}
