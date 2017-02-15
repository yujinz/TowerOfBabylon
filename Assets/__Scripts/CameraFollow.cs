using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour {
    static public CameraFollow S; // This is a Singleton of the CameraFollow class. - JB

    public Transform poi; // The Point Of Interest of the CameraFollow script. - JB
    public Vector3 offset = new Vector3(0, 2, -15);
    public float easingU = 0.05f;
    public int level = 1;
    public bool isLving = false;

    private Camera cam;


    void Awake() {
        S = this;

        cam = GetComponent<Camera>();

        // Initially position the camera exactly over the poi - JB
        transform.position = poi.position + offset;
    }

    // Update is called once per frame - JB
    void FixedUpdate() {
        Vector3 p0, p1, p01;

        p0 = transform.position;
        p1 = poi.position + offset;

        // Linear Interpolation - Look it up in Appendix B - JB
        if (Mathf.Abs(p0.x - p1.x) > 15f)
            p01 = p1;
        else
            p01 = (1 - easingU) * p0 + easingU * p1;

        p01.x = RoundToNearestPixel(p01.x, cam);
        p01.y = RoundToNearestPixel(p01.y, cam);
        p01.z = RoundToNearestPixel(p01.z, cam);

        transform.position = p01;

        if (p01.y < -20) {
            SceneManager.LoadScene("_Scene_Main");
        }
    }


    // From https://www.reddit.com/r/Unity3D/comments/34ip2j/gaps_between_tiled_sprites_help/ - JB
    private float RoundToNearestPixel(float unityUnits, Camera viewingCamera) {
        float valueInPixels = (Screen.height / (viewingCamera.orthographicSize * 2)) * unityUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float adjustedUnityUnits = valueInPixels / (Screen.height / (viewingCamera.orthographicSize * 2));
        return adjustedUnityUnits;
    }


    //    void LateUpdate () 
    //    {
    //        lerpCamX = Mathf.Lerp(this.transform.position.x, player.transform.position.x, camSpeedX);
    //        lerpCamY = this.transform.position.y;
    //        
    //        this.transform.position =  new Vector3(RoundToNearestPixel(lerpCamX, Camera.main), RoundToNearestPixel(lerpCamY, Camera.main), this.transform.position.z);
    //        
    //    }
}