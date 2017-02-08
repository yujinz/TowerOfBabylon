using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineSample : MonoBehaviour {

    public static CoroutineSample S;

    public bool play = true;

    // Use this for initialization
    void Start () {
        StartCoroutine("PrintOne");
    }

    IEnumerator PrintOne() {
        while (play) {
            print("tp");
            yield return new WaitForSeconds(1f);
        }
    }
}
