using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Facing {
    L,
    R
}

public class Hero : MonoBehaviour {
    static public Hero S;

    public GameObject propeller;
    public int hitForce = 30;
    public int jumpForce = 50;
    //public float hitRateS = 1f;
    //public float hitRateL = 2f;
    public float jumpSpeed = 10f;
    public float speedX = 4f;
    public Vector3 speedH = new Vector3(4f, 0, 0);

    public bool ____;

    //public GameObject HammerS, HammerL, RodS, RodL;
    public Rigidbody HammerRigid, bodyRigid, RodRigid;
    SphereCollider feet;
    public MeshRenderer HammerMesh;
    //public Collider HammerCollider;
    //public HammerLen len;
    public bool HammerLeft = true;
    public bool grounded = true;
    public bool prevGrounded = true;
    public bool isTakingOff = false;
    public bool needHammerCorrect = false;
    public bool needSwitch = false;
    public bool switched = false;
    public bool isHammer = true;
    public bool shouldClimb = false;
    //public Vector3 up;
    //public float dist;    
    Vector3 groudCheckOffset;
    LayerMask groundPhysicsLayerMask;

    void Awake() {
        S = this;
        bodyRigid = GetComponent<Rigidbody>();
        HammerRigid = transform.Find("Hammer").GetComponent<Rigidbody>();
        RodRigid = transform.Find("Rod").GetComponent<Rigidbody>();
        HammerMesh = transform.Find("Hammer").GetComponent<MeshRenderer>();

        Transform baseTrans = transform.Find("Feet");
        feet = baseTrans.GetComponent<SphereCollider>();
        groudCheckOffset = new Vector3(feet.radius * 0.4f, 0, 0);
        groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    }

    void Start () {
        //switchLenS();
        switchHammer();
    }		

    void FixedUpdate() {
        Vector3 checkLoc = feet.transform.position + Vector3.down * (feet.radius * 0.2f);
        prevGrounded = grounded;
        grounded = (Physics.Raycast(checkLoc + groudCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask))
                   || (Physics.Raycast(checkLoc - groudCheckOffset, Vector3.down, feet.radius, groundPhysicsLayerMask));
        //Debug.DrawLine(checkLoc + groudCheckOffset, checkLoc + groudCheckOffset - new Vector3(0, feet.radius, 0), Color.red, 1f);
        //Debug.DrawLine(checkLoc - groudCheckOffset, checkLoc - groudCheckOffset - new Vector3(0, feet.radius, 0), Color.red, 1f);

        Vector3 vel = bodyRigid.velocity;

        //============================ Move ==========================
        if (getLeftJQ() == 1 || getLeftJQ() == 8) { vel.x = speedH.x; vel.z = speedH.z; }
        else if (getLeftJQ() == 4 || getLeftJQ() == 5) { vel.x = -speedH.x; vel.z = -speedH.z; }
        else { vel.x = 0; vel.z = 0; }

        if (Mathf.Abs(vel.y) < 0.001) vel.y = 0;

        //============================ Jump ==========================
        //Vector3 up = HammerRigid.gameObject.transform.up;
        //float dist = Vector3.Distance(HammerRigid.gameObject.transform.up, -Vector3.up);
        //print(dist);

        if (!needHammerCorrect && vel.y < 0 && !prevGrounded) needHammerCorrect = true;

        if (shouldClimb) {
            vel.y = jumpSpeed/1f;
            vel.x = speedX;
            shouldClimb = false;
            print("climb!");
        }

        if (grounded && isTakingOff) { //give hero jump speed
            if (HammerLeft && hammerExactLeft()) vel.y = jumpSpeed;
            if (!HammerLeft && hammerExactRight()) vel.y = jumpSpeed;
        }

        if (grounded && !isTakingOff && needHammerCorrect) {
            if (HammerRigid.gameObject.transform.localPosition.y >= 0) { // hammer is above ground
                vel = Vector3.zero; //no bounce
                switchHammer();
                needHammerCorrect = false;
            }

            //put down hammer
            if (HammerLeft) {
                HammerRigid.AddForce(-Vector3.right * jumpForce, ForceMode.Impulse);
                HammerLeft = false;
            }
            else {
                HammerRigid.AddForce(Vector3.right * jumpForce, ForceMode.Impulse);
                HammerLeft = true;
            }
        }
        //else if (grounded && !isTakingOff && HammerRigid.gameObject.transform.localPosition.y > 0.1)        

        if (Mathf.Abs(vel.y) > 0.5f && hammerExactUp() && RotateSeq.getRightJQ() == 0) { //0.5f means !grounded
            HammerRigid.gameObject.transform.up = Vector3.right;
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            isTakingOff = false;
        }
        else if (vel.y <= -0.5f) {
            isTakingOff = false;
        }

        bodyRigid.velocity = vel;

        //========== hammer no bounce ==============
        if (!isTakingOff && HammerLeft && hammerExactRight()) {
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            HammerLeft = false;
            RodRigid.gameObject.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        if (!isTakingOff && !HammerLeft && hammerExactLeft()) {
            HammerRigid.angularVelocity = Vector3.zero;
            HammerRigid.velocity = Vector3.zero;
            HammerLeft = true;
            RodRigid.gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
        }

        //========= switch hammer/propeller =========
        if (!grounded) {
            if (Input.GetAxis("Trigger") > 0.2 && 
                HammerRigid.gameObject.transform.localPosition.y > 0) {
                switchHammer();
            }
            else if (Input.GetAxis("Trigger") <= 0) {
                switchPropeller();
            }
        }
        else { //grounded           
            if (Input.GetAxis("Trigger") > 0.2 &&
                HammerRigid.gameObject.transform.localPosition.y < -0.1) {
                switchPropeller();
                HammerRigid.gameObject.layer = LayerMask.NameToLayer("HammerJump");
                needHammerCorrect = true;
            }
            else if (prevGrounded && Input.GetAxis("Trigger") <= 0 
                && HammerRigid.gameObject.transform.localPosition.y > 0) {
                switchHammer();
            }
        }
    }
    void switchPropeller() {
        propeller.SetActive(true);
        //HammerCollider.enabled = false;
        HammerMesh.enabled = false;
        isHammer = false;
    }

    void switchHammer() {
        if (isTakingOff) return;
        propeller.SetActive(false);
        //HammerCollider.enabled = true;
        HammerMesh.enabled = true;
        HammerRigid.gameObject.layer = LayerMask.NameToLayer("Hammer");
        isHammer = true;
    }

    public void hitClockwise() {
        HammerRigid.AddForce(HammerRigid.gameObject.transform.up * hitForce, ForceMode.Impulse);
    }

    public void hitCounterClockwise() {
        HammerRigid.AddForce(-HammerRigid.gameObject.transform.up * hitForce, ForceMode.Impulse);
    }

    public void startJumpClockwise() {
        if (!checkAndSetJump()) return;
        HammerRigid.AddForce(HammerRigid.gameObject.transform.up * jumpForce, ForceMode.Impulse);
    }

    public void startJumpCounterClockwise() {
        if (!checkAndSetJump()) return;
        HammerRigid.AddForce( - HammerRigid.gameObject.transform.up * jumpForce, ForceMode.Impulse);
    }

    public bool checkAndSetJump() {
        if (isTakingOff) return false;
        if (Hammer.S.isColliding) return false;

        isTakingOff = true;
        switchPropeller();
        HammerRigid.gameObject.layer = LayerMask.NameToLayer("HammerJump");
        return true;
    }
    
    public bool hammerExactLeft() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, Vector3.up) < 0.2f;
    }

    public bool hammerExactRight() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, -Vector3.up) < 0.2f;
    }

    public bool hammerExactUp() {
        return Vector3.Distance(HammerRigid.gameObject.transform.up, Vector3.right) < 0.2f;
    }

    int getLeftJQ() {
        float x = Input.GetAxis("LeftJoystickX");
        float y = Input.GetAxis("LeftJoystickY");
        return ClockwiseSeq.getQuadrant(x, y);
    }


    /* ================== Future feature: different length of hammer ================
    
    public enum HammerLen {
        L,
        S
    }

    void Update () {
        
        if (Input.GetKeyDown(KeyCode.S)) {
            StartCoroutine("HitBackForth");
        }
        else if (Input.GetKeyUp(KeyCode.S)) {
            StopCoroutine("HitBackForth");
        }
        
        //=================== Switch Hammer Length ===================
        if (Input.GetKeyDown(KeyCode.D)) {
            if (len == HammerLen.S)
                switchLenL();
            else 
                switchLenS();
        }
    }

    IEnumerator HitBackForth() {
        while (true) {
            if (HammerLeft) {
                HammerLeft = false;
            }
            else {
                HammerLeft = true;
            }

            HammerRigid.AddForce(transform.up * hitForce, ForceMode.Impulse);
            float period = (len == HammerLen.S ? hitRateS : hitRateL);
            yield return new WaitForSeconds(period);
        }
    }
    void switchLenS() {
        len = HammerLen.S;
        HammerRigid = HammerS.GetComponent<Rigidbody>();
        RodRigid = RodS.GetComponent<Rigidbody>();
        RodS.SetActive(true);
        HammerS.SetActive(true);
        HammerL.SetActive(false);
        RodL.SetActive(false);
    }

    void switchLenL() {
        len = HammerLen.L;
        HammerRigid = HammerL.GetComponent<Rigidbody>();
        RodRigid = RodL.GetComponent<Rigidbody>();
        RodL.SetActive(true);
        HammerL.SetActive(true);
        HammerS.SetActive(false);
        RodS.SetActive(false);
    }
    */

}


