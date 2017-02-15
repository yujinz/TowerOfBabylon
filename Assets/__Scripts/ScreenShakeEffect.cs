/* A component that implements (and provides an API for) a screen-shake effect */
// Requires placement on a camera gameobject.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeEffect : MonoBehaviour {

    public static ScreenShakeEffect S;

    public float shake_timer = 10f;
    public float shake_radius = 1f;
    public float shake_speed = 1f;

    // Note: speed currently does not affect the effect.
    // Ideally, it delays each jump of the camera by 'speed' frames.
    public static void Shake(float duration, float radius, float speed)
    {
        S.shake_timer = duration;
        S.shake_radius = radius;
        S.shake_speed = speed;
    }

    // Use this for initialization
    void Start () {
        if (S != null && S != this)
        {
            Destroy(this);
            return;
        }
        else
            S = this;
    }
    
    // Update is called once per frame
    void Update () {
        if(shake_timer > 0)
        {
            transform.localPosition = UnityEngine.Random.onUnitSphere * shake_radius;
            shake_timer--;
        } else
        {
            //transform.localPosition = Vector3.zero;
        }
    }
}
