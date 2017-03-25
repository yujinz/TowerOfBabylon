using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectJoint : MonoBehaviour {
    // correction for constraint joint

	void Update () {
        if (transform.localPosition != Vector3.zero)
            transform.localPosition = Vector3.zero;
    }
}
